using PowerSpeckUtilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PowerSpeckPlayer
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            try
            {
                AppDomain.CurrentDomain.UnhandledException +=
                    (o, ex) => Utilities.Log("[Global] UnhandledException");
                Application.ThreadException += (o, ex) => Utilities.Log("[Global] ThreadException");
                Application.Run(new FormScreen());
            }
            catch (Exception ex)
            {
                Utilities.Log("[Global] MainException: " + ex.GetExceptionDetails());
            }
        }
    }
}