using Chat_GUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Chat_GUI
{
    /// <summary>
    /// Interaction logic for BookmarksWindow.xaml
    /// </summary>
    public partial class BookmarksWindow : Window
    {
        BookmarksViewModel _viewModel;
        public bool ChangesConfirmed { get; set; }
        public BookmarksWindow(BookmarksViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            gridMain.DataContext = viewModel;
        }
        private void AddNewBookmark(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewBookmark();
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
        private void ConfirmChanges(object sender, RoutedEventArgs e)
        {
            ChangesConfirmed = true;
            Close();
        }
    }
}
