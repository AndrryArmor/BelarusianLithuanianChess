using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Entities.Pieces
{
    public class Rook : Piece
    {
        public Rook(PlayerColor color, Cell cell, Chessboard chessboard) : base(PieceType.Rook, color, cell, chessboard)
        {
        }

        public override IEnumerable<Cell> GetAvailableCells()
        {
            var allAvailableCells = new List<Cell>();

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Up));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Right));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Down));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Left));

            return allAvailableCells;
        }
    }
}
