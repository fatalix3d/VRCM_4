using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using WebSocketSharp;
using WebSocketSharp.Server;
using VRCM.Network.Messages;
using VRCM.Network.Lobby;

public class Echo : WebSocketBehavior
{
    protected override void OnOpen()
    {
        string uniqueId = this.ID;
        Debug.Log($"/Echo - [{uniqueId}] New connection, send autorize request message");
        Dispatcher.InvokeAsync(() => Bootstrapper.Instance.Server.SendMessage(uniqueId, NetMessage.Command.AutorizeRequest));
    }

    protected override void OnClose(CloseEventArgs e)
    {
        string uniqueId = this.ID;
        string closeArgs = $"Code {e.Code}, Reason {e.Reason}";
        Debug.Log($"/Echo - Close connection [{uniqueId}], Code {e.Code}, Reason {e.Reason}");
        Dispatcher.InvokeAsync(() => Bootstrapper.Instance.Lobby.RemovePlayer(uniqueId, closeArgs));
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        string uniqueId = this.ID;
        Dispatcher.InvokeAsync(() => Bootstrapper.Instance.Server.RecieveMessage(uniqueId, e.RawData));
    }
}
