using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Services.Protect
{
    [Serializable]
    public class TokenInfo
    {
        public string token;
        public string expiry;
        public bool isValid;
    }
}
