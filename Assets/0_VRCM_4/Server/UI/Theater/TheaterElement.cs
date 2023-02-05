using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Media.Theater.UI;

namespace VRCM.Media.Theater.UI
{
    public class TheaterElement : MonoBehaviour
    {
        private TheaterUI _theater = null;
        private MediaFile _mediaFile;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _prevImage;
        private GameObject _gameObject;

        [SerializeField] private CanvasGroup _cnv;
        [SerializeField] private Button _button;
        [SerializeField] private Image _playIcon;
        [SerializeField] private Sprite[] _playStateSprite;

        // video preview
        [SerializeField] private Slider _videoPreviewProgress;
        private bool _videoPreview = false;

        private void Awake()
        {
            _button.onClick.AddListener(PlayVideoTrigger);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(PlayVideoTrigger);
        }

        public void ResetPreview()
        {
            _prevImage.texture = _mediaFile.videoPrev;
            _playIcon.sprite = _playStateSprite[1];

            _videoPreview = false;
            _videoPreviewProgress.value = 0f;
        }

        public void CreateElement(TheaterUI theater, MediaFile media)
        {
            _theater = theater;
            _mediaFile = media;

            _name.text = media.name;
            _prevImage.texture = media.videoPrev;
            _gameObject = gameObject;
        }

        public void PlayVideoTrigger()
        {
            if (_theater == null)
                return;

            _theater.PlayVideo(_mediaFile.name);
        }

        public void PlayPreviewVideo(float videoLength)
        {
            _prevImage.texture = _theater.PreviewRT;
            _playIcon.sprite = _playStateSprite[0];

            _videoPreviewProgress.maxValue = videoLength;
            _videoPreview = true;
        }

        public void PausePreview(bool isPaused)
        {
            if (isPaused)
            {
                _playIcon.sprite = _playStateSprite[1];
            }
            else
            {
                _playIcon.sprite = _playStateSprite[0];
            }
        }

        private void Update()
        {
            if (!_videoPreview)
                return;

            _videoPreviewProgress.value = (float)_theater.PreviewPlayer.time;
        }
    }
}
