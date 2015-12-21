using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public enum AdMobADType
{
    NoAd,
    Banner,
    Interstitial
}

public class UnityADHelper : MonoBehaviour
{

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    public AdMobADType Type = AdMobADType.NoAd;

    public AdSize BannerSize = AdSize.Banner;

    public static int AdTaps
    {
       get
       {
        if (PlayerPrefs.HasKey("AdTaps"))
            return PlayerPrefs.GetInt("AdTaps");
        return 0;
       }

       set
       {
            PlayerPrefs.SetInt("AdTaps", value);
       }
    }


    private void OnAdTap()
    {
        AdTaps++;
        if(AdTaps == 16)
            switch (Type)
            {
                case AdMobADType.Banner:
                    DeleteBanner();
                    break;
                case AdMobADType.Interstitial:
                    DeleteInterstitial();
                    break;
            }
    }

    void Awake()
    {
#if !DEBUG
        switch (Type)
        {
            case AdMobADType.NoAd:
                return;
            case AdMobADType.Banner:
                CreateBanner();
                return;
            case AdMobADType.Interstitial:
                CreateInterstitial();
                if (!interstitialAd.IsLoaded())
                    StartCoroutine(InterstitialAdCoroutine());
                else
                    interstitialAd.Show();
                return;
        }
#endif
    }

    private IEnumerator InterstitialAdCoroutine()
    {
        while (!interstitialAd.IsLoaded()) {}
        LogFile.Message("interstitialAd.IsLoaded()");
        interstitialAd.Show();
        yield return null;
    }

    public void OnDestroy()
    {
        DeleteBanner();
        DeleteInterstitial();
    }

    private void CreateBanner()
    {
        if (AdTaps > 16)
            return;

        try 
        { 
            var bannerId = String.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
            bannerId = "ca-app-pub-8526262957509496/2427972369";
#endif
#if UNITY_IOS && !UNITY_EDITOR
            bannerId = "ca-app-pub-8526262957509496/3765104764";
#endif

            if (String.IsNullOrEmpty(bannerId))
                return;

            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(
                    bannerId, BannerSize, AdPosition.Bottom);
            // Create an empty ad request.
            var request = new AdRequest.Builder()
#if DEBUG
                .AddTestDevice(AdRequest.TestDeviceSimulator)
#endif
                .Build();
            // Load the banner with the request.
            bannerView.LoadAd(request);

            GetComponent<Button>().onClick.AddListener(() => OnAdTap());
        }
        catch (Exception ex)
        {
            LogFile.Message(ex.Message + ex.StackTrace);
        }
    }

    private void DeleteBanner()
    {
        if (bannerView != null)
            bannerView.Destroy();
    }

    private void DeleteInterstitial()
    {
        if (interstitialAd != null)
            interstitialAd.Destroy();
    }

    private void CreateInterstitial()
    {
        if (AdTaps > 16)
            return;
        try
        {
            var interstitialId = String.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
            interstitialId = "ca-app-pub-8526262957509496/1817366765";
#endif
#if UNITY_IOS && !UNITY_EDITOR
        interstitialId = "ca-app-pub-8526262957509496/1817366765";
#endif
            if (String.IsNullOrEmpty(interstitialId))
                return;

            // Initialize an InterstitialAd.
            var interstitial = new InterstitialAd(interstitialId);
            // Create an empty ad request.
            var request = new AdRequest.Builder()
#if DEBUG
                .AddTestDevice(AdRequest.TestDeviceSimulator)
#endif
                .Build();
            // Load the interstitial with the request.
            interstitial.LoadAd(request);
        }
        catch (Exception ex)
        {
            LogFile.Message(ex.Message + ex.StackTrace);
        }
    }
}
