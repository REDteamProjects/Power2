#import "TAPPXUnityBanner.h"
extern "C" {
#import <TAPPX/TAPPXUtils.h>
}

extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

@implementation TAPPXUnityBanner

@synthesize bannerView;
@synthesize position;

static TAPPXUnityBanner *instance = nil;

+ (void) trackInstall:(NSString *)tappxID{
    [TAPPXUtils trackInstall:tappxID];
}

+ (void) createBanner:(int)position{
    if(instance != nil) return;
    
    // Init
    TAPPXUnityBanner *tappxUnityBanner = [[TAPPXUnityBanner alloc] init];
    instance = tappxUnityBanner;
    
    if (position==1) {
        instance.position = true;
    }else{
        instance.position = false;
    }
    TAPPXBannerView *bannerView = [[TAPPXBanner alloc] createBanner:tappxUnityBanner positionBanner:[tappxUnityBanner layoutAdView]];
    tappxUnityBanner.bannerView = bannerView;
    
}

- (CGPoint)layoutAdView{
    if(UI_USER_INTERFACE_IDIOM()==UIUserInterfaceIdiomPhone){
        if(UIInterfaceOrientationIsPortrait([[UIApplication sharedApplication] statusBarOrientation])){
            if(position)
                return CGPointMake(0, 0);
            else
                return CGPointMake(0, self.view.frame.size.height-50);
        }else{
            if(position)
                return CGPointMake((self.view.frame.size.width/2-160), 0);
            else
                return CGPointMake((self.view.frame.size.width/2-160), self.view.frame.size.height-50);
        }
    }else{
        if(UIInterfaceOrientationIsPortrait([[UIApplication sharedApplication] statusBarOrientation])){
            if(position)
                return CGPointMake(0, 0);
            else
                return CGPointMake(0, self.view.frame.size.height-90);
        }else{
            if(position)
                return CGPointMake((self.view.frame.size.width/2-256), 0);
            else
                return CGPointMake((self.view.frame.size.width/2-256), self.view.frame.size.height-90);
        }
    }
}

- (id)init {
    self = [super init];
    if (self != nil) {
        
    }
    return self;
}

- (void) showAd{
    if(self.bannerView==nil)
        self.bannerView = [[TAPPXBanner alloc] createBanner:self positionBanner:[self layoutAdView]];
}

- (void) hideAd{
    if(self.bannerView!=nil){
        self.bannerView.delegate = nil;
        self.bannerView.hidden = YES;
        [self.bannerView removeFromSuperview];
        self.bannerView = nil;
    }
}

- (UIViewController*)presentViewController{
    return UnityGetGLViewController();
}

- (void)tappxViewDidReceiveAd:(TAPPXBannerView *)view{
    UnitySendMessage("TappxManagerUnity", "tappxBannerDidReceiveAd", "");
}

- (void)tappxView:(TAPPXBannerView *)view didFailToReceiveAdWithError:(TAPPXRequestError *)error{
    UnitySendMessage("TappxManagerUnity", "tappxBannerFailedToLoad", [[NSString stringWithFormat:@"%@",[error localizedFailureReason]] UTF8String]);
}

- (void)tappxViewWillPresentScreen:(TAPPXBannerView *)adView{
    UnitySendMessage("TappxManagerUnity", "tappxViewWillPresentScreen", "");
}

- (void)tappxViewWillDismissScreen:(TAPPXBannerView *)adView{
    UnitySendMessage("TappxManagerUnity", "tappxViewWillDismissScreen", "");
}

- (void)tappxViewDidDismissScreen:(TAPPXBannerView *)adView{
    UnitySendMessage("TappxManagerUnity", "tappxViewDidDismissScreen", "");
}

- (void)tappxViewWillLeaveApplication:(TAPPXBannerView *)adView{
    UnitySendMessage("TappxManagerUnity", "tappxViewWillLeaveApplication", "");
}

- (void)dealloc {
    self.view = nil;
    [self.bannerView removeFromSuperview];
    self.bannerView.delegate = nil;
    //    [self.bannerView release];
    instance = nil;
    //  [super dealloc];
}

@end

extern "C" {
    void trackInstallIOS_(char *tappxID);
    void createBannerIOS_(int positionBanner);
    void hideAdIOS_();
    void showAdIOS_(int positionBanner);
    void releaseTappxIOS_();
}

void trackInstallIOS_(char *tappxID){
    [TAPPXUnityBanner trackInstall:[NSString stringWithCString:tappxID encoding:NSASCIIStringEncoding]];
}

void createBannerIOS_(int positionBanner){
    [TAPPXUnityBanner createBanner:positionBanner];
}

void hideAdIOS_(){
    if(instance != nil){
        [instance hideAd];
    }
}

void showAdIOS_(int positionBanner){
    if(instance != nil){
        [instance showAd];
    }else{
        [TAPPXUnityBanner createBanner:positionBanner];
    }
}

void releaseTappxIOS_(){
    if(instance != nil){
        instance = nil;
    }
}
