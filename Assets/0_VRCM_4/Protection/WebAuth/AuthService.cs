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
            _storage.Setup();
            _storage.ReadKey();

            if (_storage.Key != null)
                yield return StartCoroutine(_webManager.LoginRoutine(_storage.Key.Token));

            _storage.ValidateKey();

            Debug.Log($"Storage open : {_storage.Open}");

            if (_storage.Open)
                Application.LoadLevel(1);
            else
                _webManager.LockUI(false);

            yield return null;
        }

        public void Login(TokenInfo token)
        {
            _storage.CreateKey(token);
            _storage.ValidateKey();

            Debug.Log($"Storage open : {_storage.Open}");


            if (_storage.Open)
                Application.LoadLevel(1);
            else
                _webManager.LockUI(false);
        }
    }
}
