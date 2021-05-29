using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using BelarusChess.Core.Entities;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SendMessageFromSocket(11000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            Console.Write("Введіть шаховий хід: ");
            string move = Console.ReadLine();

            Console.WriteLine("Сокет з'єднується з {0} ", sender.RemoteEndPoint.ToString());
            //byte[] msg = Encoding.UTF8.GetBytes(message);
            Chessboard chessboard = new Chessboard();
            var moveDto = new MoveDto
            {
                PieceCell = new Cell(7, 3),
                PieceNewCell = new Cell(5, 3)
            };
            byte[] msg = JsonSerializer.SerializeToUtf8Bytes(moveDto);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            Console.WriteLine("\nВідповідь від сервера: {0}\n\n", /*Encoding.UTF8.GetString(bytes, 0, bytesRec)*/ 
                JsonSerializer.Deserialize<StringModel>(new ReadOnlySpan<byte>(bytes, 0, bytesRec)).Text);

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (move.IndexOf("quit") == -1)
                SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}