using UnityEngine;
using System.Collections;

public class DeviceButtonsHelpers : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //if (Application.platform != RuntimePlatform.Android) return;
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (Application.loadedLevelName == "MainScene")
            Application.Quit();
	    var pbs = GetComponent<PauseButtonScript>();
	    if (pbs != null)
	    {
	        pbs.OnPauseButtonClick();
	        return;
	    }

	    Application.LoadLevel("MainScene");
	}

    public static void OnButtonSoundAction(string audioClip, bool isVibrate)
    {
        var mainCamera = GameObject.Find("Main Camera");

        var audioSource = mainCamera.GetComponent<AudioSource>();

        audioSource.PlayOneShot(Resources.Load<AudioClip>(audioClip));

        if (isVibrate)
            Vibration.Vibrate(10);
    }
}
