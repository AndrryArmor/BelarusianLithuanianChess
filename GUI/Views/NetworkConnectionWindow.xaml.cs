using BelarusChess.Core.Logic.NetUtils;
using BelarusChess.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BelarusChess.UI.Views
{
    /// <summary>
    /// Interaction logic for NetworkConnectionWindow.xaml
    /// </summary>
    public partial class NetworkConnectionWindow : Window
    {
        public NetworkConnectionWindow(LocalNetworkGameViewModel gameViewModel)
        {
            InitializeComponent();
            DataContext = new NetworkConnectionViewModel(gameViewModel, this);
        }
    }
}
