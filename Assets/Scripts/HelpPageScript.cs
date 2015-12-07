using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpPageScript : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 mainStartPos;
    private Vector2 modesStartPos;
    private float minSwipeDist = 150;
    private GameObject main;
    private GameObject modes;
    private float CurrentDirection;
    private float baseX = 0;
    private float baseY = 0;
    private GameObject SelectedItem;


	// Use this for initialization
	void Start ()
	{
        //var text = GameObject.Find("MainHelpText").GetComponent<Text>();
        //text.text = LanguageManager.Instance.GetTextValue("MainHelp");

	    var manual_0 = Instantiate(LanguageManager.Instance.GetPrefab("UserHelp_easy"));
        manual_0.transform.SetParent(GameObject.Find("MainHelpText").transform);
        manual_0.transform.localScale = new Vector3(1f, 0.4f, 1);
        manual_0.transform.localPosition = new Vector3(0, -350, 0);
        //var modestext = GameObject.Find("GameModesHelp").GetComponent<Text>();
        //modestext.text = LanguageManager.Instance.GetTextValue("GameModesHelp");

        //var manual_1 = Instantiate(LanguageManager.Instance.GetPrefab("UserManual_1"));
        //manual_1.transform.parent = GameObject.Find("GameModesHelp").transform;
        //manual_1.transform.localScale = new Vector3(60, 60, 1);
        //manual_1.transform.localPosition = new Vector3(0, 210, 0);

        main = GameObject.Find("MainHelpText");
	    SelectedItem = main;
	    //modes = GameObject.Find("GameModesHelp");
	}
	
	// Update is called once per frame
	void Update ()
	{
        
	}
}
