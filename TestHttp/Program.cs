using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SocketClient;
using BelarusChess.Core.Entities;
using BelarusChess.Core.Entities.Pieces;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Очікуємо з'єднання через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    MoveDto data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться
                    
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    //data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    data = JsonSerializer.Deserialize<MoveDto>(new ReadOnlySpan<byte>(bytes, 0, bytesRec));

                    // Показываем данные на консоли
                    Chessboard chessboard = new Chessboard();
                    Console.Write("Отриманий хід: ({0}, {1}) - {2}, ({3}, {4})\n\n",
                        data.PieceCell.Row, data.PieceCell.Col, chessboard[data.PieceCell].ToString(), data.PieceNewCell.Row, data.PieceNewCell.Col);
                    
                    // Отправляем ответ клиенту\
                    string reply = $"Дякую за запит: ({data.PieceCell.Row}, {data.PieceCell.Col}), ({data.PieceNewCell.Row}, {data.PieceNewCell.Col})\n\n";
                    //byte[] msg = Encoding.UTF8.GetBytes(reply);

                    StringModel stringModel = new StringModel { Text = reply };
                    byte[] msg = JsonSerializer.SerializeToUtf8Bytes(stringModel);

                    handler.Send(msg);

                    /*if (data.IndexOf("stop") > -1)
                    {
                        Console.WriteLine("Сервер завершив з'єднання з клієнтом.");
                        break;
                    }*/
                    
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
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
    }
}