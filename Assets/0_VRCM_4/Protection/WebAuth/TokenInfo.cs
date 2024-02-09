using System;

namespace VRCM.Services.Protect
{
    [Serializable]
    public class TokenInfo
    {
        public string Token;
        public string Expiry;
        public bool IsValid;
        public string DeviceId;
        public int MaxUsers = 0;
    }
}
