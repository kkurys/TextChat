using System;

namespace Chat_GUI.ViewModels
{
    [Serializable]
    public class ConnectionDataViewModel
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public bool OpenInTab { get; set; }

    }
}
