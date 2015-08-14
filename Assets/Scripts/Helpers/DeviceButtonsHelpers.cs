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
}
