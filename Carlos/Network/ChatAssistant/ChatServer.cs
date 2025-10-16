using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Network.ChatAssistant
{
    public class ChatServer : IChatServer
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsRunning => throw new NotImplementedException();

        public ChatServer(string address, int port, string name, string description)
        {
            Address = address;
            Port = port;
            Name = name;
            Description = description;
        }

        public event EventHandler<EventArgs> OnServerStatusChanged;

        public void Start(int port)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
