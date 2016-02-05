#import <UIKit/UIKit.h>
extern "C" {
#import <TAPPX/TAPPXBanner.h>
}
@interface TAPPXUnityBanner : UIViewController  <TAPPXBannerDelegate>{
    TAPPXBannerView *bannerView;
    BOOL position;
    BOOL isBannerVisible;
}

+ (void) trackInstall:(NSString *) tappxID;
+ (void) createBanner:(int)position;
- (void) hideAd;
- (void) showAd;
- (CGPoint) layoutAdView;

@property(nonatomic, strong) TAPPXBannerView *bannerView;
@property(assign) BOOL position;

@end
