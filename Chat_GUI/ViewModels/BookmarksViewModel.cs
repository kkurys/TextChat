using Chat_GUI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chat_GUI.ViewModels
{
    public class BookmarkViewModel : INotifyPropertyChanged
    {
        private string _name, _ip, _nick;
        private int _port;
        public BookmarkViewModel(string ip, string nick, int port)
        {
            Ip = ip;
            Nick = nick;
            Port = port;
        }
        public BookmarkViewModel(Bookmark bookmark)
        {
            _name = bookmark.Name;
            ConnectionDataViewModel _conData = bookmark.GetConnectionData();
            _nick = _conData.Username;
            _port = _conData.Port;
            _ip = _conData.Ip;
        }
        public BookmarkViewModel(string name)
        {
            Name = name;
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Nick
        {
            get
            {
                return _nick;
            }
            set
            {
                _nick = value;
                OnPropertyChanged("Nick");
            }
        }
        public string Ip
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged("Ip");
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public Bookmark GetBookmark()
        {
            return new Models.Bookmark(Name, new ConnectionDataViewModel()
            {
                Ip = _ip,
                Port = _port,
                Username = _nick
            });
        }
    }
    public class BookmarksViewModel
    {
        public ObservableCollection<BookmarkViewModel> Bookmarks { get; set; }
        public BookmarksViewModel(BookmarksViewModel bvm)
        {
            Bookmarks = new ObservableCollection<BookmarkViewModel>(bvm.Bookmarks);
        }
        public BookmarksViewModel()
        {

        }
        public void AddNewBookmark()
        {
            Bookmarks.Add(new BookmarkViewModel("Nowa zakładka"));
        }
        public void AddBookmark(BookmarkViewModel bookmark)
        {
            Bookmarks.Add(bookmark);
        }
        public void RemoveBookmark(BookmarkViewModel bookmark)
        {
            Bookmarks.Remove(bookmark);
        }
        public List<Bookmark> GetBookmarks()
        {
            var bookmarks = new List<Bookmark>();
            foreach (BookmarkViewModel _bmv in Bookmarks)
            {
                bookmarks.Add(_bmv.GetBookmark());
            }
            return bookmarks;
        }
    }
}
