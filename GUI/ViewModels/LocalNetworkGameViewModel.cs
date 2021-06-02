using BelarusChess.Core.Entities;
using BelarusChess.Core.Logic;
using BelarusChess.Core.Logic.NetUtils;
using BelarusChess.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace BelarusChess.UI.ViewModels
{
    public class LocalNetworkGameViewModel : GameViewModel
    {
        private RelayCommand _startGameCommand;
        private RelayCommand _findAvailableCellsCommand;
        private RelayCommand _makeMoveCommand;
        private RelayCommand _finishGameCommand;

        public LocalNetworkGameViewModel(GameWindow gameWindow) : base(gameWindow)
        {
        }

        public GameEndPoint GameEndPoint { get; set; }
        public PlayerColor PlayerColor { get; set; }

        public override RelayCommand StartGameCommand
        {
            get
            {
                return _startGameCommand ?? (_startGameCommand = new RelayCommand(obj =>
                {
                    if (new NetworkConnectionWindow(this).ShowDialog() == true)
                    {
                        _gameController.StartGame();
                        WhitePlayerHighlight = "#FFC8FFC8";
                        BlackPlayerHighlight = "#00000000";

                        if (GameEndPoint is GameServer)
                        {
                            int a = new Random((int)DateTime.Now.Ticks).Next(0, 2);
                            PlayerColor = (PlayerColor)a;
                            switch (PlayerColor)
                            {
                                case PlayerColor.White:
                                    GameEndPoint.Send(PlayerColor.Black);
                                    break;
                                case PlayerColor.Black:
                                    GameEndPoint.Send(PlayerColor.White);
                                    break;
                            }
                        }
                        else
                        {
                            PlayerColor = GameEndPoint.Receive<PlayerColor>();
                        }

                        if (PlayerColor == PlayerColor.Black)
                        {
                            Task.Run(() =>
                            {
                                MoveDto move;
                                try
                                {
                                    move = GameEndPoint.Receive<MoveDto>();
                                }
                                catch (Exception)
                                {
                                    App.ShowMessage("Гравець завершив гру");
                                    _finishGameCommand.Execute(null);
                                    return;
                                }

                                Piece piece = _gameController.GetPieceAt(move.PieceCell);

                                _gameWindow.Dispatcher.InvokeAsync(() =>
                                {
                                    _gameController.MakeMove(piece, move.PieceNewCell);
                                    WhitePlayerState = _gameController.WhitePlayerState.GetName();
                                    BlackPlayerState = _gameController.BlackPlayerState.GetName();
                                    switch (_gameController.CurrentPlayer)
                                    {
                                        case PlayerColor.White:
                                            WhitePlayerHighlight = "#FFC8FFC8";
                                            BlackPlayerHighlight = "#00000000";
                                            break;
                                        case PlayerColor.Black:
                                            WhitePlayerHighlight = "#00000000";
                                            BlackPlayerHighlight = "#FFC8FFC8";
                                            break;
                                    }
                                });

                                if (_gameController.WhitePlayerState == PlayerState.Checkmate ||
                                    _gameController.WhitePlayerState == PlayerState.ThroneMine ||
                                    _gameController.BlackPlayerState == PlayerState.Checkmate ||
                                    _gameController.BlackPlayerState == PlayerState.ThroneMine)
                                {
                                    FinishGameCommand.Execute(null);
                                }
                            });
                        }

                        WhitePlayerState = _gameController.WhitePlayerState.GetName();
                        BlackPlayerState = _gameController.BlackPlayerState.GetName();

                        Time = TimeSpan.Zero;
                        IsNewGameButtonEnabled = false;
                        IsFinishGameButtonEnabled = true;
                        _oneSecondTimer.Start();
                    }
                }));
            }
        }

        public override RelayCommand FindAvailableCellsCommand
        {
            get
            {
                return _findAvailableCellsCommand ?? (_findAvailableCellsCommand = new RelayCommand(obj =>
                {
                    Piece choosedPiece = _choosedPieceViewModel.Piece;
                    IEnumerable<Cell> availableCells = _gameController.FindAvailableCells(choosedPiece);

                    _highlights.Add(new HighlightViewModel(HighlightType.ChoosedPiece, choosedPiece.Cell));
                    if (_gameController.CurrentPlayer != PlayerColor)
                        availableCells = new List<Cell>();

                    _highlights.AddRange(availableCells.Select(cell =>
                    {
                        var highlight = HighlightType.ValidMove;
                        if (_gameController.GetPieceAt(cell) != null)
                            highlight = HighlightType.ValidMoveOnPiece;

                        var highlightViewModel = new HighlightViewModel(highlight, cell);
                        highlightViewModel.OnImageClicked += HighlightViewModel_OnImageClicked;

                        return highlightViewModel;
                    }));

                    _highlights.ForEach(highlight => _gameWindow.grid.Children.Add(highlight.Image));

                }));
            }
        }

        private void HighlightViewModel_OnImageClicked(object sender, EventArgs e)
        {
            _cellToMakeMove = ((HighlightViewModel)sender).Cell;
            MakeMoveCommand.Execute(null);
        }

        public override RelayCommand MakeMoveCommand
        {
            get
            {
                return _makeMoveCommand ?? (_makeMoveCommand = new RelayCommand(async obj =>
                {
                    var pieceCell = new Cell(_choosedPieceViewModel.Piece.Cell.Row, _choosedPieceViewModel.Piece.Cell.Col);
                    var cellToMakeMove = new Cell(_cellToMakeMove.Row, _cellToMakeMove.Col);
                    _gameController.MakeMove(_choosedPieceViewModel.Piece, _cellToMakeMove);

                    WhitePlayerState = _gameController.WhitePlayerState.GetName();
                    BlackPlayerState = _gameController.BlackPlayerState.GetName();
                    switch (_gameController.CurrentPlayer)
                    {
                        case PlayerColor.White:
                            WhitePlayerHighlight = "#FFC8FFC8";
                            BlackPlayerHighlight = "#00000000";
                            break;
                        case PlayerColor.Black:
                            WhitePlayerHighlight = "#00000000";
                            BlackPlayerHighlight = "#FFC8FFC8";
                            break;
                    }
                    _highlights.ForEach(highlight => _gameWindow.grid.Children.Remove(highlight.Image));
                    _highlights.Clear();

                    await Task.Run(() =>
                    {
                        GameEndPoint.Send(new MoveDto
                        {
                            PieceCell = pieceCell,
                            PieceNewCell = cellToMakeMove
                        });
                    });

                    await Task.Run(() =>
                    {
                        if (_gameController.WhitePlayerState == PlayerState.Checkmate ||
                            _gameController.WhitePlayerState == PlayerState.ThroneMine ||
                            _gameController.BlackPlayerState == PlayerState.Checkmate ||
                            _gameController.BlackPlayerState == PlayerState.ThroneMine)
                        {
                            FinishGameCommand.Execute(null);
                        }
                    });

                    await Task.Run(() =>
                    {
                        if (_gameController.IsGameStarted)
                        {
                            MoveDto move;
                            try
                            {
                                move = GameEndPoint.Receive<MoveDto>();
                            }
                            catch (Exception)
                            {
                                App.ShowMessage("Гравець завершив гру");
                                _finishGameCommand.Execute(null);
                                return;
                            }

                            _gameWindow.Dispatcher.InvokeAsync(() =>
                            {
                                _gameController.MakeMove(_gameController.GetPieceAt(move.PieceCell), move.PieceNewCell);
                                WhitePlayerState = _gameController.WhitePlayerState.GetName();
                                BlackPlayerState = _gameController.BlackPlayerState.GetName();
                                switch (_gameController.CurrentPlayer)
                                {
                                    case PlayerColor.White:
                                        WhitePlayerHighlight = "#FFC8FFC8";
                                        BlackPlayerHighlight = "#00000000";
                                        break;
                                    case PlayerColor.Black:
                                        WhitePlayerHighlight = "#00000000";
                                        BlackPlayerHighlight = "#FFC8FFC8";
                                        break;
                                }

                                if (_gameController.WhitePlayerState == PlayerState.Checkmate ||
                                    _gameController.WhitePlayerState == PlayerState.ThroneMine ||
                                    _gameController.BlackPlayerState == PlayerState.Checkmate ||
                                    _gameController.BlackPlayerState == PlayerState.ThroneMine)
                                {
                                    FinishGameCommand.Execute(null);
                                }
                            });
                        }
                    });
                }));
            }
        }

        public override RelayCommand FinishGameCommand
        {
            get
            {
                return _finishGameCommand ?? (_finishGameCommand = new RelayCommand(obj =>
                {
                        _gameController.FinishGame();

                        IsNewGameButtonEnabled = true;
                        IsFinishGameButtonEnabled = false;
                        _oneSecondTimer.Stop();

                        GameEndPoint.CloseConnnection();
                }));
            }
        }
    }
}
