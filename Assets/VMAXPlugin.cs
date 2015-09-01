#if UNITY_ANDROID 
using UnityEngine;
using System.Collections;
using System;

public class VMAXPlugin : MonoBehaviour{ 
	
	AndroidJavaObject aObj;
	AndroidJavaClass aClass = null;
	public static readonly int ORIENTATION_PORTRAIT = 2;
	public static readonly int ORIENTATION_LANDSCAPE = 1;
	public static readonly int ORIENTATION_AUTO = 3;
	public static int appOrientation;
	public static readonly int UX_INTERSTITIAL = 1;
	public static readonly int UX_BANNER = 0;
	public static readonly int UX_FRAME = 2;
	private static string zone;
	private static int uxType;
		// Use this for initialization
	int screenWidht,screenHeight;
	//internal bool isAdCached = false;
	private static bool isIncent = false;
	/* 
	 *Check for current orientation 
	 * if 0 Then landscape, if 1 then portrait
	 */ 


	public VMAXPlugin(string zoneId, int mUxType, bool isIncentAd = false){
		if (isIncentAd) {

						//aObj = new AndroidJavaObject ("com.vserv.vmax.UnityBridgeIncent");
						var bridgeClass = new AndroidJavaClass ("com.vserv.vmax.UnityBridgeIncent");
						aObj = bridgeClass.CallStatic<AndroidJavaObject> ("getInstance");
						setIncentAd(true);
						isIncent = isIncentAd;
						setUxType (mUxType);
						setZoneId (zoneId);
						setIncentAd(isIncentAd);
						
				} else {
						zone = zoneId;
						uxType = mUxType;
						aObj = new AndroidJavaObject("com.vserv.vmax.UnityBridge",zone,uxType);
				}		
	}

	void Update () {

		if (screenWidht != Screen.width || screenHeight != Screen.height) 
		{
			//Debug.Log("if Update start");
			if(aObj == null)
			{
				if(isIncent)
				{
					//Debug.Log("isIncent update start");
					var bridgeClass = new AndroidJavaClass ("com.vserv.vmax.UnityBridgeIncent");
					aObj = bridgeClass.CallStatic<AndroidJavaObject> ("getInstance");
					//Debug.Log("isIncent update end");
					aObj.Call("onConfigurationChanged");
					screenHeight = Screen.height;
					screenWidht = Screen.width;
				}
//				else
//				{
//					Debug.Log("isNotIncent update start");
//					aObj = new AndroidJavaObject("com.vserv.vmax.UnityBridge",zone,uxType);
//					Debug.Log("isNotIncent update end");
//				}
			}
			else{
				//Debug.Log("aObj is not null");
			}


			//Debug.Log("if Update end");
		}	
		//Debug.Log("Update end");
	}

	void OnApplicationPause(bool pauseStatus) {

		}
	/*
	public static VMAXPlugin createVmaxPlugin(string zoneId, int mUxType,bool isIncentAd = false)
	{
		aObj = new AndroidJavaObject("com.vserv.vmax.UnityBridge",zoneId,mUxType);
		return aOb
	}
	public static void createVmaxIncentPlugin(string zoneId, int mUxType)
	{
		aObj = new AndroidJavaObject ("com.vserv.vmax.UnityBridgeIncent");
		var bridgeClass = new AndroidJavaClass ("com.vserv.vmax.UnityBridgeIncent");
		aObj = bridgeClass.CallStatic<AndroidJavaObject> ("getInstance");
		
		setUxType (mUxType);
		setZoneId (zoneId);
	}
	*/
	enum CountryAttributes
	{
		INCLUDE, EXCLUDE
	}

	public void loadAd()
	{
		aObj.Call ("getIdThread");
		aObj.Call("loadAd");
	}

	public void setAppOrientation(int app_orientation)
	{
		appOrientation = app_orientation;
	}
	public void loadAdWithOrientation(int orientation)
	{
		aObj.Call ("getIdThread");
		aObj.Call("loadAdWithOrientation",orientation);
	}
	public void setAdListener()
	{
		aObj.Call("setAdListener");
	}
    public void initializeVservCustomIncentListener()
    {
		try{
			aObj.Call("initializeVservCustomIncentListener");
		}
		catch(Exception e)
		{

		}
        
    }
	public void setViewMandatoryListener()
	{
		aObj.Call("setViewMandatoryListener");
	}

	public void cacheAd()
	{
		aObj.Call ("cacheAd");
	}
	public void cacheAdWithOrientation(int orientation)
	{
		aObj.Call ("cacheAdWithOrientation", orientation);
	}
	public void showAd()
	{
		aObj.Call ("showAd");
//		if (isAdCached) {
//						aObj.Call ("showAd");
//						isAdCached = false;
//				} else {
//			Debug.Log("isAdCached is false");
//				}

	}

	public void cancelAd()
	{
		aObj.Call ("cancelAd");
	}
	public void setRefresh(bool refresh)
	{
		aObj.Call ("setRefresh",refresh);
	}

	public void setTimeOut(int timeOutSeconds)
	{
		aObj.Call ("setTimeOut",timeOutSeconds);
	}
	public void setPosition(int position)
	{
		aObj.Call ("setPosition",position);
	}
	public void setRefreshRate(int refreshRate)
	{
		aObj.Call ("setRefreshRate",refreshRate);
	}
	public void setAge(string age)
	{
		aObj.Call ("setAge",age);
	}
	public void setCity(string city)
	{
		aObj.Call ("setCity",city);
	}
	public void setCountry(string country)
	{
		aObj.Call ("setCountry",country);
	}
	public void setDOB(DateTime dob)
	{
		aObj.Call ("setDOB",dob);
	}

	public void setUxType(int uxType)
	{
		aObj.Call ("setUxType",uxType);
	}

	public void setZoneId(string zoneId)
	{
		aObj.Call ("setZoneId",zoneId);
	}

	public void setEmail(string email)
	{
		aObj.Call ("setEmail",email);
	}
	public void setGender(string gender)
	{
		aObj.Call ("setGender",gender);
	}
	public void pauseRefresh()
	{
		aObj.Call ("pauseRefresh");
	}
	public void resumeRefresh()
	{
		aObj.Call ("resumeRefresh");
	}
	public void onPause()
	{
		aObj.Call ("onPause");
	}
	public void onResume()
	{
		aObj.Call ("onResume");
	}
	public void stopRefresh()
	{
		aObj.Call ("stopRefresh");
	}
		/*
		 * Pass Comma seperated AVIDID to setTestDevices() to set the devices for test ads
		 */
	public void setTestDevices(string testDeviceAvdIds)
	{
		aObj.Call ("setTestDevices",testDeviceAvdIds);
	}		
	
	public void setBlockAd( bool isBlocking)
	{
		aObj.Call("setBlockAd",isBlocking);		
	}
public void setShowAdsSessionCount( int showAdsAfterSession)
{
	aObj.Call("setShowAdsSessionCount",showAdsAfterSession);		
}
public void setDisplayOffline( bool pDispOffline)
{
	aObj.Call("setDisplayOffline",pDispOffline);		
}
public void setLanguageOfArticle( string loa)
{
	aObj.Call("setLanguageOfArticle",loa);		
}
public void setSection( string section)
{
	aObj.Call("setSection",section);		
}
public void setUserDidIAP( bool userDidIAP)
{
	aObj.Call("setUserDidIAP",userDidIAP);		
}
public void setUserDidIncent( bool userDidIncent)
{
	aObj.Call("setUserDidIncent",userDidIncent);		
}
public void setLoginId( string loginId)
{
	aObj.Call("setLoginId",loginId);		
}

private void setIncentAd( bool isIncent)
{
	aObj.Call("setIncentAd",isIncent);		
}

	//Set the name of the callback handler so the right component gets ad callbacks.
public void setCallbackHandlerName(string callbackHandlerName)
{
	aObj.Call("setCallbackHandlerName", callbackHandlerName);
}
public void getTotalVirtualCurrency()
{
		try{
			aObj.Call("getTotalVirtualCurrency");
		}
		catch(Exception e)
		{
			
		}
    
}
public void setSpendVirtualCurrency(string currency)
{
		try{
			aObj.Call("setSpendVirtualCurrency", currency);
		}
		catch(Exception e)
		{
			
		}
    
}
	public void awardVirtualCurrency(string currency)
{
		try{
			aObj.Call("awardVirtualCurrency", currency);
		}
		catch(Exception e)
		{
			
		}
	
}
	public void adViewDidCacheAd(string unusedMessae)
	{
//		Debug.Log ("isAdCached = true");
//		isAdCached = true;
	}
	public void didFailedToCacheAd(string unusedMessae) 
	{
//		isAdCached = false;
	}
	public void setOrientation(string orientation) 
	{
		if (string.Equals (orientation, "portrait")) {
						Debug.Log("orientation portrait");
						Screen.orientation = ScreenOrientation.Portrait;
				} else if (string.Equals (orientation, "landscape")) {
						Debug.Log("orientation landscape");
						Screen.orientation = ScreenOrientation.LandscapeLeft;
				} else if(string.Equals (orientation, "unspecified")){
			Debug.Log("orientation unspecified");
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}

	public void willDismissOverlay(string unusedMessae)
	{
		if (appOrientation == ORIENTATION_PORTRAIT) {
			Screen.orientation = ScreenOrientation.Portrait;
				}
		else if(appOrientation == ORIENTATION_LANDSCAPE){
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
		else if(appOrientation == ORIENTATION_AUTO){
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}
	public void didFailedToLoadAd(string unusedMessae) 
	{
		if (appOrientation == ORIENTATION_PORTRAIT) {
			Screen.orientation = ScreenOrientation.Portrait;
		}
		else if(appOrientation == ORIENTATION_LANDSCAPE){
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
		else if(appOrientation == ORIENTATION_AUTO){
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}
	
}
#endif