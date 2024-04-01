using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VRCM.Media;
using VRCM.Media.Theater;
using VRCM.Services.Protect;
using VRCM.Network.Messages;
using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos.UI;
using TMPro;

namespace VRCM.Media.Theater.UI
{
    public class TheaterUI : MonoBehaviour
    {
        private Dictionary<string, TheaterElement> _elements = new Dictionary<string, TheaterElement>();
        private bool _remote = false;

        [SerializeField] private CanvasGroup _window;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;
        [SerializeField] private Button _forceStopButton;

        // video preview player
        [SerializeField] private CanvasGroup _timeLineCnv;
        private string _curVideoId = string.Empty;
        private bool _isHoveringOverTimeline;
        private bool _wasPlayingBeforeTimelineDrag;
        [SerializeField] private MediaPlayer _previewPlayer;
        [SerializeField] private Slider _sliderTime;
        [SerializeField] private TextMeshProUGUI _textTimeDuration;
        [SerializeField] private TextMeshProUGUI _mediaNameLabel;
        [SerializeField] HorizontalSegmentsPrimitive _segmentsProgress = null;
        public MediaPlayer PreviewPlayer => _previewPlayer;
        public TextMeshProUGUI MediaNameLabel=> _mediaNameLabel;


        private void Awake()
        {
            MediaLibrary.MediaLibraryLoaded += OnMediaLibraryLoaded;
            _previewPlayer.Events.AddListener(HandleEvent);
        }
        public void Setup()
        {
            Bootstrapper.Instance.Lobby.RemoteModeChangeEvent += OnRemoteModeChangeEvent;
            CreateTimelineDragEvents();
            _forceStopButton.onClick.AddListener(StopVideo);
        }

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;

            if (Bootstrapper.Instance != null)
            {
                if (Bootstrapper.Instance.Lobby != null)
                {
                    Bootstrapper.Instance.Lobby.NoActivePlayerEvent -= StopPreviewPlayer;
                    Bootstrapper.Instance.Lobby.RemoteModeChangeEvent -= OnRemoteModeChangeEvent;
                }
            }

            _previewPlayer.Events.RemoveAllListeners();
            _forceStopButton.onClick.RemoveAllListeners();
        }

        private void OnMediaLibraryLoaded(Dictionary<string, MediaFile> videos)
        {
            Bootstrapper.Instance.Lobby.NoActivePlayerEvent += StopPreviewPlayer;

            //_elements = new Dictionary<string, TheaterElement>();

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
            int clientCount = Bootstrapper.Instance.Lobby.Players.Count;
            if (!_elements.ContainsKey(videoID))
                return;

            if (_previewPlayer.Control.IsPlaying() && videoID == _curVideoId)
            {
                // send to server pause command.
                //...
                Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Pause, videoID);

                _previewPlayer.Pause();
                _elements[_curVideoId].PausePreview(true);

                // Session record add .. 
                AuthService.Instance.Storage.AddPauseRecord(videoID, clientCount);
                return;
            }

            // Session record add .. 
            AuthService.Instance.Storage.AddPlayRecord(videoID, clientCount);

            if (_previewPlayer.Control.IsPaused() && videoID == _curVideoId)
            {
                // send to server play command.
                //...
                Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Resume, videoID);

                _previewPlayer.Play();
                _elements[_curVideoId].PausePreview(false);
                return;
            }

            _curVideoId = videoID;
            _previewPlayer.Control.Stop();

            // send to server play command.
            //...
            Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Play, videoID);

            // play preview
            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.ResetPreview();

            if (!_elements.ContainsKey(_curVideoId))
                return;

            string path = MediaLibrary.Instance.GetVideoUrl(videoID);
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Invalid / Empty path");
                return;
            }

            System.GC.Collect();

            _previewPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);

            _elements[_curVideoId].PlayPreviewVideo((float)_previewPlayer.Info.GetDuration());

            _mediaNameLabel.text = videoID;

        }

        public void StopVideo()
        {
            // send to server play command.
            //... 
            Bootstrapper.Instance.Server.SendMessageAll(NetMessage.Command.Stop);
            StopPreviewPlayer();

            // Session record add .. 
            int clientCount = Bootstrapper.Instance.Lobby.Players.Count;
            AuthService.Instance.Storage.AddStopRecord(_curVideoId, clientCount);
        }

        public void StopPreviewPlayer()
        {
            _curVideoId = string.Empty;

            if(_mediaNameLabel!=null)
            _mediaNameLabel.text = string.Empty;

            if (_previewPlayer != null && _previewPlayer.Control != null)
            {
                _previewPlayer.Control.Stop();
                _previewPlayer.CloseMedia();
            }

            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.ResetPreview();
        }

        private void Update()
        {
            if (_remote)
                return;

            if (_previewPlayer.Info != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                if (_sliderTime && !_isHoveringOverTimeline)
                {
                    double t = 0.0;
                    if (timelineRange.duration > 0.0)
                    {
                        t = ((_previewPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                    }
                    _sliderTime.value = Mathf.Clamp01((float)t);
                }

                // Update progress segment
                if (_segmentsProgress)
                {
                    TimeRanges times = _previewPlayer.Control.GetBufferedTimes();
                    float[] ranges = null;
                    if (times.Count > 0 && timelineRange.Duration > 0.0)
                    {
                        ranges = new float[2];
                        double x1 = (times.MinTime - timelineRange.startTime) / timelineRange.duration;
                        double x2 = ((_previewPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                        ranges[0] = Mathf.Max(0f, (float)x1);
                        ranges[1] = Mathf.Min(1f, (float)x2);
                    }
                    _segmentsProgress.Segments = ranges;
                }

                // Update time/duration text display
                if (_textTimeDuration)
                {
                    string t1 = Helper.GetTimeString((_previewPlayer.Control.GetCurrentTime() - timelineRange.startTime), false);
                    string d1 = Helper.GetTimeString(timelineRange.duration, false);
                    _textTimeDuration.text = string.Format("{0} / {1}", t1, d1);
                }
            }
        }

        private void CreateTimelineDragEvents()
        {
            EventTrigger trigger = _sliderTime.gameObject.GetComponent<EventTrigger>();

            if (trigger != null)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { OnTimeSliderBeginDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener((data) => { OnTimeSliderDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((data) => { OnTimeSliderEndDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { OnTimelineBeginHover((PointerEventData)data); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener((data) => { OnTimelineEndHover((PointerEventData)data); });
                trigger.triggers.Add(entry);
            }
        }

        private void OnTimelineBeginHover(PointerEventData eventData)
        {
            if (_remote)
                return;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                _isHoveringOverTimeline = true;
                _sliderTime.transform.localScale = new Vector3(1f, 2.5f, 1f);
            }
        }

        private void OnTimelineEndHover(PointerEventData eventData)
        {
            if (_remote)
                return;

            _isHoveringOverTimeline = false;
            _sliderTime.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private void OnTimeSliderBeginDrag()
        {
            if (_remote)
                return;

            if (_previewPlayer && _previewPlayer.Control != null)
            {
                _wasPlayingBeforeTimelineDrag = _previewPlayer.Control.IsPlaying();
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _previewPlayer.Pause();
                }
                OnTimeSliderDrag();
            }
        }

        private void OnTimeSliderDrag()
        {
            if (_remote)
                return;

            if (_previewPlayer && _previewPlayer.Control != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                double time = timelineRange.startTime + (_sliderTime.value * timelineRange.duration);
                _previewPlayer.Control.Seek(time);
                _isHoveringOverTimeline = true;
            }
        }

        private void OnTimeSliderEndDrag()
        {
            if (_remote)
                return;

            if (_previewPlayer && _previewPlayer.Control != null)
            {
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _previewPlayer.Play();
                    _wasPlayingBeforeTimelineDrag = false;                   
                }

                TimeRange timelineRange = GetTimelineRange();
                double time = timelineRange.startTime + (_sliderTime.value * timelineRange.duration);

                // send to server play command.
                //...
                NetMessage netMessage = new NetMessage(NetMessage.Command.Seek);
                netMessage.seekTime = time;
                netMessage.mediaName = _curVideoId;
                Bootstrapper.Instance.Server.SendMessageAll(netMessage);
            }
        }

        void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
        {
            if (_remote)
                return;

            if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                StopPreviewPlayer();

                // Session record add .. 
                int clientCount = Bootstrapper.Instance.Lobby.Players.Count;
                AuthService.Instance.Storage.AddStopRecord(_curVideoId, clientCount);
            }
        }

        private TimeRange GetTimelineRange()
        {
            if (_previewPlayer.Info != null)
            {
                return Helper.GetTimelineRange(_previewPlayer.Info.GetDuration(), _previewPlayer.Control.GetSeekableTimes());
            }
            return new TimeRange();
        }

        private void OnRemoteModeChangeEvent(bool remote)
        {
            _remote = remote;
            _sliderTime.interactable = !remote;

            if (remote)
            {
                _timeLineCnv.alpha = 0;
                _window.alpha = 0;
                _window.blocksRaycasts = false;
            }
            else
            {
                _timeLineCnv.alpha = 1;
                _window.alpha = 1;
                _window.blocksRaycasts = true;
            }
        }
    }
}
