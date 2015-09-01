//
//  VservCustomAdListener.h
//  VservAdSDK
//
//  Created by Tejus Adiga on 22/10/14.
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol VservCustomAd;

@protocol VservCustomAdListener <NSObject>
@required

-(void)vservCustomAdDidLoadAd:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAd:(id<VservCustomAd>)inCustomAd
    didFailWithError:(NSError*)outError;

-(void)vservCustomAdDidShowAd:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAdOnAdClicked:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAdWillLeaveApplication:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAdDidDismissAd:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAd:(id<VservCustomAd>)inCustomAd
     didLoadAdInView:(UIView*)outAdView;

@optional

-(void)vservCustomAdWillPresentOverlay:(id<VservCustomAd>)inCustomAd;

-(void)vservCustomAdWillDismissOverlay:(id<VservCustomAd>)inCustomAd;

@end
