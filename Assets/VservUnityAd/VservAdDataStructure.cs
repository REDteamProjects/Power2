using UnityEngine;
using System;

namespace VservAdiOS
{
	/// <summary>
	/// Internal States Of Ads
	/// </summary>
	public enum VservAdState
	{
		VservAdState_Error =-1,
		/// <summary>
		/// New request for ad has been sent. 
		/// Yet to receive the responce from adServer.
		/// </summary>
		VservAdState_RequestSent = 0,
		/// <summary>
		/// Ad is cached and ready to be displayed. 
		/// Ads will be inthis state if user calls cacheAd and caching is successful.
		/// </summary>
		VservAdState_Cached = 1,
		/// <summary>
		/// Ad caching is failed. Ads will be in this state when user called 
		/// cacheAd and request to server failed due to some reason. When caching fails 
		/// adView: adViewDidFailedToLoadAd: is involed with error.
		/// </summary>
		VservAdState_CacheFailed = 2,
		/// <summary>
		/// Ad is loaded and is being displayed.
		/// </summary>
		VservAdState_Loaded = 3,
		/// <summary>
		/// Communication to adserver failed.
		/// </summary>
		VservAdState_Failed = 4
	}
	
	/// <summary>
	/// Placement position of close button on Ads. Eventhough closebutton placement is set by the user,
	/// the ad can override this property and display close button in some other position.
	/// </summary>
	public enum VservAdCloseButtonPlacement
	{
		/// <summary>
		/// No preference for close button placement.
		/// </summary>
		VservAdCloseButtonPlacement_None,
		/// <summary>
		/// Close button placed top left corner of the VservAdView.
		/// </summary>
		VservAdCloseButtonPlacement_TopRight,
		/// <summary>
		/// Close button placed top right corner of the VservAdView.
		/// </summary>
		VservAdCloseButtonPlacement_TopLeft,
		/// <summary>
		/// Close button placed bottom right corner of the VservAdView.
		/// </summary>
		VservAdCloseButtonPlacement_BottomRight,
		/// <summary>
		/// Close button placed bottom left corner of the VservAdView.
		/// </summary>
		VservAdCloseButtonPlacement_BottomLeft
	}

	/// <summary>
	/// The UX Type of the required Ad. User needs to set this property to the SDK.
	/// </summary>
	public enum VservAdUX
	{
		/// <summary>
		/// Interstetial UX Type.
		/// </summary>
		VservAdUX_Interstitial,
		/// <summary>
		/// Banner UX Type.
		/// </summary>
		VservAdUX_Banner
	}

	/// <summary>
	/// The orientation for the requested ad.
	/// </summary>
	public enum VservAdOrientation
	{
		/// <summary>
		/// Ad capable of displaying in any orientation.
		/// </summary>
		VservAdOrientation_Adaptive,
		/// <summary>
		/// Ad capable of displaying in portrait orientation.
		/// </summary>
		VservAdOrientation_Portrait,
		/// <summary>
		/// Ad capable of displaying in landscape orientation.
		/// </summary>
		VservAdOrientation_Landscape
	}

	/// <summary>
	/// Gender property for Optional user property.
	/// </summary>
	public enum VservUserGender
	{
		/// <summary>
		/// Gender Male.
		/// </summary>
		Male   = 'M',
		/// <summary>
		/// Gender Female
		/// </summary>
		Female = 'F'
	}

	/// <summary>
	/// Vserv ad position.
	/// </summary>
	public enum VservAdPosition
	{
		/// <summary>
		/// Screen Top position
		/// </summary>
		Top,
		/// <summary>
		/// Screen Bottom position
		/// </summary>
		Bottom
	}

	/// <summary>
	/// Error codes definition. 
	/// This is sent by SDK in adView:didFailedToLoadAd: callback in [NSError code] field.
	/// </summary>
	public enum VservAdError
	{
		/// <summary>
		/// Connection to the Ad failed
		/// </summary>
		VservAdError_ConnectionFailed = -100,
		/// <summary>
		/// There is no Ad Content received in the responce from the ad server.
		/// </summary>
		VservAdError_NoFill,
		/// <summary>
		/// Could not load the Ad received from the Ad server.
		/// </summary>
		VservAdError_LoadingFailed
	}

	/// <summary>
	/// Vserv country option.
	/// </summary>
	public enum VservCountryOption
	{
		/// <summary>
		/// Exclude the countries listed in the country list.
		/// </summary>
		Exclude = 1,
		/// <summary>
		/// Include the countries listed in the country list.
		/// </summary>
		Allow
	}

	/// <summary>
	/// Optional User property. user needs to set this property for more accurate ad delivery.
	/// </summary>
	[System.Serializable]
	public class VservAdUser
	{
		/// <summary>
		/// Gender Of the User.
		/// </summary>
		public VservUserGender gender;
		/// <summary>
		/// Date Of Birth Of the User
		/// </summary>
		public string DOB;
		/// <summary>
		/// Age of the User.
		/// </summary>
		public string age;
		/// <summary>
		/// City of the User.
		/// </summary>
		public string city;
		/// <summary>
		/// Country of the User.
		/// </summary>
		public string country;
		/// <summary>
		/// Email of the User.
		/// </summary>
		public string email;
	}

	/// <summary>
	/// Optional Ad property. User needs to set this property for more customized Ad content.
	/// </summary>
	[System.Serializable]
	public class VservAdProperties
	{
		/// <summary>
		/// Background color of the AdView. Should be Color object. 
		/// This property is applicable for Banner and interstetial ads
		/// </summary>
		public Color BackgroundColor;

		/// <summary>
		/// Close button placement of ads. Should be a NSNumber with VservAdCloseButtonPlacement element.
		/// </summary>
		public VservAdCloseButtonPlacement CloseButtonPosition;

		/// <summary>
		/// Frame colour of adView. Should be UIColor object. Applicable to both Banner and Interstetial ads.
		/// </summary>
		public Color FrameColor;

		/// <summary>
		/// Transperence of adView. Should be NSNumber of float value. 
		/// Value between 0 and 1. 0 = Full Opaque. 1 = Full Transperent
		/// </summary>
		[Range (0f, 1f)]
		public float Transparence;

		/// <summary>
		/// Prompt Access for default constructor
		/// </summary>
		private VservAdProperties(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="VservAdProperties"/> class.
		/// </summary>
		/// <param name="BackGroundColor">Background color of the AdView. Should be Color object. 
		/// This property is applicable for Banner and interstetial ads</param>
		/// <param name="CloseButtonPosition">Close button placement of ads. Should be a NSNumber with VservAdCloseButtonPlacement element.</param>
		/// <param name="FrameColor">Frame colour of adView. Should be UIColor object. Applicable to both Banner and Interstetial ads.</param>
		/// <param name="Transparence">Transperence of adView. Should be NSNumber of float value. 
		/// Value between 0 and 1. 0 = Full Opaque. 1 = Full Transperent</param>
		public VservAdProperties(Color inBackGroundColor,VservAdCloseButtonPlacement inCloseButtonPosition,Color inFrameColor,float inTransparence)
		{
			BackgroundColor = inBackGroundColor;
			CloseButtonPosition = inCloseButtonPosition;
			FrameColor = inFrameColor;
			Transparence = inTransparence;

		}

		/// <summary>
		/// Converts Colors to hex values.
		/// </summary>
		/// <returns>The to hex.</returns>
		/// <param name="color">Color To be converted</param>
		public static string ColorToHex(Color32 color)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return hex;
		}
	}

	/// <summary>
	/// Ad failed loading event arguments.
	/// </summary>
	public class AdFailedLoadingArgs : EventArgs
	{
		public string Message { get; set; }
	}

}