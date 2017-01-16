using Chat_GUI.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        ConnectionDataViewModel _connectionData;
        public ConnectionWindow(ConnectionDataViewModel connectionData)
        {
            InitializeComponent();

            _connectionData = connectionData;
            gridMain.DataContext = _connectionData;
        }
        public ConnectionDataViewModel GetConnectionData()
        {
            return _connectionData != null ? _connectionData : null;
        }
        #region commands
        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Regex ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            int port;
            bool result = int.TryParse(tbPort.Text, out port);
            if (tbServerAddress.Text != "" && ipRegex.Match(tbServerAddress.Text).Success && result && tbUsername.Text != "")
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
            int parameter = 0;
            bool result = false;
            if (e.Parameter != null)
            {
                result = int.TryParse(e.Parameter.ToString(), out parameter);
            }
            _connectionData = new ConnectionDataViewModel()
            {
                Ip = tbServerAddress.Text,
                Port = int.Parse(tbPort.Text),
                Username = tbUsername.Text
            };
            if (result && parameter == 1)
            {
                _connectionData.OpenInTab = true;
            }
            this.DialogResult = true;
            Close();
        }

        #endregion
    }
}
