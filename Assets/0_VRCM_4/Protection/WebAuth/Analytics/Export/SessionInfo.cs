using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionsExport
{
    public string Token = string.Empty;
    public List<SessionInfo> Sessions = new List<SessionInfo>();
}


[Serializable]
public class SessionInfo
{
    public string Date = string.Empty;
    public List<MediaInfo> MediaList = new List<MediaInfo>();
    public int TotalClients = 0;
}

[Serializable]
public class MediaInfo
{
    public string MediaName = string.Empty;
    public ulong PlayTime = 0;
    public int Clients = 0;

    public MediaInfo(string mediaName)
    {
        this.MediaName = mediaName;
    }
}
