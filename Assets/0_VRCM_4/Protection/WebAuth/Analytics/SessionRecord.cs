using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Services.Protect
{
    [Serializable]
    public class SessionRecord
    {
        public string token = string.Empty;
        public int clientCount = 0;
        public string eventType = string.Empty;
        public string mediaName = string.Empty;
        public ulong playtime = 0;
        public string date = string.Empty;
        public string comment = string.Empty;

        public SessionRecord(string token, string eventType, int clientCount, string comm = null)
        {
            this.token = token;
            this.clientCount = clientCount;
            this.eventType = eventType;

            DateTime dateTimeNow = DateTime.Now;
            this.date = dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss");
            this.comment = $"{comm}";

            string json = JsonUtility.ToJson(this);
            Debug.Log(json);
        }
    }
}
