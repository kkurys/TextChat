using Chat_GUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for PrivateConversationsWindow.xaml
    /// </summary>
    public partial class PrivateConversationsWindow : Window
    {
        private ConversationsViewModel _conversations;
        public PrivateConversationsWindow(ConversationsViewModel _cv)
        {
            InitializeComponent();

            _conversations = _cv;
            tcMain.DataContext = _conversations;

        }
        private void Send_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_conversations != null && _conversations[_conversations.ActiveTab] != null && _conversations[_conversations.ActiveTab].IsActive)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void Send_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SendPrivateMessage(_conversations[_conversations.ActiveTab], tbMsg.Text);
            tbMsg.Text = "";
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_conversations[_conversations.ActiveTab].IsActive && tbMsg.Text != "")
                {
                    SendPrivateMessage(_conversations[_conversations.ActiveTab], tbMsg.Text);
                    tbMsg.Text = "";
                }
            }
        }
        private void SendPrivateMessage(PrivateConversationViewModel vm, string msg)
        {
            try
            {
                if (vm.IsActive)
                {
                    vm.Connection.SendPrivateMessage(vm.Partner.Id, msg);
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
