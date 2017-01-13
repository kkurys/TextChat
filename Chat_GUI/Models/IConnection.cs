using System.Net.Sockets;

namespace Chat_GUI.Models
{
    public interface IConnection
    {
        void Connect(string ip, int port);
        void Disconnect();
        NetworkStream GetStream();
    }
}
