using BelarusChess.UI;
using BelarusChess.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelarusChess.UI.ViewModels
{
    public class EnterWindowViewModel : ViewModelBase
    {
        private RelayCommand _start1PCGameCommand;
        private RelayCommand _startLocalNetworkGameCommand;
        private RelayCommand _helpCommand;

        public EnterWindowViewModel()
        {
        }

        #region Commands

        public RelayCommand Start1PCGameCommand
        {
            get
            {
                return _start1PCGameCommand ?? (_start1PCGameCommand = new RelayCommand(obj =>
                {
                    new GameWindow(GamePlayMode.OnePC).Show();
                    var enterWindow = obj as EnterWindow;
                    enterWindow.Close();
                }));
            }
        }

        public RelayCommand StartLocalNetworkGameCommand
        {
            get
            {
                return _startLocalNetworkGameCommand ?? (_startLocalNetworkGameCommand = new RelayCommand(obj =>
                {
                    new GameWindow(GamePlayMode.LocalNetwork).Show();
                    var enterWindow = obj as EnterWindow;
                    enterWindow.Close();
                }));
            }
        }

        public RelayCommand HelpCommand
        {
            get
            {
                return _helpCommand ?? (_helpCommand = new RelayCommand(obj =>
                {   
                    new HelpWindow().Show();
                }));
            }
        }

        #endregion
    }
}
