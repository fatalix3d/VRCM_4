using System.Collections;
using System.Collections.Generic;

public class ServerParams
{
    public string ip;
    public int port;

    public ServerParams(string _ip, int _port)
    {
        this.ip = _ip;
        this.port = _port;
    }
}
