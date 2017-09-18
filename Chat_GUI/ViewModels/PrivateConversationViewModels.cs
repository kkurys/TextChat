using Chat_GUI.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chat_GUI.ViewModels
{
    public class PrivateConversationViewModel : INotifyPropertyChanged
    {
        public PrivateConversationViewModel(ConnectedUser.ConnectedUser usr, ConnectionViewModel con)
        {
            Partner = usr;
            History = new ObservableCollection<string>();
            Connection = con;
            Connection.PropertyChanged += Connection_PropertyChanged;
            IsActive = true;
        }

        private void Connection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected")
            {
                if (!Connection.Connected)
                {
                    IsActive = false;
                }
            }
            else if (e.PropertyName == "LastPrivateMessage")
            {
                Connection.PrivateConversations.Add(this);
                ReceivePrivateMessage(Connection.LastPrivateMessage);
            }
        }

        private void ReceivePrivateMessage(PrivateMessage msg)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (msg.UserIDFrom != Partner.Id)
                {
                    History.Add(Connection.Username + ": " + msg.Content);
                }
                else
                {
                    History.Add(Partner.Nickname + ": " + msg.Content);
                }

            });
            OnPropertyChanged("History");

        }
        private bool _active;
        public ConnectionViewModel Connection { get; set; }
        public ConnectedUser.ConnectedUser Partner
        {
            get;
            set;
        }
        public ObservableCollection<string> History
        {
            get;
            set;
        }
        public bool IsActive
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                OnPropertyChanged("IsActive");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public string ServerIP
        {
            get;
            set;
        }
        public string Username
        {
            get
            {
                return Partner.Nickname;
            }
        }
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void Add(string msg)
        {
            History.Add(msg);
        }

    }
    public class ConversationsViewModel
    {
        public int ActiveTab { get; set; }
        public ConversationsViewModel()
        {
            Conversations = new ObservableCollection<PrivateConversationViewModel>();

        }
        public ObservableCollection<PrivateConversationViewModel> Conversations { get; set; }

        public void Add(PrivateConversationViewModel privConv)
        {
            foreach (var conv in Conversations)
            {
                if (conv.Partner.Id == privConv.Partner.Id)
                {
                    return;
                }
            }
            Conversations.Add(privConv);
        }


        public PrivateConversationViewModel this[ConnectedUser.ConnectedUser idx]
        {
            get
            {
                foreach (PrivateConversationViewModel vm in Conversations)
                {
                    if (vm.Partner == idx)
                    {
                        return vm;
                    }
                }
                return null;
            }
        }
        public PrivateConversationViewModel this[int idx]
        {
            get
            {
                if (idx >= Conversations.Count) return null;
                return Conversations[idx];
            }
        }
    }
}
