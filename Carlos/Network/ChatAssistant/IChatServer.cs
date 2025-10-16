using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Network.ChatAssistant
{
    public interface IChatServer
    {
        void Start(int port);
        void Stop();
        bool IsRunning { get; }
        event EventHandler<EventArgs> OnServerStatusChanged;
    }
}
