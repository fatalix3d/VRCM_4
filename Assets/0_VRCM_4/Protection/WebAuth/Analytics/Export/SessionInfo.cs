using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionInfo
{
    public string Date { get; set; } = string.Empty;
    public List<MediaInfo> MediaList { get; set; } = new List<MediaInfo>();
    public int TotalClients { get; set; } = 0;
}

[Serializable]
public class MediaInfo
{
    public ulong PlayTime = 0;
    public string MediaName = string.Empty;
    public int Clients = 0;
}
