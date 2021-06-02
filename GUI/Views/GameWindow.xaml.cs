using BelarusChess.Core.Entities;
using BelarusChess.Core.Logic;
using BelarusChess.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BelarusChess.UI.Views
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class GameWindow : Window
    {
        public GameWindow(GamePlayMode gamePlayMode)
        {
            InitializeComponent();

            switch (gamePlayMode)
            {
                case GamePlayMode.OnePC:
                    DataContext = new GameViewModel(this);
                    break;
                case GamePlayMode.LocalNetwork:
                    DataContext = new LocalNetworkGameViewModel(this);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = App.ShowMessage("Справді завершити гру?", true);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}