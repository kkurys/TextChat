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

        public ConnectionViewModel()
        {
            _chatHistory = new ObservableCollection<string>();
            _log = new ObservableCollection<string>();
            _connectedUsers = new ObservableCollection<ConnectedUser.ConnectedUser>();
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
        public ObservableCollection<ConnectedUser.ConnectedUser> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
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
                _log.Add("Próba połączenia z: " + ip + "...");
                _connection.Connect(ip, port, username);
                _log.Add("Połączono z: " + ip + "!");
                _connected = true;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");
                return true;
            }

            catch
            {
                _log.Add("Próba połączenia z: " + ip + " nie powiodła się!");
                _connected = false;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");
                return false;
            }
        }
        public bool Reconnect()
        {
            try
            {
                _log.Add("Próba połączenia z: " + Ip + "...");
                _connection = new Connection();
                _connection.Connect(Ip, Port, Username);
                _log.Add("Połączono z: " + Ip + "!");
                _connected = true;
                OnPropertyChanged("Log");
                OnPropertyChanged("Connected");
                Work();
                return true;
            }

            catch
            { 
                _log.Add("Próba połączenia z: " + Ip + " nie powiodła się!");
                _connected = false;
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
                                    _log.Add("Server has been stopped!");
                                });

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
                OnPropertyChanged("Connected");
            }

        }
        public void SendMessage(string msg)
        {
            BinaryWriter writer = new BinaryWriter(_connection.GetStream());
            try
            {
                writer.Write(Username + ": " + msg);
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
