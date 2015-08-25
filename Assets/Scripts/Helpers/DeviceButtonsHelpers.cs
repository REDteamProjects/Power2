using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeviceButtonsHelpers : MonoBehaviour
{

    private List<string> _audioSources;

    public List<string> AudioSources
    {
        get { return _audioSources ?? (_audioSources = new List<string>()); }
    }

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

    public static void OnSoundAction(string audioClip, bool isVibrate, bool stopCurrent = false)
    {
        var mainCamera = GameObject.Find("Main Camera");

        var audioSource = mainCamera.GetComponent<AudioSource>();
        if (stopCurrent)
            audioSource.Stop();

        audioSource.PlayOneShot(Resources.Load<AudioClip>(audioClip));

        if (isVibrate)
            Vibration.Vibrate(10);
    }
}
