using System;
using System.Net.Sockets;

namespace Chat_GUI.Models
{

    class ClientServerData
    {
        public ClientServerData(string nick, TcpClient client, int count, string ip)
        {
            this.NickName = nick;
            this.Client = client;
            this.Id = count;
            ConnectedFromIp = ip;
        }

        public int Id { get; set; }
        public string ConnectedFromIp { get; set; }
        public string NickName { get; set; }
        [NonSerialized]
        private TcpClient _client;
        public TcpClient Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
            }
        }
        public NetworkStream GetStream()
        {
            return Client.GetStream();
        }

    }
}
