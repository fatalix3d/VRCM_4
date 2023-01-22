using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Network.Messages
{
    [Serializable]
    public class NetMessage
    {
        public enum Command
        {
            AutorizeRequest = 0,
            AutorizeResponce = 1,
            Setup = 2,
            Ready = 3,
            Status = 4,

            Play = 20,
            Pause = 21,
            Stop = 22,
            Seek = 23,

            VideoNotFound = 30
        }

        public Command command;
        public string id;
        public string mediaName;
        public long seekTime;

        public NetMessage(Command _command)
        {
            this.command = _command;
            this.id = "test_01";
            this.mediaName = string.Empty;
            this.seekTime = 0;
        }
    }


}
