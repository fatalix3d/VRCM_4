using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Client;
using VRCM.Network.Configuration;
using VRCM.Network.Broadcast;
using VRCM.Network.Server;
using VRCM.Network.Lobby;
using VRCM.Network.Player;
using VRCM.Media.Theater.UI;
using VRCM.Media.Remote.UI;

using TMPro;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private bool isServer;
    [SerializeField] private TextMeshProUGUI _serverParams;
    public static Bootstrapper Instance { get; private set; } = null;

    private NetConfiguration _config;
    private BroadcastService _broadcastService;
    private NetworkServer _networkServer;
    private NetworkLobby _networkLobby;

    [SerializeField] private TheaterUI _theaterUI;
    [SerializeField] private RemoteUI _remoteUI;

    public NetworkServer Server => _networkServer;
    public NetworkLobby Lobby => _networkLobby;
    public NetConfiguration NetConfig => _config;
    public TheaterUI Theater => _theaterUI;
    public RemoteUI Remote => _remoteUI;

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

        if (isServer)
        {
            Application.targetFrameRate = 60;
        }
        else
        {
            Application.targetFrameRate = 72;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        _config = new NetConfiguration(isServer);

        if (_config.AllOk)
        {
            _broadcastService = new BroadcastService();

            if (isServer)
            {
                _networkLobby = new NetworkLobby();
                _networkServer = new NetworkServer(_config.Ip, _config.Port);
                _broadcastService.StartSendBroadcast(_config.Ip, _config.Port);

                if (_serverParams != null)
                    _serverParams.text = $"VRCM : ws://{_config.Ip}:{_config.Port}/Echo";

                _theaterUI.Setup();
                _remoteUI.Setup();
            }
            else
            {
                _broadcastService.ServerFound += OnBroadcastServerFound;
                _broadcastService.StartListenBroadcast(_config.Port);
            }
        }
    }

    private void OnBroadcastServerFound(ServerParams sp)
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.Setup(sp);
        }
    }

    private void OnDisable()
    {
        if (_broadcastService != null)
        {
            _broadcastService.ServerFound -= OnBroadcastServerFound;
            _broadcastService.StopBroadcast();
        }

        if (_networkServer != null)
        {
            _networkServer.StopServer();
        }
    }
}
