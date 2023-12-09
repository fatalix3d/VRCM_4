using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Linq;

public class LocalMediaPlayer : MonoBehaviour
{
    [SerializeField] private MediaPlayer _mediaPlayer;
    [SerializeField] private MeshRenderer _videoSphereRenderer;
    
    private string _localContentPath;
    private string _localVideoFile;

    private void Awake()
    {
        _mediaPlayer.Events.AddListener(HandleEvent);
    }

    private IEnumerator Start()
    {
        _localContentPath = Path.Combine("/storage/emulated/0", "360Content");
        Debug.Log($"local folder => {_localContentPath}");
        DirectoryInfo di = new DirectoryInfo(_localContentPath);

        if (!di.Exists)
        {
            Debug.Log($"[MediaLibrary] Error : directory not found");
            yield break;
        }

        string[] extensions = new string[] { ".avi", ".mp4", ".webm", ".mkv" };
        FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
        int filesLength = files.Length;

        if (filesLength == 0)
            yield break;

        _localVideoFile = files[0].FullName;
        LoadVideo();

        yield return null;
    } 

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            PlayVideo();
        }
        else
        {
            StopVideo();
        }
    }

    private void LoadVideo()
    {
        if (string.IsNullOrEmpty(_localVideoFile))
            return;

        _mediaPlayer.OpenMedia(new MediaPath(_localVideoFile, MediaPathType.AbsolutePathOrURL), autoPlay: true);
        _videoSphereRenderer.enabled = true;
    }

    private void PlayVideo()
    {
        _mediaPlayer.Control.SeekFast(0);
        _mediaPlayer.Control.Play();
        _videoSphereRenderer.enabled = true;

    }

    private void StopVideo()
    {
        _mediaPlayer.Control.Stop();
        _videoSphereRenderer.enabled = false;
    }

    void HandleEvent(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        if (eventType == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            StopVideo();
        }
    }

    private void OnDisable()
    {
        _mediaPlayer.Events.AddListener(HandleEvent);
    }
}
