using System;

[Serializable]
public class NetId
{
    public string id = string.Empty;
    public int server_port = 0;

    public NetId(string _id, int _port)
    {
        id = _id;
        server_port = _port;
    }
}
