using System;
using System.IO;
using System.Windows.Forms;

namespace ZagAPIServer
{
    // Handles application-level events for C# WinForms applications.
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Add global exception handlers
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                File.WriteAllText("majorError.txt", exception.ToString());
                if (exception.InnerException != null)
                {
                    File.WriteAllText("majorError2.txt", exception.InnerException.ToString());
                }
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            if (exception != null)
            {
                File.WriteAllText("majorError.txt", exception.ToString());
                if (exception.InnerException != null)
                {
                    File.WriteAllText("majorError2.txt", exception.InnerException.ToString());
                }
            }
        }
    }
}