using System;
using Assets.Scripts.Helpers;
using UnityEngine;

<<<<<<< .merge_file_a04628
/// <summary>
/// Code get from GitHub:
/// https://gist.github.com/aVolpe/707c8cf46b1bb8dfb363
/// </summary>
/// 
public static class Vibration
{

//#if UNITY_ANDROID
//#if !UNITY_EDITOR
//            public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//            public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//            public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
//#else
//            public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//            public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//            public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
//#endif
//#endif
=======
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
>>>>>>> .merge_file_a06544

    public static void Vibrate()
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(10));
#endif
<<<<<<< .merge_file_a04628
//#if UNITY_ANDROID 
//        if (isAndroid())
//            vibrator.Call("vibrate");
//        else
//            Handheld.Vibrate();
//#endif
=======
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate");
#endif
>>>>>>> .merge_file_a06544
#if UNITY_IOS
        Handheld.Vibrate();
#endif
    }

<<<<<<< .merge_file_a04628


=======
>>>>>>> .merge_file_a06544
    public static void Vibrate(long milliseconds)
    {
#if UNITY_WINRT || UNITY_WP8
        WinRTDeviceHelper.FireVibratePhone(TimeSpan.FromMilliseconds(milliseconds));
#endif
<<<<<<< .merge_file_a04628
//#if UNITY_ANDROID 
//        if (isAndroid())
//            vibrator.Call("vibrate", milliseconds);
//        else
//            Handheld.Vibrate();
//#endif
=======
#if UNITY_ANDROID 
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate", milliseconds);
#endif
>>>>>>> .merge_file_a06544
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

<<<<<<< .merge_file_a04628
//#if UNITY_ANDROID 
//        if (isAndroid())
//            vibrator.Call("vibrate", pattern, repeat);
//        else
//            Handheld.Vibrate();
//#endif
=======
#if UNITY_ANDROID 
        if (Application.platform == RuntimePlatform.Android)
            vibrationObj.Call("vibrate", pattern, repeat);
#endif
>>>>>>> .merge_file_a06544
#if UNITY_IOS
        Handheld.Vibrate();
#endif

    }
<<<<<<< .merge_file_a04628

    public static bool HasVibrator()
    {
        return isAndroid();
=======
    public static bool HasVibrator()
    {
#if UNITY_ANDROID
        return Application.platform == RuntimePlatform.Android && vibrationObj.Call<bool>("hasVibrator");
#else
        return false;
#endif
>>>>>>> .merge_file_a06544
    }

    public static void Cancel()
    {
<<<<<<< .merge_file_a04628
//#if UNITY_ANDROID 
//        if (isAndroid())
//            vibrator.Call("cancel");
//#endif
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
=======
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
>>>>>>> .merge_file_a06544
}

