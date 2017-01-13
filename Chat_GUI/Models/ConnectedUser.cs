using System;

namespace Chat_GUI.Models
{
    [Serializable]
    public class ConnectedUser
    {
        public string Nickname
        {
            get;
            set;
        }
        public int Id
        {
            get;
            set;
        }
        public ConnectedUser(string _nick, int _id)
        {
            Nickname = _nick;
            Id = _id;
        }
        public override string ToString()
        {
            return Id + ". " + Nickname;
        }
    }
}
