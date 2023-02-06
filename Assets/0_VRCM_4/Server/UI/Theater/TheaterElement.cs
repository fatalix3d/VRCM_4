using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Media.Theater.UI;
using RenderHeads.Media.AVProVideo;
using DG.Tweening;

namespace VRCM.Media.Theater.UI
{
    public class TheaterElement : MonoBehaviour
    {
        private TheaterUI _theater = null;
        private MediaFile _mediaFile;
        private GameObject _gameObject;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _prevImage;
        [SerializeField] private DisplayUGUI _prevVideo;
        [SerializeField] private CanvasGroup _previewVideoCanvas;
        [SerializeField] private Outline _outline;
        [SerializeField] private CanvasGroup _cnv;
        [SerializeField] private Button _button;
        [SerializeField] private Image _playIcon;
        [SerializeField] private Sprite[] _playStateSprite;

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
            _previewVideoCanvas.alpha = 0;
            _prevVideo.enabled = false;
            _prevVideo.CurrentMediaPlayer = null;
            _playIcon.sprite = _playStateSprite[1];
            _outline.enabled = false;
        }

        public void CreateElement(TheaterUI theater, MediaFile media)
        {
            _theater = theater;
            _mediaFile = media;

            _name.text = media.name;
            _prevImage.texture = media.videoPrev;
            _gameObject = gameObject;
            _prevVideo.DefaultTexture = media.videoPrev;
        }

        public void PlayVideoTrigger()
        {
            if (_theater == null)
                return;

            _theater.PlayVideo(_mediaFile.name);
        }

        public void PlayPreviewVideo(float videoLength)
        {
            _prevVideo.CurrentMediaPlayer = _theater.PreviewPlayer;
            _prevVideo.enabled = true;

            _playIcon.sprite = _playStateSprite[0];
            _previewVideoCanvas.DOFade(1f, 0.35f);

            _outline.enabled = true;
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
    }
}
