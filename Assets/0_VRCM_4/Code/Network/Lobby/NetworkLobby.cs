using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Player;

namespace VRCM.Network.Lobby
{
    public class NetworkLobby
    {
        private Dictionary<string, NetPlayer> _players;
        private NetPlayer _currentPlayer;

        public NetworkLobby()
        {
            _players = new Dictionary<string, NetPlayer>();
            Debug.Log($"[Lobby] - Created");
        }

        public bool AddPlayer(string playerId, string uniqueId)
        {
            bool res = false;
            if (!_players.ContainsKey(playerId))
            {
                NetPlayer lb = new NetPlayer();
                lb.Id = playerId;
                lb.UniqueId = uniqueId;
                _players.Add(playerId, lb);

                res = true;
            }

            if (_players.Count > 0)
            {
                string lobyList = string.Empty;
                foreach (KeyValuePair<string, NetPlayer> player in _players)
                    lobyList += $"{player.Value.Id} / {player.Value.UniqueId} \n";

                Debug.Log($"[Lobby] - player list : \n {lobyList}");
            }

            return res;
        }

        public bool RemovePlayer(string playerId)
        {
            bool res = false;

            if (_players.ContainsKey(playerId))
            {
                _players.Remove(playerId);
                res = true;
            }

            return res;
        }

        public void SelectPlayer(string playerId)
        {
            if (_players.ContainsKey(playerId))
            {
                _currentPlayer = _players[playerId];
            }
        }

        public void Deselect()
        {
            _currentPlayer = null;
        }
    }
}
