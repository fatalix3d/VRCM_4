using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using UnityEngine;
using VRCM.Network.Messages;
using VRCM.Network.Client.VideoPlayer; 

namespace VRCM.Network.Client
{
    public class NetworkClient : MonoBehaviour
    {
        public static NetworkClient Instance { get; private set; } = null;
        private NetMessageDispatcher _networkMessageDispatcher;

        private bool _isConnected = false;
        private WebSocket _websocket;
        private ServerParams _serverAdress;

        public event Action<byte[]> OnRecieveMessage;
        public event Action<byte[]> OnSendMessage;

        [SerializeField] private NetMessage.Command _status = NetMessage.Command.VideoNotFound;
        public NetMessage.Command Status { get => _status; set => _status = value; }

        public bool IsConnected => _isConnected;

        [SerializeField] private ClientMediaPlayer _mediaPlayer;
        public ClientMediaPlayer MediaPlayer => _mediaPlayer;

        private Coroutine _autoPingRoutine = null;


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
            //StartSendData();
        }
        public void Setup(ServerParams sp)
        {
            if (!string.IsNullOrEmpty(sp.ip) && sp.port > 0)
            {
                _serverAdress = sp;
                _networkMessageDispatcher = new NetMessageDispatcher(this);
                Connect();

            }
        }

        private async void Connect()
        {
            if (_serverAdress == null)
            {
                Debug.Log("[NetworkClient] - Invalid server adress");
                return;
            }

            if (_isConnected)
            {
                Debug.Log("[NetworkClient] - Is already connected");
                return;
            }

            Debug.Log($"[NetworkClient] - Try connect to ws://{_serverAdress.ip}:{_serverAdress.port}/Echo");

            _websocket = new WebSocket($"ws://{_serverAdress.ip}:{_serverAdress.port}/Echo");

            _websocket.OnOpen += () =>
            {
                Debug.Log("[NetworkClient] - Connection open!");
                _isConnected = true;
            };

            _websocket.OnError += (e) =>
            {
                Debug.Log("[NetworkClient] - Error! : " + e);
                _isConnected = false;
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("[NetworkClient] - Connection closed!");
                _isConnected = false;
                _mediaPlayer.StopVideo();
                _status = NetMessage.Command.Setup;
            };

            _websocket.OnMessage += (bytes) =>
            {
                OnMessageIn(bytes);
            };
            
            await _websocket.Connect();
        }

        private void OnMessageIn(byte[] bytes)
        {
            try
            {
                string msg = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Debug.Log($"[NetworkClient] - Incoming msg : {msg}");

                OnRecieveMessage?.Invoke(bytes);
            }
            catch (Exception e)
            {
            }
        }

        public void SendMessage(NetMessage netMessage)
        {
            if (netMessage == null)
                return;

            byte[] bytes = BinarySerializer.Serialize(netMessage);

            if (bytes != null)
            {
                string msg = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Debug.Log($"[NetworkClient] - Sending msg : {msg}");

                _websocket.Send(bytes);
                OnSendMessage?.Invoke(bytes);
            }
        }

        // Auto sender
        private void StartSendData()
        {
            if (_autoPingRoutine != null)
            {
                StopCoroutine(_autoPingRoutine);
            }
            _autoPingRoutine = StartCoroutine(ClientAutoPing());
        }

        private void StopSendData()
        {
            if (_autoPingRoutine != null)
            {
                StopCoroutine(_autoPingRoutine);
            }
        }

        private IEnumerator ClientAutoPing()
        {
            while (true)
            {
                if (IsConnected)
                {
                    var resp = new NetMessage(_status);
                    resp.mediaName = MediaPlayer.MediaName;
                    resp.mediaDuration = MediaPlayer.MediaDuration;
                    SendMessage(resp);
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (_websocket == null)
                return;

            _websocket.DispatchMessageQueue();
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (_serverAdress == null)
                return; 

            if (pause)
            {
                if (_websocket != null)
                {
                    _websocket.Close();
                    _isConnected = false;
                }
            }
            else
            {
                Connect();
            }
        }

        private async void OnDisable()
        {
            if (_websocket != null)
            {
                if (_websocket.State == WebSocketState.Open)
                {
                    await _websocket.Close();
                }
            }
        }
    }
}
