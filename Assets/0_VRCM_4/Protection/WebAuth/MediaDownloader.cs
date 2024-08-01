using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using VRCM.Services.Protect;

namespace VRCM.Services.ContentDelivery
{
    public class MediaDownloader : MonoBehaviour
    {
        private bool isEditor = false;
        private string _localContentPath;

        [SerializeField] private Slider _progressBar;
        private float _currentProgress = 0f;


        private void Awake()
        {
            _progressBar.gameObject.SetActive(false);
            _progressBar.value = 0f;
#if UNITY_EDITOR
            isEditor = true;
#endif
        }

        private void Start()
        {
            if (!isEditor)
                _localContentPath = Path.Combine(Application.persistentDataPath, "360Content");
            else
                _localContentPath = Path.Combine("C:/", "360Content");

            if (!Directory.Exists(_localContentPath))
                Directory.CreateDirectory(_localContentPath);
        }

        public IEnumerator DownloadRoutine(TokenInfo token)
        {
            Debug.Log("[MediaDownloader] Start media sync...");
            if (token.VideoInfo.Length <= 0)
                yield return null;

            _currentProgress = 0f;
            _progressBar.value = _currentProgress;
            _progressBar.gameObject.SetActive(true);

            float completeProgress = 1f / token.VideoInfo.Length;


            foreach (VideoInfo videoInfo in token.VideoInfo)
            {
                string fileName = videoInfo.fileName;
                string filePath = Path.Combine(_localContentPath, fileName);

                if (!File.Exists(filePath))
                {
                    using (UnityWebRequest www = UnityWebRequest.Get(videoInfo.link))
                    {
                        yield return www.SendWebRequest();

                        if (www.result == UnityWebRequest.Result.Success)
                        {
                            File.WriteAllBytes(filePath, www.downloadHandler.data);
                            Debug.Log($"Файл {fileName} успешно скачан");

                            _currentProgress += completeProgress;
                            _progressBar.value = _currentProgress;
                        }
                        else
                        {
                            Debug.LogError($"Ошибка при скачивании файла {fileName}: {www.error}");
                        }
                    }
                }
                else
                {
                    Debug.Log($"Файл {fileName} уже существует, пропускаем");
                    _currentProgress += completeProgress;
                    _progressBar.value = _currentProgress;
                }
            }

            _progressBar.value = 1f;

            Debug.Log("[MediaDownloader] Start media complete");

        }
    }
}
