using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using SmartLocalization;
using Assets.Scripts.DataClasses;

public class LabelShowing : MonoBehaviour {

    private int ScaleFontTo;
    private int ScaleDifference;
    private bool DestroyAfterAnimation;
    private int DestroyTimeout;
    private LabelAnimationFinishedDelegate AnimationFinished;
    private Text labelText;

	// Use this for initialization
	void Start () { 
	
	}

	// Update is called once per frame
	void Update () {
	    if (DestroyTimeout == 0) return;

        if (labelText.fontSize != ScaleFontTo && DestroyTimeout >= ScaleDifference)
	    {
	        /*if (labelText.fontSize > ScaleFontTo)
	            labelText.fontSize--;
	        else*/
                labelText.fontSize++;
	    }
	    else
	    {
            
	        if (DestroyAfterAnimation)
	        {
	            DestroyTimeout--;
	            if (DestroyTimeout == 0)
	            {
	                Destroy(gameObject);
	                DestroyAfterAnimation = false;
	                if (AnimationFinished == null) return;
	                var callback = AnimationFinished;
	                AnimationFinished = null;
	                callback();
	            }
	            else
	                if (DestroyTimeout < ScaleDifference)
	                    labelText.fontSize--;
	        }
	        else
	        {
	            DestroyTimeout = 0;
	            if (AnimationFinished == null) return;
	            var callback = AnimationFinished;
	            AnimationFinished = null;
	            callback();
	        }
	    }
	}  

    public void ShowScalingLabel(GameObject initGameObject, String text, Color textColor, Color shadowColor, int animateFromSize,
        int animateToSize, Font font = null,
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null)
    {
        var fg = GameObject.Find("/Foreground");
        //var wp = initGameObject.transform.position;
        transform.SetParent(fg.transform);

        var newPos = fg.transform.InverseTransformPoint(initGameObject.transform.position/*wp*/);
        var showOn = new Vector3(newPos.x, newPos.y + 25 * initGameObject.GetComponent<SpriteRenderer>().bounds.size.y, newPos.z);// 25 is default pixels per unit 100 / 2 (half of object size(which is size.y / 2, cause 1 in size = 2 units)

        ShowScalingLabel(showOn, text, textColor, shadowColor, animateFromSize, animateToSize, font, destroyAfterAnimation, callback);
    }

    public void ShowScalingLabel(Vector3 position, String text, Color textColor, Color shadowColor, int animateFromSize, int animateToSize, Font font = null, 
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null, bool toForeground = false)
    {
        if (toForeground)
        {
            var fg = GameObject.Find("/Foreground");
            transform.SetParent(fg.transform);
            var z = position.z;
            position = fg.transform.InverseTransformPoint(position);
            position.z = z;
        }

        transform.localPosition = position;

        if(labelText == null)
        labelText = GetComponent<Text>();
        if (animateToSize < animateFromSize)
        {
            var to = animateFromSize;
            animateFromSize = animateToSize;
            animateToSize = animateFromSize;
        }

        if (textColor != shadowColor)
        {
            var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (scalingLabelObject != null)
            {
                var shadow = scalingLabelObject.GetComponent<LabelShowing>();
                shadow.transform.SetParent(transform.parent);
                shadow.ShowScalingLabel(new Vector3(position.x - 3f, position.y, position.z),
                    text, textColor, textColor, animateFromSize + 1, animateToSize + 1, font, destroyAfterAnimation);
            }
            labelText.color = shadowColor;
        }
        else
            labelText.color = textColor;
        labelText.fontSize = animateFromSize;
        ScaleFontTo = animateToSize;
        labelText.font = font ? font : Game.textFont;
        labelText.text = text;
        DestroyAfterAnimation = destroyAfterAnimation;
        ScaleDifference = animateToSize - animateFromSize;
        DestroyTimeout = ScaleDifference + 10;
        AnimationFinished = callback;
    }
}
