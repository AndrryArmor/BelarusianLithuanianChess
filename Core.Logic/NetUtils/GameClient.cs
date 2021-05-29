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
            // Создаем сокет Tcp/Ip
            Socket = new Socket(_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketError HandshakeGameHost()
        {
            try
            {
                // Соединяем сокет с удаленной точкой
                Socket.Connect(_ipEndPoint);
                return SocketError.Success;
            }
            catch (Exception)
            {
                return SocketError.SocketError;
            }
        }
    }
}
