using System.Linq;
using Assets.Scripts.Enums;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts;
using Assets.Scripts.Interfaces;


public class Scaling2D
{
    public Vector3 Speed { get; set; }
    public MovingFinishedDelegate ScalingCallback { get; set; }
    public Vector3 ScaleToSize { get; set; }

    public int ScalesLeft;
}


public class GameItemScalingScript : MonoBehaviour
{
    private static int _scalingItems;
    private volatile bool _isScaling;

    //private Vector2 movement;
    //private LineOrientation movementOrientation = LineOrientation.Vertical;
    //private MovingFinishedDelegate _moved;
    public bool isScaling 
    {
        get { return _isScaling; }
        private set
        {
            _isScaling = value;

            if (value)
                _scalingItems++;
            else
                _scalingItems--;
            LogFile.Message("Still scaling " + _scalingItems);
        }
    }
    private System.Collections.Generic.List<Scaling2D> _scalings = new System.Collections.Generic.List<Scaling2D>();


    //public MovingFinishedDelegate Moved
    //{
    //    get { return _moved; }
    //    set
    //    {
    //        if (value == null)
    //            LogFile.Message("Moved = null");
    //        else
    //            LogFile.Message("Moved = " + value.GetType());
    //        _moved = value;
    //    }
    //}

    //private Vector3 destination;

    /// <summary>
    /// Moving direction on update
    /// </summary>
    //public Vector2 direction = new Vector2(0, -1);
  
    //public Vector2 speed = new Vector2(14, 14);


    void Update()
    {
        // 2 - Movement
        if (!isScaling) return;

        
        var finalPoint = _scalings[0];
        if (finalPoint.ScalesLeft != 0)
        {
            transform.localScale = transform.localScale + finalPoint.Speed;
            finalPoint.ScalesLeft--;
            return;
        }

        //LogFile.Message("Moved callback on go");
        var callbackVariable = finalPoint.ScalingCallback;
        _scalings.Remove(finalPoint);
        if (_scalings.Count == 0)
            isScaling = false;
        else
        {
            LogFile.Message("_scalings.Count = " + _scalings.Count);
            return;
        }

        if (callbackVariable != null)
        {
            callbackVariable(gameObject, true);
            //LogFile.Message("Moved callback rised");
        }
        else
            LogFile.Message("No Scaled() callback!");    
    }

    public void ScaleTo(Vector3 toSize, int scalingTime, MovingFinishedDelegate scalingCallback)
    {
        if (isScaling)
        {
            LogFile.Message("isScaling is true");
            //movingCallback(gameObject, false);
            //return;
        }

        var newScaling = new Scaling2D
        {
            ScaleToSize = toSize,
            Speed = (toSize - transform.localScale)/scalingTime,
            ScalesLeft = scalingTime
        };
        if (scalingCallback != null)
            newScaling.ScalingCallback = scalingCallback;
        else
        {
            LogFile.Message("ScaleTo: No scalingCallback");
        }
        //LogFile.Message("Current localPosition: " + transform.localPosition.x + " " + transform.localPosition.y + "Deirection: " + direction.x + " " + direction.y + "Destination: " + destination.x + " " + destination.y + " " + movementOrientation);

        _scalings.Add(newScaling);

        isScaling = true;
    }
}
