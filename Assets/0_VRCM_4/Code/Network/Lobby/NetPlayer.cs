using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Messages;

namespace VRCM.Network.Player
{
    [Serializable]
    public class NetPlayer
    {
        public string Id;
        public string UniqueId;
        public bool Authorized = false;
        public Texture2D PlayerPreviewTex;
        public NetMessage _state;
    }
}
