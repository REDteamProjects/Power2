//
//  VservCustomAd.h
//  VservAdSDK
//
//  Created by Tejus Adiga on 22/10/14.
//  Copyright (c) 2014 Vserv.mobi. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <UIKit/UIKit.h>

#import "VservCustomAdListener.h"

// User Info
extern NSString* const kVservCustomAdExtras_Gender;     // NSString can be either kVservCustomAdExtras_GenderMale or kVservCustomAdExtras_GenderFemale
extern NSString* const kVservCustomAdExtras_Email;      // NSString of EMail.
extern NSString* const kVservCustomAdExtras_Age;        // NSString of Age.
extern NSString* const kVservCustomAdExtras_DOB;        // NSString of DOB.
extern NSString* const kVservCustomAdExtras_Country;    // NSString Country namne.
extern NSString* const kVservCustomAdExtras_City;       // NSString City Name.

// Attributes for kVservCustomAdExtras_Gender key.
extern NSString* const kVservCustomAdExtras_GenderMale;
extern NSString* const kVservCustomAdExtras_GenderFemale;

// User Location info
extern NSString* const kVservCustomAdExtras_LocationLatitude;   // NSNumber of Double datatype
extern NSString* const kVservCustomAdExtras_LocationLongitude;  // NSNumber of Double datatype.

@protocol VservCustomAd <NSObject>
@required

-(void)loadCustomAd:(NSDictionary*)inParams
       withDelegate:(id<VservCustomAdListener>)inDelegate
     viewController:(UIViewController*)parentViewController
         withExtras:(NSDictionary*)inExtraInfo;

-(void)showAd;

-(void)invalidateAd;

@end
