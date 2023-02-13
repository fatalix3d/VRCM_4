using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace VRCM.Media
{
    public class MediaLibrary : MonoBehaviour
    {
        [SerializeField] private bool _server = false;
        public static MediaLibrary Instance { get; private set; }

        private Dictionary<string, MediaFile> _videos = new Dictionary<string, MediaFile>();
        private string _localContentPath;

        public Dictionary<string, MediaFile> Videos => _videos;
        public static event Action<Dictionary<string, MediaFile>> MediaLibraryLoaded;

        [SerializeField] private VideoPlayer ptvp;
        private bool ThumbnailReady = false;
        private CancellationTokenSource _cts;

        private bool isEditor = false;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
#if UNITY_EDITOR
            isEditor = true;
#endif
            Init();
        }

        public void Init()
        {
            ptvp.renderMode = VideoRenderMode.APIOnly;
            ptvp.prepareCompleted += Prepared;
            ptvp.sendFrameReadyEvents = true;
            ptvp.frameReady += FrameReady;

            StartCoroutine(CheckVideoDirectory());
        }

        IEnumerator CheckVideoDirectory()
        {

            if (!isEditor)
            {
                //_localContentPath = Path.Combine("/storage/emulated/0", "360Content");
                _localContentPath = Path.Combine(Application.persistentDataPath, "360Content");
            }
            else
            {
                _localContentPath = Path.Combine("C:/", "360Content");

            }

            Debug.Log($"[MediaLibrary] data path : {_localContentPath}");

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

            for (int i = 0; i < filesLength; i++)
            {
                if (File.Exists(files[i].FullName))
                {
                    if (files[i].Extension == ".avi" || files[i].Extension == ".mp4" || files[i].Extension == ".webm" || files[i].Extension == ".mkv")
                    {
                        MediaFile mediaFile = new MediaFile();
                        //mediaFile.name = files[i].Name;
                        string nameNoExt = Path.GetFileNameWithoutExtension(files[i].FullName);
                        mediaFile.name = nameNoExt;
                        mediaFile.path = files[i].FullName;

                        if (_server)
                        {
                            ThumbnailReady = false;
                            ptvp.url = files[i].FullName;
                            ptvp.Prepare();
                            yield return new WaitUntil(CheckThumbnail);
                            mediaFile.videoPrev = Resize(ptvp.texture, 256, 256);
                        }

                        _videos.Add(nameNoExt, mediaFile);
                        Debug.Log($"[MediaLibrary] : Add {mediaFile.name}");
                    }
                }
            }

            MediaLibraryLoaded?.Invoke(Videos);
            yield return null;
        }

        public string GetVideoUrl(string videoId)
        {
            if (_videos.ContainsKey(videoId))
            {
                return _videos[videoId].path;
            }
            else
            {
                return string.Empty;
            }
        }


        bool CheckThumbnail()
        {
            return (ThumbnailReady);
        }

        private void Prepared(VideoPlayer vp)
        {
            ptvp.time = vp.length / 2;
            vp.Play();
        }

        private void FrameReady(VideoPlayer vp, long frameIndex)
        {
            vp.frame = frameIndex + 1;
            ThumbnailReady = true;
        }

        private Texture2D Resize(Texture texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }


    }
}