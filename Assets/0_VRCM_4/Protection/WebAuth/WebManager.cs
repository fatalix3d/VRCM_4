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
        private string _deviceId = string.Empty;

        private string _apiUrl = "http://5.188.76.227:3000/api/token";
        private string _apiPingUrl = "https://rtstat.ru/api/ping1";

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
                webRequest.certificateHandler = new CertificateWhore();

                // ������� ���������� �������
                yield return webRequest.SendWebRequest();

                // ��������� ������� ������
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"[WebManager] [Login] Error, {webRequest.error} {webRequest.downloadHandler.text}");
                    InfoTextEvent?.Invoke("������ �����������");
                }
                else
                {
                    // �������� JSON ������
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

        IEnumerator Ping()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(_apiPingUrl))
            {
                webRequest.certificateHandler = new CertificateWhore();

                // ������� ���������� �������
                yield return webRequest.SendWebRequest();

                // ��������� ������� ������
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    // �������� JSON ������
                    string jsonResult = webRequest.downloadHandler.text;

                    // ����� �� ������ ���������� jsonResult, ��������, � ������� JsonUtility
                    // ��������, ���� � ��� ���� ����� Data, �������������� ��������� ������ JSON
                    // Data data = JsonUtility.FromJson<Data>(jsonResult);

                    // ������� ���������� ������ � �������
                    Debug.Log(jsonResult);
                }
            }
        }
    }
}
