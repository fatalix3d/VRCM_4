using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace VRCM.Services.Protect
{
    public class WebManager : MonoBehaviour
    {
        [SerializeField] private AuthService _authService;
        private string _token = string.Empty;
        private string _apiUrl = "https://rtstat.ru/api/get_user_info/";
        private string _apiPingUrl = "https://rtstat.ru/api/ping";

        public event Action<string> InfoTextEvent;
        public event Action<bool> UILockEvent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Send login cmd");
                StartCoroutine(LoginRoutine(_token));
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Send ping cmd");
                StartCoroutine(Ping());
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Send stats cmd");
                //StartCoroutine(SendPostRequest());
            }
        }

        public void LockUI(bool flag)
        {
            UILockEvent?.Invoke(flag);
        }

        public void Login(string token)
        {
            _token = token;
            StartCoroutine(LoginRoutine(_token));
        }

        public IEnumerator LoginRoutine(string tokenText)
        {
            _token = tokenText;
            string _tokenUrl = _apiUrl + _token;
            Debug.Log($"[WebManager] [Login] Token {_token}]");

            using (UnityWebRequest webRequest = UnityWebRequest.Get(_tokenUrl))
            {
                webRequest.certificateHandler = new CertificateWhore();

                // Ожидаем завершения запроса
                yield return webRequest.SendWebRequest();

                // Проверяем наличие ошибок
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"[WebManager] [Login] Error, {webRequest.error}");
                    InfoTextEvent?.Invoke("Ошибка, такого токена нет");
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
                            Debug.Log(jsonResult);
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

        IEnumerator Ping()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(_apiPingUrl))
            {
                webRequest.certificateHandler = new CertificateWhore();

                // Ожидаем завершения запроса
                yield return webRequest.SendWebRequest();

                // Проверяем наличие ошибок
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    // Получаем JSON данные
                    string jsonResult = webRequest.downloadHandler.text;

                    // Далее вы можете обработать jsonResult, например, с помощью JsonUtility
                    // Например, если у вас есть класс Data, представляющий структуру данных JSON
                    // Data data = JsonUtility.FromJson<Data>(jsonResult);

                    // Выводим полученные данные в консоль
                    Debug.Log(jsonResult);
                }
            }
        }
    }
}
