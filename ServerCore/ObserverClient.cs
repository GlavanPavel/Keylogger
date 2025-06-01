using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    /// <summary>
    /// Represents an observer client that receives updates from the server.
    /// Inherits from <see cref="ClientHandler"/> and implements <see cref="IObserver"/>.
    /// </summary>
    public class ObserverClient : ClientHandler, IObserver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObserverClient"/> class using the specified TCP client.
        /// </summary>
        /// <param name="client">The TCP client representing the observer connection.</param>
        public ObserverClient(TcpClient client)
        {
            TcpClient = client;
            Id = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
        }

        /// <summary>
        /// Sends an update message to the observer client.
        /// </summary>
        /// <param name="message">The message to send to the observer.</param>
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
