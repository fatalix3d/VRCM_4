using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRCM.Network.Player;

namespace VRCM.Lobby.UI
{
    public class NetPlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _id;
        [SerializeField] private TextMeshProUGUI _status;
        private GameObject _gameObject;

        public TextMeshProUGUI ID => _id;
        public TextMeshProUGUI Status => _status;
        public GameObject GO { get => _gameObject; set => _gameObject = value; }

        public void UpdateUI(NetPlayer player)
        {
            _id.text = player.Id;
            //_status.text = player.State.command.ToString();
        }
    }
}
