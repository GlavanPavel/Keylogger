using System.Net.Sockets;

namespace ServerCore
{
    /// <summary>
    /// Represents a connected client and manages its network stream.
    /// </summary>
    public class ClientHandler
    {
        /// <summary>
        /// Gets or sets the TCP client associated with this handler.
        /// </summary>
        public TcpClient TcpClient { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the client.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the network stream used to communicate with the client.
        /// </summary>
        public virtual NetworkStream Stream => TcpClient.GetStream();

        /// <summary>
        /// Closes the connection to the client.
        /// </summary>
        public virtual void Close() => TcpClient.Close();
    }
}
