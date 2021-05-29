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
            _listener = new Socket(_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public SocketError ListenForHandshake()
        {
            try
            {
                _listener.Bind(_ipEndPoint);
                _listener.Listen(10);

                Socket = _listener.Accept();
                return SocketError.Success;
            }
            catch (Exception)
            {
                return SocketError.SocketError;
            }
        }

        public override void CloseConnnection()
        {
            base.CloseConnnection();
            _listener.Close();
        }
    }
}
