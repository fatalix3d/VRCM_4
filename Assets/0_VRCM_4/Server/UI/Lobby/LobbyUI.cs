using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCM.Network.Lobby;
using VRCM.Network.Player;

namespace VRCM.Lobby.UI
{
    public class LobbyUI : MonoBehaviour
    {
        private NetworkLobby _networkLobby;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _playerPrefab;

        private Dictionary<string, NetPlayerUI> _players;

        public void Init(NetworkLobby lobby)
        {
            _players = new Dictionary<string, NetPlayerUI>();
            _networkLobby = lobby;
            _networkLobby.AddPlayerEvent += AddPlayer;
            _networkLobby.SelectPlayerEvent += SelectPlayer;
            _networkLobby.UpdatePlayerEvent += UpdatePlayer;
            _networkLobby.RemovePlayerEvent += RemovePlayer;
        }

        public void AddPlayer(NetPlayer player)
        {
            var playerGo = Instantiate(_playerPrefab, _root);
            var netPlayerUI = playerGo.GetComponent<NetPlayerUI>();
            netPlayerUI.GO = playerGo;
            _players.Add(player.UniqueId, netPlayerUI);
            netPlayerUI.UpdateUI(player);
        }
        public void RemovePlayer(string uniqueId)
        {
            if (!_players.ContainsKey(uniqueId))
                return;

            Destroy(_players[uniqueId].GO);
            _players.Remove(uniqueId);
        }

        public void SelectPlayer(string uniqueId)
        {

        }

        public void UpdatePlayer(NetPlayer player)
        {
            
        }


        private void OnDisable()
        {
            _networkLobby.AddPlayerEvent -= AddPlayer;
            _networkLobby.SelectPlayerEvent -= SelectPlayer;
            _networkLobby.UpdatePlayerEvent -= UpdatePlayer;
            _networkLobby.RemovePlayerEvent -= RemovePlayer;
        }
    }
}
