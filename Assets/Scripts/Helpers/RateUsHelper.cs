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

        public static void ExitApplication()
        {
            Application.Quit();
        }
    }
}
