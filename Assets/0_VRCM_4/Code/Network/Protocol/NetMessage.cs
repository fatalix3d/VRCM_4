using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Network.Player;
namespace VRCM.Network.Messages
{
    [Serializable]
    public class NetMessage
    {
        public enum Command
        {
            AutorizeRequest = 0,
            AutorizeSucces = 1,
            AutorizeError = 2,

            Setup = 10,
            Ready = 11,
            Status = 12,

            Play = 20,
            Pause = 21,
            Resume = 22,
            Stop = 23,
            Seek = 24,
            SeekError = 25,

            VideoNotFound = 30
        }

        public Command command;
        public string id;
        public string uid;
        public string mediaName;
        public string mediaDuration;
        public double seekTime;

        public NetMessage(Command _command)
        {
            this.command = _command;
            this.id = "test_01";
            this.mediaName = string.Empty;
            this.seekTime = 0;
        }

        public NetMessage(NetPlayer player, Command _command)
        {
            this.command = _command;
            this.id = player.Id;
            this.mediaName = string.Empty;
            this.seekTime = 0;
        }
    }


}
