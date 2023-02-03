using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Network.Player;
using VRCM.Network.Messages;
using System;

namespace VRCM.Lobby.UI
{
    public class NetPlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _id;
        [SerializeField] private TextMeshProUGUI _uniqueId;
        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private TextMeshProUGUI _mediaName;
        private GameObject _gameObject;

        public GameObject GO { get => _gameObject; set => _gameObject = value; }

        public void UpdateUI(NetMessage message)
        {
            _id.text = message.id;
            _uniqueId.text = message.uid;
            _status.text = message.command.ToString();

            if (!string.IsNullOrEmpty(message.mediaName))
            {
                // to do, remove to client side;
                TimeSpan allTime = TimeSpan.FromMilliseconds(message.mediaAllTime);
                TimeSpan curTime = TimeSpan.FromMilliseconds(message.mediaCurrentTime);
                string time_string = $"{curTime.ToString(@"mm\:ss")} / {allTime.ToString(@"mm\:ss")}";
                _mediaName.text = $"{message.mediaName} {time_string}";
            }
        }
    }
}
