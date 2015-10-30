using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeplPageScript : MonoBehaviour
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

	    var manual_0 = Instantiate(LanguageManager.Instance.GetPrefab("UserManual_0"));
        manual_0.transform.SetParent(GameObject.Find("MainHelpText").transform);
	    manual_0.transform.localScale = new Vector3(80, 80, 1);
        manual_0.transform.localPosition = new Vector3(0, -300, 0);
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
        var mainTransform = (main.transform as RectTransform);
        //var modesTransform = (modes.transform as RectTransform);

	    var touches = TouchActionAdapter.GetTouch();
        if (touches.Count <= 0) return;
	    var touch = touches[0];

        var realTouchPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.OriginalPosition));

	    switch (touch.Phase)
	    {
	        case TouchPhase.Began:
                startPos = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.OriginalPosition));
                mainStartPos = mainTransform.localPosition;
                //modesStartPos = modesTransform.localPosition;
	            break;

	        case TouchPhase.Moved:
                var deltaX = realTouchPosition.x - startPos.x;
                var deltaY = realTouchPosition.y - startPos.y;

                if (SelectedItem == main && Mathf.Abs(deltaY) > 0.01)
                {
                    if (mainStartPos.y + deltaY > 580)
                        baseY = 580;
                    else if (mainStartPos.y + deltaY < 0)
                        baseY = 0;
                    else
                        baseY = mainStartPos.y + deltaY;

                    mainTransform.localPosition = new Vector3(mainStartPos.x, baseY,
	                    mainTransform.localPosition.z);
	                //return;
	            }

                //if (Mathf.Abs(deltaX) > 0.01)
                //{
                //    CurrentDirection = Mathf.Sign(deltaX);
                //    //+ right
                //    //- left

                //    mainTransform.localPosition = new Vector3(mainStartPos.x + deltaX, mainStartPos.y,
                //        mainTransform.localPosition.z);
                //    modesTransform.localPosition = new Vector3(modesStartPos.x + deltaX, modesStartPos.y,
                //        modesTransform.localPosition.z);
                //}
	            break;

            //case TouchPhase.Stationary:
            //    couldBeSwipe = false;
            //    break;

	        case TouchPhase.Ended:
	            //var swipeTime = Time.time - startTime;
                //if (CurrentDirection == 0)
                //    return;
                
                //var swipeDist = Mathf.Abs(realTouchPosition.x - startPos.x);
                //if (swipeDist > minSwipeDist)
                //{
                //    if (CurrentDirection < 0)
                //    {
                //        main.GetComponent<GameItemMovingScript>().MoveTo(-480, baseY, 20, null);
                //        modes.GetComponent<GameItemMovingScript>().MoveTo(0, 0, 20, null);
                //        SelectedItem = modes;
                //    }
                //    if (CurrentDirection > 0)
                //    {
                //        main.GetComponent<GameItemMovingScript>().MoveTo(baseX, baseY, 20, null);
                //        modes.GetComponent<GameItemMovingScript>().MoveTo(480, 0, 20, null);
                //        SelectedItem = main;
                //    }
                //    //var t = mainTransform.  
                //}
                //else
                //{
                //    main.GetComponent<GameItemMovingScript>().MoveTo(mainStartPos.x, mainStartPos.y, 20, null);
                //    modes.GetComponent<GameItemMovingScript>().MoveTo(modesStartPos.x, modesStartPos.y, 20, null);
                //}

	            //CurrentDirection = 0;

	            break;
	    }
	}
}
