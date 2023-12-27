using System;
using System.IO;
using UnityEngine;

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

        public void CreateKey(TokenInfo token)
        {

            Debug.Log($"[ProtectedStorage] [Create] Processing...");

            ReadKey();

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
                SaveKey();
        }

        public void ReadKey()
        {
            Debug.Log($"[ProtectedStorage] [Read] Processing...");

#if UNITY_EDITOR
            _dataPath = Application.dataPath;
#else
            _dataPath = Application.persistentDataPath;
#endif
            _keyFilePath = Path.Combine(_dataPath, "data.key");

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

                        Debug.Log($"[ProtectedStorage] [Read] Key data loaded [{json}]");
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

                Open = true;
                return Open;
            }
            catch (Exception e)
            {
                Debug.Log($"[ProtectedStorage] [ValidateKey] Error, {e}");
                return Open;
            }
        }
    }
}

