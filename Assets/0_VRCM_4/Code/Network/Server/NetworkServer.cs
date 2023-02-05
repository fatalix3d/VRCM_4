using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text;
using VRCM.Network;
using VRCM.Network.Lobby;
using VRCM.Network.Player;
using VRCM.Network.Messages;

namespace VRCM.Network.Server
{
    public class NetworkServer
    {
        private WebSocketServer _ws;
        private NetMessageDispatcher _networkMessageDispatcher;
        private NetworkLobby _lobby;

        public event Action<string, byte[]> OnRecieveMessage;
        public event Action<string, byte[]> OnSendMessage;

        public NetworkServer(string ip, int port)
        {
            _networkMessageDispatcher = new NetMessageDispatcher(this);
            _lobby = Bootstrapper.Instance.Lobby;

            _ws = new WebSocketServer($"ws://{ip}:{port}");
            _ws.AddWebSocketService<Echo>("/Echo");
            _ws.Start();
            Debug.Log($"[NetworkServer] - Started at : ws://{ip}:{port}/Echo");
        }  

        public void SendMessage(string id, NetMessage.Command cmd, string mediaId = null)
        {
            try
            {
                NetMessage netMessage = new NetMessage(cmd);

                if (!string.IsNullOrEmpty(mediaId))
                    netMessage.mediaName = mediaId;

                byte[] bytes = BinarySerializer.Serialize(netMessage);

                if (bytes != null)
                {
                    NetMessage jMessage = BinarySerializer.Deserialize(bytes);
                    string json = JsonUtility.ToJson(jMessage);

                    Debug.Log("!!!" + json + "!!!");

                    _ws.WebSocketServices["/Echo"].Sessions[id].Context.WebSocket.Send(json);
                    Debug.Log($"[NetworkServer] - Sended to [{id}]: bytes {bytes.Length}");
                    OnSendMessage?.Invoke(id, bytes);
                }

            }
            catch (Exception e)
            {
                Debug.Log($"[NetworkServer] - Error on send : {e}");
            }
        }

        public void SendMessageAll(NetMessage.Command cmd, string mediaId = null)
        {
            if (_lobby.Players.Count == 0)
                return;

            foreach (KeyValuePair<string, NetPlayer> player in _lobby.Players)
            {
                SendMessage(player.Value.UniqueId, cmd, mediaId);
            }
        }

        public void RecieveMessage(string id, byte[] bytes)
        {
            try
            {
                if (bytes != null)
                {
                    string msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    Debug.Log($"[NetworkServer] - Recieve message : {msg}");
                    OnRecieveMessage?.Invoke(id, bytes);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[NetworkServer] - Error on recieve : {e}");
            }
        }

        public void StopServer()
        {
            if (_ws != null)
            {
                //<add> Auto send disconnect command
                _ws.Stop();
            }

            if (_networkMessageDispatcher != null)
                _networkMessageDispatcher.Stop();

        }

        
    }
}
