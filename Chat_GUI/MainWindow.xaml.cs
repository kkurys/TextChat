using System.Windows;

namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenConnectionWindow(object sender, RoutedEventArgs e)
        {
            ConnectionWindow _connectionWindow = new ConnectionWindow();

            _connectionWindow.ShowDialog();

            if (_connectionWindow.DialogResult.Value)
            {

            }
        }
    }
}
