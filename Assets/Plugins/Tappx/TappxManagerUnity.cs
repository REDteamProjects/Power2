using UnityEngine;
using System.Runtime.InteropServices;

public class TappxManagerUnity : MonoBehaviour
{
    public enum Position
    {
        TOP = 1,
        BOTTOM = 2
    }

	private static TappxManagerUnity mInstance = null;

    [SerializeField]
    private string iOSTappxID;
    [SerializeField]
	private string AndroidTappxID;
    [SerializeField]
	public Position positionBanner = Position.TOP;
	[SerializeField]
	public bool Interstitial = false;
	[SerializeField]
	public bool Banner = false;

#if UNITY_IPHONE
	//Banner
    [DllImport("__Internal")]
    private static extern void trackInstallIOS_(string tappxID);
    [DllImport("__Internal")]
	private static extern void createBannerIOS_(Position positionBanner);
	[DllImport("__Internal")]
    private static extern void hideAdIOS_();
    [DllImport("__Internal")]
	private static extern void showAdIOS_(Position positionBanner);
	[DllImport("__Internal")]
	private static extern void releaseTappxIOS_();

	//Interstitial
    [DllImport("__Internal")]
	private static extern void loadInterstitialIOS_();
	[DllImport("__Internal")]
	private static extern void showInterstitialIOS_();
	[DllImport("__Internal")]
	private static extern void releaseInterstitialTappxIOS_();
    
#elif UNITY_ANDROID
    private AndroidJavaObject bannerControl = null;
	private AndroidJavaObject interstitialControl = null;
#endif

	public static TappxManagerUnity instance
    {
        get
        {
            return mInstance;
        }
    }

    public void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        if (Application.isEditor) return;

#if UNITY_IPHONE
        if (mInstance == this)
        {
			if(Banner ^ Interstitial){
				if(Banner){
					releaseTappxIOS_();
				}else{
					releaseInterstitialTappxIOS_();
				}
			}
        }
#endif
    }

    public void Start()
    {
        if (Application.isEditor) return;

		#if UNITY_IPHONE
			trackInstallIOS_(iOSTappxID);
			if(Banner ^ Interstitial){
				if(Banner){
					createBannerIOS_(positionBanner);	
				}else{
					loadInterstitialIOS_();
				}
			}
		#elif UNITY_ANDROID
			if(Banner ^ Interstitial){
				if(Banner){
					bool posit = true;
					if((int)positionBanner == 2)
						posit = false;
					bannerControl = new AndroidJavaObject ("com.tappx.unity.bannerTappx",AndroidTappxID,posit,"TappxManagerUnity");
				}else{
					interstitialControl = new AndroidJavaObject ("com.tappx.unity.interstitialTappx",AndroidTappxID,"TappxManagerUnity");
				}
			}
		#endif

    }

    public void OnApplicationPause(bool pause)
    {
        if (Application.isEditor) return;
        if (pause)
        {
			if(Banner ^ Interstitial){
				if(Banner){
					show();	
				}else{
					interstitialShow();
				}
			}
        }
    }

    public void hide()
    {
		#if UNITY_IPHONE
			hideAdIOS_();
		#elif UNITY_ANDROID
			bannerControl.Call("hideBannerGONE");
			bannerControl = null;
		#endif
    }

    public void show()
    {
		#if UNITY_IPHONE
			showAdIOS_(positionBanner);
		#elif UNITY_ANDROID
			bool posit = true;
			if((int)positionBanner == 2)
				posit = false;
			bannerControl = new AndroidJavaObject ("com.tappx.unity.bannerTappx",AndroidTappxID,posit,"TappxManagerUnity");
		#endif
    }

    public bool isBannerVisible()
    {
		#if UNITY_ANDROID
		if(bannerControl!=null){
        	return bannerControl.Call<bool>("isBannerVisible");
		}
		#endif
        return false;
    }

	public void loadInterstitial(){
		#if UNITY_IPHONE
			loadInterstitialIOS_();
		#elif UNITY_ANDROID
		if(interstitialControl!=null){
			interstitialControl = null;
		}
		interstitialControl = new AndroidJavaObject ("com.tappx.unity.interstitialTappx",AndroidTappxID,"TappxManagerUnity");
		#endif
	}

	public void interstitialShow(){
		#if UNITY_IPHONE
			showInterstitialIOS_();
		#elif UNITY_ANDROID
		if(interstitialControl!=null){
			interstitialControl.Call("showInterstitial");
		}
		#endif
	}

	#if UNITY_IPHONE
	public void tappxBannerDidReceiveAd(){
		UnityEngine.Debug.Log("Banner Received");
	}
	
	public void tappxBannerFailedToLoad(string error){
		UnityEngine.Debug.Log("Banner Error " + error);
	}
	
	public void tappxInterstitialDidReceiveAd(){
		UnityEngine.Debug.Log("Interstitial Load");
	}
	
	public void tappxInterstitialFailedToLoad(string error){
		UnityEngine.Debug.Log("Interstitial Error " + error);
	}
	#elif UNITY_ANDROID
	public void OnAdLoaded(){
		UnityEngine.Debug.Log("Banner Received");
	}

	public void OnAdFailedToLoad(string error){
		UnityEngine.Debug.Log("Banner Error " + error);
	}

	public void InterstitialLoaded(){
		UnityEngine.Debug.Log("Interstitial Load");
	}
	
	public void InterstitialFailedToLoad(string error){
		UnityEngine.Debug.Log("Interstitial Error " + error);
	}
	#endif
}
