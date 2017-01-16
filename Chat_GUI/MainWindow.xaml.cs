using Chat_GUI.Models;
using Chat_GUI.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mainViewModel;
        private BookmarksViewModel _bookmarksViewModel;
        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            tcMain.DataContext = _mainViewModel;
            _mainViewModel.AddConnection(new ConnectionViewModel() { Ip = "Nie połączono" });
            _bookmarksViewModel = ReadBookmarksFromFile();
            miBookmarks.DataContext = _bookmarksViewModel;
        }


        private void OpenConnectionWindow(object sender, RoutedEventArgs e)
        {
            ConnectionDataViewModel _connectionData = ReadLastConnectionFromFile();
            ConnectionWindow _connectionWindow = new ConnectionWindow(_connectionData);
            _connectionWindow.ShowDialog();
            if (_connectionWindow.DialogResult.Value)
            {
                var _connection = new ConnectionViewModel();
                _connectionData = _connectionWindow.GetConnectionData();
                _connection.Connect(_connectionData.Ip, _connectionData.Port, _connectionData.Username);
                _connection.OpenInNewTab = _connectionData.OpenInTab;

                SaveLastConnectionToFile(_connectionData);

                if (_connection.Connected)
                {
                    _connection.Work();
                }

                _mainViewModel.AddConnection(_connection);
            }
        }
        private void OpenBookmarksWindow(object sender, RoutedEventArgs e)
        {
            var bookmarksWindow = new BookmarksWindow(_bookmarksViewModel);
            bookmarksWindow.Closing += BookmarksWindow_Closing;
            bookmarksWindow.Show();
        }

        private void BookmarksWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BookmarksWindow _bookmarksWindow = sender as BookmarksWindow;

            if (_bookmarksWindow.ChangesConfirmed)
            {
                _bookmarksViewModel.Bookmarks = _bookmarksWindow.GetBookmarks().Bookmarks;
                miBookmarks.DataContext = null;
                miBookmarks.DataContext = _bookmarksViewModel;
                SaveBookmarksToFile();
            }
            else
            {
                _bookmarksViewModel = ReadBookmarksFromFile();
            }
        }

        private ConnectionDataViewModel ReadLastConnectionFromFile()
        {
            ConnectionDataViewModel _connectionData = null;
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open("lastcon.bin", FileMode.Open)))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _connectionData = formatter.Deserialize(reader.BaseStream) as ConnectionDataViewModel;
                };
            }
            catch
            {
                if (_connectionData == null)
                {
                    _connectionData = new ConnectionDataViewModel();
                }
            }

            return _connectionData;
        }
        private BookmarksViewModel ReadBookmarksFromFile()
        {
            List<Bookmark> _bookmarks = null;

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open("bookmarks.bin", FileMode.Open)))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _bookmarks = formatter.Deserialize(reader.BaseStream) as List<Bookmark>;
                };
            }
            catch
            {
                if (_bookmarks == null)
                {
                    _bookmarks = new List<Bookmark>();
                }
            }

            var _bookmarkViewModels = new ObservableCollection<BookmarkViewModel>();
            foreach (var bookmark in _bookmarks)
            {
                _bookmarkViewModels.Add(new BookmarkViewModel(bookmark));
            }
            return new BookmarksViewModel() { Bookmarks = _bookmarkViewModels };
        }
        private void SaveBookmarksToFile()
        {
            var _bookmarks = _bookmarksViewModel.GetBookmarks();
            using (BinaryWriter writer = new BinaryWriter(File.Open("bookmarks.bin", FileMode.Create)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(writer.BaseStream, _bookmarks);
            };


        }
        private void SaveLastConnectionToFile(ConnectionDataViewModel _connectionData)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("lastcon.bin", FileMode.Create)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(writer.BaseStream, _connectionData);
            };

        }
        #region commands
        private void Send_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter != null && e.Parameter.ToString() != "" && _mainViewModel.ActiveConnectionTabHasConnection)
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
            _mainViewModel.SendMessage(e.Parameter.ToString());
            tbMsg.Text = "";
        }
        private void Reconnect_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            TabItem ti = e.OriginalSource as TabItem;
            var connection = ti.Header as ConnectionViewModel;


            if (ti != null && !connection.Connected && connection.Port != -1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }

        }
        private void Reconnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem ti = e.OriginalSource as TabItem;
            var connection = ti.Header as ConnectionViewModel;
            connection.ChatHistory.Add("Reconnecting..");
            if (connection.Reconnect())
            {
                connection.ChatHistory.Add("Niby dziala..");
            }
        }
        private void Connect_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Connect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var bookmark = e.Parameter as BookmarkViewModel;
            if (bookmark != null)
            {
                var _connection = new ConnectionViewModel();
                _connection.Connect(bookmark.Ip, bookmark.Port, bookmark.Nick);
                if (_connection.Connected)
                {
                    _connection.Work();
                }
                _connection.OpenInNewTab = true;
                _mainViewModel.AddConnection(_connection);
            }
        }

        private void Disconnect_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TabItem ti = e.OriginalSource as TabItem;
            ConnectionViewModel connection;
            if (ti != null)
            {
                connection = ti.Header as ConnectionViewModel;
            }
            else
            {

                connection = tcMain.SelectedItem as ConnectionViewModel;
            }

            if (connection.Connected)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void Disconnect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem ti = e.OriginalSource as TabItem;
            ConnectionViewModel connection;
            if (ti != null)
            {
                connection = ti.Header as ConnectionViewModel;
            }
            else
            {
                connection = tcMain.SelectedItem as ConnectionViewModel;
            }

            connection.Disconnect();
        }
        private void DisconnectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_mainViewModel.ConnectionsCount > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void DisconnectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainViewModel.DisconnectAll();
        }
        private void Bookmark_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_mainViewModel.ActiveConnectionTabHasConnection)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void Bookmark_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currConnection = _mainViewModel.ConnectionsTabs[_mainViewModel.ActiveConnectionTab];
            var newBookmark = new BookmarkViewModel(currConnection.Ip, currConnection.Username, currConnection.Port);
            var bookmarksWindow = new BookmarksWindow(_bookmarksViewModel, newBookmark);
            bookmarksWindow.Closing += BookmarksWindow_Closing;
            bookmarksWindow.Show();
        }
        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tcMain.Items.Count > 1)
            {
                e.CanExecute = true;
            }
        }
        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem ti = e.OriginalSource as TabItem;
            var connection = ti.Header as ConnectionViewModel;

            connection.Disconnect();
            tcMain.SelectedIndex = -1;
            _mainViewModel.RemoveConnection(ti.Header as ConnectionViewModel);
            tcMain.SelectedIndex = 0;
        }
        private void StartServer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!_mainViewModel.ServerRunning && Settings.HostedServerPort != -1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void StartServer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainViewModel.StartServer();
        }
        private void StopServer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_mainViewModel.ServerRunning)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void StopServer_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainViewModel.StopServer();
        }
        #endregion
        private void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_mainViewModel.ActiveConnectionTabHasConnection)
                {
                    if (tbMsg.Text != null)
                    {
                        _mainViewModel.SendMessage(tbMsg.Text);
                        tbMsg.Text = "";
                    }
                }
            }

        }
        private void ConnectedUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _mainViewModel.AddPrivateConversation((sender as ListViewItem).Content as ConnectedUser.ConnectedUser);
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_mainViewModel.ServerRunning)
            {
                _mainViewModel.StopServer();
            }
            _mainViewModel.DisconnectAll();

        }
    }
}
