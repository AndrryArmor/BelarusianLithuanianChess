using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SocketClient;
using BelarusChess.Core.Entities;
using BelarusChess.Core.Entities.Pieces;
using BelarusChess.Core.Logic.NetUtils;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer gameServer = new GameServer();

            try
            {
                Console.WriteLine("Очікуємо з'єднання через порт {0}", 11000);

                gameServer.ListenForHandshake();
                    
                MoveDto data = gameServer.Receive<MoveDto>();

                Chessboard chessboard = new Chessboard();
                Console.Write("Отриманий хід: ({0}, {1}) - {2}, ({3}, {4})\n\n",
                    data.PieceCell.Row, data.PieceCell.Col, chessboard[data.PieceCell].ToString(), data.PieceNewCell.Row, data.PieceNewCell.Col);
                    
                string reply = $"Дякую за запит: ({data.PieceCell.Row}, {data.PieceCell.Col}), ({data.PieceNewCell.Row}, {data.PieceNewCell.Col})\n\n";
                StringModel stringModel = new StringModel { Text = reply };
                gameServer.Send(stringModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                gameServer.CloseConnnection();
                Console.ReadLine();
            }
        }
    }
}