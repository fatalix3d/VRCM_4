using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCM.Services.Protect;

public class SendStatButton : MonoBehaviour
{
    public void Send()
    {
        if (AuthService.Instance != null)
        {
            AuthService.Instance.SendStats();
        }
    }
}
