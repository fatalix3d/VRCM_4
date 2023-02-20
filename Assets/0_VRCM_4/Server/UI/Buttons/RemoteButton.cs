using UnityEngine;
using UnityEngine.UI;

namespace VRCM.Lobby.Buttons
{
    public class RemoteButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                SetRemoteMode();
            });
        }

        private void SetRemoteMode()
        {
            if (Bootstrapper.Instance != null)
            {
                if (Bootstrapper.Instance.Lobby != null)
                {
                    Bootstrapper.Instance.Lobby.SelectRemote(true);
                }
            }
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
