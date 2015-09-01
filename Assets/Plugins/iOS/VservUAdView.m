//
//  VservUAdView.m
//  VservAdSDK
//
//  Created by RAHUL CK on 10/12/14.
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//
#import "VservUAdView.h"

//User Params
NSString* const kVservUserParam_gender     = @"gender";
NSString* const kVservUserParam_dob        = @"dob";
NSString* const kVservUserParam_age        = @"age";
NSString* const kVservUserParam_city       = @"city";
NSString* const kVservUserParam_country    = @"country";
NSString* const kVservUserParam_email      = @"email";
NSString* const kVservUserParam_Name       = @"name";

static CGRect GetAdViewRect(VservUBannerAdPlacement inPlacement)
{
    CGRect adRect = CGRectZero;
    
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;
    
    UIDevice* thisDevice = [UIDevice currentDevice];
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
    
    NSInteger adheight = (thisDevice.userInterfaceIdiom == UIUserInterfaceIdiomPad)? 90 : 50;
    
    adRect = CGRectMake(0, 0, screenSize.width, adheight);
    adRect.origin.y = (inPlacement == kVservUBannerAdPlacementBottom)? (screenSize.height - adheight) : 0;

    return adRect;
}

@interface VservUAdView () <VservAdDelegate>

@property (nonatomic, strong) VservAdView *vservAdView;

@property(nonatomic, assign) VservAdUX AdUXType;

@property (nonatomic, assign) int instanceId;

@property (nonatomic, assign) BOOL isSendingRequest;

@end

static NSMutableDictionary *vservAdViewInstanceList;

static unsigned int vservInstanceCount;

@implementation VservUAdView

+(VservUAdView*) getVservAdViewInstance: (int)instanceId
{
    VservUAdView *vservUAdView = nil;
    
    vservUAdView = [vservAdViewInstanceList valueForKey:[VservUAdView getVservInstanceId:instanceId]];
    
    return vservUAdView;
}

+(int) createInterstitialAdForZoneID:(const char*)inZoneID
{
    VservUAdView *vservUadView = [[[VservUAdView alloc]initWithZoneId:inZoneID adType:kVservAdUX_Interstitial] autorelease];
    
    if (!vservAdViewInstanceList)
        vservAdViewInstanceList = [[NSMutableDictionary alloc]init];
    
    vservInstanceCount++;
    
    vservUadView->_instanceId = vservInstanceCount;

    [vservAdViewInstanceList setObject:vservUadView forKey:[VservUAdView getVservInstanceId:vservInstanceCount]];
    
    
    return vservInstanceCount;
}

+(int) createBannerAdForZoneID:(const char*)inZoneID forPlacement: (VservUBannerAdPlacement)inPlamenent
{
    VservUAdView *vservUadView = [[[VservUAdView alloc]initWithZoneId:inZoneID adType:kVservAdUX_Banner] autorelease];
    
    UIViewController *rootViewController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    
    CGRect adRect = GetAdViewRect(inPlamenent);
    
    [vservUadView->_vservAdView setFrame:adRect];

    [rootViewController.view addSubview:vservUadView->_vservAdView];
    
    if (!vservAdViewInstanceList)
        vservAdViewInstanceList = [[NSMutableDictionary alloc]init];
    
    vservInstanceCount++;
    
    vservUadView->_instanceId = vservInstanceCount;

    [vservAdViewInstanceList setObject:vservUadView forKey:[VservUAdView getVservInstanceId:vservInstanceCount]];
    
    return vservInstanceCount;
}

+(NSString*) getVservInstanceId:(unsigned int)instanceCount
{
    return [NSString stringWithFormat:@"VservInstance_%d",instanceCount];
}

+(void) releaseVservAdViewInstance: (int)instanceId
{
    [vservAdViewInstanceList removeObjectForKey:[VservUAdView getVservInstanceId:instanceId]];
    
    vservInstanceCount--;
    
    if (vservInstanceCount == 0 ) {
        vservAdViewInstanceList = nil;
    }
}

-(void)updateBannerOrientation:(NSNotification*)inNote
{
    CGSize screenSize = [[UIScreen mainScreen] bounds].size;
    
    CGRect adRect = self.vservAdView.frame;
    
    adRect.size.width = screenSize.width;
    
    self.vservAdView.frame = adRect;
}

-(id)initWithZoneId:(const char*)inZoneId adType:(VservAdUX)inAdUX
{
    if (self = [super init]) {
        
        UIViewController *rootViewController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        
        self.AdUXType = inAdUX;
        
        self.isSendingRequest = NO;
        
        self.vservAdView = [[VservAdView alloc]initWithZoneId: [NSString stringWithUTF8String:inZoneId]
                                               viewController: rootViewController
                                                 withAdUXType: inAdUX];
        
        self.vservAdView.delegate = self;
        
        if (inAdUX == kVservAdUX_Banner) {
            [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
            
            [[NSNotificationCenter defaultCenter] addObserver: self
                                                     selector: @selector(updateBannerOrientation:)
                                                         name: UIDeviceOrientationDidChangeNotification
                                                       object: nil];
        }
    }
    
    return self;
}

-(void)dealloc
{
    if (self.AdUXType == kVservAdUX_Banner) {
        if (self.vservAdView.superview) {
            [self.vservAdView removeFromSuperview];
        }
        
        [[UIDevice currentDevice] endGeneratingDeviceOrientationNotifications];
        
        [[NSNotificationCenter defaultCenter] removeObserver:self name:UIDeviceOrientationDidChangeNotification object:nil];

    }
    
    self.vservAdView.delegate = nil;
    
    if (self.vservAdView) {
        [self.vservAdView release];
        self.vservAdView = nil;
    }
    
    [super dealloc];
}

-(void)setRefresh:(BOOL)shouldEnableRefresh
{
    [self.vservAdView setRefresh:shouldEnableRefresh];
}


-(void)setRefreshRate:(UInt32)inRefreshIntervalInSeconds
{
    [self.vservAdView setRefreshRate:inRefreshIntervalInSeconds];
}

-(void)pauseRefresh
{
    [self.vservAdView pauseRefresh];
}

-(void)stopRefresh
{
    [self.vservAdView stopRefresh];
}

-(void)resumeRefresh
{
    [self.vservAdView resumeRefresh];
}

-(void)setTimeout:(UInt32)inTimeout
{
    [self.vservAdView setTimeout:inTimeout];
}

+ (UIColor *)UIColorWithHexString:(NSString *)hexString
{
    unsigned int rgbValue = 0;
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner scanHexInt:&rgbValue];
    
    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0
                           green:((rgbValue & 0xFF00) >> 8)/255.0
                            blue:(rgbValue & 0xFF)/255.0
                           alpha:1.0];
}

-(void)setUXType:(VservAdUX)inAdUXType
      withConfig:(const char*)configString
{
    NSArray *configArray=[[NSString stringWithUTF8String:configString]componentsSeparatedByString:@","];

    NSMutableDictionary *inConfig = [NSMutableDictionary dictionary];
    
    NSString* keyPrefix = @"VservAdView_";

    for (NSString *config in configArray) {
        NSArray *configuration = [config componentsSeparatedByString:@":"];
        NSString *configKey = [keyPrefix stringByAppendingString:[configuration objectAtIndex:0]];
        NSString *configValue = [configuration objectAtIndex:1];
        [inConfig setObject:configValue forKey:configKey];
        
        if (   [configKey isEqualToString:VservAdView_BackgroundColor]
            || [configKey isEqualToString:VservAdView_FrameColor]) {
            
            UIColor* color = [[self class] UIColorWithHexString:configValue];
            [inConfig setObject:color forKey:configKey];
        }
    }
    
    [self.vservAdView setUXType:inAdUXType withConfig:inConfig];
}

-(void)cacheAd
{
    if (!self.isSendingRequest) {
        self.isSendingRequest = YES;
        [self.vservAdView cacheAd];
    }
}

-(void)cacheAdWithOrientation:(VservAdOrientation)inOrientation
{
    if (!self.isSendingRequest) {
        self.isSendingRequest = YES;
        [self.vservAdView cacheAdWithOrientation:inOrientation];
    }
}

-(void)loadAd
{
    if (!self.isSendingRequest) {
        self.isSendingRequest = YES;
        [self.vservAdView loadAd];
    }
}

-(void)loadAdWithOrientation:(VservAdOrientation)inOrientation
{
    if (!self.isSendingRequest) {
        self.isSendingRequest = YES;
        [self.vservAdView loadAdWithOrientation:inOrientation];
    }
}

-(void)showAd
{
    [self.vservAdView showAd];
}

-(void)cancelAd
{
    [self.vservAdView cancelAd];
    self.isSendingRequest = NO;
}

-(VservAdState)getAdState
{
    return [self.vservAdView getAdState];
}

-(void)setTestDevice:(const char*)deviceIDFAListString
{
    NSArray *inDeviceIDFAList=[[NSString stringWithUTF8String:deviceIDFAListString]componentsSeparatedByString:@","];
    [self.vservAdView setTestDevice:inDeviceIDFAList];
}

-(void)setUser:(const char *)userParams
{
    NSArray *userParamsArray = [[NSString stringWithUTF8String:userParams]componentsSeparatedByString:@","];
    
    VservAdUser * user = [[VservAdUser alloc]init];
    
    for (NSString *param in userParamsArray) {
        
        NSArray *userParam = [param componentsSeparatedByString:@":"];
        NSString *userParamKey = [userParam objectAtIndex:0];
        NSString *userParamValue = (userParam.count>1)?[userParam objectAtIndex:1]:@"";
        
        if ([userParamKey isEqualToString:kVservUserParam_gender])
            user.gender = userParamValue.integerValue;
        else if ([userParamKey isEqualToString:kVservUserParam_dob])
            user.DOB= userParamValue;
        else if ([userParamKey isEqualToString:kVservUserParam_age])
            user.age = userParamValue;
        else if ([userParamKey isEqualToString:kVservUserParam_city])
            user.city = userParamValue;
        else if ([userParamKey isEqualToString:kVservUserParam_country])
            user.country = userParamValue;
        else if ([userParamKey isEqualToString:kVservUserParam_email])
            user.email = userParamValue;
    }
    

    [self.vservAdView setUser:user];
}

-(void)blockAds:(BOOL)inShouldBlockAds
{
    [self.vservAdView blockAds:inShouldBlockAds];
}

-(void)showAdsAfterSessions:(int)inNoOfSessions
{
    [self.vservAdView showAdsAfterSessions:inNoOfSessions];
}

-(void)blockCountries:(const char *)countryListString blockOrAllow:(VservCountryOption)inAllowOrBlock shouldRequestAdIfMCCUnawailable:(BOOL)inShouldRequest
{
    NSArray *inCountryList=[[NSString stringWithUTF8String:countryListString]componentsSeparatedByString:@","];

    [self.vservAdView blockCountries:inCountryList blockOrAllow:inAllowOrBlock shouldRequestAdIfMCCUnawailable:inShouldRequest];
}

-(void)disableOffline:(BOOL)inShouldDisableOffline
{
    [self.vservAdView disableOffline:inShouldDisableOffline];
}

-(void)userDidIAP:(BOOL)inDidIAP
{
    [self.vservAdView userDidIAP:inDidIAP];
}

-(void)userDidIncent:(BOOL)inDidIncent
{
    [self.vservAdView userDidIncent:inDidIncent];
}

-(void)setLanguageOfArticle:(const char*)inLanguageofArticle
{
    [self.vservAdView setLanguageOfArticle:[NSString stringWithUTF8String:inLanguageofArticle]];
}

#pragma mark - VservAdDelegate

/*!
 @function  adViewDidCacheAd:
 @abstract  This method will be called by SDK when the ad is cached and cacheAdWithorientation: in the background. The ad is not displayed yet. To display the ad call showAd method.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)adViewDidCacheAd:(VservAdView *)theAdView
{
    self.isSendingRequest = NO;
    ThrowDidCacheAd(self.instanceId);
}

/*!
 @function  adViewDidLoadAd:
 @abstract  This method will be called by SDK when the ad is loaded to the ad spot and is visible to the user.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)adViewDidLoadAd:(VservAdView *)theAdView
{
    self.isSendingRequest = NO;
    ThrowDidLoadAd(self.instanceId);
}

/*!
 @function  adView: adViewDidFailedToLoadAd:
 @abstract  This method will be called by SDK when the adview failed to load the ad.
 @param     theAdView, VservAdView. The ad view which sent this event.
 @param     error, NSError. Description of error occured.
 */
- (void)adView:(VservAdView *)theAdView didFailedToLoadAd:(NSError *)error
{
    self.isSendingRequest = NO;
    ThrowDidAdViewFailedToLoadAd(self.instanceId, [error.localizedDescription UTF8String]);
}

/*!
 @function  didInteractWithAd:
 @abstract  This method will be called when used interacts with the ad.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)didInteractWithAd:(VservAdView *)theAdView
{
    ThrowDidInteractWithAd(self.instanceId);
}

/*!
 @function  willPresentOverlay:
 @abstract  This method will be called when the SDK is about to present the overlay on top of the user view.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willPresentOverlay:(VservAdView *)theAdView
{
    ThrowWillPresentOverlay(self.instanceId);
}

/*!
 @function  willDismissOverlay:
 @abstract  This method will be called when the SDK is about to dismis the Overlay put by the SDK.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willDismissOverlay:(VservAdView *)theAdView
{
    ThrowWillDismissOverlay(self.instanceId);
}

/*!
 @function  willLeaveApp:
 @abstract  This method will be called when the SDK is taking the app to background due to action of the ad. For instance when clicking on ad will launch Safari which caused this app to go to background and safari to come front.
 @param     theAdView, VservAdView. The ad view which sent this event.
 */
- (void)willLeaveApp:(VservAdView *)theAdView
{
    ThrowWillLeaveApp(self.instanceId);
}

@end
