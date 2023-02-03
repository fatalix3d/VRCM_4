using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VRCM.Media;
using VRCM.Media.Theater;

namespace VRCM.Media.Theater.UI
{
    public class TheaterUI : MonoBehaviour
    {
        private Dictionary<string, TheaterElement> _elements;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;
        [SerializeField] private VideoPlayer _previewPlayer;
        [SerializeField] private RenderTexture _previewRT;
        public RenderTexture PreviewRT => _previewRT;

        private void Awake()
        {
            MediaLibrary.MediaLibraryLoaded += OnMediaLibraryLoaded;
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;
        }

        private void OnMediaLibraryLoaded(Dictionary<string, MediaFile> videos)
        {
            _elements = new Dictionary<string, TheaterElement>();

            foreach (KeyValuePair<string, MediaFile> video in videos)
            {
                var newElement = Instantiate(_elementPrefab, _root);
                var elementUI = newElement.GetComponent<TheaterElement>();
                elementUI.CreateElement(this, video.Value);
                _elements.Add(video.Key, elementUI);
            }
        }

        public bool PlayVideo(string videoID)
        {
            bool res = false;
            if (!_elements.ContainsKey(videoID))
                return res;

            // send to server play command.

            _previewPlayer.Stop();
            _previewPlayer.targetTexture.Release();

            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.RestorePreviewTexture();

            _previewPlayer.url = MediaLibrary.Instance.GetVideoUrl(videoID);
            _previewPlayer.Play();

            res = true;
            return res;
        }
    }
}
