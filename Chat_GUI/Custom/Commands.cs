using System.Windows.Input;

namespace Chat_GUI
{
    public class Commands
    {
        public static readonly RoutedUICommand Connect = new RoutedUICommand("Connect", "Connect", typeof(Commands));
    }
}
