using System.Collections.Generic;
using System.Net.Sockets;

namespace Chat_GUI.Models
{
    interface IServer
    {
        void Start();
        void Stop();
        TcpClient AcceptTcpClient();
        List<ConnectedUser.ConnectedUser> GetConnectedClients();
    }
}
