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

        [SerializeField] private Outline _outline;
        [SerializeField] private Color[] _outlineColors;

        private GameObject _gameObject;

        public GameObject GO { get => _gameObject; set => _gameObject = value; }

        public void UpdateUI(NetMessage message)
        {
            _id.text = message.id;
            _uniqueId.text = message.uid;
            _status.text = message.command.ToString();

            if (!string.IsNullOrEmpty(message.mediaName))
            {
                _mediaName.text = $"{message.mediaName} ({message.mediaDuration})";
            }

            switch (message.command)
            {
                case NetMessage.Command.AutorizeError:
                    _outline.effectColor = _outlineColors[3];
                    break;
                case NetMessage.Command.AutorizeRequest:
                case NetMessage.Command.AutorizeSucces:
                    _outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Play:
                    _outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Pause:
                    _outline.effectColor = _outlineColors[2];
                    break;
                case NetMessage.Command.Resume:
                    _outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Stop:
                    break;
            }
        }
    }
}
