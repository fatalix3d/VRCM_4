using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRCM.Services.Protect
{
    public class WebUI : MonoBehaviour
    {
        [SerializeField] private WebManager _webManager;
        [Space(5)]
        [SerializeField] private Button _loginButton;
        [SerializeField] private TMP_InputField _tokenInput;
        [SerializeField] private TextMeshProUGUI _infoText;

        private void Awake()
        {
            OnUILockEvent(true);

            if (_webManager == null)
                return;

            _webManager.InfoTextEvent += UpdateInfoText;
            _webManager.UILockEvent += OnUILockEvent;
        }

        private void OnDisable()
        {
            if (_webManager == null)
                return;

            _webManager.InfoTextEvent -= UpdateInfoText;
            _webManager.UILockEvent -= OnUILockEvent;

        }
        private void UpdateInfoText(string msg)
        {
            _infoText.text = msg;
        }

        private void OnUILockEvent(bool flag)
        {
            _loginButton.interactable = !flag;
            _tokenInput.interactable = !flag;
        }

        public void Login()
        {
            string tokenText = _tokenInput.text;
            if (!string.IsNullOrEmpty(tokenText))
            {
                _infoText.text = "Авторизация";
                _webManager.Login(tokenText);
            }
            else
            {
                _infoText.text = "Значение не может быть пустым";
            }
        }
    }
}
