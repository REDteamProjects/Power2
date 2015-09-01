using System;
using System.Collections.Generic;
using VservAdiOS.Native;
using VservAdiOS.NativeCallbacks;
using UnityEngine;

namespace VservAdiOS
{	/// <summary>
	/// Vserv unity Ad : Use this class to create an Instance of Vserv Ad from unity.
	/// </summary>
	public class VservUnityAd 
	{
		#region Private Variables
		/// <summary>
		/// The native IOS vserv ad reference.
		/// </summary>
		private int m_nativeVservAdRef;

		/// <summary>
		/// Constant tag for destroyed VservAd 
		/// </summary>
		private const int DESTROYED = -1;

		/// <summary>
		/// Dictionary maintaining the vserv ad instances.
		/// </summary>
		private static Dictionary<int,VservUnityAd> 	VservAdInstances = new Dictionary<int, VservUnityAd>();
		#endregion

		#region Private Properties
		/// <summary>
		/// The Zone Identifier.
		/// </summary>
		private string m_zoneId;

		/// <summary>
		/// Get the zone identifier.
		/// </summary>
		public string ZoneId 
		{
			get 
			{
				return m_zoneId;
			}
		}

		/// <summary>
		/// UX Type of the Ad.
		/// </summary>
		private VservAdUX m_adUXType;

		/// <summary>
		/// Get UX Type of the ad.
		/// </summary>
		public VservAdUX AdUXType 
		{
			get 
			{
				return m_adUXType;
			}
		}

		/// <summary>
		/// Ad position.
		/// </summary>
		private VservAdPosition m_adPosition;

		/// <summary>
		/// Get UX Type of the ad.
		/// </summary>
		public VservAdPosition AdPosition 
		{
			get 
			{
				return m_adPosition;
			}
		}
		#endregion

		#region Event Handler Declarations
		/// <summary>
		/// Occurs when Ad is cached.
		/// </summary>
		public event EventHandler<EventArgs> 			AdViewDidCacheAd 		= delegate {};
		/// <summary>
		/// Occurs when adview loads ad.
		/// </summary>
		public event EventHandler<EventArgs> 			AdViewDidLoadAd 		= delegate {};
		/// <summary>
		/// Occurs when adview failed to load ad.
		/// </summary>
		public event EventHandler<AdFailedLoadingArgs> 	AdViewDidFailedToLoadAd	= delegate {};
		/// <summary>
		/// Occurs when user interacts with ad.
		/// </summary>
		public event EventHandler<EventArgs> 			DidInteractWithAd 		= delegate {};
		/// <summary>
		/// Occurs when overlay is presented.
		/// </summary>
		public event EventHandler<EventArgs> 			WillPresentOverlay 		= delegate {};
		/// <summary>
		/// Occurs when overlay is dismissed.
		/// </summary>
		public event EventHandler<EventArgs> 			WillDismissOverlay 		= delegate {};
		/// <summary>
		/// Occurs when ad leaves the app.
		/// </summary>
		public event EventHandler<EventArgs> 			WillLeaveApp 			= delegate {};
		#endregion

		#region Class Constructors&Distructors
		/// <summary>
		/// Don't Allow user to create instance using default constructor
		/// </summary>
		private VservUnityAd(){ }

		/// <summary>
		/// Initializes a new instance of the <see cref="VservUnityAd"/> class. 
		/// Use this Class to create a new Instance of Vserv Ad.
		/// </summary>
		/// <param name="zoneId">Zone identifier.</param>
		/// <param name="inAdUXType">In ad UX type.</param>
		private VservUnityAd(string inZoneId,VservAdUX inAdUXType, VservAdPosition inAdPosition)
		{
			m_zoneId = inZoneId;
			m_adUXType = inAdUXType;
			m_adPosition = inAdPosition;

			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Creating new instance of "+inAdUXType+ " Ad..."+" with zone Id "+inZoneId);
				return;
			}

			//Create new Vserv Ad instance and store the native ad reference
 			m_nativeVservAdRef = VservAdExterns._createNewVservAd(inZoneId,inAdUXType,inAdPosition);
			//Add this instance to Dictionary : VservAdInstances
			VservAdInstances.Add (m_nativeVservAdRef, this);
			//Register for callbacks, But, do this only once for the first ad
			if(VservAdInstances.Count == 1)
			{
				VservAdCallbacks.InitializeCallbacks ();
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="VservAd.VservUnityAd"/>
		/// is reclaimed by garbage collection.
		/// </summary>
		~VservUnityAd()
		{

		}
		#endregion

		#region Public Functions
		/// <summary>
		/// Creates the new vserv ad.
		/// </summary>
		/// <returns>The new vserv ad.</returns>
		/// <param name="ZoneId">Zone identifier.</param>
		/// <param name="AdUXType">Ad UX type.</param>
		/// <param name="AdPosition">Ad position.</param>
		public static VservUnityAd CreateNewVservAd(string ZoneId,VservAdUX AdUXType, VservAdPosition AdPosition = VservAdPosition.Top)
		{	
			VservUnityAd newAd=null;
			newAd = new VservUnityAd(ZoneId,AdUXType,AdPosition);
			return newAd;
		}

		/// <summary>
		/// Gets the vserv instance from the dictionary
		/// </summary>
		/// <returns>The VservAd instance</returns>
		/// <param name="key">int of the Ad received from native code</param>
		public static VservUnityAd GetVservInstance(int key)
		{
			VservUnityAd adInstance;
			VservAdInstances.TryGetValue(key,out adInstance);
			return adInstance;
		}

		/// <summary>
		/// Call this method to enable or disable refresh property. 
		/// Before setting the refresh use setRefreshRate: to override the default refresh rate of 30 seconds. 
		/// By default for banner ads refresh is ON and for interstetial ads refresh is always off.
		/// </summary>
		/// <param name="shouldEnableRefresh">If YES Refresh is enabled. If NO Refresh is disabled.</param>
		public void SetRefresh(bool inShouldEnableRefresh)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting Refresh for the Ad...");
				return;
			}
			VservAdExterns._setRefresh(m_nativeVservAdRef,inShouldEnableRefresh);
		}

		/// <summary>
		/// Call this method to set the refresh rate. 
		/// This overrides the default refresh rate of 30 seconds.
		/// </summary>
		/// <param name="inRefreshIntervalInSeconds">Refresh rate in seconds. Value should be greater than 30 seconds.</param>
		public void SetRefreshRate(int inRefreshIntervalInSeconds)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting Refresh Rate for the Ad...");
				return;
			}
			VservAdExterns._setRefreshRate(m_nativeVservAdRef,inRefreshIntervalInSeconds);
		}

		/// <summary>
		/// Pause the auto refreshing of Banner Ads.
		/// </summary>
		public void PauseRefresh()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Pausing Refresh...");
				return;
			}
			VservAdExterns._pauseRefresh(m_nativeVservAdRef);
		}

		/// <summary>
		/// Stop the Auto refreshing of Ads. Resume refresh will have no effect after stopping the refresh.
		/// </summary>
		public void StopRefresh()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Stopping Refresh...");
				return;
			}
			VservAdExterns._stopRefresh(m_nativeVservAdRef);
		}

		/// <summary>
		/// Resume the Auto refreshing of Ads. Call this method if pauseRefresh was used to pause the refresh.
		/// Resume refresh will have no effect after stopping the refresh.
		/// </summary>
		public void ResumeRefresh()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Resuming Refresh...");
				return;
			}
			VservAdExterns._resumeRefresh(m_nativeVservAdRef);
		}

		/// <summary>
		/// Set the Ad Request timeout. Default timeout is 20 seconds.
		/// </summary>
		/// <param name="inTimeout">Timeout period in seconds.</param>
		public void SetTimeOut(int inTimeout)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting Timeout fot the Ad...");
				return;
			}
			VservAdExterns._setTimeOut(m_nativeVservAdRef,inTimeout);
		}

		/// <summary>
		/// Set the Configration properties and display properties for Ads.
		/// </summary>
		/// <param name="inAdUxType">Ad Type. Must be same as passed in init method.</param>
		public void SetUXTypeWithAdProperties(VservAdUX inAdUxType,VservAdProperties inAdProperties)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting UX Type along with specified Ad Properties...");
				return;
			}
			string configString =  	"BackgroundColor:"     + VservAdProperties.ColorToHex(inAdProperties.BackgroundColor) + "," + 
									"CloseButtonPosition:" + (int)inAdProperties.CloseButtonPosition  					  + "," + 
									"FrameColor:"		   + VservAdProperties.ColorToHex(inAdProperties.FrameColor) 	  + "," + 
									"Transparence:"		   + inAdProperties.Transparence;
			VservAdExterns._setUXTypeWithConfig(m_nativeVservAdRef,inAdUxType,configString);
		}

		/// <summary>
		/// Manually cache the ad in background. Ad will not be displayed. 
		/// On success adViewDidCacheAd: method will be called by SDK.
		/// On Failure adView: adViewDidFailedToLoadAd: will be called by SDK. 
		/// When this method is called Auto Refresh is automatically disabled.
		/// </summary>
		public void CacheAd()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Caching Ad...");
				return;
			}
			VservAdExterns._cacheAd(m_nativeVservAdRef);
		}

		/// <summary>
		/// Manually cache the ad in background and requests the Ad Server with orientation. 
		/// Ad will not be displayed. On success adViewDidCacheAd: method will be called by SDK.
		/// On Failure adView: adViewDidFailedToLoadAd: will be called by SDK. 
		/// When this method is called Auto Refresh is automatically disabled.
		/// </summary>
		/// <param name="inOrientation">Orientation of the required ad.</param>
		public void CacheAdWithOrientation(VservAdOrientation inOrientation)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Caching Ad with orientation...");
				return;
			}
			VservAdExterns._cacheAdWithOriention(m_nativeVservAdRef,inOrientation);
		}

		/// <summary>
		/// Loads the Ad and displays it in the ad spot. This will start the Auto refresh property. 
		/// If user has set the refresh rate then it will use that refresh rate. 
		/// Else default refresh rate of 30 sec will be used.
		/// </summary>
		public void LoadAd()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Loading Ad...");
				return;
			}
			VservAdExterns._loadAd(m_nativeVservAdRef);
		}

		/// <summary>
		/// Loads the Ad and displays it in the ad spot and request the Ad Server the ad with orientation. 
		/// This will start the Auto refresh property. If user has set the refresh rate then it will use that refresh rate. 
		/// Else default refresh rate of 30 sec will be used.
		/// </summary>
		/// <param name="inOrientaion">Orientation of the required ad.</param>
		public void LoadAdWithOrientation(VservAdOrientation inOrientaion)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Loading Ad with "+inOrientaion +" Orientation...");
				return;
			};
			VservAdExterns._loadAdWithOriention(m_nativeVservAdRef,inOrientaion);
		}

		/// <summary>
		/// Use this method in conjunction with cacheAd. If the ad is cached in background in responce to cacheAd or cacheAdWithorientation, 
		/// this method will display the cache ad on screen. For Billboard ads, this method puts full screen overlay on screen.
		/// </summary>
		public void ShowAd()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Showing Ad...");
				return;
			}
			VservAdExterns._showAd(m_nativeVservAdRef);
		}

		/// <summary>
		/// Cancel the request sent to ad server. 
		/// Call this method if you have called cacheAd and you do not want to display that ad before adViewDidCacheAd is fired.
		/// </summary>
		public void CancelAd()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Cancelling Ad...");
				return;
			}
			VservAdExterns._cancelAd(m_nativeVservAdRef);
		}

		/// <summary>
		/// Destroyes the ad.
		/// </summary>
		public void DestroyAd()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Destroying Ad...");
				return;
			}
			VservAdExterns._releaseVservAdViewInstance(m_nativeVservAdRef);
			VservAdInstances.Remove(m_nativeVservAdRef);
			m_nativeVservAdRef = -1;
		}

		/// <summary>
		/// Retrives the state of the ad.
		/// </summary>
		/// <returns>The current Ad State</returns>
		public VservAdState GetAdState()
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Getting the Ad state...");
				return VservAdState.VservAdState_Error;
			}
			VservAdState currentState;
			currentState = VservAdExterns._getAdState(m_nativeVservAdRef);
			return currentState;
		}

		/// <summary>
		/// Sets the test device.
		/// </summary>
		/// <param name="inDeviceIDFAList">Test device array.</param>
		public void SetTestDevice(string[] inDeviceIDFAList)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting Test Device...");
				return;
			}
			VservAdExterns._setTestDevice(m_nativeVservAdRef,String.Join(",",inDeviceIDFAList));
		}

		/// <summary>
		/// Set the User information in order to deliver more appropriate ads targetter for the user.
		/// </summary>
		/// <param name="user">Instance of VservAdUser object.</param>
		public void SetUser(VservAdUser user)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting User "+user);
				return;
			}

			string userString = "age:" + user.age + "," + 
								"city:" + user.city + "," + 
								"country:" + user.country + "," +
								"dob:" + user.DOB + "," + 
								"email:" + user.email + "," +
								"gender:" + (uint)user.gender;

			VservAdExterns._setUser(m_nativeVservAdRef, userString);
		}

		/// <summary>
		/// If true blocks all the adds after this method is called. 
		/// If false will unblock the ads after this method is called
		/// </summary>
		/// <param name="inShouldBlockAds">If set to <c>true</c> should block ads.</param>
		public void BlockAds(bool inShouldBlockAds)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Blocking Ads...");
				return;
			}
			VservAdExterns._blockAds(this.m_nativeVservAdRef,inShouldBlockAds);
		}

		/// <summary>
		/// Blocks the ads for number of sessions input.
		/// </summary>
		/// <param name="inNoOfsessions">No ofsessions.</param>
		public void ShowAdsAfterSessions(int inNoOfsessions)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Show Ads After "+inNoOfsessions+" sessions...");
				return;
			}
			VservAdExterns._showAdsAfterSessions(this.m_nativeVservAdRef, inNoOfsessions);
		}

		/// <summary>
		/// Blocks the ads cased on countries.
		/// </summary>
		/// <param name="inCountryList">List of countries to block the ads of type VservCountry</param>
		/// <param name="inAllowOrBlock">Option from VservCountryOption. Block will block the counties in the country list 
		/// and allow will allow only the countries in the country list.</param>
		/// <param name="inshouldRequest">Set to <c>true</c> if ad to be requested if MCC is unawailable.</param>
		public void BlockCountries(VservCountry [] inCountryList,VservCountryOption inAllowOrBlock,bool inShouldRequest)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Blocking countries "+inCountryList);
				return;
			}
			string countryListString ="";
			foreach(VservCountry country in inCountryList)
			{
				countryListString += Enum.GetName(typeof(VservCountry),country) + ",";
			}
			countryListString.Remove(countryListString.LastIndexOf(','));
			VservAdExterns._blockCountries(this.m_nativeVservAdRef ,countryListString,inAllowOrBlock,inShouldRequest);
		}

		/// <summary>
		/// Disables the App access when offline. 
		/// If set to YES then SDK will prompt an allert whenever internet connection is not awailable. 
		/// </summary>
		/// <param name="inShouldDisableOffline">If set to <c>true</c> block the App access while offline.</param>
		public void DisableOffline(bool inShouldDisableOffline)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Set Disable Offline "+inShouldDisableOffline);
				return;
			}
			VservAdExterns._disableOffline(this.m_nativeVservAdRef ,inShouldDisableOffline);
		}

		/// <summary>
		/// Users did IAP.
		/// </summary>
		/// <param name="inDidIAP">If set to <c>true</c> in did IA.</param>
		public void UserDidIAP(bool inDidIAP)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Set User Did IAP "+inDidIAP);
				return;
			}
			VservAdExterns._userDidIAP(this.m_nativeVservAdRef ,inDidIAP);
		}

		/// <summary>
		/// Users did incent.
		/// </summary>
		/// <param name="inDidIncent">If set to <c>true</c> in did incent.</param>
		public void UserDidIncent(bool inDidIncent)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting user did Incent to "+inDidIncent);
				return;
			}
			VservAdExterns._userDidIncent(this.m_nativeVservAdRef, inDidIncent);
		}

		/// <summary>
		/// Sets the language of article.
		/// </summary>
		/// <param name="inLanguageOfArticle">In language of article.</param>
		public void SetLanguageOfArticle(string inLanguageOfArticle)
		{
			if(Application.platform != RuntimePlatform.IPhonePlayer  || m_nativeVservAdRef == DESTROYED)
			{
				Debug.Log("Setting Laguange Of Article "+ inLanguageOfArticle);
				return;
			}
			VservAdExterns._setLanguageOfArticle(this.m_nativeVservAdRef, inLanguageOfArticle);
		}

		#region User Callbacks
		/// <summary>
		/// Throws the did cache ad.
		/// </summary>
		public void ThrowDidCacheAd()
		{
			AdViewDidCacheAd (this,EventArgs.Empty);
		}

		/// <summary>
		/// Throws the did load ad.
		/// </summary>
		public void ThrowDidLoadAd()
		{
			AdViewDidLoadAd (this,EventArgs.Empty);
		}

		/// <summary>
		/// Throws the did ad view failed to load ad.
		/// </summary>
		/// <param name="message">Message.</param>
		public void ThrowDidAdViewFailedToLoadAd(string message)
		{
			AdFailedLoadingArgs args = new AdFailedLoadingArgs (){
				Message = message
			};
			AdViewDidFailedToLoadAd(this,args);
		}

		/// <summary>
		/// Throws the did interact with ad.
		/// </summary>
		public void ThrowDidInteractWithAd()
		{
			DidInteractWithAd (this,EventArgs.Empty);
		}

		/// <summary>
		/// Throws the will present overlay.
		/// </summary>
		public void ThrowWillPresentOverlay()
		{
			WillPresentOverlay (this,EventArgs.Empty);
		}

		/// <summary>
		/// Throws the will dismiss overlay.
		/// </summary>
		public void ThrowWillDismissOverlay()
		{
			WillDismissOverlay (this,EventArgs.Empty);
		}

		/// <summary>
		/// Throws the will leave app.
		/// </summary>
		public void ThrowWillLeaveApp()
		{
			WillLeaveApp (this,EventArgs.Empty);
		}
		#endregion User Callbacks
		#endregion Public Functions
	}
}