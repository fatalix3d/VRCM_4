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
            string path = MediaLibrary.Instance.GetVideoUrl(mediaName);

            if (string.IsNullOrEmpty(path))
                return false;

            if (_mediaName != mediaName)
                _mediaName = mediaName;

            if (_mediaPlayer.Control.IsPlaying() || _mediaPlayer.Control.IsPaused())
            {
                _mediaPlayer.Control.Stop();
                _mediaPlayer.CloseMedia();
            }

            _mediaPlayer.OpenMedia(new MediaPath(path, MediaPathType.AbsolutePathOrURL), autoPlay: true);

            return true;
        }

        public bool PauseVideo(string mediaName)
        {
            if (_mediaName != mediaName)
                return false;

            if (_mediaPlayer.Control.IsPlaying())
            {
                _mediaPlayer.Control.Pause();
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

            return true;
        }

        public bool SeekVideo(string mediaName, double time)
        {
            if (_mediaName != mediaName)
                return false;

            if (_mediaPlayer.Control.IsPlaying())
            {
                _mediaPlayer.Control.Pause();
            }

            _mediaPlayer.Control.Seek(time);

            _mediaPlayer.Control.Play();

            return true;
        }

        public void StopVideo()
        {
            _mediaPlayer.Control.Stop();
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
