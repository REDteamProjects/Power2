//
//  VservUAdView.h
//  VservAdSDK
//
//  Created by RAHUL CK on 10/12/14.
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "VservAdView.h"

typedef NS_ENUM(NSInteger, VservUBannerAdPlacement) {
    kVservUBannerAdPlacementTop,
    kVservUBannerAdPlacementBottom
};

@interface VservUAdView : NSObject

/*!
 @function      createInterstitialVservAdView
 @abstract      Call this method to create Interstitial VservAd View
 */
+(int) createInterstitialAdForZoneID: (const char*)inZoneID;

/*!
 @function      createBannerVservAdView
 @abstract      Call this method to create Banner VservAd View
 */
+(int) createBannerAdForZoneID: (const char*)inZoneID
                  forPlacement: (VservUBannerAdPlacement)inPlamenent;
/*!
 @function      getVservAdViewInstance
 @abstract      Call this method to get the VservAdview Instance
 */

+(VservUAdView*) getVservAdViewInstance: (int)instanceId;
/*!
 @function      releaseVservAdViewInstance
 @abstract      Call this method to release the Vserv AdView Instance
 */
+(void) releaseVservAdViewInstance: (int)instanceId;
/*!
 @function      setRefresh:
 @abstract      Call this method to enable or disable refresh property. Before setting the refresh use setRefreshRate: to override the default refresh rate of 30 seconds. By default for banner ads refresh is ON and for interstetial ads refresh is always off.
 @param         shouldEnableRefresh, BOOL. If YES Refresh is enabled. If NO Refresh is disabled.
 */
-(void)setRefresh:(BOOL)shouldEnableRefresh;

/*!
 @function      setRefreshRate:
 @abstract      Call this method to set the refresh rate. This overrides the default refresh rate of 30 seconds.
 @param         inRefreshIntervalInSeconds, UInt32. Refresh rate in seconds. Value should be greater than 30 seconds.
 */
-(void)setRefreshRate:(UInt32)inRefreshIntervalInSeconds;

/*!
 @function      pauseRefresh
 @abstract      Pause the auto refreshing of Banner ads.
 */
-(void)pauseRefresh;

/*!
 @function      stopRefresh
 @abstract      Stop the Auto refreshing of Ads. Resume refresh whill have no effect after stoping the refresh.
 */
-(void)stopRefresh;

/*!
 @function      resumeRefresh
 @abstract      Resume the Auto refreshing of Ads. Call this method if pauseRefresh was used to pause the refresh.Resume refresh whill have no effect after stoping the refresh.
 */
-(void)resumeRefresh;

/*!
 @function      setTimeout:
 @abstract      Set the Ad Request timeout. Default timeout is 20 seconds.
 @param         inTimeout, UInt32. Timeout period in seconds.
 */
-(void)setTimeout:(UInt32)inTimeout;

/*!
 @function      setUXType: withConfig:
 @abstract      Set the Configration properties and display properties for Ads.
 @param         inAdUXType, VservAdUX. Ad Type. Must be same as passed in init method.
 @param         inConfig, NSDictionary. Dictionary specifying the display properties of Ads. Posible properties are VservAdView_BackgroundColor, VservAdView_CloseButtonPosition, VservAdView_FrameColorVservAdView_Transparence;
 */
-(void)setUXType:(VservAdUX)inAdUXType
      withConfig:(const char*)configString;

/*!
 @function      cacheAd
 @abstract      Manually cache the ad in background. Ad will not be displayed. On success adViewDidCacheAd: method will be called by SDK.
 On Failure adView: adViewDidFailedToLoadAd: will be called by SDK. When this method is called Auto Refresh is automatically disabled.
 */
-(void)cacheAd;

/*!
 @function      cacheAdWithOrientation:
 @abstract      Manually cache the ad in background and request the Ad Server the ad with orientation. Ad will not be displayed. On success adViewDidCacheAd: method will be called by SDK.
 On Failure adView: adViewDidFailedToLoadAd: will be called by SDK. When this method is called Auto Refresh is automatically disabled.
 @param         inOrientation, VservAdOrientation. Orientation of the required ad.
 */
-(void)cacheAdWithOrientation:(VservAdOrientation)inOrientation;

/*!
 @function      loadAd
 @abstract      Loads the Ad and displays it in the ad spot. This will start the Auto refresh property. If user has set the refresh rate then it will use that refresh rate. Else default refresh rate of 30 sec will be used.
 */
-(void)loadAd;

/*!
 @function      loadAdWithOrientation:
 @abstract      Loads the Ad and displays it in the ad spot and request the Ad Server the ad with orientation. This will start the Auto refresh property. If user has set the refresh rate then it will use that refresh rate. Else default refresh rate of 30 sec will be used.
 @param         inOrientation, VservAdOrientation. Orientation of the required ad.
 */
-(void)loadAdWithOrientation:(VservAdOrientation)inOrientation;

/*!
 @function      showAd
 @abstract      Use this method in conjunction with cacheAd. If the ad is cached in background in responce to cacheAd or cacheAdWithorientation, this method will display the cache ad on screen. For Billboard ads, this method puts full screen overlay on screen.
 */
-(void)showAd;

/*!
 @function      cancelAd
 @abstract      Cancel the request sent to ad server. Call this method if you have called cacheAd and you do not want to display that ad before adViewDidCacheAd is fired.
 */
-(void)cancelAd;

/*!
 @function      getAdState
 @abstract      Retrives the state of the ad.
 @return        VservAdState, value from VservAdState structure.
 */
-(VservAdState)getAdState;

/*!
 @function      setTestDevice
 @abstract      Set the test devices.
 @param         deviceIDFAListString, deviceIDFA List seperated by comma.
 */
-(void)setTestDevice:(const char*)deviceIDFAListString;

/*!
 @function      setUser:
 @abstract      Set the User information in order to deliver more appropriate ads targetter for the user.
 @param         userDetails, User details seperated by comma.
 */
-(void)setUser:(const char *)userParams;

/*!
 @function      blockAds:
 @abstract      If true blocks all the adds after this method is called. If false will unblock the ads after this method is called.
 @param         inShouldBlockAds, Bool indicating whether to block the ads.
 */
-(void)blockAds:(BOOL)inShouldBlockAds;

/*!
 @function      setShowAfterSessions:
 @abstract      Blocks the ads for number of sessions input.
 @param         inNoOfSessions, Number of sessions to block.
 */
-(void)showAdsAfterSessions:(int)inNoOfSessions;

/*!
 @function      blockCountries:blockOrAllow:shouldRequestAdIfMCCUnawailable:
 @abstract      Blocks the ads cased on countries.
 @param         inCountryList, List of countries to block the ads of type VservCountry mentioned in VservCountryCodex.h.
 @param         inAllowOrBlock, option from VservCountryOption. Block will block the counties in the country list and allow will allow only the countries in the country list.
 @param         inShouldRequest, YES if ad to be requested if MCC is unawailable.
 */
-(void)blockCountries:(const char*)countryListString blockOrAllow:(VservCountryOption)inAllowOrBlock shouldRequestAdIfMCCUnawailable:(BOOL)inShouldRequest;

/*!
 @function      disableOffline:
 @abstract      Disables the App access when offline. If set to YES then SDK will prompt an allert whenever internet connection is not awailable.
 To access the App the user has to either enable the internet connection or Quit the app.
 @param         inShouldDisableoffline, YES to block the App access while offline.
 */
-(void)disableOffline:(BOOL)inShouldDisableOffline;

-(void)userDidIAP:(BOOL)inDidIAP;

-(void)userDidIncent:(BOOL)inDidIncent;

-(void)setLanguageOfArticle:(const char*)inLanguageofArticle;
@end
