using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VRCM.Media
{
    public class MediaLibrary : MonoBehaviour
    {
        public static MediaLibrary Instance { get; private set; }

        private Dictionary<string, MediaFile> _videos = new Dictionary<string, MediaFile>();
        private string _localContentPath;

        public Dictionary<string, MediaFile> Videos => _videos;
        public static event Action<Dictionary<string, MediaFile>> MediaLibraryLoaded;

        [SerializeField] private DemoTest _test;


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
            Init();
        }

        public void Init()
        {
            StartCoroutine(CheckVideoDirectory());
        }

        IEnumerator CheckVideoDirectory()
        {
#if UNITY_EDITOR
            _localContentPath = Path.Combine("C:/", "360Content");
#else
            _localContentPath = Path.Combine("/storage/emulated/0", "360Content");
#endif

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
                        mediaFile.name = files[i].Name;
                        mediaFile.path = files[i].FullName;
                        _videos.Add(files[i].Name, mediaFile);
                        Debug.Log($"[MediaLibrary] : Add {mediaFile.name}");
                    }
                }
            }

            MediaLibraryLoaded?.Invoke(Videos);

            _test.Init();
            yield return null;
        }

    }
}