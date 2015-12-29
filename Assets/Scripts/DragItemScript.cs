using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class DragItemScript : MonoBehaviour
{
    private Point _touchedItem = null;
    private Vector3 touchedItemOriginalPosition;
    private Vector3 touchOriginalPosition;
    private MoveDirections? touchDirection;
    //private bool MouseActive = false;
    private float exchangeSpeedMultiple = 0;
    private MovingFinishedDelegate _cancelDraggingCallback = null;
    //private Int32 _ExchangingItems = 0;


    public Point TouchedItem
    {
        get { return _touchedItem; }
        private set
        {
            if (_touchedItem != null)
            {
                var pg = gameObject.GetComponent<IPlayground>();
                var gobj = pg.Items[_touchedItem.X][_touchedItem.Y] as GameObject;
                if (gobj != null)
                gobj.GetComponent<GameItem>().IsTouched = false;
            }
            if(value != null)
            {
                var pg = gameObject.GetComponent<IPlayground>();
                var gobj = pg.Items[value.X][value.Y] as GameObject;
                if (gobj != null)
                    gobj.GetComponent<GameItem>().IsTouched = true;
            }
            _touchedItem = value;
        }
    }

    public bool IsDragging
    {
        get { return TouchedItem != null; }
    }

    // Update is called once per frame
    void Update()
    {
        var touch = TouchActionAdapter.GetTouch();
        if (touch.Count == 0) return;

        if (PauseButtonScript.PauseMenuActive) return;

        var pg = gameObject.GetComponent<IPlayground>();

        if (pg == null || pg.IsGameOver || pg.IsMixing) return;

        Vector3 realTouchPosition;

        var touchPhase = TouchPhase.Canceled;

        if (_cancelDraggingCallback != null /*&& _ExchangingItems == 0*/)
            touchPhase = TouchPhase.Ended;
        else
            touchPhase = touch[0].Phase;
            

        realTouchPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch[0].OriginalPosition));
        //if (Application.platform != RuntimePlatform.WindowsEditor)
        //{
        //    if (Input.touchCount == 0) return;
        //    var touch = Input.GetTouch(0);
        //    touchPhase = touch.phase;
        //    realTouchPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touch.position));
        //    //LogFile.Message("touch.position: " + touch.position.x + " " + touch.position.y);
        //    //LogFile.Message("realTouchPosition: " + realTouchPosition.x + " " + realTouchPosition.y);
        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        touchPhase = TouchPhase.Began;
        //        MouseActive = true;
        //        //LogFile.Message("MouseButtonDown");
        //    }
        //    else if (Input.GetMouseButtonUp(0))
        //    {
        //        //if (Input.GetAxisRaw("Mouse X") == 0 && Input.GetAxisRaw("Mouse Y") == 0)
        //        //    touchPhase = TouchPhase.Stationary;
        //        //else
        //        touchPhase = TouchPhase.Ended;
        //        MouseActive = false;
        //        //LogFile.Message("MouseButtonUp");
        //    }
        //    else if (Input.GetMouseButton(0))
        //    {
        //        touchPhase = TouchPhase.Moved;
        //        //LogFile.Message("MouseButton");
        //    }
        //    if (!MouseActive && touchPhase != TouchPhase.Ended) return;
        //    LogFile.Message("touchPhase = " + touchPhase);
        //    realTouchPosition = gameObject.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //    //LogFile.Message("Input.mousePosition: " + Input.mousePosition.x + " " + Input.mousePosition.y);
        //    //LogFile.Message("realTouchPosition: " + realTouchPosition.x + " " + realTouchPosition.y);
        //}

        // -- Drag ------------------------------------------------
        switch (touchPhase)
        {
            case TouchPhase.Stationary:
                if (!(pg is ModeDropsPlayground)) return;
                var dropsPG = pg as ModeDropsPlayground;

                touchOriginalPosition = realTouchPosition;

                if (dropsPG.Items == null ||
                    dropsPG.Items[dropsPG.CurrentDroppingItem.X][dropsPG.CurrentDroppingItem.Y] == null
                    ||
                    dropsPG.Items[dropsPG.CurrentDroppingItem.X][dropsPG.CurrentDroppingItem.Y] == dropsPG.DisabledItem)
                    break;

                var tapItem = dropsPG.CurrentDroppingItem;

                var dropGO = (dropsPG.Items[tapItem.X][tapItem.Y] as GameObject);
                if (dropGO == null) break;

                var dropGIMS = dropGO.GetComponent<GameItemMovingScript>();

                if (dropGIMS == null || !dropGIMS.IsMoving) break;

                dropGIMS.ChangeSpeed(dropGIMS.CurrentDestination.Speed + 16);

                touchDirection = null;

                #region old
                //touchOriginalPosition = realTouchPosition;

                //for (var col = 0; col < pg.Items.Length; col++)
                //{
                //    for (var row = 0; row < pg.Items[col].Length; row++)
                //    {
                //        if (pg.Items[col][row] == null && pg.Items[col][row] == pg.DisabledItem) continue;
                //        var gobj = (pg.Items[col][row] as GameObject);
                //        if (gobj == null) continue;
                //        var gobjPosition = new Vector2(gobj.transform.localPosition.x, gobj.transform.localPosition.y);

                //        var gobjCollider = gobj.GetComponent<BoxCollider2D>();
                //        var half = gobjCollider.size.x/2;
                //        if ((!(realTouchPosition.x > gobjPosition.x - half)) ||
                //            (!(realTouchPosition.x < gobjPosition.x + half)) ||
                //            (!(realTouchPosition.y > gobjPosition.y - half)) ||
                //            (!(realTouchPosition.y < gobjPosition.y + half))) continue;

                //        var gims = gobj.GetComponent<GameItemMovingScript>();

                //        if (gims == null || !gims.IsMoving) continue;

                //        gims.ChangeSpeed(gims.CurrentDestination.Speed.x + 16);

                //        touchDirection = null;

                //        return;
                //    }
                //}
                #endregion

                break;
            case TouchPhase.Began:
                touchOriginalPosition = realTouchPosition;
                var found = false;
                for (var col = 0; col < pg.Items.Length; col++)
                {
                    if (!found)
                        for (var row = 0; row < pg.Items[col].Length; row++)
                        {
                            if (pg.Items[col][row] == null || pg.Items[col][row] == pg.DisabledItem) continue;
                            var gobj = (pg.Items[col][row] as GameObject);
                            if (gobj == null) continue;
                            //var gobjPosition = new Vector2(gobj.transform.localPosition.x, gobj.transform.localPosition.y);

                            var half = gobj.GetComponent<BoxCollider2D>().size.x / 2;
                            if ((!(realTouchPosition.x > gobj.transform.localPosition.x - half)) ||
                                (!(realTouchPosition.x < gobj.transform.localPosition.x + half)) ||
                                (!(realTouchPosition.y > gobj.transform.localPosition.y - half)) ||
                                (!(realTouchPosition.y < gobj.transform.localPosition.y + half))) continue;

                            var gims = gobj.GetComponent<GameItemMovingScript>();
                            var giss = gobj.GetComponent<GameItemScalingScript>();
                            var gi = gobj.GetComponent<GameItem>();
                            if (gims == null || gi == null || (!gi.IsDraggableWhileMoving && gims.IsMoving) || giss.isScaling || gi.MovingType == GameItemMovingType.Static) continue;

                            TouchedItem = new Point { X = col, Y = row };
                            touchedItemOriginalPosition = pg.GetCellCoordinates(col, row);
                            touchedItemOriginalPosition.z = gobj.transform.localPosition.z;

                            touchDirection = null;

                            //Vibration.Vibrate();
                            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.KeyPress, true);
                            found = true;
                            break;
                        }
                    else
                        break;
                }
                if (TouchedItem != null) return;
                if (TouchedItem == null && pg is ModeDropsPlayground)
                {
                    var movingItem = (pg as ModeDropsPlayground).CurrentDroppingItem;
                    if (movingItem == null) break;
                    var movingItemGameObject = pg.Items[movingItem.X][movingItem.Y] as GameObject;
                    if (
                        movingItemGameObject == null || !(movingItemGameObject.GetComponent<GameItemMovingScript>()
                            .CurrentDestination.Visible)) break;
                    TouchedItem = movingItem;
                    touchedItemOriginalPosition = pg.GetCellCoordinates(movingItem.X, movingItem.Y);
                }
                break;
            case TouchPhase.Moved:
                if (TouchedItem == null) break;
                var firstX = TouchedItem.X;
                var firstY = TouchedItem.Y;
                var gobj1 = pg.Items[firstX][firstY] as GameObject;
                if (gobj1 == null)
                    return;
                var giObject = gobj1.GetComponent<GameItem>();
                /*
                if (giObject != null &&
                    giObject.MovingType == GameItemMovingType.Static) return;

                var gobj1Gims = gobj1.GetComponent<GameItemMovingScript>();
                if (gobj1Gims.CurrentDestination != null && gobj1Gims.CurrentDestination.ChangingDirection)
                {
                    touchedItemOriginalPosition = new Vector3(gobj1.transform.localPosition.x,
                    gobj1.transform.localPosition.y,
                    gobj1.transform.localPosition.z);
                    touchOriginalPosition = touchedItemOriginalPosition;
                    return;
                }*/


                var deltaX = Math.Abs(realTouchPosition.x - touchOriginalPosition.x);
                var deltaY = Math.Abs(realTouchPosition.y - touchOriginalPosition.y);

                //if (deltaX < 0.0000001 && deltaY < 0.0000001) touchDirection = null;
                float deltaXYZ = 0;
                switch (giObject.MovingType)
                {
                    case GameItemMovingType.Standart:
                    case GameItemMovingType.StandartChangable:
                    case GameItemMovingType.StandartExchangable:
                        if (touchDirection != null)
                        {
                            switch(touchDirection)
                            {
                                case MoveDirections.Right:
                                case MoveDirections.Left:
                                deltaXYZ = deltaX;
                                break;
                                case MoveDirections.Up:
                                case MoveDirections.Down:
                                deltaXYZ = deltaY;
                                break;
                            }
                            break;
                        }
                        if (deltaX > deltaY && deltaX > pg.DeltaToMove)
                        {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                        (touchDirection = realTouchPosition.x > touchOriginalPosition.x
                                            ? MoveDirections.Right
                                            : MoveDirections.Left).Value))
                                {

                                    if (
                                        !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                            (touchDirection = realTouchPosition.y > touchOriginalPosition.y
                                                ? MoveDirections.Up
                                                : MoveDirections.Down).Value))
                                    {
                                        /*var movingItemGameObject = pg.Items[TouchedItem.X][TouchedItem.Y] as GameObject;
                                        if (touchDirection == MoveDirections.Down && pg is ModeDropsPlayground
                                            && movingItemGameObject != null && movingItemGameObject.GetComponent<GameItemMovingScript>()
                                                .IsMoving)*/ touchDirection = null;
                                        return;
                                    }
                                    deltaXYZ = deltaY;
                                    break;
                                }
                                deltaXYZ = deltaX;
                        }
                        else if (deltaX < deltaY && deltaY > pg.DeltaToMove)
                        {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                        (touchDirection = realTouchPosition.y > touchOriginalPosition.y
                                            ? MoveDirections.Up
                                            : MoveDirections.Down).Value))
                                {
                                    if (
                                        !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                            (touchDirection = realTouchPosition.x > touchOriginalPosition.x
                                                ? MoveDirections.Right
                                                : MoveDirections.Left).Value))
                                    {
                                    /*var movingItemGameObject = pg.Items[TouchedItem.X][TouchedItem.Y] as GameObject;
                                    if( touchDirection == MoveDirections.Down && pg is ModeDropsPlayground
                                             && movingItemGameObject != null &&
                                              movingItemGameObject.GetComponent<GameItemMovingScript>()
                                                  .IsMoving)*/ touchDirection = null;
                                        return;
                                    }
                                    deltaXYZ = deltaX;
                                    break;
                                }
                                deltaXYZ = deltaY;
                        }
                        break;
                    case GameItemMovingType.Diagonal:

                        deltaXYZ = deltaX > deltaY ? deltaX : deltaY;

                        if (realTouchPosition.y > touchOriginalPosition.y)
                        {
                            if (realTouchPosition.x > touchOriginalPosition.x)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                       (touchDirection = MoveDirections.UL).Value))
                                    return;

                            }
                            else
                            {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                       (touchDirection = MoveDirections.UR).Value))
                                    return;
                            }
                        }
                        else
                        {
                            if (realTouchPosition.x > touchOriginalPosition.x)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                       (touchDirection = MoveDirections.DL).Value))
                                    return;

                            }
                            else
                            {
                                if (
                                    !pg.IsItemMovingAvailable(TouchedItem.X, TouchedItem.Y,
                                       (touchDirection = MoveDirections.DR).Value))
                                    return;
                            }
                        }
                        break;
                    case GameItemMovingType.Free:
                        deltaXYZ = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                        break;
                }

                if (!touchDirection.HasValue) return;

                /*var secondX = touchedItem.X + (int)pg.AvailableMoveDirections[touchDirection.Value].x;
                var secondY = touchedItem.Y - (int)pg.AvailableMoveDirections[touchDirection.Value].y;
                if (!pg.isDisabledItemActive && pg.Items[secondX][secondY] != pg.DisabledItem)
                {
                    var gobjCHck = pg.Items[secondX][secondY] as GameObject;
                    if (gobjCHck == null) return;
                    var gims2 = gobjCHck.GetComponent<GameItemMovingScript>();
                    var gi = gobjCHck.GetComponent<GameItem>();
                    if (gims2 == null || gi == null || gims2.IsMoving) return;
                }*/
                //LogFile.Message("dELTA: " + deltaXYZ + "D2E: " + pg.DeltaToExchange);
                if (deltaXYZ > pg.DeltaToExchange)
                {
                    var secondX = TouchedItem.X + (int)pg.AvailableMoveDirections[touchDirection.Value].x;
                    var secondY = TouchedItem.Y - (int)pg.AvailableMoveDirections[touchDirection.Value].y;
                    //here we check touchDirection , select GameItem for exchange and call pg.GameItemsExchange(...)
                    exchangeSpeedMultiple += 1.7f;
                    LogFile.Message("From:" + firstX + " " + firstY, true);
                    LogFile.Message("To:" + secondX + " " + secondY, true);

                    var result = pg.TryMakeMove(firstX, firstY, secondX, secondY);
                    LogFile.Message("Result:" + result, true);
                    touchDirection = null;
                    TouchedItem = null;
                    /*if (gobj != null)
                    {
                        var gi = gobj.GetComponent<GameItem>();
                        gi.IsTouched = false;
                    }*/
                    float speed = 10 * exchangeSpeedMultiple;
                    //_ExchangingItems+=2;
                    if (result)
                    {
                        if (!pg.GameItemsExchange(firstX, firstY, secondX, secondY, speed, false, (obj, e) =>
                            {
                                //_ExchangingItems--;
                                if (_cancelDraggingCallback != null && pg.CallbacksCount == 0)
                                {
                                    var _cdc = _cancelDraggingCallback;
                                    _cancelDraggingCallback = null;
                                    _cdc(obj, true);
                                }
                            })) return;
                        //DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);
                        //var o = pg.Items[secondX][secondY] as GameObject;

                        //if (o != null)
                        //{
                        //    var cd = o.GetComponent<GameItemMovingScript>().CurrentDestination;
                        //    touchedItem = cd != null && o.GetComponent<GameItemMovingScript>().ChangingDirection//cd.ChangingDirection
                        //        ? new Point {X = secondX, Y = secondY}
                        //        : null;
                        //}
                        //else
                        LogFile.Message("After exchange From:" + firstX + " " + firstY, true);
                        LogFile.Message("After exchange To:" + secondX + " " + secondY, true);

                    }
                    else
                    {
                        pg.GameItemsExchange(firstX, firstY, secondX, secondY, speed, true, (obj, e) =>
                        {
                            //_ExchangingItems--;
                            if (_cancelDraggingCallback != null && pg.CallbacksCount == 0)
                            {
                                var _cdc = _cancelDraggingCallback;
                                _cancelDraggingCallback = null;
                                _cdc(obj, true);
                            }
                        });
                        DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Fault, false);
                        //var gi = gobj1.GetComponent<GameItem>();
                        
                    }
                }
                else
                {
                    exchangeSpeedMultiple = 0;

                    var gims = gobj1.GetComponent<GameItemMovingScript>();
                    if (!gims.IsMoving)
                    {
                        switch (giObject.MovingType)
                        {
                            case GameItemMovingType.Standart:
                            case GameItemMovingType.StandartChangable:
                            case GameItemMovingType.Diagonal:
                                gims.transform.localPosition = new Vector3(touchedItemOriginalPosition.x + pg.AvailableMoveDirections[touchDirection.Value].x * deltaXYZ,
                                    touchedItemOriginalPosition.y + pg.AvailableMoveDirections[touchDirection.Value].y * deltaXYZ, touchedItemOriginalPosition.z);
                                break;
                            case GameItemMovingType.Free:
                                gims.transform.localPosition = new Vector3(realTouchPosition.x,
                                    realTouchPosition.y, touchedItemOriginalPosition.z);
                                break;
                        }
                    }
                    //var gi = gobj1.GetComponent<GameItem>();
                    //if (gi != null && gi.IsDraggableWhileMoving)
                    //    touchDirection = null;
                }
                break;
            case TouchPhase.Ended:
                exchangeSpeedMultiple = 0;
                if (TouchedItem != null)
                {
                    if (pg.Items == null || pg.Items[TouchedItem.X][TouchedItem.Y] == null ||
                        pg.Items[TouchedItem.X][TouchedItem.Y] == pg.DisabledItem)
                    {
                        TouchedItem = null;
                        return;
                    };
                    var gobj = pg.Items[TouchedItem.X][TouchedItem.Y] as GameObject;
                    if (gobj != null)
                    {
                        if (touchDirection == null)
                        {
                            var gims = gobj.GetComponent<GameItemMovingScript>();
                            if (gims.IsMoving)
                            {
                                TouchedItem = null;
                                gims.ChangeSpeed(gims.CurrentDestination.Speed + 16);
                                //gobj.GetComponent<GameItem>().IsTouched = false;
                                return;
                            }
                        }
                        //gobj.GetComponent<GameItem>().IsTouched = false;
                        //var gi = gobj.GetComponent<GameItem>();
                        //if (gi != null && gi.IsDraggableWhileMoving)
                        touchDirection = null;
                        var _cdc = _cancelDraggingCallback;
                        _cancelDraggingCallback = null;
                        pg.RevertMovedItem(TouchedItem.X, TouchedItem.Y, _cdc);
                    }
                    //LogFile.Message("TouchPhase.Ended");
                    //if (Math.Abs(realTouchPosition.x - touchedItemOriginalPosition.x) > 0.000000000000000001 &&
                    // Math.Abs(realTouchPosition.y - touchedItemOriginalPosition.y) > 0.000000000000000001)  
                    TouchedItem = null;
                }
                break;
        }
    }

    public void CancelDragging(MovingFinishedDelegate callback)
    {
        if (_cancelDraggingCallback != null) return;
        _cancelDraggingCallback = callback;
    }
}
