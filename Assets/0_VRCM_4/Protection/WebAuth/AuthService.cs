using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Services.Protect
{
    public class AuthService : MonoBehaviour
    {
        [SerializeField] private WebManager _webManager;

        private ProtectedStorage _storage;
        public ProtectedStorage Storage => _storage;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        private IEnumerator Start()
        {
            _webManager.LockUI(true);
            _storage = new ProtectedStorage();

            // silent start with sync
            if (PlayerPrefs.HasKey("token"))
            {
                string t = PlayerPrefs.GetString("token");
                _webManager.Login(t);
                yield return StartCoroutine(_webManager.LoginRoutine(t));
            }

            _storage.ReadKey();

            Debug.Log($"Storage open : {_storage.Open}");

            if (_storage.Open)
            {
                Application.LoadLevel(1);
            }
            else
            {
                _webManager.LockUI(false);
            }

            yield return null;
        }

        public void Login(TokenInfo token)
        {
            _webManager.LockUI(true);

            _storage.CreateKey(token);
            
            Debug.Log($"Storage open : {_storage.Open}");
            if (_storage.Open)
            {
                Application.LoadLevel(1);
            }
            else
            {
                _webManager.LockUI(false);
            }
        }
    }
}
