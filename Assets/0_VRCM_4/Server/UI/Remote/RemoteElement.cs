using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using VRCM.Network.Messages;

namespace VRCM.Media.Remote.UI
{
    public class RemoteElement : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private RemoteUI _remoteUI = null;
        private MediaFile _mediaFile;
        private GameObject _gameObject;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _mediaDuration;
        [SerializeField] private RawImage _prevImage;
        [SerializeField] private Outline _outline;
        [SerializeField] private Image _progress;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;

        [SerializeField] private Color _baseClr, _playClr, _pauseClr, _stopClr;

        private bool _isActive = false;
        private double _totalTime = 0;

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

            _isActive = true;
            _mediaDuration.text = message.mediaDuration;

            if (message.totalTime > 0)
            {
                _totalTime = message.totalTime;
                var t = message.curTime / message.totalTime;
                _progress.fillAmount = (float)t;
            }

            if (message.command == NetMessage.Command.Play)
            {
                _playButton.image.color = _playClr;
            }
            else
            {
                _playButton.image.color = _baseClr;
            }

            if (message.command == NetMessage.Command.Pause)
            {
                _pauseButton.image.color = _pauseClr;
            }
            else
            {
                _pauseButton.image.color = _baseClr;
            }

            if (message.command == NetMessage.Command.Stop)
            {
                _stopButton.image.color = _stopClr;
            }
            else
            {
                _stopButton.image.color = _baseClr;
            }
        }

        public void Deselect()
        {
            _isActive = false;
            _totalTime = 0;

            _outline.enabled = false;
            _progress.fillAmount = 0f;
            _mediaDuration.text = string.Empty;
            _playButton.image.color = _baseClr;
            _pauseButton.image.color = _baseClr;
            _stopButton.image.color = _baseClr;
        }

        private void PlayVideoEvent()
        {
            Debug.Log("[Remote element] - Play");
            _remoteUI.RemotePlayVideo(_mediaFile.name);

            // optional
            Bootstrapper.Instance.Theater.StopPreviewPlayer();
        }

        private void PauseVideoEvent()
        {
            Debug.Log("[Remote element] - Pause");
            _remoteUI.RemotePauseVideo(_mediaFile.name);
        }

        private void StopVideoEvent()
        {
            Debug.Log("[Remote element] - Stop");
            _stopButton.image.color = _stopClr;
            _remoteUI.RemoteStopVideo(_mediaFile.name);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.RemoveAllListeners();
            _stopButton.onClick.RemoveAllListeners();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isActive)
                return;

            if (_totalTime > 0)
            {
                TrySkip(eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isActive)
                return;

            if (_totalTime > 0)
            {
                TrySkip(eventData);
            }
        }

        private void TrySkip(PointerEventData eventData)
        {
            if (!_isActive)
                return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_progress.rectTransform, eventData.position, null, out localPoint))
            {
                float pct = Mathf.InverseLerp(_progress.rectTransform.rect.xMin, _progress.rectTransform.rect.xMax, localPoint.x);
                SeekTo(pct);
            }
        }

        private void SeekTo(float pct)
        {
            if (!_isActive)
                return;

            if (_totalTime > 0)
            {
                var timeToSeek = Mathf.Lerp(0, (float)_totalTime, pct);

                _remoteUI.RemoteSeek(timeToSeek, _mediaFile.name);
            }
        }
    }
}
