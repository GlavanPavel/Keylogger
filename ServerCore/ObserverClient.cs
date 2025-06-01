/*************************************************************************
 *                                                                       *
 *  File:        ObserverClient.cs                                       *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Defines the ObserverClient class that represents a       *
 *               client observing keylogger data from the server. This   *
 *               client receives updates asynchronously from monitored   *
 *               keylogger clients via the server's observer pattern.    *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

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
        public bool Update(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                Stream.Write(data, 0, data.Length);
                
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException($"[Observer] Failed to send to {Id}. Connection may be closed.", ex);
                Console.WriteLine(kex);
                return false;
            }
            return true;
        }
    }
}
