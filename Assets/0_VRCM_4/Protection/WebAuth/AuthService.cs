using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRCM.Services.Protect
{
    public class AuthService : MonoBehaviour
    {
        public static AuthService Instance { get; private set; }

        [SerializeField] private WebManager _webManager;
        private ProtectedStorage _storage;
        public ProtectedStorage Storage => _storage;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance == this)
                Destroy(gameObject);

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
                //Application.LoadLevel(1);
                SceneManager.LoadScene(1);
            else
            {
                _webManager.LockUI(false);
                _webManager.InfoMessage("Ошибка авторизации");
            }

            yield return null;
        }

        public void Login(TokenInfo token)
        {
            _storage.CreateKey(token);
            _storage.ValidateKey();

            Debug.Log($"Storage open : {_storage.Open}");


            if (_storage.Open)
                //Application.LoadLevel(1);
                SceneManager.LoadScene(1);
            else
                _webManager.LockUI(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _storage.RemoveAllRecords();
        }
    }
}
