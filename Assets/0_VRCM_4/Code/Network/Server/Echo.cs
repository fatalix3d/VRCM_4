using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using WebSocketSharp;
using WebSocketSharp.Server;
using VRCM.Network.Messages;

public class Echo : WebSocketBehavior
{
    protected override void OnOpen()
    {
        string uniqueId = this.ID;
        Debug.Log($"/Echo - [{uniqueId}] New connection, send autorize request message");
        NetMessage message = new NetMessage(NetMessage.Command.AutorizeRequest);
        byte[] bytes = BinarySerializer.Serialize(message);
        Dispatcher.InvokeAsync(() => Bootstrapper.Instance.Server.SendMessage(uniqueId, bytes));
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log("/Echo Close");
        //string uniqueId = this.ID;
        //Dispatcher.InvokeAsync(() => NetLobby.RemovePlayer(uniqueId));
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        string uniqueId = this.ID;
        Dispatcher.InvokeAsync(() => Bootstrapper.Instance.Server.RecieveMessage(uniqueId, e.RawData));
    }
}
