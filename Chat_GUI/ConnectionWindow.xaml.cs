using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Chat_GUI.ViewModels;
namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        ConnectionDataViewModel _connectionData;
        public ConnectionWindow()
        {
            InitializeComponent();
        }
        #region commands
        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Regex ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            if (tbServerAddress.Text != "" && ipRegex.Match(tbServerAddress.Text).Success)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int? parameter = e.Parameter as int?;
            _connectionData = new ConnectionDataViewModel()
            {
                Ip = tbServerAddress.Text,
                Port = int.Parse(tbPort.Text),
                Username = tbUsername.Text
            };
            if (parameter.HasValue && parameter.Value == 1)
            {
                _connectionData.OpenInTab = true;
            }
        }

        #endregion
    }
}
