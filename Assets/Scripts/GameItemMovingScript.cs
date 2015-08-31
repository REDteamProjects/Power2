﻿using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public delegate void MovingFinishedDelegate(UnityEngine.Object movedItem, bool result);
//public delegate void ContinueMovingDelegate(UnityEngine.Object movedItem, bool result);


public class Destination2D
{
    private Vector2 _showFrom;
    public Vector2 Direction { get; set; }
    public Vector3 Destination { get; set; }
    public Vector3 StartPoint { get; set; }
    public Vector2 Speed { get; set; }
    public LineOrientation MovementOrientation { get; set; }
    public MovingFinishedDelegate MovingCallback { get; set; }

    public Vector2 ShowFrom
    {
        get { return _showFrom; }
        set
        {
            _showFrom = value;
            Visible = (_showFrom == Vector2.zero);
        }
    }

    public Vector3 ScaleTo { get; set; }
    public bool Visible { get; set; }
    public bool ChangingDirection { get; set; }
    public String MoveSound { get; set; }
}

public delegate void MovingItemFinishedEventHandler(object sender, int count);


public class GameItemMovingScript : MonoBehaviour
{
    public static event MovingItemFinishedEventHandler MovingItemFinished;

    private static int _movingItems;
    private volatile bool _isMoving;

    //private Vector2 movement;
    //private LineOrientation movementOrientation = LineOrientation.Vertical;
    //private MovingFinishedDelegate _moved;
    public bool IsMoving 
    {
        get { return _isMoving; }
        private set
        {
            if (_isMoving == value) return;

            _isMoving = value;
            if (value)
            {
                _movingItems++;
            }
            else
            {
                _movingItems--;
                var eventHandler = MovingItemFinished;
                if (eventHandler != null)
                    eventHandler(this, _movingItems);
            }

            LogFile.Message("Still moving " + _movingItems, true);
        }
    }

    public float DelayTime = 0.47f;
    private readonly List<Destination2D> _destinations = new List<Destination2D>();
    
    private bool _isDirectionChangable;
    private float _currentDelayTime;

    public bool ChangingDirection { get; private set; }
    public Destination2D CurrentDestination { get { return _destinations.FirstOrDefault(); } }


    void Update()
    {
        // 2 - Movement
        if (!IsMoving) return;

        
        //var finalPoint = _destinations[0];
        if (Math.Abs(transform.localPosition.x - CurrentDestination.Destination.x) > 0.01 || Math.Abs(transform.localPosition.y - CurrentDestination.Destination.y) > 0.01)
        {
            if (!CurrentDestination.Visible && CurrentDestination.ScaleTo != Vector3.zero &&
                (Math.Abs(CurrentDestination.ShowFrom.x - transform.localPosition.x) < 0.01 ||
                 transform.localPosition.x > CurrentDestination.ShowFrom.x)
                &&
                (Math.Abs(transform.localPosition.y - CurrentDestination.ShowFrom.y) < 0.01 ||
                 transform.localPosition.y < CurrentDestination.ShowFrom.y))
            {
                transform.localScale = CurrentDestination.ScaleTo;
                CurrentDestination.Visible = true;
            }
            var movement = new Vector3(
                CurrentDestination.Speed.x * CurrentDestination.Direction.x,
                CurrentDestination.Speed.y * CurrentDestination.Direction.y, 0f);

            movement *= Time.deltaTime;

            if (CurrentDestination.MovementOrientation == LineOrientation.Vertical)
            {
                //if (Math.Abs(transform.localPosition.y - finalPoint.Destination.y) < Math.Abs(movement.y))
                if (Math.Abs(CurrentDestination.StartPoint.y - CurrentDestination.Destination.y) - Math.Abs(CurrentDestination.StartPoint.y - transform.localPosition.y) <= Math.Abs(movement.y))
                {
                    //movement.y = destination.y - transform.localPosition.y;
                    transform.localPosition = new Vector3(transform.localPosition.x, CurrentDestination.Destination.y, CurrentDestination.Destination.z);
                    return;
                    //isMoving = false;
                }
            }
            else if (CurrentDestination.MovementOrientation == LineOrientation.Horizontal)
            {
                //if (Math.Abs(transform.localPosition.x - finalPoint.Destination.x) < Math.Abs(movement.x))
                if (Math.Abs(CurrentDestination.StartPoint.x - CurrentDestination.Destination.x) - Math.Abs(CurrentDestination.StartPoint.x - transform.localPosition.x) <= Math.Abs(movement.x))
                {
                    //movement.x = destination.x - transform.localPosition.x;
                    transform.localPosition = new Vector3(CurrentDestination.Destination.x, transform.localPosition.y, CurrentDestination.Destination.z);
                    return;
                    //isMoving = false;
                }
            }
            else if (CurrentDestination.MovementOrientation == LineOrientation.Both)
            {
                if (//Math.Abs(transform.localPosition.x - finalPoint.Destination.x) < Math.Abs(movement.x) ||
                    //Math.Abs(transform.localPosition.y - finalPoint.Destination.y) < Math.Abs(movement.y))
                    Math.Abs(CurrentDestination.StartPoint.y - CurrentDestination.Destination.y) - Math.Abs(CurrentDestination.StartPoint.y - transform.localPosition.y) <= Math.Abs(movement.y) ||
                    Math.Abs(CurrentDestination.StartPoint.x - CurrentDestination.Destination.x) - Math.Abs(CurrentDestination.StartPoint.x - transform.localPosition.x) <= Math.Abs(movement.x))
                {
                    //movement.x = destination.x - transform.localPosition.x;
                    //movement.y = destination.y - transform.localPosition.y;

                    transform.localPosition = new Vector3(CurrentDestination.Destination.x, CurrentDestination.Destination.y, CurrentDestination.Destination.z);
                    return;
                    //isMoving = false;
                }
            }

            //if (!GetComponent<GameItem>().IsTouched)
            //{
            //    if (_currentDelayTime <= 0)
            //    {
            //        DeviceButtonsHelpers.OnSoundAction(CurrentDestination.MoveSound, false);
            //        _currentDelayTime = DelayTime;
            //    }
            //    else
            //    {
            //        _currentDelayTime -= Time.deltaTime;
            //    }
            //}
            transform.Translate(movement);

            return;
        }

        //LogFile.Message("Moved callback on go");
        var callbackVariable = CurrentDestination.MovingCallback;
        LogFile.Message("callbackVariable = " + (callbackVariable != null ? callbackVariable.ToString() : "null"), true);

        _destinations.Remove(CurrentDestination);

        if (CurrentDestination == null)
            IsMoving = false;
        else
        {
            LogFile.Message("_destinations.Count = " + _destinations.Count, true);
            return;
        }

        if (callbackVariable != null)
        {
            callbackVariable(gameObject, true);
            //LogFile.Message("Moved callback rised");
        }
        else
            LogFile.Message("No Moved() callback!", true);    
    }

    public void MoveTo(float? x, float? y, float movingSpeed, MovingFinishedDelegate movingCallback, Vector2? showFrom = null, Vector3? scaleTo = null, bool changingDirection = false, Int32? isHighPriority = null, String moveSound = null)
    {
        if (IsMoving)
        {
            LogFile.Message("isMoving is true", true);
            //movingCallback(gameObject, false);
            //return;
        }

        var newMove = new Destination2D();

        float Xdir = 0, Ydir = 0, toX = transform.localPosition.x, toY = transform.localPosition.y, toZ = transform.localPosition.z;
        newMove.MovementOrientation = LineOrientation.Both;
        if (x.HasValue && Math.Abs(x.Value - transform.localPosition.x) > 0.01)
        {
            Xdir = transform.localPosition.x > x.Value ? -1 : 1;
            toX = x.Value;
            newMove.MovementOrientation = LineOrientation.Horizontal;
        }
        if (y.HasValue && Math.Abs(y.Value - transform.localPosition.y) > 0.01)
        {
            Ydir = transform.localPosition.y > y.Value ? -1 : 1;
            if (x.HasValue)
            {
                var X2dir = x.Value - transform.localPosition.x;
                var Y2dir = y.Value - transform.localPosition.y;
                if (Math.Abs(X2dir) > Math.Abs(Y2dir))
                    Ydir = Y2dir / Math.Abs(X2dir);
                else
                    Xdir = X2dir / Math.Abs(Y2dir);
            }
            toY = y.Value;
            newMove.MovementOrientation = newMove.MovementOrientation == LineOrientation.Both ? LineOrientation.Vertical : LineOrientation.Both;
        }
        newMove.Direction = new Vector2(Xdir, Ydir);
        newMove.Destination = new Vector3(toX, toY, toZ);
        newMove.StartPoint = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        newMove.Speed = new Vector2(movingSpeed, movingSpeed);
        newMove.ShowFrom = showFrom.HasValue ? showFrom.Value : Vector2.zero;
        newMove.ScaleTo = scaleTo.HasValue ? scaleTo.Value : Vector3.zero;
        newMove.MoveSound = moveSound;

        if (movingCallback != null)
            newMove.MovingCallback = movingCallback;
        else
        {
            LogFile.Message("MoveTo: No movingCallback", true);
        }
        //LogFile.Message("Current localPosition: " + transform.localPosition.x + " " + transform.localPosition.y + "Deirection: " + direction.x + " " + direction.y + "Destination: " + destination.x + " " + destination.y + " " + movementOrientation);

        if (isHighPriority.HasValue && isHighPriority.Value >= 0 && isHighPriority.Value < _destinations.Count)
            _destinations.Insert(isHighPriority.Value, newMove);
        else
            _destinations.Add(newMove);

        LogFile.Message("new move added", true);

        _isDirectionChangable = changingDirection;
        //newMove.ChangingDirection = changingDirection;

        IsMoving = true;
    }

    public void ChangeDirection(float? x, float? y, float movingSpeed, MovingFinishedDelegate movingCallback, Vector2? showFrom = null, Vector3? scaleTo = null)
    {
        if (!IsMoving || !_isDirectionChangable)
            return;

        IsMoving = false;

        if (!showFrom.HasValue) showFrom = CurrentDestination.ShowFrom;
        if (!scaleTo.HasValue) scaleTo = CurrentDestination.ScaleTo;
        _destinations.Clear();

        MoveTo(x, y, movingSpeed, (gO, result) =>
            {
                //TODO: FIX: Additional direction added to moving and speeddrop tap not working right

                ChangingDirection = false;
                if (movingCallback != null)
                    movingCallback(gO, result);

            }, showFrom, scaleTo, false, 1);

        CurrentDestination.ChangingDirection = true;

        ChangingDirection = true;
    }

    public void ChangeSpeed(float speed)
    {
        if (!(speed > 0) || !IsMoving) return;
        //foreach (var destination2D in _destinations)
        //{
        //    destination2D.Speed = new Vector2(speed, speed);
        //}
        CurrentDestination.Speed = new Vector2(speed, speed);
    }

    public void CancelMoving(bool all = false)
    {
        if (!IsMoving || CurrentDestination == null) return;

        if (all)
        {

            IsMoving = false;
            var callbacks = _destinations.Select(d => d.MovingCallback).ToList();
            _destinations.Clear();
            foreach (var cb in callbacks)
            {
                cb(gameObject, false);
            }
        }
        else
        {
            var callback = CurrentDestination.MovingCallback;
            _destinations.Remove(CurrentDestination);
            if (callback != null)
                callback(gameObject, false);
            if (_destinations.Count == 0) IsMoving = false;
        }
    }

    //public void CancelAllMoving()
    //{
    //    if (!isMoving || CurrentDestination == null) return;

    //    isMoving = false;
    //    var callbacks = _destinations.Select(d => d.MovingCallback).ToList();
    //    _destinations.Clear();
    //    foreach (var cb in callbacks)
    //    {
    //        cb(gameObject, false);
    //    }
    //}
}
