using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRCM.Media.Remote.UI
{
    public class RemoteUI : MonoBehaviour
    {
        private Dictionary<string, RemoteElement> _elements;

        [SerializeField] private CanvasGroup _window;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;

        private void Awake()
        {
            MediaLibrary.MediaLibraryLoaded += OnMediaLibraryLoaded;
        }

        public void Setup()
        {
            Bootstrapper.Instance.Lobby.RemoteModeChangeEvent += OnRemoteModeChangeEvent;
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;
            Bootstrapper.Instance.Lobby.RemoteModeChangeEvent -= OnRemoteModeChangeEvent;
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
    }
}
