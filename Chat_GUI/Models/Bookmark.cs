using Chat_GUI.ViewModels;
using System;

namespace Chat_GUI.Models
{
    [Serializable]
    public class Bookmark
    {
        private string _name;
        private ConnectionDataViewModel _connectionData;
        public Bookmark(string name, ConnectionDataViewModel connectionData)
        {
            _name = name;
            _connectionData = connectionData;
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public ConnectionDataViewModel GetConnectionData()
        {
            return _connectionData;
        }
    }
}
