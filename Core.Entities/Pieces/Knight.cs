using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Entities.Pieces
{
    public class Knight : Piece
    {
        public Knight(PlayerColor color, Cell cell, Chessboard chessboard) : base(PieceType.Knight, color, cell, chessboard)
        {
        }

        public override IEnumerable<Cell> GetAvailableCells()
        {
            var allAvailableCells = new List<Cell>();

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpUpLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpUpRight));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.RightRightUp));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.RightRightDown));

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownDownRight));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownDownLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.LeftLeftDown));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.LeftLeftUp));

            return allAvailableCells;
        }

        protected override IEnumerable<Cell> FindAvailableCellsInDirection(MoveDirection direction)
        {
            var availableCells = new List<Cell>();
            var currentCell = GetNextCell(direction, Cell);

            if (IsCellAvailable(currentCell))
                availableCells.Add(currentCell);

            return availableCells;
        }
    }
}
