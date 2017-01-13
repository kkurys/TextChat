using Chat_GUI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Chat_GUI.ViewModels
{
    class ConnectionViewModel : INotifyPropertyChanged
    {
        private IConnection _connection;
        private ObservableCollection<string> _chatHistory;
        private ObservableCollection<string> _log;
        private ObservableCollection<ConnectedUser> _connectedUsers;
        static readonly object _lock = new object();
        private bool _connected;
        public ConnectionViewModel()
        {
            _chatHistory = new ObservableCollection<string>();
            _log = new ObservableCollection<string>();
            _connectedUsers = new ObservableCollection<ConnectedUser>();
        }
        public bool Connected
        {
            get
            {
                return _connected;
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
        public ObservableCollection<ConnectedUser> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
        }
        public void Connect(string ip, int port)
        {
            _connection = new Connection();
            try
            {
                _log.Add("Próba połączenia z: " + ip + "...");
                _connection.Connect(ip, port);
                _connected = true;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");

            }

            catch
            {
                _log.Add("Próba połączenia z: " + ip + " nie powiodła się!");
                _connected = false;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");
            }
        }

        public void ReceiveMessages()
        {
            try
            {
                var _connectionStream = _connection.GetStream();
                BinaryReader reader = new BinaryReader(_connectionStream);
                var serializer = new BinaryFormatter();
                while (true)
                {
                    var msg = serializer.Deserialize(_connectionStream);

                    if (msg is string)
                    {
                        string _msg = msg as string;
                        if (_msg != "")
                        {
                            lock (_lock)
                            {
                                _chatHistory.Add(_msg);
                                OnPropertyChanged("ChatHistory");
                            }
                        }
                    }
                    else if (msg is List<ConnectedUser>)
                    {
                        var _serverConnectedUsers = msg as List<ConnectedUser>;

                        _connectedUsers.Clear();
                        foreach (var _user in _serverConnectedUsers)
                        {
                            _connectedUsers.Add(_user);
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
                                _log.Add("Server has been stopped!");
                                OnPropertyChanged("Log");
                            }
                            _connected = false;
                        }
                    }
                }
            }
            catch
            {
                _connected = false;
                return;
            }

        }
        public void SendMessage(string msg)
        {
            BinaryWriter writer = new BinaryWriter(_connection.GetStream());
            try
            {
                writer.Write(Settings.Nickname + ": " + msg);
            }
            catch
            {
                return;
            }

        }
        public void Work(string ip, int port)
        {
            new Thread(ReceiveMessages);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
