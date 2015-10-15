using System;
using Assets.Scripts.Helpers;
using UnityEngine;

/// <summary>
/// Code get from GitHub:
/// https://gist.github.com/aVolpe/707c8cf46b1bb8dfb363
/// </summary>
/// 
public static class Vibration
{

#if UNITY_ANDROID
#if !UNITY_EDITOR
            public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
            public static AndroidJavaClass unityPlayer;
            public static AndroidJavaObject currentActivity;
            public static AndroidJavaObject vibrator;
#endif
#endif

    public static void Vibrate()
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(10));
#endif
#if UNITY_ANDROID 
        if (isAndroid())
            vibrator.Call("vibrate");
        else
            Handheld.Vibrate();
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
        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
        else
            Handheld.Vibrate();
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
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
        else
            Handheld.Vibrate();
#endif
#if UNITY_IOS
        Handheld.Vibrate();
#endif

    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
#if UNITY_ANDROID 
        if (isAndroid())
            vibrator.Call("cancel");
#endif
    }

    private static bool isAndroid()
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(0));
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
	    return true;
#else
        return false;
#endif
    }
}

