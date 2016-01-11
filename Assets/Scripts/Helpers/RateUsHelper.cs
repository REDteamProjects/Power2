using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    class RateUsHelper : MonoBehaviour
    {
        public static GameObject RateUsModule;

        public static EventHandler RateUsCalled;

        public static void GoToStore()
        {
            DestroyRateUsModule();
#if UNITY_WINRT || UNITY_WP8
            if (RateUsCalled != null)
                RateUsCalled(null, EventArgs.Empty);
            //Application.OpenURL("http://windowsphone.com/s?appid=040e292a-81bf-4fef-88d1-437d314038a7");
#endif
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=com.REDteam.TwoX");
#endif
#if UNITY_IOS
            Application.OpenURL("http://www.itunes.com");
#endif
        }

        public void RateUsNow()
        {
            GoToStore();
            PlayerPrefs.SetInt("RateUsUserMessage", 1);
        }

        public void CloseRateUsModule()
        {
            DestroyRateUsModule();
            PlayerPrefs.SetInt("RateUsUserMessage", 1);
        }

        public static void DestroyRateUsModule()
        {
            if (RateUsModule == null) return;
            Destroy(RateUsModule);
            PlayerPrefs.SetInt("RateUsUserMessage", 1);
        }
    }
}
