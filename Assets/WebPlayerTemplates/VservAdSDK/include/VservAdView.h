//
//  VServAdView.h
//  VServAdSDK
//
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "VservAdDelegate.h"


/*!
 Keys to coustumize the VservAdView appearience.
 
  @field VservAdView_BackgroundColor:
 Background color of the AdView. Should be UIColor object. This property is applicable for Banner and interstetial ads.
 
  @field VservAdView_CloseButtonPosition:
 Close button placement of ads. Should be a NSNumber with VservAdCloseButtonPlacement element.
 
  @field VservAdView_FrameColor:
 Frame colour of adView. Should be UIColor object. Applicable to both Banner and Interstetial ads.
 
  @field VservAdView_Transparence:
 Transperence of adView. Should be NSNumber of float value. Value between 0 and 1. 0 = Full Opaque. 1 = Full Transperent.
 */
extern NSString* const VservAdView_BackgroundColor;
extern NSString* const VservAdView_CloseButtonPosition;
extern NSString* const VservAdView_FrameColor;
extern NSString* const VservAdView_Transparence;

/*!
 @enum      VservAdState
 @abstract  Internal states of ads.
 @field     kVservAdState_RequestSent, New request for ad has been sent. Yet to receive the responce from adServer.
 @field     kVservAdState_Cached, Ad is cached and ready to be displayed. Ads will be inthis state if user 
            calls cacheAd and caching is successful.
 @field     kVservAdState_CacheFailed, Ad caching is failed. Ads will be in this state when user called 
            cacheAd and request to server failed due to some reason. When caching fails 
            adView: adViewDidFailedToLoadAd: is involed with error.
 @field     kVservAdState_Loaded, Ad is loaded and is being displayed.
 @field     kVservAdState_Failed, Communication to adserver failed.
 */
typedef NS_ENUM(NSUInteger, VservAdState) {
    kVservAdState_RequestSent,
    kVservAdState_Cached,
    kVservAdState_CacheFailed,
    kVservAdState_Loaded,
    kVservAdState_Failed,
};

/*!
 @enum      VservAdCloseButtonPlacement
 @abstract  Placement position of close button on ads. Eventhough closebutton placement is set by the user,
            the ad can override this property and display close button in some other position.
 @field     kVservAdCloseButtonPlacement_None, No preference for close button placement.
 @field     kVservAdCloseButtonPlacement_TopLeft, Close button placed top left corner of the VservAdView.
 @field     kVservAdCloseButtonPlacement_TopRight, Close button placed top right corner of the VservAdView.
 @field     kVservAdCloseButtonPlacement_BottomLeft, Close button placed bottom left corner of the VservAdView.
 @field     kVservAdCloseButtonPlacement_BottomRight, Close button placed bottom right corner of the VservAdView.
 */
typedef NS_ENUM(NSUInteger, VservAdCloseButtonPlacement) {
    kVservAdCloseButtonPlacement_None,
    kVservAdCloseButtonPlacement_TopRight,
    kVservAdCloseButtonPlacement_TopLeft,
    kVservAdCloseButtonPlacement_BottomRight,
    kVservAdCloseButtonPlacement_BottomLeft,
};

/*!
 @enum      VservAdUX
 @abstract  The UX Type of the required Ad. User needs to set this property to the SDK.
 @field     kVservAdUX_Interstitial, Interstetial UX Type.
 @field     kVservAdUX_Banner, Banner UX Type.
 */
typedef NS_ENUM(NSUInteger, VservAdUX) {
    kVservAdUX_Interstitial,
    kVservAdUX_Banner,
};

/*!
 @enum      VservAdOrientation
 @abstract  The orientation for the requested ad.
 @field     kVservAdOrientation_Adaptive, Ad capable of displaying in any orientation.
 @field     kVservAdOrientation_Portrait, Ad capable of displaying in portrait orientation.
 @field     kVservAdOrientation_Landscape, Ad capable of displaying in landscape orientation.
 */
typedef NS_ENUM(NSUInteger, VservAdOrientation) {
    kVservAdOrientation_Adaptive,
    kVservAdOrientation_Portrait,
    kVservAdOrientation_Landscape
};

/*!
 @enum      VservUserGender
 @abstract  Gender property for Optional user property.
 @field     kVservUserGender_Male, Gender Male.
 @field     kVservUserGender_Female, Gender Female.
 */
typedef NS_ENUM(NSUInteger, VservUserGender) {
    kVservUserGender_Male   = 'M',
    kVservUserGender_Female = 'F'
};

/*!
 @enum      VservCountryOption
 @abstract  Gender property for Optional user property.
 @field     kVservCountries_Exclude, Exclude the countries listed in the country list.
 @field     kVservCountries_Include, Include the countries listed in the country list.
 */
typedef NS_ENUM(NSUInteger, VservCountryOption) {
    kVservCountries_Exclude = 1,
    kVservCountries_Allow
};

/*!
 @enum      VservAdUser
 @abstract  Optional User property. user needs to set this property for more accurate ad delivery.
 @field     gender, Gender of User.
 @field     DOB, Date of birth string as returned by NSDateFormatter.
 @field     age, Age of the user.
 @field     city, City of user.
 @field     country, Country of user.
 @field     email, Mail ID of user.
 */
@interface VservAdUser : NSObject
@property (assign, nonatomic) VservUserGender   gender;
@property (strong, nonatomic) NSString*         DOB;
@property (strong, nonatomic) NSString*         age;
@property (strong, nonatomic) NSString*         city;
@property (strong, nonatomic) NSString*         country;
@property (strong, nonatomic) NSString*         email;
@end

/*!
 @enum      VservAdView
 @abstract  This is the main API interface of VservAdSDK. Confirm to VservAdDelegate protocol to receive the SDK messages.
            App Developers needs to set the frame of VservAdView by calling setFrame:(CGRect)inFrame before any of the Ad request is made.
            Ad Request methods are cacheAd, loadAd and showAd.
 
            For proper display of Ads the Zone ID must be properly set according to the AdType (Interstetial or Banner).
            For interstetial ad type Auto Refresh iof ads are not allowed. Hence any of the refresh methods dosent have any impact.
 
            For banner Ad Type If user wanto to have presize control over the ads he can opt for manual handling of ad caching and display. 
            So he can use cacheAd to cache the ad in background. On cache complete SDK notifies user by delegate method 
            - (void)adViewDidCacheAd:(VservAdView *)bannerView.
            The nuser can display the ad using showAd method. On ad display complete the SDK notifies user by delegate method
            - (void)adViewDidLoadAd:(VservAdView *)bannerView
            For banner Ad type if the user doesent want to hace presize control ever the ad display, he can set the frame of VservAdView and call loadAd.
            The user will get callback
            - (void)adViewDidLoadAd:(VservAdView *)bannerView
            when first ad is loaded. Subsequest refreshed ads dosent notifies the user of adRefresh. The default Refresh time is 20 seconds.
            User can set the Refresh time other than this by calling -(void)setRefreshRate:(UInt32)inRefreshIntervalInSeconds
 
            NOTE: Before any ads can be requested the user needs to set the frame of the VservADView to display the ad properly.
 */
@interface VservAdView : UIView


@property (weak, nonatomic) id<VservAdDelegate> delegate;

@property (strong, nonatomic, readonly) NSString* zoneId;

@property (weak, nonatomic) UIViewController* hostController;

/*!
 @function      initWithZoneId: viewController: withAdUXType
 @abstract      Init method to initilize the VservAdView.
 @param         inZoneId, NSString. Zone ID for the Ad. Zone ID can be for Banner or Bilboard.
 @param         inHostViewController, UIViewController. The video controller in which the Ad will be rendered. This will be used for displaying Full screen Billoard ads.
 @param         inAdUXType, VservAdUX. Ad UX type from struct VservAdUX.
 */
-(id)initWithZoneId:(NSString*)inZoneId
     viewController:(UIViewController*)inHostViewController
       withAdUXType:(VservAdUX)inAdUXType;

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
      withConfig:(NSDictionary*)inConfig;

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
 @param         inDeviceIDFAList, NSArray, Test device array.
 */
-(void)setTestDevice:(NSArray*)inDeviceIDFAList;

/*!
 @function      setUser:
 @abstract      Set the User information in order to deliver more appropriate ads targetter for the user.
 @param         user, VservAdUser, instance of filled VservAdUser object.
 */
-(void)setUser:(VservAdUser *)user;

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
-(void)blockCountries:(NSArray*)inCountryList blockOrAllow:(VservCountryOption)inAllowOrBlock shouldRequestAdIfMCCUnawailable:(BOOL)inShouldRequest;

/*!
 @function      disableOffline:
 @abstract      Disables the App access when offline. If set to YES then SDK will prompt an allert whenever internet connection is not awailable. 
                To access the App the user has to either enable the internet connection or Quit the app.
 @param         inShouldDisableoffline, YES to block the App access while offline.
 */
-(void)disableOffline:(BOOL)inShouldDisableOffline;

/*!
 @function      userDidIAP:
 @abstract      User Did IAP
 @param         inDidIAP, YES/NO.
 */
-(void)userDidIAP:(BOOL)inDidIAP;

/*!
 @function      userDidIncent:
 @abstract      User Did Incent
 @param         inDidIncent, YES/NO.
 */
-(void)userDidIncent:(BOOL)inDidIncent;

/*!
 @function      setLanguageOfArticle:
 @abstract      Sets the Language of Article.
 @param         inlanguageofArticle, Language of Article.
 */
-(void)setLanguageOfArticle:(NSString*)inlanguageofArticle;

@end
