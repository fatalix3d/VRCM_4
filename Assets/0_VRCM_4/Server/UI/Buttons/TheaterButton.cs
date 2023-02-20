using UnityEngine;
using UnityEngine.UI;

namespace VRCM.Lobby.Buttons
{
    public class TheaterButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                SetTheaterMode();
            });
        }

        private void SetTheaterMode()
        {
            if (Bootstrapper.Instance != null)
            {
                if (Bootstrapper.Instance.Lobby != null)
                {
                    Bootstrapper.Instance.Lobby.SelectRemote(false);
                }
            }
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

    }
}
