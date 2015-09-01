//
//  VservAdDelegate.h
//  VServAdSDK
//
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//

/*!
 @enum      VservAdRequestError
 @abstract  Error codes definition. This is sent by SDK in adView:didFailedToLoadAd: callback in [NSError code] field.
 @field     kVservAdRequestError_ConnectionFailed, Connection to the Ad Server failed.
 @field     kVservAdRequestError_NoFill, There is no ad content received in the responce from the ad server.
 @field     kVservAdRequestError_LoadingFailed, Could not load the ad received from ad server.
 @field     kVservAdError_InvalidResponce, Requested Ad Type and Received Ad Type does not match.
 */
typedef NS_ENUM(NSInteger, VservAdError) {
    kVservAdError_ConnectionFailed = -100,
    kVservAdError_NoFill,
    kVservAdError_LoadingFailed,
    kVservAdError_InvalidResponce
};

@class VservAdView;

/*!
 @protocol      VservAdDelegate
 @abstract      This interface declares the delegate methods that will be called by VservAdView class on events.
 */
@protocol VservAdDelegate <NSObject>
@optional

/*!
 @function  adViewDidCacheAd:
 @abstract  This method will be called by SDK when the ad is cached and cacheAdWithorientation: in the background. The ad is not displayed yet. To display the ad call showAd method.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)adViewDidCacheAd:(VservAdView *)theAdView;

/*!
 @function  adViewDidLoadAd:
 @abstract  This method will be called by SDK when the ad is loaded to the ad spot and is visible to the user.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)adViewDidLoadAd:(VservAdView *)theAdView;

/*!
 @function  adView: adViewDidFailedToLoadAd:
 @abstract  This method will be called by SDK when the adview failed to load the ad.
 @param     theAdView, VservAdView. The ad view which sent this event.
 @param     error, NSError. Description of error occured.
 */
- (void)adView:(VservAdView *)theAdView didFailedToLoadAd:(NSError *)error;

/*!
 @function  didInteractWithAd:
 @abstract  This method will be called when used interacts with the ad.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)didInteractWithAd:(VservAdView *)theAdView;

/*!
 @function  willPresentOverlay:
 @abstract  This method will be called when the SDK is about to present the overlay on top of the user view.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willPresentOverlay:(VservAdView *)theAdView;

/*!
 @function  willDismissOverlay:
 @abstract  This method will be called when the SDK is about to dismis the Overlay put by the SDK.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willDismissOverlay:(VservAdView *)theAdView;

/*!
 @function  willLeaveApp:
 @abstract  This method will be called when the SDK is taking the app to background due to action of the ad. For instance when clicking on ad will launch Safari which caused this app to go to background and safari to come front.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willLeaveApp:(VservAdView *)theAdView;

@end