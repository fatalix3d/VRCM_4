using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VRCM.Network.Messages
{
    public static class BinarySerializer
    {
        public static byte[] Serialize(NetMessage netMessage)
        {
            if (netMessage == null)
            {
                Debug.Log("Target message is null");
                return null;
            }

            try
            {
                string json = JsonUtility.ToJson(netMessage);
                Debug.Log($"[BinarySerializer] SJson : {json}");

                BinaryFormatter bf = new BinaryFormatter();
                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, netMessage);
                    bytes = ms.ToArray();
                }

                return bytes;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return null;
            }
        }

        public static NetMessage Deserialize(byte[] bytes)
        {
            try
            {
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(bytes, 0, bytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    NetMessage netMessage = (NetMessage)binForm.Deserialize(memStream);

                    string json = JsonUtility.ToJson(netMessage);
                    Debug.Log($"[BinarySerializer] DJson : {json}");

                    return netMessage;
                }
            }
            catch (Exception e)
            {
                //Debug.Log(e.ToString());
                return null;
            }
        }
    }
}
