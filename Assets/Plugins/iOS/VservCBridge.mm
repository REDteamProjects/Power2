//
//  VservCBridge.m
//  
//
//  Created by Jesudas Lobo on 10/12/14.
//
//

#import "VservCBridge.h"
#import "VservUAdView.h"

@implementation VservCBridge

@end

extern "C"
{
    // Callbacks
    
    /// <summary>
    /// Throws the did cache ad.
    /// </summary>
    void ThrowDidCacheAd(int adInstance)
    {
        adViewDidCacheAd(adInstance);
    }
    /// <summary>
    /// Throws the did load ad.
    /// </summary>
    void ThrowDidLoadAd(int adInstance)
    {
        adViewDidLoadAd(adInstance);
    }
    /// <summary>
    /// Throws the did ad view failed to load ad.
    /// </summary>
    /// <param name="message">Message.</param>
    void ThrowDidAdViewFailedToLoadAd(int adInstance, const char * message)
    {
        adViewDidFailedToLoadAd(adInstance,message);
    }
    /// <summary>
    /// Throws the did interact with ad.
    /// </summary>
    void ThrowDidInteractWithAd(int adInstance)
    {
        didInteractWithAd(adInstance);
    }
    
    /// <summary>
    /// Throws the will present overlay.
    /// </summary>
    void ThrowWillPresentOverlay(int adInstance)
    {
        willPresentOverlay(adInstance);
    }
    
    /// <summary>
    /// Throws the will dismiss overlay.
    /// </summary>
    void ThrowWillDismissOverlay(int adInstance)
    {
        willDismissOverlay(adInstance);
    }
    
    /// <summary>
    /// Throws the will leave app.
    /// </summary>
    void ThrowWillLeaveApp(int adInstance)
    {
        willLeaveApp(adInstance);
    }
    
    //Native Functions
    
    void _initializeCallbacks( VservAdCallback _adViewDidCacheAd,
                               VservAdCallback _adViewDidLoadAd,
                               VservAdCallbackWithString _adViewDidFailedToLoadAd,
                               VservAdCallback _didInteractWithAd,
                               VservAdCallback _willPresentOverlay,
                               VservAdCallback _willDismissOverlay,
                               VservAdCallback _willLeaveApp)
    {
        NSLog(@"Registering Callbacks...");
        adViewDidCacheAd        = _adViewDidCacheAd;
        adViewDidLoadAd         = _adViewDidLoadAd;
        adViewDidFailedToLoadAd = _adViewDidFailedToLoadAd;
        didInteractWithAd       = _didInteractWithAd;
        willPresentOverlay      = _willPresentOverlay;
        willDismissOverlay      = _willDismissOverlay;
        willLeaveApp            = _willLeaveApp;
    }
    int _createNewVservAd(const char * zoneId,GLuint adUXType, GLuint adPosition)
    {
        NSLog(@"Create Ad is Called %s %u %u",zoneId,adUXType,adPosition);
        int adInstance;
        if( adUXType == 0)
        {
            adInstance = [VservUAdView createInterstitialAdForZoneID:zoneId];
        }
        else
        {
            adInstance = [VservUAdView createBannerAdForZoneID:zoneId
                                                  forPlacement:adPosition];

        }
        return adInstance;
    }
    void _releaseVservAdViewInstance(int adInstance)
    {
        [VservUAdView releaseVservAdViewInstance : adInstance];
    }
    void _setRefresh(int adInstance, bool shouldEnableRefresh)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setRefresh:shouldEnableRefresh];
        }
        NSLog(@"Enabling Refresh Rate...");

    }
    void _setRefreshRate(int adInstance, int refreshIntervalInseconds)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setRefreshRate:refreshIntervalInseconds];
        }
        NSLog(@"Setting Refresh Rate...");
    }
    void _pauseRefresh(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView pauseRefresh];
        }
        NSLog(@"Pausing Refresh...");
    }
    void _stopRefresh(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView stopRefresh];
        }
        NSLog(@"Stopping Refresh...");
    }
    void _resumeRefresh(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView resumeRefresh];
        }
        NSLog(@"Resuming Refresh...");
    }
    void _setTimeOut(int adInstance, int timeOut)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setTimeout:timeOut];
        }
        NSLog(@"Setting Timeout...");
    }
    void _setUXTypeWithConfig(int adInstance,int adUXType, const char * configString)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setUXType:adUXType
                   withConfig:configString];
        }
        NSLog(@"Setting UXType... %s",configString);
    }
    void _cacheAd(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView cacheAd];
        }
        NSLog(@"Caching Ad...");
    }
    void _cacheAdWithOriention(int adInstance, int orientation)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView cacheAdWithOrientation:orientation];
        }
        NSLog(@"Caching Ad with orientation...");
    }
    void _loadAd(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView loadAd];
        }
        NSLog(@"Loading Ad...");
        ThrowDidLoadAd(adInstance);
    }
    void _loadAdWithOriention(int adInstance, int orientation)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView loadAdWithOrientation:orientation];
        }
        NSLog(@"Loading Ad with orientation...");
    }
    void _showAd(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView showAd];
        }
        NSLog(@"Showing Ad...");
    }
    void _cancelAd(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView cancelAd];
        }
        NSLog(@"Cancelling Ad...");
    }
    int _getAdState(int adInstance)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        int currentAdState;
        if(AdView)
        {
            currentAdState = [AdView getAdState];
        }
        NSLog(@"Getiing Current State of the Ad...");
        return currentAdState;
    }
    void _setTestDevice(int adInstance, const char * deviceIDFAListString)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setTestDevice:deviceIDFAListString];
        }
        NSLog(@"Setting Test device...");
    }
    void _setUser(int adInstance, const char * userString)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setUser:userString];
        }
        NSLog(@"Assigning User...");
    }
    void _blockAds(int adInstance, bool shouldBlockAds)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView blockAds:shouldBlockAds];
        }
        NSLog(@"Blocking Ads %s...",shouldBlockAds);
    }
    void _showAdsAfterSessions(int adInstance, int noOfSessions)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView showAdsAfterSessions:noOfSessions];
        }
        NSLog(@"Show Ads after %d number of sessions...",noOfSessions);
    }
    void _blockCountries(int adInstance, const char * countryListString, int allowOrBlock, bool shouldRequest)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
                    [AdView blockCountries:countryListString
                              blockOrAllow:allowOrBlock
           shouldRequestAdIfMCCUnawailable:shouldRequest];
        }
        NSLog(@"Blocking Contries %d, List %s %d...",allowOrBlock,countryListString,shouldRequest);
    }
    
    void _disableOffline(int adInstance, bool shouldDisableOffline)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView disableOffline:shouldDisableOffline];
        }
        NSLog(@"Disable Offline %d...", shouldDisableOffline);
    }
    void _userDidIAP(int adInstance, bool didIAP)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView userDidIAP:didIAP];
        }
        NSLog(@"User did IAP %d...",didIAP);
    }
    void _userDidIncent(int adInstance, bool didIncent)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView userDidIncent:didIncent];
        }
        NSLog(@"User did incent %d...",didIncent);
    }
    void _setLanguageOfArticle(int adInstance, const char * languageOfArticle)
    {
        VservUAdView* AdView = [VservUAdView getVservAdViewInstance:adInstance];
        
        if(AdView)
        {
            [AdView setLanguageOfArticle:languageOfArticle];
        }
        NSLog(@"Setting Language Of Article %s...",languageOfArticle);
    }
}