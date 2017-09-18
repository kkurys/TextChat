using Chat_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Chat_GUI.ViewModels
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        private IConnection _connection;
        private ObservableCollection<string> _chatHistory;
        private ObservableCollection<string> _log;
        private ObservableCollection<ConnectedUser.ConnectedUser> _connectedUsers;
        static readonly object _lock = new object();
        private bool _connected;
        private PrivateMessage lastPrivateMessage;
        public PrivateMessage LastPrivateMessage
        {
            get
            {
                return lastPrivateMessage;
            }
            set
            {
                lastPrivateMessage = value;
                OnPropertyChanged("LastPrivateMessage");
            }
        }
        private ConversationsViewModel _privateConvs;
        private PrivateConversationsWindow _privateWindow;
        public ConversationsViewModel PrivateConversations
        {
            get
            {
                return _privateConvs;
            }
        }
        private List<PrivateConversationViewModel> _conversations;
        public ConnectionViewModel()
        {
            _chatHistory = new ObservableCollection<string>();
            _log = new ObservableCollection<string>();
            _connectedUsers = new ObservableCollection<ConnectedUser.ConnectedUser>();
            _privateConvs = new ConversationsViewModel();

        }
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                OnPropertyChanged("Connected");
            }
        }
        public ObservableCollection<string> ChatHistory
        {
            get
            {
                return _chatHistory;
            }
        }
        public ObservableCollection<string> Log
        {
            get
            {
                return _log;
            }
        }
        public ObservableCollection<ConnectedUser.ConnectedUser> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
        }
        int myId = -1;
        private ConnectedUser.ConnectedUser GetUserById(int id)
        {
            foreach (var user in ConnectedUsers)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }
            return null;
        }
        public string Ip { get; set; } = "";
        public int Port { get; set; } = -1;
        public string Username { get; set; }
        public bool Connect(string ip, int port, string username)
        {
            _connection = new Connection();
            Ip = ip;
            Port = port;
            Username = username;
            try
            {
                _chatHistory.Add("Próba połączenia z: " + ip + "...");
                _connection.Connect(ip, port, username);
                _chatHistory.Add("Połączono z: " + ip + " jako " + username + "!");
                BinaryFormatter formatter = new BinaryFormatter();
                myId = int.Parse(formatter.Deserialize(_connection.GetStream()).ToString());
                Connected = true;
                return true;
            }

            catch
            {
                _chatHistory.Add("Próba połączenia z: " + ip + " nie powiodła się!");
                Connected = false;
                OnPropertyChanged("Log");
                return false;
            }
        }
        public bool Reconnect()
        {
            try
            {
                _chatHistory.Add("Próba połączenia z: " + Ip + "...");
                _connection = new Connection();
                _connection.Connect(Ip, Port, Username);
                _chatHistory.Add("Połączono z: " + Ip + "!");
                Connected = true;
                OnPropertyChanged("Log");
                Work();
                return true;
            }

            catch
            {
                _chatHistory.Add("Próba połączenia z: " + Ip + " nie powiodła się!");
                Connected = false;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");
                return false;
            }
        }



        public void Disconnect()
        {
            if (_connected)
            {
                _connection.Disconnect();
                Connected = false;
                ConnectedUsers.Clear();
                ChatHistory.Add("Rozłączono");
            }

        }
        public void ReceiveMessages()
        {
            _conversations = new List<PrivateConversationViewModel>();
            try
            {
                var _connectionStream = _connection.GetStream();
                BinaryReader reader = new BinaryReader(_connectionStream);
                var serializer = new BinaryFormatter();
                while (true)
                {
                    var msg = serializer.Deserialize(_connectionStream);
                    if (msg == null) continue;
                    if (msg is string)
                    {
                        string _msg = msg as string;
                        if (_msg != "")
                        {
                            lock (_lock)
                            {

                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    _chatHistory.Add(_msg);
                                });

                                OnPropertyChanged("ChatHistory");
                            }
                        }
                    }
                    else if (msg is List<ConnectedUser.ConnectedUser>)
                    {
                        var _serverConnectedUsers = msg as List<ConnectedUser.ConnectedUser>;


                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            _connectedUsers.Clear();
                        });
                        foreach (var _user in _serverConnectedUsers)
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                _connectedUsers.Add(_user);
                            });
                        }

                        OnPropertyChanged("ConnectedUsers");
                    }
                    else if (msg is int)
                    {
                        int msgValue = int.Parse(msg.ToString());
                        if (msgValue == -1)
                        {
                            lock (_lock)
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    ChatHistory.Add("Server has been stopped!");
                                });

                                OnPropertyChanged("Log");
                            }
                            _connected = false;
                        }
                    }
                    else if (msg is PrivateMessage)
                    {
                        var _msg = msg as PrivateMessage;
                        if (_msg.UserIDTo == myId)
                        {
                            var conv = _conversations.Find(x => x.Partner.Id == _msg.UserIDFrom);
                            if (conv == null)
                            {
                                var newPrivate = new PrivateConversationViewModel(GetUserById(_msg.UserIDFrom), this);
                                lock (_lock)
                                {
                                    _conversations.Add(newPrivate);
                                }

                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    PrivateConversations.Add(newPrivate);
                                    newPrivate.IsActive = true;
                                    newPrivate.Add(GetUserById(_msg.UserIDFrom).Nickname + ": " + _msg.Content);
                                    if (_privateWindow == null)
                                    {
                                        _privateWindow = new PrivateConversationsWindow(PrivateConversations);
                                        _privateWindow.Show();
                                    }

                                });
                            }
                            else
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    conv.Add(GetUserById(_msg.UserIDFrom).Nickname + ": " + _msg.Content);

                                    if (_privateWindow == null)
                                    {
                                        _privateWindow = new PrivateConversationsWindow(PrivateConversations);
                                        _privateWindow.Show();
                                    }
                                });

                            }
                        }
                        else
                        {
                            var conv = _conversations.Find(x => x.Partner.Id == _msg.UserIDTo);
                            if (conv == null)
                            {
                                var user = GetUserById(_msg.UserIDTo);
                                if (user == null) continue;
                                var newPrivate = new PrivateConversationViewModel(user, this);
                                newPrivate.IsActive = true;
                                conv = newPrivate;

                                lock (_lock)
                                {
                                    _conversations.Add(conv);
                                }

                            }
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                //  PrivateConversations.Add(conv);
                                conv.Add(GetUserById(_msg.UserIDFrom).Nickname + ": " + _msg.Content);
                            });
                        }

                    }
                }
            }
            catch
            {
                //  _connected = false;
                throw;
                OnPropertyChanged("Connected");
            }

        }
        public void AddConversation(ConnectedUser.ConnectedUser _targetUser)
        {
            _privateConvs.Add(new PrivateConversationViewModel(_targetUser, this));
            _conversations.Add(_privateConvs[0]);
            if (_privateWindow == null)
            {
                _privateWindow = new PrivateConversationsWindow(_privateConvs);
            }
            _privateWindow.Show();
        }


        internal void SendPrivateMessage(int id, string msg)
        {
            try
            {
                PrivateMessage pm = new PrivateMessage();
                pm.UserIDTo = id;
                pm.Content = msg;
                BinaryFormatter writer = new BinaryFormatter();
                try
                {
                    writer.Serialize(_connection.GetStream(), pm);
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }

        }
        public void SendMessage(string msg)
        {
            BinaryFormatter writer = new BinaryFormatter();
            try
            {
                writer.Serialize(_connection.GetStream(), Username + ": " + msg);
            }
            catch
            {
                return;
            }

        }
        public void Work()
        {
            (new Thread(ReceiveMessages)).Start();
        }
        public bool OpenInNewTab
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
