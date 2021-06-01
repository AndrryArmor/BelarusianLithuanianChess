using BelarusChess.UI.ViewModels;
using System.Windows;

namespace BelarusChess.UI.Views
{
    /// <summary> Interaction logic for EnterWindow.xaml </summary>
    public partial class EnterWindow : Window
    {
        public EnterWindow()
        {
            InitializeComponent();
            DataContext = new EnterWindowViewModel();
        }
    }
}
