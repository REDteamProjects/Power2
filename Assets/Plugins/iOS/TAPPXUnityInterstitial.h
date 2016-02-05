#import <UIKit/UIKit.h>
extern "C" {
#import <TAPPX/TAPPXInterstitial.h>
}

@interface TAPPXUnityInterstitial : UIViewController <TAPPXInterstitialDelegate>{
    TAPPXInterstitialView* interstitialView;
}

+ (void) createInterstitial;
- (void) showInterstitial;
- (void) loadInterstitial;

@property(nonatomic, strong) TAPPXInterstitialView *interstitialView;

@end
