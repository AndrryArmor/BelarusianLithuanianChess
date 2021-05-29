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
        private const int _portNumber = 11000;
        protected readonly IPEndPoint _ipEndPoint;
        protected readonly IPAddress _ipAddr;

        protected GameEndPoint()
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            _ipAddr = ipHost.AddressList[0];
            _ipEndPoint = new IPEndPoint(_ipAddr, _portNumber);            
        }

        ~GameEndPoint()
        {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }

        public Socket Socket { get; protected set; }

        public void Send<T>(T data)
        {
            byte[] msg = JsonSerializer.SerializeToUtf8Bytes(data);

            // Отправляем данные через сокет
            int bytesSent = Socket.Send(msg);
        }

        public T Receive<T>()
        {
            // Мы дождались клиента, пытающегося с нами соединиться

            byte[] bytes = new byte[1024];
            int bytesRec = Socket.Receive(bytes);

            //data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(bytes, 0, bytesRec));
        }
    }
}
