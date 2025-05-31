using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class ClientHandler
    {
        public TcpClient TcpClient { get; set; }
        public string Id { get; set; }

        public virtual NetworkStream Stream => TcpClient.GetStream();

        public virtual void Close() => TcpClient.Close();
    }
}
