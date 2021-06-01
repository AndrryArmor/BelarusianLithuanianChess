using System.Windows;

namespace BelarusChess.UI.Views
{
    /// <summary> Interaction logic for HelpWindow.xaml </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
