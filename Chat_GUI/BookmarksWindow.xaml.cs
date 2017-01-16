using Chat_GUI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for BookmarksWindow.xaml
    /// </summary>
    public partial class BookmarksWindow : Window, INotifyPropertyChanged
    {
        BookmarksViewModel _viewModel;
        private bool _inputErrors;
        public bool ChangesConfirmed { get; set; }
        private bool InputErrors
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
        public BookmarksWindow(BookmarksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = new BookmarksViewModel(viewModel);
            gridMain.DataContext = _viewModel;
        }
        public BookmarksWindow(BookmarksViewModel viewModel, BookmarkViewModel newBookmark)
        {
            InitializeComponent();
            _viewModel = new BookmarksViewModel(viewModel);
            _viewModel.AddBookmark(newBookmark);
            gridMain.DataContext = _viewModel;

            lvBookmarks.SelectedIndex = _viewModel.Bookmarks.Count - 1;
        }
        private void AddNewBookmark(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewBookmark();
            lvBookmarks.SelectedIndex = lvBookmarks.Items.Count - 1;
        }
        private void RemoveBookmark_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lvBookmarks != null && lvBookmarks.SelectedIndex != -1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void RemoveBookmark_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.RemoveBookmark(lvBookmarks.SelectedItem as BookmarkViewModel);
        }
        private void ConfirmChanges_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tbAddress.Text != "" && tbName.Text != "" && !InputErrors)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void ConfirmChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChangesConfirmed = true;

            Close();
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
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public BookmarksViewModel GetBookmarks()
        {
            return _viewModel;
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
