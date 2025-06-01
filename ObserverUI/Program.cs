using System;
using System.Windows.Forms;

namespace KeyloggerServer
{
    /// <summary>
    /// Contains the main entry point for the Windows Forms application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Starts the application with the ClientObserver form as the main window.
            Application.Run(new ClientObserver());
        }
    }
}
