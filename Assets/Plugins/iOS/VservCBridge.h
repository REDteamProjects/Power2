//
//  VservCBridge.h
//  
//
//  Created by Jesudas Lobo on 10/12/14.
//
//

#import <Foundation/Foundation.h>

@interface VservCBridge : NSObject

@end

extern "C" {
    //Defining function pointers for callbacks
    //Function Pointer : For function with no return value and one parameter(string)
    typedef void (*VservAdCallbackWithString)(int ,const char *);
    //Function Pointer : For function with no return value and no parameters
    typedef void (*VservAdCallback)(int);
}
static VservAdCallback adViewDidCacheAd;
static VservAdCallback adViewDidLoadAd;
static VservAdCallbackWithString adViewDidFailedToLoadAd;
static VservAdCallback didInteractWithAd;
static VservAdCallback willPresentOverlay;
static VservAdCallback willDismissOverlay;
static VservAdCallback willLeaveApp;



