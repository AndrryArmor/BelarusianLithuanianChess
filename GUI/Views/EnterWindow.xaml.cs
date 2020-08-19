using System.Windows;

namespace BelarusChess.UI.Views
{
    /// <summary> Interaction logic for EnterWindow.xaml </summary>
    public partial class EnterWindow : Window
    {
        private HelpWindow _helpWindow;

        public EnterWindow()
        {
            InitializeComponent();
        }

        private void ButtonGame_Click(object sender, RoutedEventArgs e)
        {
            new GameWindow().Show();
            Close();
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            if (_helpWindow == null)
                _helpWindow = new HelpWindow();
            _helpWindow.Show();
        }
    }
}
