using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Player;
using VRCM.Lobby.UI;
using VRCM.Network.Messages;

namespace VRCM.Network.Lobby
{
    public class NetworkLobby
    {
        private Dictionary<string, NetPlayer> _players;
        private NetPlayer _currentPlayer = null;
        private LobbyUI _lobbyUI;

        public Dictionary<string, NetPlayer> Players => _players;

        public event Action<NetPlayer> AddPlayerEvent;
        public event Action<string> SelectPlayerEvent;
        public event Action<string, NetMessage> UpdatePlayerEvent;
        public event Action<string> RemovePlayerEvent;
        public event Action NoActivePlayerEvent;

        public NetworkLobby()
        {
            _players = new Dictionary<string, NetPlayer>();
            _lobbyUI = GameObject.FindObjectOfType<LobbyUI>();
            _lobbyUI.Init(this);
            Debug.Log($"[Lobby] - Created");
        }

        public bool IsAuthorized(string uid)
        {
            bool res = false;
            if (_players.ContainsKey(uid))
            {
                res = true;
            }
            
            return false;
        }

        public bool AddPlayer(string uniqueId, string playerId)
        {
            bool res = false;
            if (!_players.ContainsKey(uniqueId))
            {
                NetPlayer lb = new NetPlayer();
                lb.Id = playerId;
                lb.UniqueId = uniqueId;
                lb.Authorized = true;
                _players.Add(uniqueId, lb);
                res = true;

                AddPlayerEvent?.Invoke(lb);
            }

            return res;
        }

        public bool RemovePlayer(string uniqueId)
        {
            bool res = false;
            
            if (_players.ContainsKey(uniqueId))
            {
                Debug.Log("[Remove player] playerId - removed");
                _players.Remove(uniqueId);
                res = true;

                RemovePlayerEvent?.Invoke(uniqueId);
            }

            if (_players.Count == 0)
                NoActivePlayerEvent?.Invoke();

            return res;
        }

        public void SelectPlayer(string uniqueId)
        {
            if (_players.ContainsKey(uniqueId))
            {
                _currentPlayer = _players[uniqueId];
            }
        }

        public void UpdatePlayer(string uniqueId, NetMessage message)
        {
            UpdatePlayerEvent?.Invoke(uniqueId, message);
        }

        public void Deselect()
        {
            _currentPlayer = null;
        }

        

    }
}
