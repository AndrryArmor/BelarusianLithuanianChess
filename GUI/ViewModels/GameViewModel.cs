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
        private readonly GameController _gameController;
        private readonly GameWindow _gameWindow;
        private readonly List<PieceViewModel> _pieces;
        private readonly Timer _oneSecondTimer = new Timer { Interval = 1000 };
        private readonly List<HighlightViewModel> _highlights = new List<HighlightViewModel>();
        private PieceViewModel _choosedPieceViewModel;
        private Cell _cellToMakeMove;
        private string _whitePlayerState;
        private string _blackPlayerState;
        private TimeSpan _time;
        private bool _isNewGameButtonEnabled;
        private bool _isFinishGameButtonEnabled;
        private RelayCommand _startGameCommand;
        private RelayCommand _finishGameCommand;
        private RelayCommand _findAvailableCellsCommand;
        private RelayCommand _makeMoveCommand;

        public GameViewModel(GameController gameController, GameWindow gameWindow)
        {
            _gameController = gameController;
            _gameWindow = gameWindow;

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

        public RelayCommand StartGameCommand
        {
            get
            {
                return _startGameCommand ?? (_startGameCommand = new RelayCommand(obj =>
                {
                    _gameController.StartGame();

                    var whitePlayerState = _gameController.WhitePlayerState;
                    var blackPlayerState = _gameController.BlackPlayerState;
                    WhitePlayerState = Enum.GetName(whitePlayerState.GetType(), whitePlayerState);
                    BlackPlayerState = Enum.GetName(blackPlayerState.GetType(), blackPlayerState);

                    Time = TimeSpan.Zero;
                    IsNewGameButtonEnabled = false;
                    IsFinishGameButtonEnabled = true;
                    _oneSecondTimer.Start();
                }));
            }
        }

        public RelayCommand FindAvailableCellsCommand
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

        public RelayCommand MakeMoveCommand
        {
            get
            {
                return _makeMoveCommand ?? (_makeMoveCommand = new RelayCommand(obj =>
                {
                    _gameController.MakeMove(_choosedPieceViewModel.Piece, _cellToMakeMove);
                    WhitePlayerState = _gameController.WhitePlayerState.ToString();
                    BlackPlayerState = _gameController.BlackPlayerState.ToString();
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

        public RelayCommand FinishGameCommand
        {
            get
            {
                return _finishGameCommand ?? (_finishGameCommand = new RelayCommand(obj =>
                {
                    _gameController.FinishGame();

                    IsNewGameButtonEnabled = true;
                    IsFinishGameButtonEnabled = false;
                    _oneSecondTimer.Stop();
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
