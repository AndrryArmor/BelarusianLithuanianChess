using BelarusChess.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.GUI.ViewModels
{
    public class NetworkConnectionViewModel : ViewModelBase
    {
        private RelayCommand _createGameCommand;
        private RelayCommand _joinGameCommand;

        public bool IsCreateGameButtonEnabled { get; set; } = true;
        public bool IsJoinGameButtonEnabled { get; set; } = true;
        public string CreateGameMessage { get; set; }
        public string JoinGameMessage { get; set; }

        public RelayCommand CreateGameCommand
        {
            get
            {
                return _createGameCommand ?? (_createGameCommand = new RelayCommand(obj =>
                {
                    IsJoinGameButtonEnabled = false;

                }));
            }
        }

        public RelayCommand JoinGameCommand
        {
            get
            {
                return _joinGameCommand ?? (_joinGameCommand = new RelayCommand(obj =>
                {
                    IsCreateGameButtonEnabled = false;
                }));
            }
        }
    }
}
