using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.DataClasses;
using Assets.Scripts;
using Assets.Scripts.Helpers;

public class AboutPageScript : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake()
    {
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = GameColors.BackgroundColor;
        var modestext = GameObject.Find("AboutText").GetComponent<Text>();
        modestext.text = LanguageManager.Instance.GetTextValue("AboutText");
        GameObject.Find("AboutTextShadow").GetComponent<Text>().text = modestext.text;
        GameObject.Find("GUI/RateNowButton").GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("RateUs");
        GameObject.Find("GUI/RateNowButtonShadow").GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("RateUs");
    }

    
}
