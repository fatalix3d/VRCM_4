using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace VRCM.Network.Configuration
{
    public class NetConfiguration
    {
        private string _id;
        private string _ip;
        private int _port;
        private bool _allOk = false;

        private bool _isEditor = false;
        private string _dataPath;

        public string Id => _id;
        public string Ip => _ip;
        public int Port => _port;
        public bool AllOk => _allOk;

        public NetConfiguration(bool isServer)
        {
#if UNITY_EDITOR
            _isEditor = true;
#endif
            if (_isEditor)
            {
                _dataPath = Path.Combine(Application.dataPath, "360Content");
                Debug.Log($"[Configuration] EDITOR -> {_dataPath}");
            }
            else
            {
                _dataPath = Path.Combine("/storage/emulated/0", "360Content");
                Debug.Log($"[Configuration] DEVICE -> {_dataPath}");
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(_dataPath);
                if (di.Exists)
                {
                    Debug.Log($"[Configuration] Data folder found");
                    string idFilePath = Path.Combine(_dataPath, "id.txt");

                    FileInfo fi = new FileInfo(idFilePath);
                    Debug.Log($"[Configuration] Checking id file in folder {di.Name}");

                    if (fi.Exists)
                    {
                        Debug.Log($"[Configuration] id file found...");
                        string _json = File.ReadAllText(idFilePath);
                        NetId netId = JsonUtility.FromJson<NetId>(_json);

                        if (netId != null)
                        {
                            _id = netId.id;
                            _ip = IPManager.GetLocalIPAddress();
                            _port = netId.server_port;
                            _allOk = true;
                            Debug.Log($"[Configuration] Setup complete ...[{_id} / {_ip} / {_port}]");
                        }
                        else
                        {
                            Debug.Log($"[Configuration] NetId file error!");
                        }
                    }
                    else
                    {
                        Debug.Log($"[Configuration] id file NOT found, creating new file");

                        WriteIdFile(idFilePath, isServer);
                    }
                }
                else
                {
                    Debug.Log($"[Configuration] Data folder not found, created new.");
                    Directory.CreateDirectory(_dataPath);

                    string idFilePath = Path.Combine(_dataPath, "id.txt");
                    WriteIdFile(idFilePath, isServer);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"[Configuration] - {e}");
            }
        }

        private void WriteIdFile(string filePath, bool isServer)
        {
            string idJson = string.Empty;

            NetId netId;

            if (isServer)
            {
                netId = new NetId("VRCM_SERVER", GetAvailablePort());
            }
            else
            {
                netId = new NetId("VRCM_CLIENT_ID", 11000);
            }

            idJson = JsonUtility.ToJson(netId);

            File.WriteAllText(filePath, idJson);

            Debug.Log($"[Configuration] id file created...Ok");
        }

        private static int GetAvailablePort()
        {
            int port = 0;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                port = ((IPEndPoint)socket.LocalEndPoint).Port;
            }
            return port;
        }
    }
}
