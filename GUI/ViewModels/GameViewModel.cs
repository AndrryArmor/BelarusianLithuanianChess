using BelarusChess.Core.Entities;
using BelarusChess.Core.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using BelarusChess.Core.Entities.Pieces;
using BelarusChess.UI.Views;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BelarusChess.UI.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        protected readonly GameWindow _gameWindow;
        protected readonly GameController _gameController;
        private readonly List<PieceViewModel> _pieces;
        protected readonly Timer _oneSecondTimer = new Timer { Interval = 1000 };
        protected readonly List<HighlightViewModel> _highlights = new List<HighlightViewModel>();
        protected PieceViewModel _choosedPieceViewModel;
        protected Cell _cellToMakeMove;
        private string _whitePlayerState;
        private string _blackPlayerState;
        private string _whitePlayerHighlight;
        private string _blackPlayerHighlight;
        private TimeSpan _time;
        private bool _isNewGameButtonEnabled;
        private bool _isFinishGameButtonEnabled;
        private RelayCommand _startGameCommand;
        private RelayCommand _finishGameCommand;
        private RelayCommand _findAvailableCellsCommand;
        private RelayCommand _makeMoveCommand;

        public GameViewModel(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _gameController = new GameController(new ChessEngine(new Chessboard()));

            _pieces = new List<PieceViewModel>(_gameController.GetAllPieces()
                .Select(piece => new PieceViewModel(piece)));
            _pieces.ForEach(pieceViewModel =>
            {
                pieceViewModel.OnImageClicked += PieceViewModel_OnImageClicked;
                _gameWindow.grid.Children.Add(pieceViewModel.Image);
            });

            _isNewGameButtonEnabled = true;
            _isFinishGameButtonEnabled = false;
            _oneSecondTimer.Elapsed += OneSecond_Elapsed;
        }

        public string WhitePlayerState
        {
            get => _whitePlayerState;
            set
            {
                _whitePlayerState = value;
                OnPropertyChanged();
            }
        }
        public string BlackPlayerState
        {
            get => _blackPlayerState;
            set
            {
                _blackPlayerState = value;
                OnPropertyChanged();
            }
        }
        public string WhitePlayerHighlight
        {
            get => _whitePlayerHighlight;
            set
            {
                _whitePlayerHighlight = value;
                OnPropertyChanged();
            }
        }
        public string BlackPlayerHighlight
        {
            get => _blackPlayerHighlight;
            set
            {
                _blackPlayerHighlight = value;
                OnPropertyChanged();
            }
        }
        public TimeSpan Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        public bool IsNewGameButtonEnabled
        {
            get => _isNewGameButtonEnabled;
            set
            {
                _isNewGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsFinishGameButtonEnabled
        {
            get => _isFinishGameButtonEnabled;
            set
            {
                _isFinishGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public virtual RelayCommand StartGameCommand
        {
            get
            {
                return _startGameCommand ?? (_startGameCommand = new RelayCommand(obj =>
                {
                    _gameController.StartGame();

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

                    Time = TimeSpan.Zero;
                    IsNewGameButtonEnabled = false;
                    IsFinishGameButtonEnabled = true;
                    _oneSecondTimer.Start();
                }));
            }
        }

        public virtual RelayCommand FindAvailableCellsCommand
        {
            get
            {
                return _findAvailableCellsCommand ?? (_findAvailableCellsCommand = new RelayCommand(obj =>
                {
                    Piece choosedPiece = _choosedPieceViewModel.Piece;
                    IEnumerable<Cell> availableCells = _gameController.FindAvailableCells(choosedPiece);

                    _highlights.Add(new HighlightViewModel(HighlightType.ChoosedPiece, choosedPiece.Cell));
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

        public virtual RelayCommand MakeMoveCommand
        {
            get
            {
                return _makeMoveCommand ?? (_makeMoveCommand = new RelayCommand(obj =>
                {
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

                    if (_gameController.WhitePlayerState == PlayerState.Checkmate || 
                        _gameController.WhitePlayerState == PlayerState.ThroneMine ||
                        _gameController.BlackPlayerState == PlayerState.Checkmate || 
                        _gameController.BlackPlayerState == PlayerState.ThroneMine)
                    {
                        FinishGameCommand.Execute(null);
                    }
                }));
            }
        }

        public virtual RelayCommand FinishGameCommand
        {
            get
            {
                return _finishGameCommand ?? (_finishGameCommand = new RelayCommand(obj =>
                {
                    var messageAnswer = App.ShowMessage("Справді завершити гру?", true);
                    if (messageAnswer == MessageBoxResult.Yes)
                    {
                        _gameController.FinishGame();

                        IsNewGameButtonEnabled = true;
                        IsFinishGameButtonEnabled = false;
                        _oneSecondTimer.Stop();
                    }
                }));
            }
        }

        #endregion

        private void PieceViewModel_OnImageClicked(object sender, EventArgs e)
        {
            if (_choosedPieceViewModel == null)
            {
                _choosedPieceViewModel = (PieceViewModel)sender;
                FindAvailableCellsCommand.Execute(null);
            }
            else
            {
                _choosedPieceViewModel = null;
                _highlights.ForEach(highlight => _gameWindow.grid.Children.Remove(highlight.Image));
                _highlights.Clear();
            }
        }

        private void HighlightViewModel_OnImageClicked(object sender, EventArgs e)
        {
            _cellToMakeMove = ((HighlightViewModel)sender).Cell;
            MakeMoveCommand.Execute(null);
        }

        private void OneSecond_Elapsed(object sender, ElapsedEventArgs e)
        {
            long second = TimeSpan.TicksPerSecond;
            Time += new TimeSpan(second);
        }
    }
}
