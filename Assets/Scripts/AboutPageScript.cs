using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AboutPageScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

        var modestext = GameObject.Find("AboutText").GetComponent<Text>();
        modestext.text = LanguageManager.Instance.GetTextValue("AboutText");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
