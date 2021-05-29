using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using BelarusChess.Core.Entities;
using BelarusChess.Core.Logic.NetUtils;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            GameClient gameClient = new GameClient();
             
            try
            {
                SendMessageFromSocket(gameClient);
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

        static void SendMessageFromSocket(GameClient gameClient)
        {
            gameClient.HandshakeGameHost();

            Console.Write("Введіть шаховий хід: ");
            string move = Console.ReadLine();

            Console.WriteLine("Сокет з'єднується з {0} ", gameClient.Socket.RemoteEndPoint.ToString());

            Chessboard chessboard = new Chessboard();
            var moveDto = new MoveDto
            {
                PieceCell = new Cell(7, 3),
                PieceNewCell = new Cell(5, 3)
            };
            
            gameClient.Send(moveDto);

            StringModel stringModel = gameClient.Receive<StringModel>();

            Console.WriteLine("\nВідповідь від сервера: {0}\n\n", stringModel.Text);
        }
    }
}