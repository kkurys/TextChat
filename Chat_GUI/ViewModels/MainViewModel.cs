using Chat_GUI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chat_GUI.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ConnectionViewModel> _connectionsTabs;
        private Server _server;
        private int _activeConnectionTab = -1;
        private int _connectionsCount = 0;
        bool _serverRunning;
        public MainViewModel()
        {
            _connectionsTabs = new ObservableCollection<ConnectionViewModel>();
            _server = new Server();
        }
        public bool ServerRunning
        {
            get
            {
                return _serverRunning;
            }
            set
            {
                _serverRunning = value;
                OnPropertyChanged("ServerRunning");
            }
        }
        public void StartServer()
        {
            _server.Start();
            ServerRunning = true;
        }
        public void StopServer()
        {
            _server.Stop();
            ServerRunning = false;
        }
        public void AddConnection(ConnectionViewModel _connectionViewModel)
        {
            if (ActiveConnectionTab == -1 || _connectionViewModel.OpenInNewTab)
            {
                _connectionsTabs.Add(_connectionViewModel);
                ActiveConnectionTab = _connectionsTabs.Count - 1;
            }
            else
            {
                _connectionsTabs[ActiveConnectionTab].Disconnect();
                int _tmpActiveTab = ActiveConnectionTab;
                ActiveConnectionTab = -1;
                _connectionsTabs[_tmpActiveTab] = _connectionViewModel;
                ActiveConnectionTab = _tmpActiveTab;

            }
        }
        public int ActiveConnectionTab
        {
            get
            {
                return _activeConnectionTab;
            }
            set
            {
                _activeConnectionTab = value;
                OnPropertyChanged("ActiveConnectionTab");
            }
        }

        public int ConnectionsCount
        {
            get
            {
                return _connectionsCount;
            }
            set
            {
                _connectionsCount = value;
                OnPropertyChanged("ConnectionsCount");

            }
        }
        public bool ActiveConnectionTabHasConnection
        {
            get
            {
                return _connectionsTabs[ActiveConnectionTab].Connected;
            }

        }
        public void DisconnectFromCurrent()
        {
            _connectionsTabs[ActiveConnectionTab].Disconnect();
        }
        public void ReconnectOnCurrent()
        {
            _connectionsTabs[ActiveConnectionTab].Reconnect();
        }
        public void DisconnectAll()
        {
            foreach (var connection in _connectionsTabs)
            {
                if (connection.Connected)
                {
                    connection.Disconnect();
                }
            }
        }
        public ObservableCollection<ConnectionViewModel> ConnectionsTabs
        {
            get
            {
                return _connectionsTabs;
            }

        }
        public int GetTabIndex(ConnectionViewModel tab)
        {
            return _connectionsTabs.IndexOf(tab);
        }
        public void RemoveConnection(ConnectionViewModel connectionToRemove)
        {
            _connectionsTabs.Remove(connectionToRemove);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void SendMessage(string msg)
        {
            _connectionsTabs[ActiveConnectionTab].SendMessage(msg);
        }
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
