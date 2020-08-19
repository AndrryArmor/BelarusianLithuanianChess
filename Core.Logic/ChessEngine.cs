using BelarusChess.Core.Entities;
using BelarusChess.Core.Entities.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.Core.Logic
{
    public class ChessEngine
    {
        private readonly Chessboard _chessboard;
        private readonly Chessboard _savedChessboard = new Chessboard();

        public ChessEngine(Chessboard chessboard)
        {
            _chessboard = chessboard;
        }

        public IEnumerable<Cell> FindAvailableCells(Piece piece)
        {
            List<Cell> availableCells = piece.GetAvailableCells().ToList();
            if (piece.Type != PieceType.King)
                availableCells.Remove(Cell.Throne);

            piece.HasToRaiseOnCellChangeEvent = false;

            SaveChessboard();
            foreach (Cell cell in piece.GetAvailableCells())
            {
                MakeMove(piece, cell);
                if (IsCheck(piece.Color) || IsOverthrowing(piece.Color) || 
                    (IsSeizure(SwitchPlayer(piece.Color)) && !IsOverthrowing(SwitchPlayer(piece.Color))))
                {
                    availableCells.Remove(cell);
                }

                RestoreChessboard();
            }

            piece.HasToRaiseOnCellChangeEvent = true; 

            return availableCells;
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            _chessboard.MakeMove(piece, cell);
        }

        public PlayerState GetPlayerState(PlayerColor player)
        {
            if (IsIntronisation(player))
                return PlayerState.ThroneMine;
            else if (IsOverthrowing(player))
                return PlayerState.Overthrowing;
            else if (IsSeizure(player))
                return PlayerState.Throne;
            else if (IsCheckmate(player))
                return PlayerState.Checkmate;
            else if (IsCheck(player))
                return PlayerState.Check;
            else if (IsStalemate(player))
                return PlayerState.Stalemate;
            else
                return PlayerState.Regular;
        }

        private PlayerColor SwitchPlayer(PlayerColor player)
        {
            switch (player)
            {
                case PlayerColor.White:
                    return PlayerColor.Black;
                case PlayerColor.Black:
                    return PlayerColor.White;
                default:
                    return default;
            }
        }

        public void ResetChessboard()
        {
            _chessboard.Reset();
        }

        public Piece GetPieceAt(Cell cell)
        {
            return _chessboard[cell];
        }

        public IEnumerable<Piece> GetAllPieces()
        {
            var pieces = new List<Piece>();

            foreach (var piece in _chessboard.StartBoard)
                if (piece != null)
                    pieces.Add(piece);

            return pieces;
        }

        private bool IsCheck(PlayerColor player)
        {
            Piece prince = _chessboard.GetPrince(player);
            // If prince is alive there is no check
            if (prince.Cell != null)
                return false;

            var opponentPlayerAvailableCells = new List<Cell>();
            foreach (Piece piece in _chessboard.Board)
            {
                // If piece is opponent's piece
                if (piece != null && piece.Color != player)
                    opponentPlayerAvailableCells.AddRange(piece.GetAvailableCells());
            }

            Piece king = _chessboard.GetKing(player);
            return opponentPlayerAvailableCells.Contains(king.Cell);
        }

        private bool IsStalemate(PlayerColor player)
        {
            var currentPlayerAvailableCells = new List<Cell>();
            foreach (Piece piece in _chessboard.Board)
            {
                // If piece is a friendly piece
                if (piece != null && piece.Color == player)
                {
                    piece.HasToRaiseOnCellChangeEvent = false;

                    SaveChessboard();
                    foreach (Cell cell in piece.GetAvailableCells())
                    {
                        MakeMove(piece, cell);
                        if (!IsCheck(player))
                            currentPlayerAvailableCells.Add(cell);
                        RestoreChessboard();
                    }

                    piece.HasToRaiseOnCellChangeEvent = true;
                }
            }
            
            return currentPlayerAvailableCells.Count == 0;
        }

        private bool IsCheckmate(PlayerColor player)
        {
            if (!IsCheck(player))
                return false;

            return IsStalemate(player);
        }

        private bool IsSeizure(PlayerColor player)
        {
            Piece king = _chessboard.GetKing(player);
            return king.Cell == Cell.Throne;
        }

        private bool IsIntronisation(PlayerColor player)
        {
            // If king didn't seize the throne, intronisation is not possible
            if (!IsSeizure(player))
                return false;

            bool isIntronisation = true;
            foreach (Piece piece in _chessboard.Board)
            {
                // If piece is opponent's piece
                if (piece != null && piece.Color != player)
                {
                    piece.HasToRaiseOnCellChangeEvent = false;

                    SaveChessboard();
                    foreach (Cell cell in piece.GetAvailableCells())
                    {
                        MakeMove(piece, cell);
                        if (IsOverthrowing(player))
                            isIntronisation = false;
                        RestoreChessboard();
                    }

                    piece.HasToRaiseOnCellChangeEvent = true;
                }
            }

            return isIntronisation;
        }

        private bool IsOverthrowing(PlayerColor player)
        {
            // If opponent's king didn't seize the throne, overthrowing is not possible
            if (!IsSeizure(player))
                return false;

            var opponentPlayerAvailableCells = new List<Cell>();
            foreach (Piece piece in _chessboard.Board)
            {
                // If piece is opponent's piece
                if (piece != null && piece.Color != player)
                    opponentPlayerAvailableCells.AddRange(piece.GetAvailableCells());
            }

            Piece king = _chessboard.GetKing(player);
            return opponentPlayerAvailableCells.Contains(king.Cell);
        }

        private void SaveChessboard()
        {
            for (int i = 0; i < Chessboard.Length; i++)
            {
                for (int j = 0; j < Chessboard.Length; j++)
                {
                    var cell = new Cell(i, j);
                    _savedChessboard[cell] = _chessboard[cell];
                }
            }
        }

        private void RestoreChessboard()
        {
            for (int i = 0; i < Chessboard.Length; i++)
            {
                for (int j = 0; j < Chessboard.Length; j++)
                {
                    var cell = new Cell(i, j);
                    _chessboard[cell] = _savedChessboard[cell];
                }
            }
        }
    }
}
