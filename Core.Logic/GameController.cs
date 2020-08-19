using BelarusChess.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BelarusChess.Core.Logic
{
    public class GameController
    {
        private readonly ChessEngine _engine;

        public GameController(ChessEngine engine)
        {
            _engine = engine;
        }

        public bool IsGameStarted { get; set; }
        public PlayerColor CurrentPlayer { get; private set; }
        public PlayerState WhitePlayerState { get; private set; }
        public PlayerState BlackPlayerState { get; private set; }

        public void StartGame()
        {
            _engine.ResetChessboard();
            IsGameStarted = true;

            CurrentPlayer = PlayerColor.White;
            WhitePlayerState = BlackPlayerState = PlayerState.Regular;
        }

        public IEnumerable<Cell> FindAvailableCells(Piece piece)
        {
            return piece.Color == CurrentPlayer
                ? _engine.FindAvailableCells(piece)
                : new List<Cell>();
        }

        public void MakeMove(Piece piece, Cell cell)
        {
            _engine.MakeMove(piece, cell);
            WhitePlayerState = _engine.GetPlayerState(PlayerColor.White);
            BlackPlayerState = _engine.GetPlayerState(PlayerColor.Black);

            SwitchPlayer();
        }

        public void FinishGame()
        {
            IsGameStarted = false;
        }

        public Piece GetPieceAt(Cell cell)
        {
            return _engine.GetPieceAt(cell);
        }

        public IEnumerable<Piece> GetAllPieces()
        {
            return _engine.GetAllPieces();
        }

        private void SwitchPlayer()
        {
            switch (CurrentPlayer)
            {
                case PlayerColor.White:
                    CurrentPlayer = PlayerColor.Black;
                    break;
                case PlayerColor.Black:
                    CurrentPlayer = PlayerColor.White;
                    break;
            }
        }
    }
}
