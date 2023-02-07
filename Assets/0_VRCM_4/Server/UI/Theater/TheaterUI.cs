using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VRCM.Media;
using VRCM.Media.Theater;
using VRCM.Network.Messages;
using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos.UI;
using TMPro;

namespace VRCM.Media.Theater.UI
{
    public class TheaterUI : MonoBehaviour
    {
        private Dictionary<string, TheaterElement> _elements;
        [SerializeField] private RectTransform _root;
        [SerializeField] private GameObject _elementPrefab;

        // video preview player
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

        private void OnDisable()
        {
            MediaLibrary.MediaLibraryLoaded -= OnMediaLibraryLoaded;
            Bootstrapper.Instance.Lobby.NoActivePlayerEvent -= StopPreviewPlayer;
            _previewPlayer.Events.RemoveAllListeners();
        }

        private void Start()
        {
            CreateTimelineDragEvents();
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

            if (_previewPlayer.Control.IsPlaying() && videoID == _curVideoId)
            {
                // send to server pause command.
                //...
                Bootstrapper.Instance.Server.SendMessageAll(Network.Messages.NetMessage.Command.Pause, videoID);

                _previewPlayer.Pause();
                _elements[_curVideoId].PausePreview(true);
                return;
            }

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

            _previewPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);

            _elements[_curVideoId].PlayPreviewVideo((float)_previewPlayer.Info.GetDuration());

            _mediaNameLabel.text = videoID;

        }

        private void StopPreviewPlayer()
        {
            _curVideoId = string.Empty;
            _mediaNameLabel.text = string.Empty;

            _previewPlayer.Control.Stop();
            _previewPlayer.CloseMedia();

            foreach (KeyValuePair<string, TheaterElement> element in _elements)
                element.Value.ResetPreview();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                StopPreviewPlayer();

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
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                _isHoveringOverTimeline = true;
                _sliderTime.transform.localScale = new Vector3(1f, 2.5f, 1f);
            }
        }

        private void OnTimelineEndHover(PointerEventData eventData)
        {
            _isHoveringOverTimeline = false;
            _sliderTime.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private void OnTimeSliderBeginDrag()
        {
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
            if (_previewPlayer && _previewPlayer.Control != null)
            {
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _previewPlayer.Play();
                    _wasPlayingBeforeTimelineDrag = false;

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
        }

        void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
        {
            if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                StopPreviewPlayer();
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
    }
}
