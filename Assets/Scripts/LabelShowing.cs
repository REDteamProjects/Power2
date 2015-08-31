using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.Scripts;

public class LabelShowing : MonoBehaviour {

    private int ScaleFontTo;
    private int ScaleDifference;
    private bool DestroyAfterAnimation;
    private int DestroyTimeout;
    private LabelAnimationFinishedDelegate AnimationFinished;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (DestroyTimeout == 0) return;

	    var labelText = GetComponent<Text>();

        if (labelText.fontSize != ScaleFontTo && DestroyTimeout >= ScaleDifference)
	    {
	        if (labelText.fontSize > ScaleFontTo)
	            labelText.fontSize--;
	        else
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
        var wp = initGameObject.transform.position;
        transform.SetParent(fg.transform);

        var newPos = fg.transform.InverseTransformPoint(wp);
        

        ShowScalingLabel(newPos, text, textColor, shadowColor, animateFromSize, animateToSize, font, destroyAfterAnimation, callback);
    }

    public void ShowScalingLabel(Vector3 position, String text, Color textColor, Color shadowColor, int animateFromSize, int animateToSize, Font font = null, 
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null, bool toForeground = false)
    {
        if (toForeground)
        {
            var fg = GameObject.Find("/Foreground");
            transform.SetParent(fg.transform);
            position = fg.transform.InverseTransformPoint(position);
        }

        transform.localPosition = position;

        var labelText = GetComponent<Text>();
        if (textColor != shadowColor)
        {
            var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (scalingLabelObject != null)
            {
                var shadow = scalingLabelObject.GetComponent<LabelShowing>();
                shadow.transform.SetParent(transform.parent);
                shadow.ShowScalingLabel(new Vector3(position.x - 0.3f, position.y, position.z),
                    text, textColor, textColor, animateFromSize + 1, animateToSize + 1, font, destroyAfterAnimation);
            }
            labelText.color = shadowColor;
        }
        else
            labelText.color = textColor;
        labelText.fontSize = animateFromSize;
        ScaleFontTo = animateToSize;
        
        if (font != null)
            labelText.font = font;
        labelText.text = text;
        DestroyAfterAnimation = destroyAfterAnimation;
        ScaleDifference = animateToSize - animateFromSize;
        DestroyTimeout = ScaleDifference + 10;
        AnimationFinished = callback;
    }
}
