using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRCM.Services.Protect;

public class ExportStatisticWindow : MonoBehaviour
{
    [SerializeField] private CanvasGroup _cnv;
    [SerializeField] private TextMeshProUGUI _status;

    private void Awake()
    {
        if (AuthService.Instance.WebManager != null)
        {
            AuthService.Instance.WebManager.InfoTextEvent += UpdateStatus;
        }
    }

    private void OnDisable()
    {
        if (AuthService.Instance.WebManager != null)
        {
            AuthService.Instance.WebManager.InfoTextEvent -= UpdateStatus;
        }
    }

    public void Open()
    {
        _cnv.alpha = 1f;
        _cnv.interactable = true;
        _cnv.blocksRaycasts = true;
    }

    public void Close()
    {
        _cnv.alpha = 0f;
        _cnv.interactable = false;
        _cnv.blocksRaycasts = false;
    }

    private void UpdateStatus(string msg)
    {
        _status.text = msg;
    }
}
