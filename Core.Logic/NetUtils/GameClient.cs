using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Logic.NetUtils
{
    public class GameClient : GameEndPoint
    {
        public GameClient() : base()
        {
            Socket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketError HandshakeGameHost(string hostName)
        {
            var addressList = Dns.GetHostEntry(hostName).AddressList
                .Where(address => address.AddressFamily == IPAddress.AddressFamily);
            foreach (var ipAdress in addressList)
            {
                try
                {
                    Socket.Connect(ipAdress, _portNumber);
                    return SocketError.Success;
                }
                catch (SocketException ex)
                {
                }
            }
            
            return SocketError.SocketError;
        }
    }
}
