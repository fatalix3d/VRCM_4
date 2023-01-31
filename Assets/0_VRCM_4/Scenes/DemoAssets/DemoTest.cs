using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RenderHeads.Media.AVProVideo;
using VRCM.Media;

public class DemoTest : MonoBehaviour
{
    [SerializeField] MediaPlayer _mediaPlayer = null;
    [SerializeField] TextMeshProUGUI _mediaName;
    [SerializeField] CanvasGroup _controlsGroup;

    [SerializeField] private int curIndex = 0;
    private List<string> _videos = new List<string>();
    private List<string> _names = new List<string>();
    private bool _wasPlayingBeforeTimelineDrag;
    [SerializeField] Slider _sliderTime = null;

    private bool show = true;
    [SerializeField] private CanvasGroup toggleGroup;

    public void ToggleUI()
    {
        show = !show;
        if (show){
            toggleGroup.alpha = 1;
            toggleGroup.blocksRaycasts = true;
        }
        else
        {
            toggleGroup.alpha = 0;
            toggleGroup.blocksRaycasts = false;
        }
    }

    private void Awake()
    {
        _mediaName.text = "Loading please wait";
    }

    public void Init()
    {
        foreach (KeyValuePair<string, MediaFile> file in MediaLibrary.Instance.Videos)
        {
            _names.Add(file.Value.name);
            _videos.Add(file.Value.path);
        }

        if (_videos.Count > 0)
        {

            _controlsGroup.interactable = true;
            _mediaName.text = $"Найдено {_videos.Count} файлов";
        }
        else
        {
            _mediaName.text = "Ошибка, файлы не обнаружены";

        }

    }

    private void OpenVideo()
    {
        if (_mediaPlayer.Control.IsPlaying())
        {
            _mediaPlayer.Control.Stop();
            _mediaPlayer.Control.CloseMedia();
        }
        _mediaName.text = _names[curIndex];
        bool isOpening = _mediaPlayer.OpenMedia(new MediaPath(_videos[curIndex], MediaPathType.AbsolutePathOrURL), autoPlay: true);

    }


    public void Play()
    {
        if (!_mediaPlayer.Control.IsPlaying() && _mediaPlayer.Control.CanPlay())
        {
            _mediaPlayer.Play();
        }
    }

    public void Pause()
    {
        if (_mediaPlayer.Control.IsPlaying())
        {
            _mediaPlayer.Pause();
        }
    }

    public void PreviousVideo()
    {
        curIndex --;
        if (curIndex < 0)
        {
            curIndex = _videos.Count - 1;
        }

        OpenVideo();
    }

    public void NextVideo()
    {
        curIndex++;
        if (curIndex > _videos.Count - 1)
        {
            curIndex = 0;
        }

        OpenVideo();
    }


    public void Forward10()
    {
        if (_mediaPlayer && _mediaPlayer.Control != null)
        {
            _mediaPlayer.Control.Seek(_mediaPlayer.Control.GetCurrentTime() + 10);
        }
    }

    public void Forward30()
    {
        if (_mediaPlayer && _mediaPlayer.Control != null)
        {
            _mediaPlayer.Control.Seek(_mediaPlayer.Control.GetCurrentTime() + 30);
        }
    }
}
