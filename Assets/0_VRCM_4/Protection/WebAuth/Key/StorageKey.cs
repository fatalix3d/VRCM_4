using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Services.Protect
{
    [Serializable]
    public class StorageKey
    {
        public string Token;
        public string Expiry;
        public bool IsValid;
        public string DeviceId;

        public StorageKey(TokenInfo tokenInfo)
        {
            Token = tokenInfo.token;
            Expiry = tokenInfo.expiry;
            IsValid = tokenInfo.isValid;
            DeviceId = SystemInfo.deviceUniqueIdentifier;

            Debug.Log($"[Key][Create] {this.Token} / DeviceId : {this.DeviceId}");
        }

        public void UpdateKey(TokenInfo tokenInfo)
        {
            string prevKey = JsonUtility.ToJson(this);
            string newKey = JsonUtility.ToJson(tokenInfo);

            Token = tokenInfo.token;
            Expiry = tokenInfo.expiry;
            IsValid = tokenInfo.isValid;
            DeviceId = SystemInfo.deviceUniqueIdentifier;

            Debug.Log($"[Key][Update] old key {prevKey} / new key {newKey}");
        }
    }
}