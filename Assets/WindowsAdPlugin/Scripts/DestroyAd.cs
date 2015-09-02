using UnityEngine;
using System.Collections;

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using Windows_Ad_Plugin;
#endif

/// <summary>
/// Calls the plugins HandleDestruction method when the game object is destroyed
/// </summary>
public class DestroyAd : MonoBehaviour
{
#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    void OnDestroy()
    {
        Windows_Ad_Plugin.Helper.Instance.HandleDestruction();
    }
#endif
}
