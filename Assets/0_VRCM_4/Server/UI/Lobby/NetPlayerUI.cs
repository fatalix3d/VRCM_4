using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Network.Player;
using VRCM.Network.Messages;
using System;
using DG.Tweening;
using System.Globalization;

namespace VRCM.Lobby.UI
{
    public class NetPlayerUI : MonoBehaviour
    {
        private string _uniqueId;
        public string UniqueId { get => _uniqueId; set => _uniqueId = value; }

        private bool _selected = false;
        public bool Selected => _selected;

        [SerializeField] private TextMeshProUGUI _id;
        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private TextMeshProUGUI _mediaName;

        [SerializeField] private Outline _outline;
        [SerializeField] private Color[] _outlineColors;

        [SerializeField] private GameObject _selectionGo;
        [SerializeField] private Image _selectionIcon;

        [SerializeField] private Image _batteryIndicator;
        [SerializeField] private TextMeshProUGUI _batteryLabel;

        [SerializeField] private Image _tempIndicator;
        [SerializeField] private TextMeshProUGUI _tempLabel;

        private GameObject _gameObject;
        public GameObject GO { get => _gameObject; set => _gameObject = value; }

        public void Select(bool val)
        {
            if (val)
            {
                _selected = true;
                _selectionIcon.DOFade(1f, 0.25f);
            }
            else
            {
                _selected = false;
                _selectionIcon.DOFade(0.2f, 0.25f);
            }
        }

        public void UpdateUI(NetMessage message)
        {
            _id.text = message.id;
            _status.text = message.command.ToString();

            if (!string.IsNullOrEmpty(message.mediaName))
            {
                _mediaName.text = $"{message.mediaName} ({message.mediaDuration})";
            }

            try
            {
                if (!string.IsNullOrEmpty(message.battery))
                {
                    string[] split = message.battery.Split(new char[0]);
                    float bat = float.Parse(split[0], CultureInfo.InvariantCulture.NumberFormat);
                    float temp = float.Parse(split[1], CultureInfo.InvariantCulture.NumberFormat);

                    _batteryIndicator.fillAmount = bat / 100;
                    _tempIndicator.fillAmount = temp / 100;
                    _batteryLabel.text = $"{bat}%";
                    _tempLabel.text = $"{temp}�";
                }
            }
            catch(Exception e)
            {
                Debug.Log("[NetPlayer UI] Error can't parse data [battery]");
            }

            switch (message.command)
            {
                case NetMessage.Command.AutorizeError:
                    //_outline.effectColor = _outlineColors[3];
                    break;
                case NetMessage.Command.AutorizeRequest:
                case NetMessage.Command.AutorizeSucces:
                    //_outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Play:
                    //_outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Pause:
                    //_outline.effectColor = _outlineColors[2];
                    break;
                case NetMessage.Command.Resume:
                    //_outline.effectColor = _outlineColors[1];
                    break;
                case NetMessage.Command.Stop:
                    break;
            }
        }

        public void ThrowSelectEvent()
        {
            if (Bootstrapper.Instance != null)
            {
                if (Bootstrapper.Instance.Lobby != null)
                {
                    Bootstrapper.Instance.Lobby.SelectPlayer(_uniqueId);
                }
            }
        }

        public void SetRemoteMode(bool flag)
        {
            if (flag)
            {
                _selectionGo.SetActive(true);
            }
            else
            {
                _selectionGo.SetActive(false);
            }
        }


    }
}
