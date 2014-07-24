using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PowerSpeckLib;
using PowerSpeckUtilities;

namespace PowerSpeckPlayer
{
    public partial class FormScreen : Form
    {
        private readonly bool _debugMode;
        private readonly object _drawing = new object();
        private readonly SlideCollection _slides;
        private readonly Stopwatch _timer;
        private Configuration _config;

        public FormScreen()
        {
            _slides = new SlideCollection();
            _debugMode = Debugger.IsAttached || Environment.GetCommandLineArgs().Any(s => s.Contains("-debug"));
            _config.State = State.Preparing;

            InitializeComponent();
            PowerSpeckUtilities.Utilities.Log("[General] Starting");

            if (_debugMode)
            {
                _timer = Stopwatch.StartNew();
                Utilities.Log("[General] Debug mode enabled");
            }

            // Window mode
            bool windowMode = Environment.GetCommandLineArgs().Any(s => s.Contains("-window"));
            if (!windowMode)
            {
                TopMost = true;
                FormBorderStyle = FormBorderStyle.None;

                var s = Screen.PrimaryScreen.WorkingArea;
                StartPosition = FormStartPosition.CenterScreen;
                WindowState = FormWindowState.Maximized;
                Width = s.Width;
                Height = s.Height;
                ShowIcon = false;
                ShowInTaskbar = false;
            }
            else
            {
                Utilities.Log("[General] Window mode");
            }

            Application.DoEvents();
            string configFile = Path.ChangeExtension(Process.GetCurrentProcess().MainModule.FileName, "ini").Replace(".vshost", "");

            if (!File.Exists(configFile))
                _config.State = State.Error;
            else
            {
                var c = new IniParser(configFile);
                timerUpdates.Interval = c.GetSettingAsInteger("general", "interval", 1000);
                _config.Background = Utilities.ParseColorOrDefault(c.GetSetting("general", "background"), Color.Black);

                var args = Environment.GetCommandLineArgs();
                var tmp = args.Length > 1? args[1]: String.Empty;


                if (String.IsNullOrEmpty(tmp) || !File.Exists(tmp))
                    tmp = c.GetSetting("general", "load");

                if (!String.IsNullOrEmpty(tmp) && File.Exists(tmp))
                {
                    Text = tmp;
                    _slides = SlideParser.Parse(tmp, _config.Background);

                    timerUpdates.Start();
                    _config.State = State.Running;
                }
                else
                {
                    _config.State = State.NoPresentation;
                }

                if (!windowMode)
                    Cursor = new Cursor(c.GetSetting("general", "cursor"));
            }
        }

        private void FormScreen_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                lock (_drawing)
                {
                    var localState = _config.State;

                    switch (localState)
                    {
                        case State.Running:
                            if (_slides.Draw(e.Graphics))
                                Invalidate();
                            break;

                        case State.Error:
                        case State.Preparing:
                            e.Graphics.DrawString(localState == State.Preparing
                                    ? "Initialising...": "ERROR: Check the configuration file (config.ini) and retry",
                                new Font("Arial", 20),localState == State.Preparing ? Brushes.LightBlue : Brushes.Red,10, 10);
                            break;

                        case State.Waiting:
                        case State.NoPresentation:
                        default:
                            // Empty screen
                            break;
                    }
                }

                if (_debugMode)
                {
                    string s;

                    lock (_drawing)
                    {
                        var str = new StringBuilder();
                        str.AppendLine("*********** DEBUG ***********");
                        str.Append("Current state: ");
                        str.AppendLine(_config.State.ToString());
                        str.AppendLine();
                        str.AppendLine("Last redraw (ms): ");
                        str.AppendLine(_timer.ElapsedMilliseconds.ToString());
                        s = str.ToString();
                        _timer.Restart();
                    }
                    var f = new Font("Arial", 10);

                    var r = e.Graphics.MeasureString(s, f);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), 20, 20, r.Width, r.Height);
                    e.Graphics.DrawString(s, f, Brushes.Black, 20, 20);
                }
            }
            catch (Exception ex)
            {
                Utilities.Log("[Drawing] " + ex.Message);
            }
        }

        private void FormScreen_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void FormScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utilities.Log("[General] Exiting, reason: " + e.CloseReason);
        }

        private void timerUpdates_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
    }


    public enum State
    {
        Error,
        Running,
        Preparing,
        NoPresentation,
        Waiting
    }

    internal struct Configuration
    {
        public Color Background;
        public State State;
    }
}