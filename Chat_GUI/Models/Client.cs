using System;
using System.IO;
using System.Net.Sockets;

namespace Chat_GUI.Models
{
    public class Connection : IConnection
    {
        #region fields
        private TcpClient _tcpClient;
        #endregion
        #region constructors
        public Connection()
        {
            _tcpClient = new TcpClient();
        }
        #endregion
        #region connection methods
        public void Connect(string ip, int port)
        {
            var _result = _tcpClient.BeginConnect(ip, port, null, null);

            var _success = _result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (!_success)
            {
                throw new Exception("Failed to connect.");
            }

            BinaryWriter writer = new BinaryWriter(_tcpClient.GetStream());

            writer.Write(Settings.Nickname);
        }

        public void Disconnect()
        {
            _tcpClient.Close();
        }
        #endregion
        #region messages
        public NetworkStream GetStream()
        {
            try
            {
                return _tcpClient.GetStream();
            }
            catch
            {
                return null;
            }
        }


        #endregion
    }
}
