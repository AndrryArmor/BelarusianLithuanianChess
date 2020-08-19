using BelarusChess.Core.Entities.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BelarusChess.Core.Entities
{
    public class Chessboard
    {
        public static readonly int Length = 9;

        private Piece _whiteKing;
        private Piece _blackKing;
        private Piece _whitePrince;
        private Piece _blackPrince;
        
        public Chessboard()
        {
            #region Black pieces initialisation

            StartBoard[0, 0] = new Rook(PlayerColor.Black, new Cell(0, 0), this);
            StartBoard[0, 1] = new Knight(PlayerColor.Black, new Cell(0, 1), this);
            StartBoard[0, 2] = new Bishop(PlayerColor.Black, new Cell(0, 2), this);
            StartBoard[0, 3] = new Queen(PlayerColor.Black, new Cell(0, 3), this);
            StartBoard[0, 4] = new King(PlayerColor.Black, new Cell(0, 4), this);
            StartBoard[0, 5] = new Prince(PlayerColor.Black, new Cell(0, 5), this);
            StartBoard[0, 6] = new Knight(PlayerColor.Black, new Cell(0, 6), this);
            StartBoard[0, 7] = new Bishop(PlayerColor.Black, new Cell(0, 7), this);
            StartBoard[0, 8] = new Rook(PlayerColor.Black, new Cell(0, 8), this);

            StartBoard[1, 0] = new Pawn(PlayerColor.Black, new Cell(1, 0), this);
            StartBoard[1, 1] = new Pawn(PlayerColor.Black, new Cell(1, 1), this);
            StartBoard[1, 2] = new Pawn(PlayerColor.Black, new Cell(1, 2), this);
            StartBoard[1, 3] = new Pawn(PlayerColor.Black, new Cell(1, 3), this);
            StartBoard[1, 4] = new Pawn(PlayerColor.Black, new Cell(1, 4), this);
            StartBoard[1, 5] = new Pawn(PlayerColor.Black, new Cell(1, 5), this);
            StartBoard[1, 6] = new Pawn(PlayerColor.Black, new Cell(1, 6), this);
            StartBoard[1, 7] = new Pawn(PlayerColor.Black, new Cell(1, 7), this);
            StartBoard[1, 8] = new Pawn(PlayerColor.Black, new Cell(1, 8), this);

            #endregion

            #region White pieces initialisation

            StartBoard[7, 0] = new Pawn(PlayerColor.White, new Cell(7, 0), this);
            StartBoard[7, 1] = new Pawn(PlayerColor.White, new Cell(7, 1), this);
            StartBoard[7, 2] = new Pawn(PlayerColor.White, new Cell(7, 2), this);
            StartBoard[7, 3] = new Pawn(PlayerColor.White, new Cell(7, 3), this);
            StartBoard[7, 4] = new Pawn(PlayerColor.White, new Cell(7, 4), this);
            StartBoard[7, 5] = new Pawn(PlayerColor.White, new Cell(7, 5), this);
            StartBoard[7, 6] = new Pawn(PlayerColor.White, new Cell(7, 6), this);
            StartBoard[7, 7] = new Pawn(PlayerColor.White, new Cell(7, 7), this);
            StartBoard[7, 8] = new Pawn(PlayerColor.White, new Cell(7, 8), this);

            StartBoard[8, 0] = new Rook(PlayerColor.White, new Cell(8, 0), this);
            StartBoard[8, 1] = new Bishop(PlayerColor.White, new Cell(8, 1), this);
            StartBoard[8, 2] = new Knight(PlayerColor.White, new Cell(8, 2), this);
            StartBoard[8, 3] = new Prince(PlayerColor.White, new Cell(8, 3), this);
            StartBoard[8, 4] = new King(PlayerColor.White, new Cell(8, 4), this);
            StartBoard[8, 5] = new Queen(PlayerColor.White, new Cell(8, 5), this);
            StartBoard[8, 6] = new Bishop(PlayerColor.White, new Cell(8, 6), this);
            StartBoard[8, 7] = new Knight(PlayerColor.White, new Cell(8, 7), this);
            StartBoard[8, 8] = new Rook(PlayerColor.White, new Cell(8, 8), this);

            #endregion

            _whiteKing = StartBoard[8, 4];
            _blackKing = StartBoard[0, 4];
            _whitePrince = StartBoard[8, 3];
            _blackPrince = StartBoard[0, 5];

            Reset();
        }

        public event EventHandler<PlayerColor> OnInauguration;

        public Piece[,] StartBoard { get; } = new Piece[Length, Length];
        public Piece[,] Board { get; } = new Piece[Length, Length];

        public Piece this[Cell cell]
        {
            get => cell.IsValid
                ? Board[cell.Row, cell.Col]
                : null;
            set
            {
                Board[cell.Row, cell.Col] = value;
                if (value != null)
                    value.Cell = cell;
            }
        }

        public Piece GetKing(PlayerColor color)
        {
            Piece king = null;
            switch (color)
            {
                case PlayerColor.White:
                    king = _whiteKing;
                    break;
                case PlayerColor.Black:
                    king = _blackKing;
                    break;
            }
            return king;
        }

        public Piece GetPrince(PlayerColor color)
        {
            Piece prince = null;
            switch (color)
            {
                case PlayerColor.White:
                    prince = _whitePrince;
                    break;
                case PlayerColor.Black:
                    prince = _blackPrince;
                    break;
            }
            return prince;
        }

        public bool IsKingAlone(PlayerColor color)
        {
            Piece prince = GetPrince(color);
            return prince.Cell == null;
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            Piece beatenPiece = this[cell];

            if (beatenPiece != null)
            {
                beatenPiece.Cell = null;

                if (beatenPiece.Type == PieceType.King && !IsKingAlone(beatenPiece.Color) && 
                    beatenPiece.HasToRaiseOnCellChangeEvent)
                {
                    DoInauguration(beatenPiece.Color);
                }
            }

            this[piece.Cell] = null;
            this[cell] = piece;
        }

        public void Reset()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                    this[new Cell(i, j)] = StartBoard[i, j];
            }
        }

        private void DoInauguration(PlayerColor player)
        {
            Piece prince = GetPrince(player);
            Piece king = GetKing(player);

            this[prince.Cell] = king;
            prince.Cell = null;

            OnInauguration?.Invoke(this, player);
        }
    }
}