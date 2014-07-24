using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PowerSpeckConverter
{
    public partial class FormMain : Form
    {
        private const string AllowedData = "FileDrop";
        public FormMain()
        {
            InitializeComponent();
            UpdateGui();
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (ContainsValidContent(e.Data.GetFormats()))
                e.Effect = DragDropEffects.Copy;
        }

        private bool ContainsValidContent(IEnumerable<string> formats)
        {
            return !backgroundWorkerProcess.IsBusy && formats.Any(d => d == AllowedData);
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            if (ContainsValidContent(e.Data.GetFormats()))
            {
                var f = e.Data.GetData(AllowedData) as string[];

                if (f != null && f.Length > 0)
                    ProcessFile(f[0]);
            }
        }

        private void ProcessFile(string file)
        {
            if(!backgroundWorkerProcess.IsBusy)
                backgroundWorkerProcess.RunWorkerAsync(file);

            UpdateGui();
        }

        private void backgroundWorkerProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                FileHandling.ConvertFile(e.Argument as String);
                e.Result = null;
            }
            catch(Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void backgroundWorkerProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var r = e.Result as String;

            if (!String.IsNullOrEmpty(r))
                MessageBox.Show(r, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                timerFinish.Start();

            UpdateGui();
        }

        private void UpdateGui()
        {
            labelStatus.Text = timerFinish.Enabled?"Done!":(backgroundWorkerProcess.IsBusy?"Working...": "Drop a file here");
        }

        private void timerFinish_Tick(object sender, EventArgs e)
        {
            ((Timer) sender).Stop();
            UpdateGui();
        }
    }
}
