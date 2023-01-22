using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Network.Lobby
{
    public class NetworkLobby
    {
        private Dictionary<string, LobbyPlayer> players;
        private LobbyPlayer currentPlayer;

        public NetworkLobby()
        {
            players = new Dictionary<string, LobbyPlayer>();
            Debug.Log($"[Lobby] - Created");
        }

        public void AddPlayer()
        {

        }

        public void RemovePlayer()
        {

        }

        public void SelectPlayer(string playerId)
        {
            if (players.ContainsKey(playerId))
            {
                currentPlayer = players[playerId];
            }
        }

        public void Deselect()
        {
            currentPlayer = null;
        }
    }
}
