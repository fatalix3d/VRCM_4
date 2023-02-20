using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Network.Messages;

namespace VRCM.Media.Remote.UI
{
    public class RemoteElement : MonoBehaviour
    {
        private RemoteUI _remoteUI = null;
        private MediaFile _mediaFile;
        private GameObject _gameObject;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _prevImage;
        [SerializeField] private Outline _outline;
        [SerializeField] private Image _progress;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;

        public void CreateElement(RemoteUI remoteUI, MediaFile media)
        {
            _remoteUI = remoteUI;
            _mediaFile = media;

            _name.text = media.name;
            _prevImage.texture = media.videoPrev;
            _gameObject = gameObject;

            _playButton.onClick.AddListener(PlayVideoEvent);
            _pauseButton.onClick.AddListener(PauseVideoEvent);
            _stopButton.onClick.AddListener(StopVideoEvent);
        }

        public void Select(NetMessage message)
        {
            _outline.enabled = true;

            if (message == null)
                return;

            if(message.totalTime > 0)
            {
                var t = message.curTime / message.totalTime;
                _progress.fillAmount = (float)t;
            }

        }

        public void Deselect()
        {
            _outline.enabled = false;

        }

        private void PlayVideoEvent()
        {
            Debug.Log("[Remote element] - Play");
            _remoteUI.RemotePlayVideo(_mediaFile.name);
        }

        private void PauseVideoEvent()
        {
            Debug.Log("[Remote element] - Pause");
            _remoteUI.RemotePauseVideo(_mediaFile.name);
        }

        private void StopVideoEvent()
        {
            Debug.Log("[Remote element] - Stop");
            _remoteUI.RemoteStopVideo(_mediaFile.name);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.RemoveAllListeners();
            _stopButton.onClick.RemoveAllListeners();
        }
    }
}
