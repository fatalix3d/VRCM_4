using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace VRCM.Network.Broadcast
{
    public class BroadcastService
    {
        private bool _autoStopBroadcasting = false;
        private bool _listen = false;

        private UdpClient _udpClient;
        private IPEndPoint _groupEP;
        private Thread _broadcastThread;

        private int _port;
        private byte[] _msg;    

        private ServerParams _remoteServerParameters = null;
        public ServerParams Server => _remoteServerParameters;

        public event Action<ServerParams> ServerFound;

        public BroadcastService()
        {
            Debug.Log("[Broadcast Service] - Created");
        }

        public void StartSendBroadcast(string ip, int port)
        {
            ServerParams sp = new ServerParams(ip, port);

            if (_listen)
            {
                Debug.Log("[Broadcast Service] - Already listen");
                return;
            }

            if (!string.IsNullOrEmpty(sp.ip) && sp.port > 0 )
            {
                _port = sp.port;
                string msg = JsonUtility.ToJson(sp);

                _udpClient = new UdpClient();
                _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, sp.port));

                var from = new IPEndPoint(0, 0);
                _msg = Encoding.UTF8.GetBytes(msg);

                _broadcastThread = new Thread(Send);
                _broadcastThread.Start();
                _listen = true;
            }

            Debug.Log("[Broadcast Service] - Start listen broadcast");
        }

        private void Send()
        {
            try
            {
                string msg = Encoding.ASCII.GetString(_msg, 0, _msg.Length);
                Debug.Log($"[[Broadcast Service] - Start sending : {msg}]");

                while (_listen)
                {
                    _udpClient.Send(_msg, _msg.Length, "255.255.255.255", _port);
                    Thread.Sleep(2500);
                }
            }
            catch (SocketException e)
            {
                Debug.Log(e);
                _listen = false;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _listen = false;
            }

            _listen = false;
            _udpClient.Close();
            _udpClient.Dispose();
        }

        public void StartListenBroadcast(int port)
        {
            if (_listen)
            {
                Debug.Log("[Broadcast Service] - Already listen");
                return;
            }

            Debug.Log("[Broadcast Service] - Start listen broadcast");

            _udpClient = new UdpClient(port);
            _groupEP = new IPEndPoint(IPAddress.Any, port);
            _listen = true;
            _broadcastThread = new Thread(Recieve);
            _broadcastThread.Start();
        }

        private void Recieve()
        {
            try
            {
                while (_listen)
                {
                    byte[] bytes = _udpClient.Receive(ref _groupEP);
                    string msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    if (!string.IsNullOrEmpty(msg))
                    {
                        Debug.Log($"Received broadcast from {_groupEP} : {msg}");
                        ServerParams sp = JsonUtility.FromJson<ServerParams>(msg);

                        if (sp != null)
                        {
                            if(!string.IsNullOrEmpty(sp.ip) && sp.port > 0)
                            {
                                Debug.Log($"[Broadcast Service] - Server found [{sp},{sp.port}]");
                                _remoteServerParameters = sp;
                                StopBroadcast();
                                ServerFound?.Invoke(sp);
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.Log(e);
                _listen = false;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _listen = false;
            }

            _listen = false;
            _udpClient.Close();
            _udpClient.Dispose();
        }

        public void StopBroadcast()
        {
            if (!_listen)
            {
                Debug.Log("[Broadcast Service] - Already stoped");
                return;
            }

            _listen = false;
            Debug.Log("[Broadcast Service] - Stop");
        }
    }


}
