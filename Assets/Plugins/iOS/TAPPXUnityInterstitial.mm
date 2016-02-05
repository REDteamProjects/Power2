#import "TAPPXUnityInterstitial.h"
extern "C" {
#import <TAPPX/TAPPXUtils.h>
}
extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

@implementation TAPPXUnityInterstitial

@synthesize interstitialView;

static TAPPXUnityInterstitial *instance = nil;

- (id)init {
    self = [super init];
    if (self != nil) {
        
    }
    return self;
}

+ (void) createInterstitial{
    if(instance != nil) return;
    
    TAPPXUnityInterstitial* inte = [[TAPPXUnityInterstitial alloc]init];
    instance = inte;
    
    TAPPXInterstitialView *interstitial = [[TAPPXInterstitial alloc] createInterstitial:inte];
    inte.interstitialView = interstitial;
    
}

- (void) loadInterstitial{
    self.interstitialView = [[TAPPXInterstitial alloc] createInterstitial:self];
}

- (void) showInterstitial{
    [self.interstitialView interstitialShow:self.interstitialView delegate:self];
}
- (UIViewController*)presentViewController{
    return UnityGetGLViewController();
}

- (void)dealloc {
    self.view = nil;
    instance = nil;
}
- (void)tappxInterstitialDidReceiveAd:(TAPPXInterstitialView *)ad{
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDidReceiveAd", "");
}
- (void)tappxInterstitial:(TAPPXInterstitialView *)ad didFailToReceiveAdWithError:(TAPPXRequestError *)error{
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialFailedToLoad", [[NSString stringWithFormat:@"%@",[error localizedFailureReason]] UTF8String]);
}
- (void)tappxInterstitialWillDismissScreen:(TAPPXInterstitialView *)ad{
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialWillDismissScreen", "");
}
- (void)tappxInterstitialWillPresentScreen:(TAPPXInterstitialView *)ad{
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialWillPresentScreen", "");
}
- (void)tappxInterstitialDidDismissScreen:(TAPPXInterstitialView *)ad{
    UnitySendMessage("TappxManagerUnity", "tappxInterstitialDidDismissScreen", "");
}
- (void)interstitialWillLeaveApplication:(TAPPXInterstitialView *)ad{
    UnitySendMessage("TappxManagerUnity", "interstitialWillLeaveApplication", "");
}

@end
extern "C" {
    void loadInterstitialIOS_();
    void showInterstitialIOS_();
    void releaseInterstitialTappxIOS_();
}

void loadInterstitialIOS_(){
    if(instance != nil){
        [instance loadInterstitial];
    }else{
        [TAPPXUnityInterstitial createInterstitial];
    }
}

void showInterstitialIOS_(){
    if(instance != nil){
        [instance showInterstitial];
    }
}

void releaseInterstitialTappxIOS_(){
    if(instance != nil){
        instance = nil;
    }
}