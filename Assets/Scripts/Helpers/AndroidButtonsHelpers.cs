using UnityEngine;
using System.Collections;

public class AndroidButtonsHelpers : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //if (Application.platform != RuntimePlatform.Android) return;
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        Application.LoadLevel("MainScene");
	}
}
