using UnityEngine;
using System;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using System.Collections.Generic;


public class LabelShowing : MonoBehaviour {

    public static Int32 minLabelFontSize = 70;
    public static Int32 maxLabelFontSize = 100;
    private static List<LabelShowing> PointLabels = new List<LabelShowing>();
    private Vector3 _localScale;
    private GameItemType? _type = null;
    private int _animateFromSize;
    private int _scaleFontTo;
    private bool _destroyAfterAnimation;
    private int _pauseTimeout;
    private LabelAnimationFinishedDelegate _animationFinished;
    private Text _labelText;
    private ScaleState _scaleState = ScaleState._increase;
    private int _step = 1;
    private int _currentFontSize;
    private LabelShowing Shadow = null;

	// Update is called once per frame
	void Update () {

        switch(_scaleState)
        {
            case ScaleState._increase:
                if (_currentFontSize >= _scaleFontTo)
                {
                    _scaleState = ScaleState._static;
                    break;
                }
                _currentFontSize += _step;
                _labelText.fontSize = _currentFontSize;
                if (Shadow != null)
                {
                    Shadow._currentFontSize += _step;
                    Shadow._labelText.fontSize = Shadow._currentFontSize;
                }
                return;
            case ScaleState._static:
                _pauseTimeout--;
                if (_pauseTimeout <= 0)
                    _scaleState = ScaleState._decrease;
                return;
            case ScaleState._decrease:
                if (_destroyAfterAnimation)
                {
                    if (_currentFontSize > _animateFromSize)
                    {
                        _currentFontSize -= _step;
                        _labelText.fontSize = _currentFontSize;
                        if (Shadow != null)
                        {
                            Shadow._currentFontSize -= _step;
                            Shadow._labelText.fontSize = Shadow._currentFontSize;
                        }
                    }
                    else
                    {
                        if (_type.HasValue)
                        {
                            _scaleState = ScaleState._none;
                            transform.localScale = Vector3.zero;
                            if (Shadow != null)
                                Shadow.transform.localScale = Vector3.zero;
                        }
                        else
                        {
                            if (Shadow != null)
                                Destroy(Shadow.gameObject);
                            Destroy(gameObject);
                        }
                        //_destroyAfterAnimation = false;
                        if (_animationFinished == null) return;
                        var callback = _animationFinished;
                        _animationFinished = null;
                        callback();
                    }
                }
                else
                {
                    if (_animationFinished == null) return;
                    var callback = _animationFinished;
                    _animationFinished = null;
                    callback();
                    _scaleState = ScaleState._none;
                }
                return;
        }
	}  

    public static void ShowScalingLabel(GameObject initGameObject, String text, Color textColor, Color shadowColor, int animateFromSize,
         int animateToSize, int step = 1, Font font = null,
        bool destroyAfterAnimation = false, LabelAnimationFinishedDelegate callback = null, int rotateAngle = 0, GameItemType? type = null)
    {
        var fg = GameObject.Find("/Foreground");
        //var wp = initGameObject.transform.position;
        var newPos = fg.transform.InverseTransformPoint(initGameObject.transform.position/*wp*/);
        var showOn = new Vector3(newPos.x, newPos.y + 25 * initGameObject.GetComponent<SpriteRenderer>().bounds.size.y, newPos.z);// 25 is default pixels per unit 100 / 2 (half of object size(which is size.y / 2, cause 1 in size = 2 units)
        if(type.HasValue)
        {
            for(int i = 0;i < PointLabels.Count;i++)
                if(PointLabels[i]._type.HasValue && PointLabels[i]._type.Value == type && PointLabels[i]._scaleState == ScaleState._none)
                {
                    PointLabels[i]._labelText.text = text;
                    PointLabels[i].transform.localPosition = showOn;
                    PointLabels[i].transform.localScale = PointLabels[i]._localScale;
                    if (PointLabels[i].Shadow != null)
                    {
                        PointLabels[i].Shadow._labelText.text = text;
                        PointLabels[i].Shadow.transform.localPosition = new Vector3(showOn.x - 3,showOn.y,showOn.z);
                        PointLabels[i].Shadow.transform.localScale = PointLabels[i]._localScale;
                    }
                    PointLabels[i]._scaleState = ScaleState._increase;
                    return;
                }
        }
        var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
        var label = o.GetComponent<LabelShowing>();
        label._type = type;
        label.transform.SetParent(fg.transform);
        label.ShowScalingLabel(showOn, text, textColor, shadowColor, animateFromSize, animateToSize, step, font, destroyAfterAnimation, callback, false, rotateAngle);
        if(type.HasValue)
            PointLabels.Add(label);
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
        //animateToSize += (animateToSize - animateFromSize) % _step;

        transform.localPosition = position;

        if (rotateAngle != 0)
            transform.RotateAround(transform.position, Vector3.forward, rotateAngle);

        if(_labelText == null)
        _labelText = GetComponent<Text>();
        /*if (animateToSize < animateFromSize)
        {
            var to = animateFromSize;
            animateFromSize = animateToSize;
            animateToSize = to;
        }*/

        if (textColor != shadowColor)
        {
            var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            
            if (scalingLabelObject != null)
            {
                //scalingLabelObject.name = name + "(Shadow)";
                Shadow = scalingLabelObject.GetComponent<LabelShowing>();
                Shadow.transform.SetParent(transform.parent);
                Shadow.transform.localScale = transform.localScale;
                Shadow.ShowWithShadowLabel(new Vector3(position.x - (rotateAngle == 0 ? animateToSize > 60 ? 3f : 2f : 0), position.y, position.z),
                    text, textColor, textColor, animateFromSize, animateToSize, font, rotateAngle);
                animateFromSize += 1;
                animateToSize += 1;
            }
            _labelText.color = shadowColor;
        }
        else
            _labelText.color = textColor;
        _labelText.fontSize = _currentFontSize =_animateFromSize = animateFromSize;
        _scaleFontTo = animateToSize;
        _labelText.font = font ? font : Game.textFont;
        _labelText.text = text;
        _destroyAfterAnimation = destroyAfterAnimation;
        _pauseTimeout = 30 / _step;
        _animationFinished = callback;
        _localScale = transform.localScale;
    }

     private void ShowWithShadowLabel(Vector3 position, String text, Color textColor, Color shadowColor, int animateFromSize, int animateToSize, Font font = null, int rotateAngle = 0)
     {
         transform.localPosition = position;

        if (rotateAngle != 0)
            transform.RotateAround(transform.position, Vector3.forward, rotateAngle);

        if(_labelText == null)
        _labelText = GetComponent<Text>();

        _labelText.color = textColor;
        _labelText.fontSize = _currentFontSize = animateFromSize;
        _labelText.font = font ? font : Game.textFont;
        _labelText.text = text;
     }
}
