using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketChatServer
{
    class User
    {
        public string Name;
        public WebSocketServer.WebSocketConnection Connection;
    }
}
