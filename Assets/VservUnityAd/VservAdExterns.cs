using System;
using System.Runtime.InteropServices;
using VservAdiOS.NativeCallbacks;

namespace VservAdiOS.Native
{
	/// <summary>
	/// Vserv ad native iOS externs.
	/// </summary>
	public class VservAdExterns
	{
		/// <summary>
		/// Initializes the callbacks.
		/// </summary>
		/// <param name="_adViewDidCacheAd">Callback for Ad view did cache ad.</param>
		/// <param name="_adViewDidLoadAd">Callback for Ad view did load ad.</param>
		/// <param name="_adViewDidFailedToLoadAd">Callback for Ad view did failed to load ad.</param>
		/// <param name="_didInteractWithAd">Callback for Did interact with ad.</param>
		/// <param name="_willPresentOverlay">Callback for Will present overlay.</param>
		/// <param name="_willDismissOverlay">Callback for Will dismiss overlay.</param>
		/// <param name="_willLeaveApp">Callback for Will leave app.</param>
		[DllImport("__Internal")]
		internal static extern void _initializeCallbacks
		(	VservAdCallback 			_adViewDidCacheAd,
			VservAdCallback 			_adViewDidLoadAd,
			VservAdCallbackWithString 	_adViewDidFailedToLoadAd,
			VservAdCallback 			_didInteractWithAd,
			VservAdCallback 			_willPresentOverlay,
			VservAdCallback 			_willDismissOverlay,
			VservAdCallback 			_willLeaveApp
		);

		/// <summary>
		/// Creates the new vserv ad.
		/// </summary>
		/// <returns>The new vserv ad.</returns>
		/// <param name="zoneId">Zone identifier.</param>
		/// <param name="adUXType">Ad UX type.</param>
		/// <param name="adPosition">Ad position.</param>
		[DllImport("__Internal")]
		internal static extern int _createNewVservAd(string zoneId, VservAdUX adUXType, VservAdPosition adPosition);

		/// <summary>
		/// Sets the refresh for the adView.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="shouldEnableRefresh">If set to <c>true</c> should enable refresh.</param>
		[DllImport("__Internal")]
		internal static extern void _setRefresh(int VservAdRef, bool shouldEnableRefresh);

		/// <summary>
		/// Sets the refresh rate.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="refreshIntervalInSeconds">Refresh interval in seconds.</param>
		[DllImport("__Internal")]
		internal static extern void _setRefreshRate(int VservAdRef, int refreshIntervalInSeconds);

		/// <summary>
		/// Pauses the refresh.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _pauseRefresh(int VservAdRef);

		/// <summary>
		/// Stops the refresh.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _stopRefresh(int VservAdRef);

		/// <summary>
		/// Resumes the refresh.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _resumeRefresh(int VservAdRef);

		/// <summary>
		/// Sets the time out.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="timeout">Timeout.</param>
		[DllImport("__Internal")]
		internal static extern void _setTimeOut(int VservAdRef, int timeout);

		/// <summary>
		/// Sets the type of the UX.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _setUXTypeWithConfig(int VservAdRef, VservAdUX adUXType, string config);

		/// <summary>
		/// Caches the ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _cacheAd(int VservAdRef);

		/// <summary>
		/// Caches the ad with oriention.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="orientation">Orientation.</param>
		[DllImport("__Internal")]
		internal static extern void _cacheAdWithOriention(int VservAdRef, VservAdOrientation orientation);

		/// <summary>
		/// Loads the ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _loadAd(int VservAdRef);

		/// <summary>
		/// Loads the ad with oriention.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="orientation">Orientation.</param>
		[DllImport("__Internal")]
		internal static extern void _loadAdWithOriention(int VservAdRef, VservAdOrientation orientation);

		/// <summary>
		/// Loads the ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _showAd(int VservAdRef);

		/// <summary>
		/// Cancels the ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _cancelAd(int VservAdRef);

		/// <summary>
		/// Releases the vserv ad view instance.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern void _releaseVservAdViewInstance(int VservAdRef);

		/// <summary>
		/// Gets the state of the ad.
		/// </summary>
		/// <returns>The ad state.</returns>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[DllImport("__Internal")]
		internal static extern VservAdState _getAdState(int VservAdRef);

		/// <summary>
		/// Sets the test device.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="deviceIDFAList">Device IDFA list.</param>
		[DllImport("__Internal")]
		internal static extern void _setTestDevice(int VservAdRef, string deviceIDFAList);

		/// <summary>
		/// Sets the user.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="user">User String</param>
		[DllImport("__Internal")]
		internal static extern void _setUser(int VservAdRef, string user);

		/// <summary>
		/// If true blocks all the adds after this method is called. 
		/// If false will unblock the ads after this method is called
		/// </summary>
		/// <param name="inShouldBlockAds">If set to <c>true</c> should block ads.</param>
		[DllImport("__Internal")]
		internal static extern void _blockAds(int VservAdRef, bool shouldBlockAds);
		
		/// <summary>
		/// Blocks the ads for number of sessions input.
		/// </summary>
		/// <param name="inNoOfsessions">No ofsessions.</param>
		[DllImport("__Internal")]
		internal static extern void _showAdsAfterSessions(int VservAdRef, int noOfsessions);

		
		/// <summary>
		/// Blocks the ads cased on countries.
		/// </summary>
		/// <param name="inCountryList">List of countries to block the ads of type VservCountry</param>
		/// <param name="inAllowOrBlock">Option from VservCountryOption. Block will block the counties in the country list 
		/// and allow will allow only the countries in the country list.</param>
		/// <param name="inshouldRequest">Set to <c>true</c> if ad to be requested if MCC is unawailable.</param>
		[DllImport("__Internal")]
		internal static extern void _blockCountries(int VservAdRef, string countryListString,VservCountryOption allowOrBlock,bool shouldRequest);

		
		/// <summary>
		/// Disables the App access when offline. 
		/// If set to YES then SDK will prompt an allert whenever internet connection is not awailable. 
		/// </summary>
		/// <param name="inShouldDisableOffline">If set to <c>true</c> block the App access while offline.</param>
		[DllImport("__Internal")]
		internal static extern void _disableOffline(int VservAdRef, bool shouldDisableOffline);

		
		/// <summary>
		/// Users the did IAP.
		/// </summary>
		/// <param name="inDidIAP">If set to <c>true</c> in did IA.</param>
		[DllImport("__Internal")]
		internal static extern void _userDidIAP(int VservAdRef, bool didIAP);
		
		/// <summary>
		/// Users the did incent.
		/// </summary>
		/// <param name="inDidIncent">If set to <c>true</c> in did incent.</param>
		[DllImport("__Internal")]
		internal static extern void _userDidIncent(int VservAdRef, bool didIncent);
		
		/// <summary>
		/// Sets the language of article.
		/// </summary>
		/// <param name="inLanguageOfArticle">In language of article.</param>
		[DllImport("__Internal")]
		internal static extern void _setLanguageOfArticle(int VservAdRef, string languageOfArticle);
	}
}