using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace VRCM.Services.Protect
{
    public class ProtectedStorage
    {
        public bool Open { get; private set; } = false;
        public StorageKey Key { get; private set; } = null;
        private string _dataPath = string.Empty;
        private string _keyFilePath = string.Empty;
        private string _deviceId = string.Empty;

        public ProtectedStorage()
        {
            _deviceId = SystemInfo.deviceUniqueIdentifier;
            Debug.Log($"[ProtectedStorage] Device ID : {_deviceId}");
        }

        public void Setup()
        {
#if UNITY_EDITOR
            _dataPath = Application.dataPath;
#else
            _dataPath = Application.persistentDataPath;
#endif
            _keyFilePath = Path.Combine(_dataPath, "data.key");
        }

        public void CreateKey(TokenInfo token)
        {
            Debug.Log($"[ProtectedStorage] [Create] ... {token.Token}, {token.IsValid}");

            if (Key == null)
            {
                Key = new StorageKey(token);
                Debug.Log($"[ProtectedStorage] [Create] Created new key");
            }
            else
            {
                Key.UpdateKey(token);
                Debug.Log($"[ProtectedStorage] [Create] Previous key found, update key");
            }

            if (Key != null)
            {
                SaveKey();
            }
        }

        public void ReadKey()
        {
            Debug.Log($"[ProtectedStorage] [Read] ...");

            FileInfo key = new FileInfo(_keyFilePath);

            try
            {
                if (key.Exists)
                {
                    string json = string.Empty;
                    //json = File.ReadAllText(key.FullName);
                    byte[] cryptedJson = File.ReadAllBytes(key.FullName);
                    json = CryptSys.DecryptJson(cryptedJson);

                    if (!string.IsNullOrEmpty(json))
                    {
                        var keyData = JsonUtility.FromJson<StorageKey>(json);

                        if (_deviceId.Equals(keyData.DeviceId))
                            Key = keyData;
                        else
                        {
                            Debug.Log($"[ProtectedStorage] [Read] Error, invalid key [wrong deviceId]");
                            return;
                        }

                        Debug.Log($"[ProtectedStorage] [Read] Key data loaded");
                        Debug.Log($"[ProtectedStorage] [KEY] => {json}");
                    }
                    else
                    {
                        Debug.Log($"[ProtectedStorage] [Read] Error, key data null");
                        return;
                    }
                }
                else
                    Debug.Log($"[ProtectedStorage] [Read] Key file not found.");
            }
            catch (Exception e)
            {
                Debug.Log($"[ProtectedStorage] [Read] Error, {e}");
            }
        }

        private void SaveKey()
        {
            Debug.Log($"[ProtectedStorage][Save] Processing...");

            if (Key == null)
            {
                Debug.Log($"[ProtectedStorage][Save] Error, key is null. Not saved");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(Key);
                byte[] encryptedJson = CryptSys.EncryptJson(json);
                File.WriteAllBytes(_keyFilePath, encryptedJson);
                //File.WriteAllText(_keyFilePath, json);

                Debug.Log($"[ProtectedStorage][Save] Key saved. {_keyFilePath}");
            }
            catch (Exception e)
            {
                Debug.Log($"[ProtectedStorage][Save] Error, {e}");
            }
        }

        public bool ValidateKey()
        {
            Debug.Log($"[ProtectedStorage][Validate] ...");
            try
            {
                Open = false;

                if (Key == null)
                {
                    Debug.Log($"[ProtectedStorage] [ValidateKey] Error, key is null");
                    return Open;
                }

                if (string.IsNullOrEmpty(_deviceId) || string.IsNullOrEmpty(Key.DeviceId))
                {
                    Debug.Log($"[ProtectedStorage] [ValidateKey] Error, deviceId is null, device [{_deviceId}] / [{Key.DeviceId}]");
                    return Open;
                }

                if (!string.Equals(_deviceId, Key.DeviceId))
                {
                    Debug.Log($"[ProtectedStorage] [ValidateKey] Error, deviceId not equals [{_deviceId} / {Key.DeviceId}]");
                    return Open;
                }

                if (!Key.IsValid)
                {
                    Debug.Log($"[ProtectedStorage] [ValidateKey] Info, key is not valid");
                    return Open;
                }

                DateTime currentDate = DateTime.Now;
                DateTime targetDate = DateTime.ParseExact(Key.Expiry, "yyyy-MM-dd", null);
                int result = DateTime.Compare(currentDate, targetDate);

                if (result > 0)
                {
                    Debug.Log($"[ProtectedStorage] [ValidateKey] Info, key expired");
                    return Open;
                }

                Debug.Log($"[ProtectedStorage] [ValidateKey] Max users is {Key.MaxUsers}");

                PlayerPrefs.SetInt("maxUsers", Key.MaxUsers);
                PlayerPrefs.Save();

                Open = true;
                return Open;
            }
            catch (Exception e)
            {
                Debug.Log($"[ProtectedStorage] [ValidateKey] Error, {e}");
                return Open;
            }
        }

        public void AddRecord(SessionRecord record)
        {
            if (Key == null)
                return;

            Key.sessions.Add(record);
            SaveKey();
        }

        public void AddPlayRecord(string mediaName, int clientCount)
        {
            if (Key == null)
                return;

            var lastRecord = GetLastRecord();

            if (lastRecord != null)
            {
                if (lastRecord.mediaName != mediaName)
                {
                    lastRecord.playtime = Timer.GetTime();

                    Timer.StartTime();
                    var record = new SessionRecord(Key.Token, "Play", clientCount);
                    record.mediaName = mediaName;

                    Key.sessions.Add(record);
                }
                else
                {
                    lastRecord.playtime = Timer.GetTime();

                    Timer.StartTime();
                    var record = new SessionRecord(Key.Token, "Play", clientCount);
                    record.mediaName = mediaName;
                    Key.sessions.Add(record);
                }
            }
            else
            {
                Timer.StartTime();
                var record = new SessionRecord(Key.Token, "Play", clientCount);
                record.mediaName = mediaName;

                Key.sessions.Add(record);
            }

            SaveKey();
        }

        public void AddPauseRecord(string mediaName, int clientCount)
        {
            if (Key == null)
                return;

            //var lastRecord = GetLastRecord();
            //if (lastRecord != null)
            //{
            //    if (lastRecord.eventType == "Play")
            //    {
            //        lastRecord.playtime = Timer.GetTime();
            //        SaveKey();
            //    }
            //}
            Timer.PauseTime();
            //var record = new SessionRecord(Key.Token, "Pause", clientCount);
            //record.mediaName = mediaName;

            //Key.sessions.Add(record);
            //SaveKey();
        }

        public void AddStopRecord(string mediaName, int clientCount)
        {
            if (Key == null)
                return;

            var lastRecord = GetLastRecord();

            if (lastRecord != null)
            {
                if (lastRecord.eventType != "Stop")
                    lastRecord.playtime = Timer.GetTime();
                Timer.StopTime();

            }

            var record = new SessionRecord(Key.Token, "Stop", clientCount);
            record.mediaName = mediaName;

            Key.sessions.Add(record);
            SaveKey();
        }

        public SessionRecord GetLastRecord()
        {
            if (Key == null)
                return null;

            SessionRecord s = null;

            if (Key.sessions.Count > 0)
            {
                int lastId = Key.sessions.Count - 1;
                s = Key.sessions[lastId];
            }

            return s;
        }

        public bool RemoveRecord()
        {
            if (Key == null)
                return false;

            if (Key.sessions.Count < 1)
                return false;

            Key.sessions.RemoveAt(0);
            SaveKey();
            return true;
        }

        public void RemoveAllRecords()
        {
            if (Key == null)
                return;

            Key.sessions.Clear();
            SaveKey();
        }

        public string ExportSessionsInfo()
        {
            string json = string.Empty;

            if (Key == null)
                return null;

            if (Key.sessions.Count == 0)
                return null;

            return json;
        }
    }
}

