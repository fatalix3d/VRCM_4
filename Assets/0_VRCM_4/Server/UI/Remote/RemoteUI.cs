using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCM.Network.Lobby;
using VRCM.Network.Messages;
using VRCM.Services.Protect;

namespace VRCM.Media.Remote.UI
{
    public class RemoteUI : MonoBehaviour
    {
        private Dictionary<string, RemoteElement> _elements = new Dictionary<string, RemoteElement>();
        private NetworkLobby _lobby;
        private Coroutine _linkRoutine = null;

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
            _linkRoutine = StartCoroutine(Link());
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;

            if (_lobby != null)
                _lobby.RemoteModeChangeEvent -= OnRemoteModeChangeEvent;
        }

        private IEnumerator Link()
        {
            while (true)
            {
                if (_elements!=null)
                {
                    foreach (KeyValuePair<string, RemoteElement> element in _elements)
                        element.Value.Deselect();

                    if (_lobby.CurrentPlayer != null)
                    {
                        if (_lobby.CurrentPlayer._state != null)
                        {
                            string mediaNameIndex = _lobby.CurrentPlayer._state.mediaName;
                            if (!string.IsNullOrEmpty(mediaNameIndex))
                            {
                                if (_elements.ContainsKey(mediaNameIndex))
                                {
                                    _elements[mediaNameIndex].Select(_lobby.CurrentPlayer._state);
                                }
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
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
            //_elements = new Dictionary<string, RemoteElement>();

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
            if (_lobby.CurrentPlayer == null)
            {
                Debug.Log("[Remote UI] Player not selected");
                return;
            }

            if (string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                Debug.Log("[Remote UI] Player not valid");
                return;
            }

            if (_lobby.CurrentPlayer._state != null)
            {

                if (_lobby.CurrentPlayer._state.command == NetMessage.Command.Play)
                    return;
                

                if (_lobby.CurrentPlayer._state.command == NetMessage.Command.Pause && _lobby.CurrentPlayer._state.mediaName == videoID)
                {
                    Debug.Log($"[Remote UI] Send (Resume), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                    Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, NetMessage.Command.Resume, videoID);

                    // Session record add .. 
                    Debug.Log("aaa");
                    AuthService.Instance.Storage.AddPlayRecord(videoID, 1);
                }
                else
                {
                    Debug.Log($"[Remote UI] Send (Play), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                    Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, NetMessage.Command.Play, videoID);

                    // Session record add .. 
                    Debug.Log("bbb");
                    AuthService.Instance.Storage.AddPlayRecord(videoID, 1);
                }
            }
            else
            {
                Debug.Log($"[Remote UI] Send (Play), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, NetMessage.Command.Play, videoID);

                // Session record add .. 
                Debug.Log("ccc");
                AuthService.Instance.Storage.AddPauseRecord(videoID, 1);
            }

        }

        public void RemotePauseVideo(string videoID)
        {
            
            if (_lobby.CurrentPlayer != null && !string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                if (_lobby.CurrentPlayer._state != null)
                {
                    if (_lobby.CurrentPlayer._state.mediaName != videoID)
                    {
                        return;
                    }

                    Debug.Log($"[Remote UI] Send (Pause), file [{videoID}] to [{_lobby.CurrentPlayer.Id}]");
                    Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, Network.Messages.NetMessage.Command.Pause, videoID);

                    // Session record add .. 
                    AuthService.Instance.Storage.AddPauseRecord(videoID, 1);
                }
            }
            else
            {
                Debug.Log("[Remote UI] Player not selected");
            }
        }

        public void RemoteStopVideo(string videoID)
        {
            if (_lobby.CurrentPlayer != null && !string.IsNullOrEmpty(_lobby.CurrentPlayer.UniqueId))
            {
                if (_lobby.CurrentPlayer._state != null)
                {
                    if (_lobby.CurrentPlayer._state.mediaName != videoID)
                    {
                        return;
                    }

                    Debug.Log($"[Remote UI] Send (Stop) to [{_lobby.CurrentPlayer.Id}]");
                    Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, Network.Messages.NetMessage.Command.Stop);

                    // Session record add .. 
                    AuthService.Instance.Storage.AddStopRecord(videoID, 1);
                }
            }
            else
            {
                Debug.Log("[Remote UI] Player not selected");
            }
        }

        public void RemoteSeek(float seekTime, string videoID)
        {
            // send to server play command.
            //...
            NetMessage netMessage = new NetMessage(NetMessage.Command.Seek);
            netMessage.seekTime = seekTime;
            netMessage.mediaName = videoID;
            Bootstrapper.Instance.Server.SendMessage(_lobby.CurrentPlayer.UniqueId, netMessage);
        }
    }
}
