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

        private void Awake()
        {
            _button.onClick.AddListener(PlayVideoTrigger);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(PlayVideoTrigger);
        }

        public void RestorePreviewTexture()
        {
            _prevImage.texture = _mediaFile.videoPrev;
            _playIcon.sprite = _playStateSprite[1];
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

            if (_theater.PlayVideo(_mediaFile.name))
            {
                _prevImage.texture = _theater.PreviewRT;
                _playIcon.sprite = _playStateSprite[0];
            }
        }
    }
}
