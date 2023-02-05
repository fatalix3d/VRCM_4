using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VRCM.Network.Client;
using VRCM.Network.Server;
using VRCM.Network.Lobby;
using VRCM.Network.Player;
using System;

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
            NetMessage message = BinarySerializer.Deserialize(bytes);

            if (message == null)
            {
                Debug.Log($"[Message Dispatcher] - Income cmd parse error / null");
                return;
            }

            message.uid = uniqueId;

            switch (message.command)
            {
                // Autorize procedure
                case NetMessage.Command.AutorizeSucces:

                    Debug.Log($"[Message Dispatcher] - Connection [{uniqueId}], id [{message.id}] - Authorization [Processing]");
                    if (_lobby.AddPlayer(uniqueId, message.id))
                    {
                        _server.SendMessage(uniqueId, NetMessage.Command.AutorizeSucces);
                        _lobby.UpdatePlayer(uniqueId, message);

                        Debug.Log($"[Message Dispatcher] - Player with id [{message.id}][{uniqueId}] - Authorization [Succes]");
                    }
                    else
                    {
                        _server.SendMessage(uniqueId, NetMessage.Command.AutorizeError);

                        Debug.Log($"[Message Dispatcher] - Player with id [{message.id}][{uniqueId}] - Authorization [Error]");
                    }
                    break;
            }

            if (!_lobby.IsAuthorized(uniqueId))
                return;

            _lobby.UpdatePlayer(uniqueId, message);

            switch (message.command)
            {
                // Setup device
                case NetMessage.Command.Setup:
                    break;

                case NetMessage.Command.Ready:

                    break;

                case NetMessage.Command.Status:

                    break;

                // Play
                case NetMessage.Command.Play:
                    break;

                // Pause
                case NetMessage.Command.Pause:
                    break;

                // Resume
                case NetMessage.Command.Resume:
                    break;

                case NetMessage.Command.Stop:
                    break;

                case NetMessage.Command.Seek:
                    break;

                case NetMessage.Command.VideoNotFound:
                    break;
            }
        }

        public void Client_MessageIn(byte[] bytes)
        {
            try
            {
                NetMessage message = BinarySerializer.Deserialize(bytes);

                if (message == null)
                    return;

                NetMessage resp;

                switch (message.command)
                {
                    case NetMessage.Command.AutorizeRequest:
                        resp = new NetMessage(NetMessage.Command.AutorizeSucces);
                        _client.SendMessage(resp);
                        break;

                    case NetMessage.Command.AutorizeSucces:
                        resp = new NetMessage(NetMessage.Command.Ready);
                        _client.SendMessage(resp);
                        break;

                    case NetMessage.Command.Setup:
                        break;

                    case NetMessage.Command.Status:
                        resp = new NetMessage(_client.Status);
                        _client.SendMessage(resp);
                        break;
                    case NetMessage.Command.Play:
                        resp = new NetMessage(NetMessage.Command.Play);
                        _client.SendMessage(resp);
                        break;
                    case NetMessage.Command.Pause:
                        resp = new NetMessage(NetMessage.Command.Pause);
                        _client.SendMessage(resp);
                        break;
                    case NetMessage.Command.Stop:
                        resp = new NetMessage(NetMessage.Command.Stop);
                        _client.SendMessage(resp);
                        break;
                    case NetMessage.Command.Seek:
                        resp = new NetMessage(NetMessage.Command.Seek);
                        _client.SendMessage(resp);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
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
