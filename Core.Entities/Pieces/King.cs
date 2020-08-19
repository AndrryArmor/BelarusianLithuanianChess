using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Entities.Pieces
{
    public class King : Piece
    {
        public King(PlayerColor color, Cell cell, Chessboard chessboard) : base(PieceType.King, color, cell, chessboard)
        {
        }

        public override IEnumerable<Cell> GetAvailableCells()
        {
            var allAvailableCells = new List<Cell>();

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Up));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Right));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Down));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Left));

            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpRight));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownLeft));
            allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownRight));

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
