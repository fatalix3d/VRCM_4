using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCM.Network.Lobby;

namespace VRCM.Media.Remote.UI
{
    public class RemoteUI : MonoBehaviour
    {
        private Dictionary<string, RemoteElement> _elements;
        private NetworkLobby _lobby;

        [SerializeField] private CanvasGroup _window;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;

        private void Awake()
        {
            MediaLibrary.MediaLibraryLoaded += OnMediaLibraryLoaded;
        }

        public void Setup()
        {
            _lobby = Bootstrapper.Instance.Lobby;
            _lobby.RemoteModeChangeEvent += OnRemoteModeChangeEvent;
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;
            _lobby.RemoteModeChangeEvent -= OnRemoteModeChangeEvent;
        }

        private void OnRemoteModeChangeEvent(bool remote)
        {
            if (remote)
            {
                _window.alpha = 1;
                _window.blocksRaycasts = true;
            }
            else
            {
                _window.alpha = 0;
                _window.blocksRaycasts = false;
            }
        }

        private void OnMediaLibraryLoaded(Dictionary<string, MediaFile> videos)
        {
            _elements = new Dictionary<string, RemoteElement>();

            foreach (KeyValuePair<string, MediaFile> video in videos)
            {
                var newElement = Instantiate(_elementPrefab, _root);
                var elementUI = newElement.GetComponent<RemoteElement>();
                elementUI.CreateElement(this, video.Value);
                _elements.Add(video.Key, elementUI);
            }
        }

        public void RemotePlayVideo(string videoID)
        {
            if (_lobby.CurrentPlayer != null && !string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                Debug.Log($"[Remote UI] Send (Play), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, Network.Messages.NetMessage.Command.Play, videoID);
            }
            else
            {
                Debug.Log("[Remote UI] Player not selected");
            }
        }

        public void RemotePauseVideo(string videoID)
        {
            
            if (_lobby.CurrentPlayer != null && !string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                Debug.Log($"[Remote UI] Send (Pause), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, Network.Messages.NetMessage.Command.Pause, videoID);
            }
            else
            {
                Debug.Log("[Remote UI] Player not selected");
            }
        }

        public void RemoteStopVideo()
        {
            if (_lobby.CurrentPlayer != null && !string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                Debug.Log($"[Remote UI] Send (Stop) to [{_lobby.CurrentPlayer.Id}]");
                Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, Network.Messages.NetMessage.Command.Stop);
            }
            else
            {
                Debug.Log("[Remote UI] Player not selected");
            }
        }
    }
}
