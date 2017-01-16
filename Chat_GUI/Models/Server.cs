using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Chat_GUI.Models
{
    public class Server : IServer
    {
        #region fields
        private TcpListener _listener;
        private List<ClientServerData> _connectedClients;
        private List<ConnectedUser.ConnectedUser> _clientsToBroadcast;
        #endregion
        #region constructors
        public Server()
        {
            _listener = new TcpListener(GetLocalIPAddress(), Settings.HostedServerPort);
            _connectedClients = new List<ClientServerData>();
            _clientsToBroadcast = new List<ConnectedUser.ConnectedUser>();

        }
        #endregion
        public void Start()
        {
            _listener = new TcpListener(GetLocalIPAddress(), Settings.HostedServerPort);
            _listener.Start();
            new Thread(Listen).Start();
        }

        public void Stop()
        {
            _listener.Server.Close();
            _listener.Stop();
            BinaryFormatter writer = new BinaryFormatter();
            foreach (ClientServerData client in _connectedClients)
            {
                writer.Serialize(client.GetStream(), -1);
            }
        }
        public TcpClient AcceptTcpClient()
        {
            try
            {
                return _listener.AcceptTcpClient();
            }
            catch
            {
                return null;
            }

        }
        private IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        private void Listen()
        {
            TcpClient client;
            BinaryReader reader = null;
            while (true)
            {
                client = AcceptTcpClient();
                if (client == null)
                {
                    break;
                }
                reader = new BinaryReader(client.GetStream());

                string nick = reader.ReadString();
                string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                ClientServerData cl = new ClientServerData(nick, client, _connectedClients.Count, clientIp);

                _connectedClients.Add(cl);
                _clientsToBroadcast.Add(new ConnectedUser.ConnectedUser(cl.NickName, cl.Id));
                OnNewMessageArrived();
                BroadcastClients();
                new Thread(() => Broadcast(cl)).Start();
            }
        }
        private void Broadcast(ClientServerData client)
        {
            BinaryReader reader = new BinaryReader(client.GetStream());
            while (true)
            {
                try
                {
                    var _msg = reader.ReadString();
                    if (_msg is string)
                    {
                        string msg = _msg as string;

                        OnNewMessageArrived();
                        BinaryFormatter writer = new BinaryFormatter();
                        foreach (ClientServerData cl in _connectedClients)
                        {
                            writer.Serialize(cl.GetStream(), msg);
                        }
                        continue;
                    }

                }
                catch
                {
                    break;
                }
            }
            if (_connectedClients.Find(item => item == client) != null)
            {
                _connectedClients.Remove(client);
            }
            if (_clientsToBroadcast.Find(item => item.Nickname == client.NickName) != null)
            {
                _clientsToBroadcast.Remove(_clientsToBroadcast.Find(item => item.Nickname == client.NickName));
            }


            BroadcastClients();
        }
        private IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        private void BroadcastClients()
        {
            BinaryFormatter bin = new BinaryFormatter();
            foreach (ClientServerData cl in _connectedClients)
            {
                try
                {
                    NetworkStream stream = cl.GetStream();
                    bin.Serialize(stream, _clientsToBroadcast);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public delegate void NewMessageArrivedEventHandler(object sender, EventArgs e);
        public event NewMessageArrivedEventHandler NewMessageArrived;

        protected virtual void OnNewMessageArrived()
        {
            if (NewMessageArrived != null)
            {
                NewMessageArrived(this, EventArgs.Empty);
            }
        }
        public List<ConnectedUser.ConnectedUser> GetConnectedClients()
        {
            return _clientsToBroadcast;
        }
    }
}
