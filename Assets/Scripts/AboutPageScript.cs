using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.DataClasses;
using Assets.Scripts;

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
        var btnTextShadow = MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", modestext.transform.parent, Vector3.one, new Vector3(3, -350, 0), LanguageManager.Instance.GetTextValue("RateUs"), 60, null, GameColors.DefaultDark).GetComponentInChildren<Text>();
        MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", modestext.transform.parent, Vector3.one, new Vector3(0, -350, 0), btnTextShadow.text, btnTextShadow.fontSize,
                GoToStore);
    }

    void GoToStore()
    {
    #if UNITY_WINRT || UNITY_WP8
        Application.OpenURL("https://www.windowsphone.com");
    #endif
    #if UNITY_ANDROID
        Application.OpenURL("http://play.google.com");
    #endif
    #if UNITY_IOS
        Application.OpenURL("http://www.itunes.com");
    #endif
    }
}
