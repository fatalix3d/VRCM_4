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

        // video preview player
        private string _curVideoId = string.Empty;
        [SerializeField] private VideoPlayer _previewPlayer;
        [SerializeField] private RenderTexture _previewRT;

        public VideoPlayer PreviewPlayer => _previewPlayer;
        public RenderTexture PreviewRT => _previewRT;

        private void Awake()
        {
            MediaLibrary.MediaLibraryLoaded += OnMediaLibraryLoaded;
            _previewPlayer.prepareCompleted += prepareCompleted;
            _previewPlayer.loopPointReached += loopPointReached;
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;
            _previewPlayer.prepareCompleted -= prepareCompleted;
            _previewPlayer.loopPointReached -= loopPointReached;
            Bootstrapper.Instance.Lobby.NoActivePlayerEvent -= StopPreviewPlayer;
        }

        private void OnMediaLibraryLoaded(Dictionary<string, MediaFile> videos)
        {
            Bootstrapper.Instance.Lobby.NoActivePlayerEvent += StopPreviewPlayer;

            _elements = new Dictionary<string, TheaterElement>();

            foreach (KeyValuePair<string, MediaFile> video in videos)
            {
                var newElement = Instantiate(_elementPrefab, _root);
                var elementUI = newElement.GetComponent<TheaterElement>();
                elementUI.CreateElement(this, video.Value);
                _elements.Add(video.Key, elementUI);
            }
        }

        public void PlayVideo(string videoID)
        {
            if (!_elements.ContainsKey(videoID))
                return;

            if (_previewPlayer.isPlaying && videoID == _curVideoId)
            {
                // send to server pause command.
                //...
                Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Pause);

                _previewPlayer.Pause();
                _elements[_curVideoId].PausePreview(true);
                return;
            }

            if (_previewPlayer.isPaused && videoID == _curVideoId)
            {
                // send to server play command.
                //...
                Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Resume, videoID);

                _previewPlayer.Play();
                _elements[_curVideoId].PausePreview(false);
                return;
            }


            _curVideoId = videoID;
            _previewPlayer.Stop();
            _previewPlayer.targetTexture.Release();

            // send to server play command.
            //...
            Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Play, videoID);

            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.ResetPreview();

            _previewPlayer.url = MediaLibrary.Instance.GetVideoUrl(videoID);
            _previewPlayer.Prepare();
        }

        private void prepareCompleted(VideoPlayer vp)
        {
            if (!_elements.ContainsKey(_curVideoId))
                return;

            vp.Play();

            _elements[_curVideoId].PlayPreviewVideo((float)_previewPlayer.length);
            Debug.Log(_previewPlayer.length);

        }

        private void loopPointReached(VideoPlayer source)
        {
            StopPreviewPlayer();
        }

        private void StopPreviewPlayer()
        {
            _curVideoId = string.Empty;

            _previewPlayer.Stop();
            _previewPlayer.targetTexture.Release();

            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.ResetPreview();
        }
    }
}
