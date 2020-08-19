using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Entities.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(PlayerColor color, Cell cell, Chessboard chessboard) : base(PieceType.Bishop, color, cell, chessboard)
        {
        }

        public override IEnumerable<Cell> GetAvailableCells()
        {
            var allAvailableCells = new List<Cell>();

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpRight));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownRight));

            return allAvailableCells;
        }
    }
}
