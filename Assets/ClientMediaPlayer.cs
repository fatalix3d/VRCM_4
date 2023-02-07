using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using VRCM.Media;
using VRCM.Network.Messages;

namespace VRCM.Network.Client.VideoPlayer {
    public class ClientMediaPlayer : MonoBehaviour
    {
        // Media player
        [SerializeField] private MediaPlayer _mediaPlayer;
        [SerializeField] private NetworkClient _client;

        private string _mediaName;
        private string _mediaDuration;

        public string MediaName => _mediaName;
        public string MediaDuration => _mediaDuration;

        private void Awake()
        {
            _mediaPlayer.Events.AddListener(HandleEvent);
        }

        private void OnDisable()
        {
            _mediaPlayer.Events.RemoveAllListeners();
        }

        void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
        {
            if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
            {
                StopVideo();
            }
        }

        public bool PlayVideo(string mediaName)
        {
            if (_mediaName != mediaName)
            {
                _mediaPlayer.Control.Stop();
                _mediaPlayer.CloseMedia();
                _mediaName = mediaName;
            }

            string path = MediaLibrary.Instance.GetVideoUrl(mediaName);

            if (string.IsNullOrEmpty(path))
                return false;

            _mediaPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            _client.Status = NetMessage.Command.Play;
            return true;
        }

        public bool PauseVideo(string mediaName)
        {
            if (_mediaName != mediaName)
                return false;

            if (_mediaPlayer.Control.IsPlaying())
            {
                _mediaPlayer.Control.Pause();
                _client.Status = NetMessage.Command.Pause;
            }

            return true;
        }

        public bool ResumeVideo(string mediaName)
        {
            if (_mediaName != mediaName)
                return false;

            if (!_mediaPlayer.Control.IsPaused())
            {
                return false;
            }

            _mediaPlayer.Control.Play();
            _client.Status = NetMessage.Command.Play;
            return true;
        }

        public bool SeekVideo(string mediaName, double time)
        {
            if (_mediaName != mediaName)
                return false;
            _client.Status = NetMessage.Command.Seek;

            bool isPlayed = _mediaPlayer.Control.IsPlaying();

            if (_mediaPlayer.Control.IsPlaying())
            {
                _mediaPlayer.Control.Pause();
            }

            _mediaPlayer.Control.Seek(time);

            if(isPlayed)
            _mediaPlayer.Control.Play();
            _client.Status = NetMessage.Command.Play;

            return true;
        }

        public void StopVideo()
        {
            _mediaPlayer.Control.Stop();
            _mediaPlayer.CloseMedia();
            _mediaName = string.Empty;

            _client.Status = NetMessage.Command.Stop;

            var resp = new NetMessage(NetMessage.Command.Stop);
            _client.SendMessage(resp);
        }

        private void Update()
        {
            if (_mediaPlayer.Info != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                string t1 = Helper.GetTimeString((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime), false);
                string d1 = Helper.GetTimeString(timelineRange.duration, false);
                _mediaDuration = string.Format("{0} / {1}", t1, d1);
            }
        }

        private TimeRange GetTimelineRange()
        {
            if (_mediaPlayer.Info != null)
            {
                return Helper.GetTimelineRange(_mediaPlayer.Info.GetDuration(), _mediaPlayer.Control.GetSeekableTimes());
            }
            return new TimeRange();
        }
    }

}