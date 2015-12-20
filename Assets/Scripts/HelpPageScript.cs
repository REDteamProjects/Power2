using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.DataClasses;
using System;

public class HelpPageScript : MonoBehaviour
{
    private GameObject InGameHelpModule;
    private GameObject currentHelp;
    private GameObject nextbtn;
    private GameObject prevbtn;
    private List<string> _allModulesPostfixList;
    private int _currentIndex;

	// Use this for initialization
	void Awake ()
	{
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = GameColors.BackgroundColor;
        var fg = GameObject.Find("/Foreground");
        if (fg == null) return;
        InGameHelpModule = Instantiate(Resources.Load("Prefabs/HelpPageInGameHelper")) as GameObject;
        InGameHelpModule.transform.SetParent(fg.transform);
        InGameHelpModule.transform.localScale = Vector3.one;
        InGameHelpModule.transform.localPosition = new Vector3(0, -25, 0);
        GameObject.Find("/Foreground/ToMainMenuButton").transform.SetParent(InGameHelpModule.transform);
        nextbtn = GameObject.Find("/Foreground/HelpPageInGameHelper(Clone)/NextButton");
        nextbtn.GetComponent<Button>().onClick.AddListener(() => NextHelpItem());
        prevbtn = GameObject.Find("/Foreground/HelpPageInGameHelper(Clone)/PrevButton");
        prevbtn.GetComponent<Button>().onClick.AddListener(() => PrevHelpItem());
        _allModulesPostfixList = new List<String>(LanguageManager.Instance.GetKeysWithinCategory("UserHelp."));
        for (int i = 0; i < _allModulesPostfixList.Count;i++)
            if (!PlayerPrefs.HasKey(_allModulesPostfixList[i].Substring(9)))
            {
                _allModulesPostfixList.RemoveAt(i);
                i--;
            }
        if (!_allModulesPostfixList.Contains("UserHelp._easy"))
        {
            _allModulesPostfixList.Insert(0, "UserHelp._easy");
        }
        _allModulesPostfixList.Insert(1, "UserHelp.UserProgress");
        if (UnityADHelper.AdTaps <= 16)
            _allModulesPostfixList.Insert(2, "UserHelp.RemoveAds");
        LoadHelp(0);
        var imgprev = prevbtn.GetComponent<Image>();
        imgprev.color = new Color(imgprev.color.r, imgprev.color.g, imgprev.color.b, 0.5f);
        if (_currentIndex == _allModulesPostfixList.Count - 1)
        {
            var imgnext = nextbtn.GetComponent<Image>();
            imgnext.color = new Color(imgnext.color.r, imgnext.color.g, imgnext.color.b, 0.5f);
        }
	}


    private bool LoadHelp(int index)
    {
        if (index < 0 || index >= _allModulesPostfixList.Count)
            return false;
        var manualPrefab = LanguageManager.Instance.GetPrefab(_allModulesPostfixList[index]);
        if (manualPrefab == null)
            return false;
        if (currentHelp != null)
            Destroy(currentHelp);
        currentHelp = Instantiate(manualPrefab);
        currentHelp.transform.SetParent(InGameHelpModule.transform);
        currentHelp.transform.localScale = new Vector3(45, 45, 0);
        currentHelp.transform.localPosition = new Vector3(0, 30, -4);
        return true;
    }

	// Update is called once per frame
	void Update ()
	{
        
	}

    public void NextHelpItem()
    {
        if (LoadHelp(_currentIndex + 1))
            _currentIndex++;
        Image img;
        if (_currentIndex == _allModulesPostfixList.Count-1)
        {
            img = nextbtn.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
        }
        img = prevbtn.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
    }

    public void PrevHelpItem()
    {
        if (LoadHelp(_currentIndex - 1))
            _currentIndex--;
        Image img;
        if (_currentIndex == 0)
        {
            img = prevbtn.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
        }
        img = nextbtn.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
    }
}
