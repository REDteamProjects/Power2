using AOT;
using System;

namespace VservAdiOS.NativeCallbacks
{
	#region Callback Delegate Declaration
	/// <summary>
	/// Delegate for Vserv ad callback accepting Vserv Ad Reference.
	/// </summary>
	public delegate void VservAdCallback(int VservAdRef);

	/// <summary>
	/// Delegate for Vserv ad callback accepting Vserv Ad Reference and a string.
	/// </summary>
	public delegate void VservAdCallbackWithString(int VservAdRef,string message);
	#endregion

	/// <summary>
	/// Vserv ad callbacks.
	/// </summary>
	public class VservAdCallbacks 
	{
		/// <summary>
		/// Initializes the callbacks. Do this only once for entire game to receive callbacks.
		/// </summary>
		public static void InitializeCallbacks()
		{
		  	Native.VservAdExterns._initializeCallbacks 
			(
				AdViewDidCacheAd, 
	            AdViewDidLoadAd, 
	            AdViewDidFailedToLoadAd, 
	            DidInteractWithAd, 
	            WillPresentOverlay, 
	            WillDismissOverlay, 
	            WillLeaveApp
			);
		}

		#region Native Callbacks
		/// <summary>
		/// Call Back for Ad view cached
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void AdViewDidCacheAd(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowDidCacheAd ();
		}

		/// <summary>
		/// Call Back for Ad the view did load ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void AdViewDidLoadAd(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowDidLoadAd ();
		}

		/// <summary>
		/// Call Back for Ad the view did failed to load ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		/// <param name="message">Message.</param>
		[MonoPInvokeCallback(typeof(VservAdCallbackWithString))]
		public static void AdViewDidFailedToLoadAd(int VservAdRef,string message) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowDidAdViewFailedToLoadAd (message);
		}

		/// <summary>
		/// Call Back for Did the interact with ad.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void  DidInteractWithAd(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowDidInteractWithAd ();
		}

		/// <summary>
		/// Call Back for Will the present overlay.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void WillPresentOverlay(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowWillPresentOverlay ();
		}

		/// <summary>
		/// Call Back for Will the dismiss overlay.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void WillDismissOverlay(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowWillDismissOverlay ();
		}

		/// <summary>
		/// Call Back for Will the leave app.
		/// </summary>
		/// <param name="VservAdRef">Vserv ad reference.</param>
		[MonoPInvokeCallback(typeof(VservAdCallback))]
		public static void WillLeaveApp(int VservAdRef) 
		{
			VservUnityAd.GetVservInstance (VservAdRef).ThrowWillLeaveApp ();
		}
		#endregion
	}
}