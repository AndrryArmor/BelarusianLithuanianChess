using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Logic.NetUtils
{
    public class GameServer : GameEndPoint
    {
        private readonly Socket _listener;

        public GameServer() : base()
        {
            // Создаем сокет Tcp/Ip
            _listener = new Socket(_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketError ListenForHandshake()
        {
            try
            {
                // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
                _listener.Bind(_ipEndPoint);
                _listener.Listen(10);
                // Программа приостанавливается, ожидая входящее соединение
                Socket = _listener.Accept();
                return SocketError.Success;
            }
            catch (Exception)
            {
                return SocketError.SocketError;
            }
        }
    }
}
