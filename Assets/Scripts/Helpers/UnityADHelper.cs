using UnityEngine;
using UnityEngine.Advertisements;

public class UnityADHelper : MonoBehaviour {

    void Awake()
    {
        #if UNITY_ANDROID
        if (Advertisement.isSupported)
            Advertisement.Initialize("92452");
        #endif
            
        #if UNITY_IOS
        if (Advertisement.isSupported)
            Advertisement.Initialize("92468");
        #endif
    }

    void OnGUI()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(null, new ShowOptions
            {
                resultCallback = result => LogFile.Message(result.ToString())
            });
        }
    }
}
