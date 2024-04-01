using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCM.Services.Protect
{
    public class Timer : MonoBehaviour
    {
        private static bool _countdownFlag = false;
        private static float _totalTime = 0f;

        private void Update()
        {
            if (_countdownFlag)
                _totalTime += Time.deltaTime;
        }

        public static void StartTime()
        {
            _totalTime = 0f;
            _countdownFlag = true;
        }

        public static void StopTime()
        {
            _totalTime = 0f;
            _countdownFlag = false;
        }

        public static void PauseTime()
        {
            _countdownFlag = false;
        }

        public static void ResumeTime()
        {
            _countdownFlag = true;
        }

        public static ulong GetTime()
        {
            return (ulong)_totalTime;
        }
    }
}
