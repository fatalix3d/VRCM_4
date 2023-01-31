using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VRCM.Network.Client;
using VRCM.Network.Server;
using VRCM.Network.Lobby;

namespace VRCM.Network.Messages
{
    public class NetMessageDispatcher
    {
        private NetworkServer _server;
        private NetworkClient _client;
        private NetworkLobby _lobby;

        public NetMessageDispatcher(NetworkServer server)
        {
            _server = server;
            _server.OnRecieveMessage += Server_MessageIn;

            _lobby = Bootstrapper.Instance.Lobby;
        }

        public NetMessageDispatcher(NetworkClient client)
        {
            _client = client;
            _client.OnRecieveMessage += Client_MessageIn;
        }

        public void Server_MessageIn(string uniqueId, byte[] bytes)
        {
            //NetMessage message = BinarySerializer.Deserialize(bytes);

            string json= Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            NetMessage message = JsonUtility.FromJson<NetMessage>(json);

            if (message == null)
            {
                Debug.Log($"[Message Dispatcher] - Income cmd parse error / null");
                return;
            }

            switch (message.command)
            {
                case NetMessage.Command.AutorizeSucces:
                    Debug.Log($"[Message Dispatcher] - Connection [{uniqueId}], id [{message.id}] - Authorization [Processing]");
                    if (_lobby.AddPlayer(message.id, uniqueId))
                    {
                        // TODO : add server side binary serialization and cmd input;
                        NetMessage resp = new NetMessage(NetMessage.Command.AutorizeSucces);
                        byte[] respBytes = BinarySerializer.Serialize(resp);
                        _server.SendMessage(uniqueId, respBytes);

                        Debug.Log($"[Message Dispatcher] - Player with id [{message.id}][{uniqueId}] - Authorization [Succes]");
                    }
                    else
                    {
                        NetMessage resp = new NetMessage(NetMessage.Command.AutorizeError);
                        byte[] respBytes = BinarySerializer.Serialize(resp);
                        _server.SendMessage(uniqueId, respBytes);

                        Debug.Log($"[Message Dispatcher] - Player with id [{message.id}][{uniqueId}] - Authorization [Error]");
                    }
                    break;

                case NetMessage.Command.Setup:
                case NetMessage.Command.Ready:
                case NetMessage.Command.Status:
                case NetMessage.Command.Play:
                case NetMessage.Command.Pause:
                case NetMessage.Command.Stop:
                case NetMessage.Command.Seek:
                case NetMessage.Command.VideoNotFound:
                    break;
            }
        }

        public void Client_MessageIn(byte[] bytes)
        {
            NetMessage message = BinarySerializer.Deserialize(bytes);
            if (message == null)
                return;

            switch (message.command)
            {
                case NetMessage.Command.AutorizeRequest:

                    break;

                case NetMessage.Command.AutorizeSucces:
                case NetMessage.Command.Setup:
                case NetMessage.Command.Ready:
                case NetMessage.Command.Status:
                case NetMessage.Command.Play:
                case NetMessage.Command.Pause:
                case NetMessage.Command.Stop:
                case NetMessage.Command.Seek:
                case NetMessage.Command.VideoNotFound:
                    Debug.Log($"[Message Dispatcher] - Income cmd {message.command}");
                    break;
            }
        }

        public void Stop()
        {
            if(_server!=null)
                _server.OnRecieveMessage -= Server_MessageIn;

            if (_client != null)
                _client.OnRecieveMessage -= Client_MessageIn;
        }
    }
}