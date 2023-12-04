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
        private int _maxPlayers = 1;
        private Dictionary<string, NetPlayer> _players;
        private NetPlayer _currentPlayer = null;
        public NetPlayer CurrentPlayer => _currentPlayer;
        private LobbyUI _lobbyUI;

        public Dictionary<string, NetPlayer> Players => _players;

        public event Action<NetPlayer> AddPlayerEvent;
        public event Action<string> SelectPlayerEvent;
        public event Action DeselectPlayerEvent;
        public event Action<string, NetMessage> UpdatePlayerEvent;
        public event Action<string> RemovePlayerEvent;
        public event Action NoActivePlayerEvent;

        private bool _remoteMode = false;
        public bool RemoteMode => _remoteMode;
        public event Action<bool> RemoteModeChangeEvent;

        public NetworkLobby()
        {
            _players = new Dictionary<string, NetPlayer>();
            _lobbyUI = GameObject.FindObjectOfType<LobbyUI>();
            _lobbyUI.Init(this);
            Debug.Log($"[Lobby] - Created");

            // dev
            //for (int i = 0; i < 12; i++)
            //{
            //    AddPlayer($"1254{i}", $"Pico Emul-{i}");
            //}
        }

        public bool IsAuthorized(string uid)
        {
            bool res = false;
            if (_players.ContainsKey(uid))
            {
                res = true;
            }

            return res;
        }

        public bool AddPlayer(string uniqueId, string playerId)
        {
            if (_players.Count >= _maxPlayers)
                return false;

            if (!_players.ContainsKey(uniqueId))
            {
                NetPlayer lb = new NetPlayer();
                lb.Id = playerId;
                lb.UniqueId = uniqueId;
                lb.Authorized = true;
                _players.Add(uniqueId, lb);

                Debug.Log($"[Lobby] Add => {_players[uniqueId].Id} - added");
                AddPlayerEvent?.Invoke(lb);
                return true;
            }

            return false;
        }

        public bool RemovePlayer(string uniqueId)
        {
            bool res = false;

            if (_players.ContainsKey(uniqueId))
            {
                Debug.Log($"[Lobby] Remove => {_players[uniqueId].Id} - removed");
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
            if (!_remoteMode)
                return;

            if(_currentPlayer!=null && _currentPlayer.UniqueId == uniqueId)
            {
                Deselect();
                return;
            }

            if (_players.ContainsKey(uniqueId))
            {
                _currentPlayer = _players[uniqueId];
                SelectPlayerEvent?.Invoke(_currentPlayer.UniqueId);
            }

        }
        public void Deselect()
        {
            _currentPlayer = null;
            DeselectPlayerEvent?.Invoke();
        }

        public void UpdatePlayer(string uniqueId, NetMessage message)
        {
            UpdatePlayerEvent?.Invoke(uniqueId, message);

            if (_currentPlayer == null)
                return;

            if (_currentPlayer.UniqueId == uniqueId)
                _currentPlayer._state = message;
        }


        public void SelectRemote(bool val)
        {
            _remoteMode = val;

            if (_remoteMode)
            {
                Debug.Log("[Remove player] Set mode -> Remote");
            }
            else
            {
                Deselect();
                Debug.Log("[Remove player] Set mode -> Theater");
            }

            RemoteModeChangeEvent?.Invoke(_remoteMode);
        }
    }
}
