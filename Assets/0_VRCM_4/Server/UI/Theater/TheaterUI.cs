using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Media;
using VRCM.Media.Theater;

namespace VRCM.Media.Theater.UI
{
    public class TheaterUI : MonoBehaviour
    {
        private Dictionary<string, TheaterElement> _videos;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;

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
            _videos = new Dictionary<string, TheaterElement>();

            foreach (KeyValuePair<string, MediaFile> video in videos)
            {
                var newElement = Instantiate(_elementPrefab, _root);
                var elementUI = newElement.GetComponent<TheaterElement>();
                elementUI.GO = newElement;
                elementUI.UpdateElement(video.Key, video.Value);
                _videos.Add(video.Key, elementUI);
            }
        }
    }
}
