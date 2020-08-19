using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Entities.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(PlayerColor color, Cell cell, Chessboard chessboard) : base(PieceType.Pawn, color, cell, chessboard)
        {
        }

        public override IEnumerable<Cell> GetAvailableCells()
        {
            var allAvailableCells = new List<Cell>();

            switch (Color)
            {
                case PlayerColor.White:
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Up));
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpLeft));
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.UpRight));
                    break;
                case PlayerColor.Black:
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.Down));
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownLeft));
                    allAvailableCells.AddRange(FindAvailableCellsInDirection(MoveDirection.DownRight));
                    break;
            }

            return allAvailableCells;
        }

        protected override IEnumerable<Cell> FindAvailableCellsInDirection(MoveDirection direction)
        {
            var availableCells = new List<Cell>();
            var currentCell = GetNextCell(direction, Cell);

            if (!currentCell.IsValid)
                return availableCells;

            if (direction == MoveDirection.Up || direction == MoveDirection.Down)
            {
                if (Chessboard[currentCell] == null)
                {
                    availableCells.Add(currentCell);

                    // If pawn did no move before
                    if ((direction == MoveDirection.Up && Cell.Row == 7) ||
                        (direction == MoveDirection.Down && Cell.Row == 1))
                    {
                        currentCell = GetNextCell(direction, currentCell);
                        if (Chessboard[currentCell] == null)
                            availableCells.Add(currentCell);
                    }
                }
            }
            else
            {
                // If cell contains opponent's piece
                if (Chessboard[currentCell] != null && Chessboard[currentCell].Color != Color)
                    availableCells.Add(currentCell);
            }

            return availableCells;
        }
    }
}
