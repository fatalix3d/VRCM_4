using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VRCM.Network.Player;
using VRCM.Network.Lobby;
using VRCM.Network.Messages;

namespace VRCM.Lobby.UI
{
    public class LobbyUI : MonoBehaviour
    {
        private NetworkLobby _networkLobby;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _playerPrefab;

        private Dictionary<string, NetPlayerUI> _players;

        [SerializeField] private Image _theaterIcon;
        [SerializeField] private Image _remoteIcon;

        public void Init(NetworkLobby lobby)
        {
            _players = new Dictionary<string, NetPlayerUI>();
            _networkLobby = lobby;
            _networkLobby.AddPlayerEvent += AddPlayer;
            _networkLobby.SelectPlayerEvent += SelectPlayer;
            _networkLobby.DeselectPlayerEvent += DeselectPlayer;
            _networkLobby.UpdatePlayerEvent += UpdatePlayer;
            _networkLobby.RemovePlayerEvent += RemovePlayer;

            _networkLobby.RemoteModeChangeEvent += RemoteModeUpdate;
        }

        public void AddPlayer(NetPlayer player)
        {
            var playerGo = Instantiate(_playerPrefab, _root);
            var netPlayerUI = playerGo.GetComponent<NetPlayerUI>();
            netPlayerUI.GO = playerGo;
            netPlayerUI.UniqueId = player.UniqueId;
            _players.Add(player.UniqueId, netPlayerUI);
        }

        public void RemovePlayer(string uniqueId)
        {
            if (!_players.ContainsKey(uniqueId))
                return;

            Destroy(_players[uniqueId].GO);
            _players.Remove(uniqueId);
        }

        public void SelectPlayer(string uniqueId = null)
        {
            foreach (KeyValuePair<string, NetPlayerUI> player in _players)
                player.Value.Select(false);

            if (_players.ContainsKey(uniqueId))
            {
                _players[uniqueId].Select(true);
            }
        }

        private void DeselectPlayer()
        {
            foreach (KeyValuePair<string, NetPlayerUI> player in _players)
                player.Value.Select(false);
        }

        public void UpdatePlayer(string uniqueId, NetMessage message)
        {
            if (_players.ContainsKey(uniqueId))
                _players[uniqueId].UpdateUI(message);
        }

        public void RemoteModeUpdate(bool remote)
        {
            // to do remove to ext. component
            if (remote)
            {
                if (_remoteIcon != null)
                {
                    _remoteIcon.DOKill();
                    _remoteIcon.DOFade(1f, 0.25f);
                }

                if (_theaterIcon != null)
                {
                    _theaterIcon.DOKill();
                    _theaterIcon.DOFade(0.2f, 0.25f);
                }
            }
            else
            {
                if (_remoteIcon != null)
                {
                    _remoteIcon.DOKill();
                    _remoteIcon.DOFade(0.2f, 0.25f);
                }

                if (_theaterIcon != null)
                {
                    _theaterIcon.DOKill();
                    _theaterIcon.DOFade(1f, 0.25f);
                }
            }


            foreach (KeyValuePair<string, NetPlayerUI> player in _players)
                player.Value.SetRemoteMode(remote);
        }

        private void OnDisable()
        {
            _networkLobby.AddPlayerEvent -= AddPlayer;
            _networkLobby.SelectPlayerEvent -= SelectPlayer;
            _networkLobby.DeselectPlayerEvent -= DeselectPlayer;
            _networkLobby.UpdatePlayerEvent -= UpdatePlayer;
            _networkLobby.RemovePlayerEvent -= RemovePlayer;

            _networkLobby.RemoteModeChangeEvent -= RemoteModeUpdate;
        }
    }
}
