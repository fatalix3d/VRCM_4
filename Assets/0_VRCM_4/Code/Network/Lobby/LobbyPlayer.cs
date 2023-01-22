using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Messages;

namespace VRCM.Network.Lobby
{
    [Serializable]
    public class LobbyPlayer
    {
        public string Id;
        public Texture2D playerPreviewTex;
        NetMessage state;
    }
}
