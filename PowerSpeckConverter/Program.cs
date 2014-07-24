using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PowerSpeckConverter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                try
                {
                    FileHandling.ConvertFile(Environment.GetCommandLineArgs()[1]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                Application.Run(new FormMain());
        }
    }
}
