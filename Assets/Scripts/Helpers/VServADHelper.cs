using UnityEngine;

#if !UNITY_EDITOR && UNITY_IOS 
using VservAdiOS;
#endif

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using VservPlugin;
#endif

public class VServADHelper : MonoBehaviour
{

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    public VservWP8Plugin pluginVservAd;
#endif

#if !UNITY_EDITOR && UNITY_IOS 
    public VservUnityAd pluginVservAd;
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
    public VmaxPlugin pluginVservAd;
#endif


    void Start()
    {

#if !UNITY_EDITOR && UNITY_ANDROID
        pluginVservAd = new VmaxPlugin ("20846",0); // the 2 nd
        //parameter is 0 passed for
        //creating object of VmaxPlugin for Banner
        pluginVservAd.setAppOrientation (VmaxPlugin.ORIENTATION_PORTRAIT);//It is mandatory
#endif

#if !UNITY_EDITOR && UNITY_IOS 
        pluginVservAd = VservUnityAd.CreateNewVservAd("20846", VservAdUX.VservAdUX_Banner, VservAdPosition.Bottom);
#endif

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
        /*This is a test ZoneID, make sure you replace it before going live*/
        pluginVservAd = new VservWP8Plugin();
        
        pluginVservAd.SetAdUXType(VservWP8Plugin.AdUxType.Banner);
        pluginVservAd.DidCacheAd += (sender, args) => Debug.Log("DidCacheAd: " + args.ToString());
        pluginVservAd.FailedToCacheAd += (sender, args) => Debug.Log("FailedToCacheAd: " + args.ToString());
        pluginVservAd.FailedToLoadAd += (sender, args) => Debug.Log("FailedToLoadAd: " + args.ToString());
        pluginVservAd.DidLoadAd += (sender, args) => Debug.Log("DidLoadAd: " + args.ToString());
        pluginVservAd.WillDismissOverlay += (sender, args) => Debug.Log("WillDismissOverlay: " + args.ToString());
        pluginVservAd.WillPresentOverlay += (sender, args) => Debug.Log("WillPresentOverlay: " + args.ToString());
        pluginVservAd.WillLeaveApp += (sender, args) => Debug.Log("WillLeaveApp: " + args.ToString());
        pluginVservAd.DidInteractWithAd += (sender, args) => Debug.Log("DidInteractWithAd: " + args.ToString());

        pluginVservAd.Age = 25;
        pluginVservAd.City = "Moscow";
        pluginVservAd.Country = "Russia";
        pluginVservAd.Dob = System.DateTime.Now;
        pluginVservAd.Email = @"abc@xyz.com";
        pluginVservAd.Gender = VservWP8Plugin.eGender.Male;

#endif

    }

    void Update(){
    }

    void OnGUI()
    {

#if !UNITY_EDITOR && UNITY_IOS 
        pluginVservAd.LoadAd();
#endif
        
#if !UNITY_EDITOR && UNITY_ANDROID
        pluginVservAd.setPosition(8); // for Bottom Center
        pluginVservAd.loadAd();
        //OR
        //pluginVservAd.loadAdWithOrientation(1);
        // adOrientation : 0 For
        //Landscape, 1 Portrait
#endif

#if (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
        /*SetAdPosition() API takes one parameter, an integer representing the position.*/

        pluginVservAd.SetAdPosition(8);
        pluginVservAd.SetRefreshRate(45);

        //pluginVservAd.SetTestDevices();

        pluginVservAd.LoadAd();
#endif
    }

    void Awake(){
    }
}
