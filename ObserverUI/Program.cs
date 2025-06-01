/*************************************************************************
 *                                                                       *
 *  File:        Program.cs                                              *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Entry point for the Keylogger Observer application.     *
 *               Initializes and launches the observer UI.               *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

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
