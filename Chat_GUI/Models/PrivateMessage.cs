using System;

namespace Chat_GUI.Models
{
    [Serializable]
    public class PrivateMessage
    {
        public int UserIDTo, UserIDFrom;
        public string Content;
    }
}
