using System;
using Assets.Scripts.Helpers;
using UnityEngine;

 //<summary>
 //Code get from GitHub:
 //https://gist.github.com/aVolpe/707c8cf46b1bb8dfb363
 //</summary>
 
public static class Vibration
{
#if UNITY_ANDROID
    #if !UNITY_EDITOR
        private static AndroidJavaObject vibrationObj = VibrationActivity.activityObj.Get<AndroidJavaObject>("vibration");
    #else
        private static AndroidJavaObject vibrationObj;
    #endif
#endif

    public static void Vibrate()
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(10));
#endif
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate");
#endif
#if UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long milliseconds)
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(milliseconds));
#endif
#if UNITY_ANDROID 
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate", milliseconds);
#endif
#if UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
#if UNITY_WINRT || UNITY_WP8
        while(repeat-- > 0)
        foreach (var l in pattern)
        {
            WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(l));
        }     
#endif

#if UNITY_ANDROID 
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate", pattern, repeat);
#endif
#if UNITY_IOS
        Handheld.Vibrate();
#endif

    }
    public static bool HasVibrator()
    {
#if UNITY_ANDROID
        return Application.platform == RuntimePlatform.Android && vibrationObj.Call<bool>("hasVibrator");
#else
        return false;
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID 
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("cancel");
#endif
    }

//    private static bool isAndroid()
//    {
//#if UNITY_WINRT || UNITY_WP8
//        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(0));
//#endif
//#if UNITY_ANDROID && !UNITY_EDITOR
//        return true;
//#else
//        return false;
//#endif
//    }
}

