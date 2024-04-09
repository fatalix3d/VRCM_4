using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VRCM.Services.Protect
{
    public class WebManager : MonoBehaviour
    {
        [SerializeField] private AuthService _authService;
        private string _token = string.Empty;
        private string _deviceId = string.Empty;

        // Токен авторизации
        private bool _isBusy = false;
        private bool _serviceAvailable = false;
        private string _authToken = "1|yebBBajnZyydOY33nUMXPd0EdCDmo5Aw10w54qpmaec9249b";

        private string _apiUrl = "http://5.188.76.227:3000/api/token";
        private string _apiPingUrl = "https://vrcm.ru/api/ping";
        private string _apiStatUrl = "https://vrcm.ru/api/stat";


        public event Action<string> InfoTextEvent;
        public event Action<bool> UILockEvent;

        private void Awake()
        {
            _deviceId = SystemInfo.deviceUniqueIdentifier;
        }

        public void LockUI(bool state)
        {
            UILockEvent?.Invoke(state);
        }

        public void Login(string token)
        {
            _token = token;
            StartCoroutine(LoginRoutine(_token));
        }

        public IEnumerator LoginRoutine(string tokenText)
        {
            _token = tokenText;
            string url = $"{_apiUrl}?token={_token}&deviceID={_deviceId}";
            //string url = $"{_apiUrl}?token={_token}&deviceID=00002";

            Debug.Log($"[WebManager] [Login] Token {_token}]");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.timeout = 5;
                webRequest.certificateHandler = new CertificateWhore();

                // Ожидаем завершения запроса
                yield return webRequest.SendWebRequest();

                // Проверяем наличие ошибок
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"[WebManager] [Login] Error, {webRequest.error} {webRequest.downloadHandler.text}");
                    InfoTextEvent?.Invoke("Ошибка авторизации");
                }
                else
                {
                    // Получаем JSON данные
                    string jsonResult = webRequest.downloadHandler.text;
                    if (!string.IsNullOrEmpty(jsonResult))
                    {
                        var token = JsonUtility.FromJson<TokenInfo>(jsonResult);
                        if(token != null)
                        {
                            Debug.Log($"Incoming data : {jsonResult}");
                            _authService.Login(token);
                        }
                        else
                        {
                            Debug.Log($"[WebManager] [Login] Error, token is null");
                        }
                    }
                    else
                    {
                        Debug.Log($"[WebManager] [Login] Error, invalid json");
                    }
                }
            }
        }

        public void InfoMessage(string msg)
        {
            InfoTextEvent?.Invoke(msg);
        }


        public void SendStats(string statsJson)
        {
            if (_isBusy)
            {
                Debug.Log("[WebManager] Is busy!");
                return;
            }

            _isBusy = true;
            StartCoroutine(SendStatsRoutine(statsJson));
        }


        IEnumerator SendStatsRoutine(string statsJson)
        {
            InfoTextEvent?.Invoke("Подготовка...");

            _serviceAvailable = false;
            UnityWebRequest webRequestPing = UnityWebRequest.Get(_apiPingUrl);
            
                webRequestPing.certificateHandler = new CertificateWhore();
            webRequestPing.SetRequestHeader("Authorization", $"Bearer {_authToken}");

            // Ожидаем завершения запроса
            yield return webRequestPing.SendWebRequest();

            // Проверяем наличие ошибок
            if (webRequestPing.result == UnityWebRequest.Result.ConnectionError ||
                webRequestPing.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequestPing.error);
                InfoTextEvent?.Invoke("Ошибка, сервер не доступен");
            }
            else
            {
                // Получаем JSON данные
                string jsonResult = webRequestPing.downloadHandler.text;
                Debug.Log(jsonResult);
                _serviceAvailable = true;
                Debug.Log("[WebManager] Ok, stats server online");
                InfoTextEvent?.Invoke("Ок, сервер доступен");
            }

            if (!_serviceAvailable)
            {
                Debug.Log("[WebManager] Error, stats server is offline");
                _isBusy = false;
                yield return null;
            }

            UnityWebRequest webRequestStats = new UnityWebRequest(_apiStatUrl, "POST");

            webRequestStats.SetRequestHeader("Authorization", "Bearer " + _authToken);

            // Установка тела запроса с JSON-данными
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(statsJson);
            webRequestStats.uploadHandler = new UploadHandlerRaw(jsonBytes);
            webRequestStats.downloadHandler = new DownloadHandlerBuffer();
            webRequestStats.SetRequestHeader("Content-Type", "application/json");

            // Отправка запроса и ожидание ответа
            yield return webRequestStats.SendWebRequest();

            // Проверяем наличие ошибок
            if (webRequestStats.result == UnityWebRequest.Result.ConnectionError ||
                    webRequestStats.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("[WebManager] Error, send failed");
                Debug.LogError("Error: " + webRequestStats.error);
                InfoTextEvent?.Invoke("Ошибка, при отправке на сервер");

                _isBusy = false;
                yield return null;
            }
            else
            {
                string jsonResult = webRequestStats.downloadHandler.text;
                Debug.Log(jsonResult);
                Debug.Log("[WebManager] Ok, send complete");
                InfoTextEvent?.Invoke("Ок, данные успешно отправленны");

                AuthService.Instance.Storage.RemoveAllRecords();
            }

            _isBusy = false;
        }
    }
}
