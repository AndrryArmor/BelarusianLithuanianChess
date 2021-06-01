using BelarusChess.Core.Logic.NetUtils;
using BelarusChess.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BelarusChess.UI.ViewModels
{
    public class NetworkConnectionViewModel : ViewModelBase
    {
        private readonly NetworkConnectionWindow _networkConnectionWindow;
        private readonly LocalNetworkGameViewModel _gameViewModel;
        private RelayCommand _createGameCommand;
        private RelayCommand _joinGameCommand;
        private bool isCreateGameButtonEnabled;
        private bool isJoinGameButtonEnabled;
        private string createGameStatus;
        private string serverNameToJoin;

        public NetworkConnectionViewModel(LocalNetworkGameViewModel gameViewModel, NetworkConnectionWindow networkConnectionWindow)
        {
            _networkConnectionWindow = networkConnectionWindow;
            _gameViewModel = gameViewModel;

            IsGroupBoxCreateGameEnabled = true;
            IsGroupBoxJoinGameEnabled = true;
        }

        public bool IsGroupBoxCreateGameEnabled
        {
            get => isCreateGameButtonEnabled;
            set
            {
                isCreateGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsGroupBoxJoinGameEnabled
        {
            get => isJoinGameButtonEnabled;
            set
            {
                isJoinGameButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public string CreateGameStatus
        {
            get => createGameStatus;
            set
            {
                createGameStatus = value;
                OnPropertyChanged();
            }
        }
        public string ServerNameToJoin
        {
            get => serverNameToJoin;
            set
            {
                serverNameToJoin = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CreateGameCommand
        {
            get
            {
                return _createGameCommand ?? (_createGameCommand = new RelayCommand(obj =>
                {
                    IsGroupBoxJoinGameEnabled = false;

                    GameServer gameServer = new GameServer();

                    CreateGameStatus = "Сервер створено. Очікуємо на гравця...";

                    Task.Run(async () =>
                    {
                        while (gameServer.ListenForHandshake() != SocketError.Success)
                        {
                        }

                        _gameViewModel.GameEndPoint = gameServer;
                        await _networkConnectionWindow.Dispatcher.InvokeAsync(() =>
                        {
                            CreateGameStatus = $"Клієнт {gameServer.Socket.RemoteEndPoint} успішно приєднався до гри";
                            
                        });

                        await _networkConnectionWindow.Dispatcher.InvokeAsync(() =>
                        {
                            Thread.Sleep(1000);
                            _networkConnectionWindow.DialogResult = true;
                        });
                    });
                }));
            }
        }

        public RelayCommand JoinGameCommand
        {
            get
            {
                return _joinGameCommand ?? (_joinGameCommand = new RelayCommand(obj =>
                {
                    IsGroupBoxCreateGameEnabled = false;

                    GameClient gameClient = new GameClient();

                    Task.Run(() =>
                    {
                        switch (gameClient.HandshakeGameHost(ServerNameToJoin))
                        {
                            case SocketError.Success:
                                App.ShowMessage($"Клієнт успішно під'єднався до сервера {ServerNameToJoin}!");

                                _gameViewModel.GameEndPoint = gameClient;
                                _networkConnectionWindow.Dispatcher.InvokeAsync(() =>
                                {
                                    _networkConnectionWindow.DialogResult = true;
                                });
                                break;
                            default:
                                App.ShowMessage($"Помилка: з'єднання не вдалося встановити. Перевірте ім'я сервера і спробуйте ще раз.");
                                break;
                        }
                    });
                }));
            }
        }
    }
}
