using System.Windows.Input;

namespace Chat_GUI
{
    public class Commands
    {
        public static readonly RoutedUICommand Connect = new RoutedUICommand("Connect", "Connect", typeof(Commands));
        public static readonly RoutedUICommand Send = new RoutedUICommand("Send", "Send", typeof(Commands));
        public static readonly RoutedUICommand Reconnect = new RoutedUICommand("Reconnect", "Reconnect", typeof(Commands));
        public static readonly RoutedUICommand Disconnect = new RoutedUICommand("Disconnect", "Disconnect", typeof(Commands));
        public static readonly RoutedUICommand DisconnectAll = new RoutedUICommand("DisconnectAll", "DisconnectAll", typeof(Commands));
        public static readonly RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof(Commands));
        public static readonly RoutedUICommand RemoveBookmark = new RoutedUICommand("RemoveBookmark", "RemoveBookmark", typeof(Commands));
        public static readonly RoutedUICommand ConfirmChanges = new RoutedUICommand("ConfirmChanges", "ConfirmChanges", typeof(Commands));
        public static readonly RoutedUICommand Bookmark = new RoutedUICommand("Bookmark", "Bookmark", typeof(Commands));
        public static readonly RoutedUICommand StartServer = new RoutedUICommand("StartServer", "StartServer", typeof(Commands));
        public static readonly RoutedUICommand StopServer = new RoutedUICommand("StopServer", "StopServer", typeof(Commands));
    }
}
