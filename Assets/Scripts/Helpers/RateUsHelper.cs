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

        public static void GoToStore()
        {
        DestroyRateUsModule();
        #if UNITY_WINRT || UNITY_WP8
                    Application.OpenURL("https://www.windowsphone.com");
        #endif
        #if UNITY_ANDROID
                Application.OpenURL("http://play.google.com");
        #endif
        #if UNITY_IOS
                Application.OpenURL("http://www.itunes.com");
        #endif
        }

        public void RateUsNow()
        {
            GoToStore();
        }

        public void CloseRateUsModule()
        {
            DestroyRateUsModule();
        }

        public static void DestroyRateUsModule()
        {
            if (RateUsModule != null)
            {
                Destroy(RateUsModule);
                PlayerPrefs.SetInt("RateUsUserMessage", 1);
            }
        }
    }
}
