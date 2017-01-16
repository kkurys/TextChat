using Chat_GUI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window, INotifyPropertyChanged
    {
        ConnectionDataViewModel _connectionData;
        private bool _inputErrors;
        public bool InputErrors
        {
            get
            {
                return _inputErrors;
            }
            set
            {
                _inputErrors = value;
                OnPropertyChanged("InputErrors");
            }
        }
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
            if (!InputErrors)
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool HasErrors(DependencyObject gridInfo)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(gridInfo))
            {
                TextBox element = child as TextBox;
                if (element == null)
                {
                    continue;
                }
                if (Validation.GetHasError(element) || HasErrors(element))
                {
                    return true;
                }
            }
            return false;
        }
        private void ValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                InputErrors = true;
            }
            else if (!HasErrors(gridMain))
            {
                InputErrors = false;
            }
        }
        #endregion
    }
}
