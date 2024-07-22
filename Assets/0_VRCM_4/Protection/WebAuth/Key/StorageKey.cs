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
        public int MaxUsers;

        public List<SessionRecord> sessions = new List<SessionRecord>();


        public StorageKey(TokenInfo tokenInfo)
        {
            this.Token = tokenInfo.Token;
            this.Expiry = tokenInfo.Expiry;
            this.IsValid = tokenInfo.IsValid;
            this.DeviceId = SystemInfo.deviceUniqueIdentifier;
            this.MaxUsers = tokenInfo.MaxUsers;

            Debug.Log($"[Key][Create] {this.IsValid} / DeviceId : {this.DeviceId} / MaxUsers : {this.MaxUsers}");
        }

        public void UpdateKey(TokenInfo tokenInfo)
        {
            string prevKey = JsonUtility.ToJson(this);
            string newKey = JsonUtility.ToJson(tokenInfo);

            Token = tokenInfo.Token;
            Expiry = tokenInfo.Expiry;
            IsValid = tokenInfo.IsValid;
            DeviceId = SystemInfo.deviceUniqueIdentifier;
            MaxUsers = tokenInfo.MaxUsers;

            Debug.Log($"[Key][Update] old key {prevKey} / new key {newKey}");
        }
    }
}