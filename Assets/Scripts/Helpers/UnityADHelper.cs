using System;
using System.Collections;
using Assets.Scripts.Helpers;
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

    public static Int32 AdTaps
    {
       get
       {
           return PlayerPrefs.HasKey("AdTaps") ? PlayerPrefs.GetInt("AdTaps") : 0;
       }

       set
       {
            PlayerPrefs.SetInt("AdTaps", value);
#if UNITY_WINRT || UNITY_WP8
            if (AdTaps != 16) return;
            WinRTDeviceHelper.FireHideAd();
#endif
       }
    }


    void Awake()
    {

        if (AdTaps >= 16)
            return;

#if UNITY_WINRT || UNITY_WP8
            WinRTDeviceHelper.FireShowAd();
#else
        switch (Type)
        {
            case AdMobADType.NoAd:
                return;
            case AdMobADType.Banner:
                CreateBanner();
                return;
            case AdMobADType.Interstitial:
                CreateInterstitial();
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
#if UNITY_WINRT || UNITY_WP8
            WinRTDeviceHelper.FireHideAd();
#else
        DeleteBanner();
        DeleteInterstitial();
#endif
    }

    private void CreateBanner()
    {
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
                .AddTestDevice("98446DEEDA9D51B42BD040E4FCE04273")
                .Build();

            bannerView.AdOpened += (sender, args) =>
            {
                AdTaps++;
                if (AdTaps != 16) return;
                DeleteBanner();
            };
            // Load the banner with the request.
            bannerView.LoadAd(request);
        }
        catch (Exception ex)
        {
            LogFile.Message(ex.Message + ex.StackTrace);
        }
    }
    
    private void CreateInterstitial()
    {
        try
        {
            var interstitialId = String.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
            interstitialId = "ca-app-pub-8526262957509496/1817366765";
#endif
//#if UNITY_IOS && !UNITY_EDITOR
//        interstitialId = "ca-app-pub-8526262957509496/1817366765";
//#endif
            if (String.IsNullOrEmpty(interstitialId))
                return;

            // Initialize an InterstitialAd.
            var interstitial = new InterstitialAd(interstitialId);
            // Create an empty ad request.
            var request = new AdRequest.Builder()
#if DEBUG
                .AddTestDevice(AdRequest.TestDeviceSimulator)
#endif
                .AddTestDevice("98446DEEDA9D51B42BD040E4FCE04273")
                .Build();

            // Load the interstitial with the request.
            interstitial.AdOpened += (sender, args) =>
            {
                AdTaps++;
                if (AdTaps != 16) return;
                DeleteInterstitial();
            };

            interstitial.LoadAd(request);

            if (!interstitialAd.IsLoaded())
                StartCoroutine(InterstitialAdCoroutine());
            else
                interstitialAd.Show();
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

    
}
