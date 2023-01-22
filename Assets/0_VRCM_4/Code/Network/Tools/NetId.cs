using System;

[Serializable]
public class NetId
{
    public string client_id = string.Empty;
    public int server_port = 0;

    public NetId(string _id, int _port)
    {
        client_id = _id;
        server_port = _port;
    }
}
