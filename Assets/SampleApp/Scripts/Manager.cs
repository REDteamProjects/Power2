using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using VservAdiOS;
using System.Threading;

public class Manager : MonoBehaviour 
{
	public VservAdProperties adProperties;
	public VservAdUser user;
	public VservCountry [] countryList;
	public InputField inputFieldZoneId;
	public Text status;
	public Button buttonToggleRefresh;

	public Button adaptiveButton;
	public Button portraitButton;
	public Button landscapeButton;

	private VservUnityAd adRef;
	private float nextToggleTime;
	private string zoneId ;
	private bool isBanner = true;
	private bool isRefreshing = false;
	VservAdUX adUX = VservAdUX.VservAdUX_Banner;

	private int orientation = -1;

	private void SafeDestroy () {
		if (adRef != null) {
			adRef.DestroyAd();
			adRef = null;
		}
	}
	
	void Start()
	{
		inputFieldZoneId.text = "8063";
		CreateBanner();
		print ((int)user.gender);
	}

	void CreateBanner () 
	{
		SafeDestroy();
		zoneId = inputFieldZoneId.text;
		adRef =		VservUnityAd.CreateNewVservAd 
						( 
							 zoneId,						//Zone Id
							 VservAdUX.VservAdUX_Banner,	//Ad Type
							 VservAdPosition.Top			//Ad Position 
			 			);

		adUX = VservAdUX.VservAdUX_Banner;
		adRef.AdViewDidFailedToLoadAd += HandleAdViewDidFailedToLoadAd;
		adRef.AdViewDidLoadAd += HandleAdViewDidLoadAd;
		adRef.AdViewDidCacheAd += HandleAdViewDidCacheAd;
		adRef.DidInteractWithAd += HandleDidInteractWithAd;
		adRef.WillLeaveApp += HandleAdWillLeaveApp;
		adRef.WillPresentOverlay += HandleAdWillPresentOverlay;
		adRef.WillDismissOverlay += HandleAdWillDismissOverlay;
	}

	void HandleDidInteractWithAd (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: Interacted With Ad");
		
		SetStatus("Interacted With Ad");
	}

	void HandleAdWillLeaveApp (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: WillLeaveApp");
		
		SetStatus("Will Leave App");
	}

	void HandleAdWillDismissOverlay (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: WillDismissOverlay");
		
		SetStatus("Will Dismiss Overlay");
	}

	void HandleAdWillPresentOverlay (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: WillPresentOverlay");
		
		SetStatus("Will Present Overlay");
	}

	void HandleAdViewDidCacheAd (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: DidCacheAd");

		SetStatus("Did Cache Ad");
	}

	void HandleAdViewDidLoadAd (object sender, EventArgs e)
	{
		Debug.Log("AdCallback: DidLoadAd");

		SetStatus("Ad View Loaded Ad");
	}

	void HandleAdViewDidFailedToLoadAd (object sender, AdFailedLoadingArgs e)
	{
		Debug.Log("AdCallback: Failed to laodAd");
		SetStatus("Ad View Failed to load the Ad");
	} 

	void CreateBillboard () 
	{
		SafeDestroy();
		zoneId = inputFieldZoneId.text;
		adRef  =    VservUnityAd.CreateNewVservAd 
						( 
							 zoneId,				 		  //Zone Id
							 VservAdUX.VservAdUX_Interstitial //Ad Type	
						); 

		adUX = VservAdUX.VservAdUX_Interstitial;

		adRef.AdViewDidFailedToLoadAd += HandleAdViewDidFailedToLoadAd;
		adRef.AdViewDidLoadAd += HandleAdViewDidLoadAd;
		adRef.AdViewDidCacheAd += HandleAdViewDidCacheAd;
		adRef.DidInteractWithAd += HandleDidInteractWithAd;
		adRef.WillLeaveApp += HandleAdWillLeaveApp;
		adRef.WillPresentOverlay += HandleAdWillPresentOverlay;
		adRef.WillDismissOverlay += HandleAdWillDismissOverlay;
	}

	public void SetOrientation(Button selected)
	{
		if (selected.image.color == Color.green) {
			selected.image.color = Color.white;
			this.orientation = -1;
		} else {

			this.adaptiveButton.image.color = Color.white;
			this.portraitButton.image.color = Color.white;
			this.landscapeButton.image.color = Color.white;

			selected.image.color = Color.green;

			if (selected.gameObject.name == "Adaptive") {
				this.orientation = 0;
			}
			else if (selected.gameObject.name == "Portrait") {
				this.orientation = 1;
			}
			else if (selected.gameObject.name == "LandScape") {
				this.orientation = 2;
			}
		}
	}

	void SetStatus(string statusString)
	{
		status.text = statusString;
		status.GetComponent<Animator>().SetTrigger("Show");
	}

	public void HandleZoneIdChange()
	{
		if(isBanner)
		{
			CreateBanner();
		}
		else
		{
			CreateBillboard();
		}
	}

	public void HandleRefreshRateChange(InputField refreshRate)
	{
		print ("Inside" + int.Parse(refreshRate.text));
		adRef.SetRefreshRate (int.Parse(refreshRate.text));
	}
	public void ToggleToBanner(Toggle other)
	{
		if(isBanner)
			return;
		if(nextToggleTime<Time.time)
		{
			nextToggleTime = Time.time+0.5f;
			inputFieldZoneId.text = "a4956ac4";
			isBanner = true;
			CreateBanner();
			other.isOn = false;
		}
	}
	public void ToggleToBillboard(Toggle other)
	{
		if(!isBanner)
		{
			return;
		}
		if(nextToggleTime<Time.time)
		{
			nextToggleTime = Time.time+0.5f;
			inputFieldZoneId.text = "d3925a52";
			isBanner = false;
			CreateBillboard();
			other.isOn = false;
		}
	}

	public void DestroyAd()
	{
		adRef.DestroyAd();
		SetStatus("Destroying Ad");
	}
	public void CacheAd()
	{
		switch (this.orientation) {
		case -1:
			adRef.CacheAd();
			SetStatus("Caching Ad");
			break;

		case 0:
			adRef.CacheAdWithOrientation(VservAdOrientation.VservAdOrientation_Adaptive);
			SetStatus("Caching Ad With orientation Adaptive");
			break;

		case 1:
			adRef.CacheAdWithOrientation(VservAdOrientation.VservAdOrientation_Portrait);
			SetStatus("Caching Ad With orientation Portrait");
			break;

		case 2:
			adRef.CacheAdWithOrientation(VservAdOrientation.VservAdOrientation_Landscape);
			SetStatus("Caching Ad With orientation Landscape");
			break;
		}
	}
	
	public void CancelAd()
	{
		adRef.CancelAd();
		SetStatus("Cancelling Ad");
	}
	public void ShowAd()
	{
		adRef.ShowAd();
		SetStatus("Showing Ad");
	}
	public void LoadAd()
	{
		switch (this.orientation) {
		case -1:
			adRef.LoadAd();
			SetStatus("Caching Ad");
			break;
			
		case 0:
			adRef.LoadAdWithOrientation(VservAdOrientation.VservAdOrientation_Adaptive);
			SetStatus("LoadAd Ad With orientation Adaptive");
			break;
			
		case 1:
			adRef.LoadAdWithOrientation(VservAdOrientation.VservAdOrientation_Portrait);
			SetStatus("LoadAd Ad With orientation Portrait");
			break;
			
		case 2:
			adRef.LoadAdWithOrientation(VservAdOrientation.VservAdOrientation_Landscape);
			SetStatus("LoadAd Ad With orientation Landscape");
			break;
		}
	}

	public void ToggleRefresh()
	{
		isRefreshing = !isRefreshing;
		if (isRefreshing) 
		{
			buttonToggleRefresh.image.color = Color.green;
			adRef.SetRefresh(true);
		}
		else
		{
			buttonToggleRefresh.image.color = Color.red;
			adRef.SetRefresh(false);
		}
		SetStatus("Setting Refresh");
	}
	public void ResumeRefresh()
	{
		adRef.ResumeRefresh();
		SetStatus("Resuming Refresh");
	}
	public void PauseRefresh()
	{
		adRef.PauseRefresh();
		SetStatus("Pausing refresh");
	}
	public void StopRefresh()
	{
		adRef.StopRefresh();
		SetStatus("Stopping Refresh");
	}
	public void SetUser()
	{
		adRef.SetUser(user);
		adRef.SetTimeOut (40);
		SetStatus("Setting User and Timeout 40");
	}
	public void SetUXWithAdProperties()
	{
		adRef.SetUXTypeWithAdProperties( adUX, adProperties);
		adRef.SetTimeOut (10);
		SetStatus("Setting AdProperties + Timeout 10");
	}
}
