using UnityEngine;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.DataClasses;

public class LabelShowing : MonoBehaviour {

    private int _scaleFontTo;
    private int _scaleDifference;
    private bool _destroyAfterAnimation;
    private int _destroyTimeout;
    private LabelAnimationFinishedDelegate _animationFinished;
    private Text _labelText;
    private int _step = 1;

	// Update is called once per frame
	void Update () {
	    if (_destroyTimeout == 0) return;

        if (_labelText.fontSize != _scaleFontTo && _destroyTimeout >= _scaleDifference)
	    {
	        /*if (labelText.fontSize > ScaleFontTo)
	            labelText.fontSize--;
	        else*/
                _labelText.fontSize+=_step;
	    }
	    else
	    {
            
	        if (_destroyAfterAnimation)
	        {
	            _destroyTimeout--;
	            if (_destroyTimeout == 0)
	            {
	                Destroy(gameObject);
	                _destroyAfterAnimation = false;
	                if (_animationFinished == null) return;
	                var callback = _animationFinished;
	                _animationFinished = null;
	                callback();
	            }
	            else
	                if (_destroyTimeout < _scaleDifference)
	                    _labelText.fontSize-=_step;
	        }
	        else
	        {
	            _destroyTimeout = 0;
	            if (_animationFinished == null) return;
	            var callback = _animationFinished;
	            _animationFinished = null;
	            callback();
	        }
	    }
	}  

    public void ShowScalingLabel(GameObject initGameObject, String text, Color textColor, Color shadowColor, int animateFromSize,
         int animateToSize, int step = 1, Font font = null,
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null, int rotateAngle = 0)
    {
        var fg = GameObject.Find("/Foreground");
        //var wp = initGameObject.transform.position;
        transform.SetParent(fg.transform);

        var newPos = fg.transform.InverseTransformPoint(initGameObject.transform.position/*wp*/);
        var showOn = new Vector3(newPos.x, newPos.y + 25 * initGameObject.GetComponent<SpriteRenderer>().bounds.size.y, newPos.z);// 25 is default pixels per unit 100 / 2 (half of object size(which is size.y / 2, cause 1 in size = 2 units)

        ShowScalingLabel(showOn, text, textColor, shadowColor, animateFromSize, animateToSize, step, font, destroyAfterAnimation, callback, false, rotateAngle);
    }

    public void ShowScalingLabel(Vector3 position, String text, Color textColor, Color shadowColor, int animateFromSize, int animateToSize, int step = 1, Font font = null, 
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null, bool toForeground = false, int rotateAngle = 0)
    {
        if (toForeground)
        {
            var fg = GameObject.Find("/Foreground");
            transform.SetParent(fg.transform);
            var z = position.z;
            position = fg.transform.InverseTransformPoint(position);
            position.z = z;
        }

        _step = step;
        animateToSize += (animateToSize - animateFromSize) % _step;

        transform.localPosition = position;

        if (rotateAngle != 0)
            transform.RotateAround(transform.position, Vector3.forward, rotateAngle);

        if(_labelText == null)
        _labelText = GetComponent<Text>();
        if (animateToSize < animateFromSize)
        {
            var to = animateFromSize;
            animateFromSize = animateToSize;
            animateToSize = to;
        }

        if (textColor != shadowColor)
        {
            var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            
            if (scalingLabelObject != null)
            {
                scalingLabelObject.name = name + "(Shadow)";
                var shadow = scalingLabelObject.GetComponent<LabelShowing>();
                shadow.transform.SetParent(transform.parent);
                shadow.transform.localScale = transform.localScale;
                shadow.ShowShadowLabel(new Vector3(position.x - (rotateAngle == 0 ? 3f : 0), position.y, position.z),
                    text, textColor, textColor, animateFromSize, animateToSize, _step, font, destroyAfterAnimation, /*null,*/ false, rotateAngle);
                animateFromSize += 1;
                animateToSize += 1;
            }
            _labelText.color = shadowColor;
        }
        else
            _labelText.color = textColor;
        _labelText.fontSize = animateFromSize;
        _scaleFontTo = animateToSize;
        _labelText.font = font ? font : Game.textFont;
        _labelText.text = text;
        _destroyAfterAnimation = destroyAfterAnimation;
        _scaleDifference = animateToSize - animateFromSize;
        if(_scaleDifference != 0)
        _destroyTimeout = _scaleDifference + 16;
        else
            _destroyTimeout = 0;
        _animationFinished = callback;
    }

     private void ShowShadowLabel(Vector3 position, String text, Color textColor, Color shadowColor, int animateFromSize, int animateToSize, int step = 1, Font font = null, 
        bool destroyAfterAnimation = false, /*LabelAnimationFinishedDelegate callback = null,*/ bool toForeground = false, int rotateAngle = 0)
     {
         _step = step;
         transform.localPosition = position;

        if (rotateAngle != 0)
            transform.RotateAround(transform.position, Vector3.forward, rotateAngle);

        if(_labelText == null)
        _labelText = GetComponent<Text>();

        _labelText.color = textColor;
        _labelText.fontSize = animateFromSize;
        _scaleFontTo = animateToSize;
        _labelText.font = font ? font : Game.textFont;
        _labelText.text = text;
        _destroyAfterAnimation = destroyAfterAnimation;
        _scaleDifference = animateToSize - animateFromSize;
        if(_scaleDifference != 0)
        _destroyTimeout = _scaleDifference + 16;
        else
            _destroyTimeout = 0;

     }
}
