using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionRecord
{
    public string token = string.Empty;
    public string mediaName = string.Empty;
    public int clientCount = 0;
    public string eventType = string.Empty;
    public string deviceDate = string.Empty;
    public string deviceTime = string.Empty;
    public string comment = string.Empty;

    public SessionRecord(string token, string eventType, int clientCount)
    {
        this.token = token;
        this.clientCount = clientCount;
        this.eventType = eventType;

        DateTime dateTimeNow = DateTime.Now;
        this.deviceDate = dateTimeNow.ToString("yyyy-MM-dd");
        this.deviceTime = dateTimeNow.ToString("HH:mm:ss");
        this.comment = "test 123";

        string json = JsonUtility.ToJson(this);
        Debug.Log(json);
    }

}
