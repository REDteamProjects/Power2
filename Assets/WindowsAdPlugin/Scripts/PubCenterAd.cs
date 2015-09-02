using UnityEngine;
using System.Collections;

#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using Windows_Ad_Plugin;
#endif

public class PubCenterAd : MonoBehaviour
{

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    //PubCenter Ad
    public string AppId = "test_client";
    public string UnitId = "Image480_80";
    public float adWidth = 480;
    public float adHeight = 80;
    public bool autoRefreshAd = true;

    //Position Settings
    public Windows_Ad_Plugin.Helper.HORIZONTAL_ALIGNMENT horizontalAlignment;
    public Windows_Ad_Plugin.Helper.VERTICAL_ALIGNMENT verticalAlignment;

    public bool printDebug = true;

    //Reference to an adfiller
    public AdFiller adFiller;

    // Use this for initialization
    void Start()
    {
        if (Windows_Ad_Plugin.Helper.Instance.HasGrid)
        {
            CreateAd();
        }
        //Add in our hook to the Plugins messenger
        Windows_Ad_Plugin.Helper.Instance.Messenger += UnityMessenger;

        if (adFiller != null)
        {
            UpdateAdFiller();
        }
    }

    /// <summary>
    /// Updates the AdFiller's horizontal and vertical alignment settings
    /// </summary>
    private void UpdateAdFiller()
    {
        adFiller.horizontalAlignment = horizontalAlignment;
        adFiller.verticalAlignment = verticalAlignment;
        adFiller.UpdateAdRect();
    }

    /*<Summary>
     * Wahoo for an update
     * The helper plugin sends up its current error
     * If the ad is not built then the script tries to make it again
     <Summary>*/
    void Update()
    {
        if (!Windows_Ad_Plugin.Helper.Instance.IsAdBuilt && Windows_Ad_Plugin.Helper.Instance.HasGrid)
        {
            CreateAd();
        }
    }

    /// <summary>
    /// Used to print out messages sent from the plugin
    /// </summary>
    void UnityMessenger(string msg)
    {
        if (printDebug)
            Debug.Log(msg);
    }

    /*<Summary>
     * Function calls the Create Ad function in the plugin
     * Based on what information is passed in, you will get an admob or windows ad
     * Thats if you were to call it but in this case the code is separated into to scripts
     <Summary>*/
    public void CreateAd()
    {
        Windows_Ad_Plugin.Helper.Instance.CreateAd(
            AppId,
            UnitId,
            (double)adHeight,
            (double)adWidth,
            autoRefreshAd,
            horizontalAlignment,
            verticalAlignment
            );
    }
#endif
}
