using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketChatServer
{
    using WebSocketServer;

    class ChatServer
    {
        WebSocketServer wss;
        List<User> Users = new List<User>();
        string unknownName = "john doe";

        public ChatServer()
        {
            wss = new WebSocketServer(8181, "http://localhost:8080", "ws://localhost:8181/chat");
            wss.Logger = Console.Out;
            wss.LogLevel = ServerLogLevel.Subtle;
            wss.ClientConnected += new ClientConnectedEventHandler(OnClientConnected);
            wss.Start();
            KeepAlive();
        }

        private void KeepAlive()
        {
            string r = Console.ReadLine();
            while (r != "quit")
            {
                if(r == "users")
                {
                    Console.WriteLine(Users.Count);
                }
                r = Console.ReadLine();
            }
        }

        void OnClientConnected(WebSocketConnection sender, EventArgs e)
        {
            Users.Add(new User() { Connection = sender });
            sender.Disconnected += new WebSocketDisconnectedEventHandler(OnClientDisconnected);
            sender.DataReceived += new DataReceivedEventHandler(OnClientMessage);
            
        }

        void OnClientMessage(WebSocketConnection sender, DataReceivedEventArgs e)
        {
            User user = Users.Single(a => a.Connection == sender);
            if (e.Data.Contains("/nick"))
            {
                string[] tmpArray = e.Data.Split(new char[] { ' ' });
                if (tmpArray.Length > 1)
                {
                    string myNewName = tmpArray[1];
                    while (Users.Where(a => a.Name == myNewName).Count() != 0)
                    {
                        myNewName += "_";
                    }
                    if (user.Name != null)
                        wss.SendToAll("server: '" + user.Name + "' changed name to '" + myNewName + "'");
                    else
                        sender.Send("you are now know as '" + myNewName + "'");
                    user.Name = myNewName;
                }
            }
            else
            {
                string name = (user.Name == null) ? unknownName : user.Name;
                wss.SendToAllExceptOne(name + ": " + e.Data, sender);
                sender.Send("me: " + e.Data);
            }
        }

        void OnClientDisconnected(WebSocketConnection sender, EventArgs e)
        {
            try
            {
                User user = Users.Single(a => a.Connection == sender);
                string name = (user.Name == null) ? unknownName : user.Name;
                wss.SendToAll("server: "+name + " disconnected");
                Users.Remove(user);
            }
            catch (Exception exc)
            {
                Console.WriteLine("ehm...");
            }

        }
    }
}
