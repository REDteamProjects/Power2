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

	// Use this for initialization
	void Start ()
	{
	    var text = GameObject.Find("MainHelpText").GetComponent<Text>();
        text.text = LanguageManager.Instance.GetTextValue("MainHelp");

        var modestext = GameObject.Find("GameModesHelp").GetComponent<Text>();
        modestext.text = LanguageManager.Instance.GetTextValue("GameModesHelp");

        main = GameObject.Find("MainHelpText");
        modes = GameObject.Find("GameModesHelp");
	}
	
	// Update is called once per frame
	void Update ()
	{
        var mainTransform = (main.transform as RectTransform);
        var modesTransform = (modes.transform as RectTransform);

	    var touches = TouchActionAdapter.GetTouch();
        if (touches.Count <= 0) return;
	    var touch = touches[0];

        var realTouchPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.OriginalPosition));

	    switch (touch.Phase)
	    {
	        case TouchPhase.Began:
                startPos = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.OriginalPosition));
                mainStartPos = mainTransform.localPosition;
                modesStartPos = modesTransform.localPosition;
	            break;

	        case TouchPhase.Moved:
                var deltaX = realTouchPosition.x - startPos.x;
                CurrentDirection = Mathf.Sign(deltaX);
                //+ right
                //- left

                mainTransform.localPosition = new Vector3(mainStartPos.x + deltaX, mainStartPos.y, mainTransform.localPosition.z);
                modesTransform.localPosition = new Vector3(modesStartPos.x + deltaX, modesStartPos.y, modesTransform.localPosition.z);
	            break;

            //case TouchPhase.Stationary:
            //    couldBeSwipe = false;
            //    break;

	        case TouchPhase.Ended:
	            //var swipeTime = Time.time - startTime;
                var swipeDist = Mathf.Abs(realTouchPosition.x - startPos.x);
	            if (swipeDist > minSwipeDist)
	            {
                    if (CurrentDirection < 0)
	                {
                        main.GetComponent<GameItemMovingScript>().MoveTo(-480, 0, 20, null);
	                    modes.GetComponent<GameItemMovingScript>().MoveTo(0, 0, 20, null);
	                }
                    if (CurrentDirection > 0)
                    {
                        main.GetComponent<GameItemMovingScript>().MoveTo(0, 0, 20, null);
                        modes.GetComponent<GameItemMovingScript>().MoveTo(480, 0, 20, null);
                    }
	                //var t = mainTransform.  
	            }
	            else
	            {
                    main.GetComponent<GameItemMovingScript>().MoveTo(mainStartPos.x, mainStartPos.y, 20, null);
                    modes.GetComponent<GameItemMovingScript>().MoveTo(modesStartPos.x, modesStartPos.y, 20, null);
	            }

	            CurrentDirection = 0;

	            break;
	    }
	}
}
