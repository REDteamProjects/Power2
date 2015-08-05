using SmartLocalization;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeplPageScript : MonoBehaviour
{
    private bool couldBeSwipe;
    private Vector2 startPos;
    private float startTime;
    private float comfortZone;
    private float maxSwipeTime;
    private float minSwipeDist;

	// Use this for initialization
	void Start () {
	    var text = GetComponentInChildren<Text>();
        text.text = LanguageManager.Instance.GetTextValue("MainHelp");
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.touchCount > 0)
        {
            var touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    couldBeSwipe = true;
                    startPos = touch.position;
                    startTime = Time.time;
                    break;

                case TouchPhase.Moved:
                    if (Mathf.Abs(touch.position.y - startPos.y) > comfortZone)
                    {
                        couldBeSwipe = false;
                    }
                    break;

                case TouchPhase.Stationary:
                    couldBeSwipe = false;
                    break;

                case TouchPhase.Ended:
                    var swipeTime = Time.time - startTime;
                    var swipeDist = (touch.position - startPos).magnitude;

                    if (couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist))
                    {
                        // It's a swiiiiiiiiiiiipe!
                        var swipeDirection = Mathf.Sign(touch.position.y - startPos.y);

                        // Do something here in reaction to the swipe.
                    }
                    break;
            }
        }
	}
}
