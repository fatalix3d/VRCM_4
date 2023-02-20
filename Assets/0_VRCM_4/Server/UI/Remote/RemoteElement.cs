using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRCM.Media.Remote.UI
{
    public class RemoteElement : MonoBehaviour
    {
        private RemoteUI _remoteUI = null;
        private MediaFile _mediaFile;
        private GameObject _gameObject;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _prevImage;
        [SerializeField] private Outline _outline;

        public void CreateElement(RemoteUI remoteUI, MediaFile media)
        {
            _remoteUI = remoteUI;
            _mediaFile = media;

            _name.text = media.name;
            _prevImage.texture = media.videoPrev;
            _gameObject = gameObject;
        }
    }
}
