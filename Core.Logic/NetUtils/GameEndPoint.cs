using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BelarusChess.Core.Logic.NetUtils
{
    public abstract class GameEndPoint
    {
        protected const int _portNumber = 11000;

        protected GameEndPoint()
        {
            IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            Console.WriteLine(IPAddress);
        }

        public IPAddress IPAddress { get; }
        public Socket Socket { get; protected set; }

        public void Send<T>(T data)
        {
            byte[] dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
            Socket.Send(dataBytes);
        }

        public T Receive<T>()
        {
            byte[] buffer = new byte[1024];
            int bytesCount = Socket.Receive(buffer);

            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(buffer, 0, bytesCount));
        }

        public virtual void CloseConnnection()
        {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }
    }
}
