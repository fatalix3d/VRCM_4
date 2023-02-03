using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRCM.Media.Theater
{
    public class TheaterElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _prevImage;
        private GameObject _gameObject;
        public GameObject GO { get => _gameObject; set => _gameObject = value; }


        public void UpdateElement(string name, MediaFile media)
        {
            _name.text = name;

            if(media.videoPrev!=null)
            _prevImage.texture = media.videoPrev;
        }
    }
}
