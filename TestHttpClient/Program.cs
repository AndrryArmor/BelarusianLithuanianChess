using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using BelarusChess.Core.Entities;
using BelarusChess.Core.Logic.NetUtils;
using System.Collections.Generic;
using System.Linq;

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
                if (gameClient.Socket.Connected)
                    gameClient.CloseConnnection();
                Console.ReadLine();
            }
        }

        static void SendMessageFromSocket(GameClient gameClient)
        {
            Console.WriteLine(Dns.GetHostName());

            Console.Write("Введіть ім'я сервера: ");
            if (gameClient.HandshakeGameHost(Console.ReadLine()) == SocketError.SocketError)
            {
                Console.WriteLine("Помилка під'єднання. Введіть правильну адресу сервера.");
                return;
            }

            Console.WriteLine("Сокет з'єднується з {0} ", gameClient.Socket.RemoteEndPoint.ToString());

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