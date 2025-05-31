using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class ObserverClient : ClientHandler, IObserver
    {
        public ObserverClient(TcpClient client)
        {
            TcpClient = client;
            Id = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
        }

        public void Update(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                Stream.Write(data, 0, data.Length);
            }
            catch
            {
                Console.WriteLine($"[Observer] Failed to send to {Id}. Connection may be closed.");
            }
        }
    }
}
