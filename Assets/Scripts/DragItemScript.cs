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
    private Point touchedItem;
    private Vector3 touchedItemOriginalPosition;
    private Vector3 touchOriginalPosition;
    private MoveDirections? touchDirection;
    //private bool MouseActive = false;
    private float exchangeSpeedMultiple = 0;

    // Update is called once per frame
    void Update()
    {
        var pg = gameObject.GetComponent<IPlayground>();

        if (pg == null || pg.IsGameOver) return;

        Vector3 realTouchPosition;

        var touchPhase = TouchPhase.Canceled;

        var touch = TouchActionAdapter.GetTouch();
        if (touch.Count == 0) return;
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

                dropGIMS.ChangeSpeed(dropGIMS.CurrentDestination.Speed.x + 16);

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

                for (var col = 0; col < pg.Items.Length; col++)
                {
                    for (var row = 0; row < pg.Items[col].Length; row++)
                    {
                        if (pg.Items[col][row] == null || pg.Items[col][row] == pg.DisabledItem) continue;
                        var gobj = (pg.Items[col][row] as GameObject);
                        if (gobj == null) continue;
                        var gobjPosition = new Vector2(gobj.transform.localPosition.x, gobj.transform.localPosition.y);

                        var gobjCollider = gobj.GetComponent<BoxCollider2D>();
                        var half = gobjCollider.size.x / 2;
                        if ((!(realTouchPosition.x > gobjPosition.x - half)) ||
                            (!(realTouchPosition.x < gobjPosition.x + half)) ||
                            (!(realTouchPosition.y > gobjPosition.y - half)) ||
                            (!(realTouchPosition.y < gobjPosition.y + half))) continue;

                        var gims = gobj.GetComponent<GameItemMovingScript>();
                        var gi = gobj.GetComponent<GameItem>();
                        if (gims == null || gi == null || (!gi.IsDraggableWhileMoving && gims.IsMoving)) continue;

                        touchedItem = new Point { X = col, Y = row };
                        touchedItemOriginalPosition = pg.GetCellCoordinates(col, row);

                        touchDirection = null;

                        Vibration.Vibrate(10);
                        break;
                    }
                }
                if (touchedItem != null) return;
                if (touchedItem == null && pg is ModeDropsPlayground)
                {
                    var movingItem = (pg as ModeDropsPlayground).CurrentDroppingItem;
                    if (movingItem == null) break;
                    var movingItemGameObject = pg.Items[movingItem.X][movingItem.Y] as GameObject;
                    if (
                        movingItemGameObject == null || !(movingItemGameObject.GetComponent<GameItemMovingScript>()
                            .CurrentDestination.Visible)) break;
                    touchedItem = movingItem;
                    touchedItemOriginalPosition = pg.GetCellCoordinates(movingItem.X, movingItem.Y);
                }
                break;
            case TouchPhase.Moved:
                if (touchedItem == null) break;
                var firstX = touchedItem.X;
                var firstY = touchedItem.Y;
                var gobj1 = pg.Items[firstX][firstY] as GameObject;
                if (gobj1 == null)
                    return;
                var giObject = gobj1.GetComponent<GameItem>();
                if (giObject != null &&
                    giObject.MovingType == GameItemMovingType.Static /*|| giObject.Type == GameItemType._DropDownItem)*/) return;

                var gobj1Gims = gobj1.GetComponent<GameItemMovingScript>();
                if (gobj1Gims.CurrentDestination != null && gobj1Gims.CurrentDestination.ChangingDirection)
                {
                    touchedItemOriginalPosition = new Vector3(gobj1.transform.localPosition.x,
                    gobj1.transform.localPosition.y,
                    gobj1.transform.localPosition.z);
                    touchOriginalPosition = touchedItemOriginalPosition;
                    //gobj1.GetComponent<GameItemMovingScript>().CurrentDestination.ChangingDirection = false;
                    return;
                }


                var deltaX = Math.Abs(realTouchPosition.x - touchOriginalPosition.x);
                var deltaY = Math.Abs(realTouchPosition.y - touchOriginalPosition.y);

                //if (deltaX < 0.0000001 && deltaY < 0.0000001) touchDirection = null;
                float deltaXYZ = 0;
                switch (giObject.MovingType)
                {
                    case GameItemMovingType.Standart:

                        if (deltaX > deltaY)
                        {
                            deltaXYZ = deltaX;
                            if (deltaXYZ > pg.DeltaToMove)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                        (touchDirection = realTouchPosition.x > touchOriginalPosition.x
                                            ? MoveDirections.Right
                                            : MoveDirections.Left).Value))
                                {

                                    if (
                                        !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                            (touchDirection = realTouchPosition.y > touchOriginalPosition.y
                                                ? MoveDirections.Up
                                                : MoveDirections.Down).Value))
                                    {
                                        var movingItemGameObject = pg.Items[touchedItem.X][touchedItem.Y] as GameObject;
                                        if (touchDirection == MoveDirections.Down && pg is ModeDropsPlayground
                                            && movingItemGameObject != null && movingItemGameObject.GetComponent<GameItemMovingScript>()
                                                .IsMoving) touchDirection = null;
                                        return;
                                    }
                                    deltaXYZ = deltaY;
                                }
                            }
                        }
                        else if (deltaX < deltaY)
                        {
                            deltaXYZ = deltaY;
                            if (deltaXYZ > pg.DeltaToMove)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                        (touchDirection = realTouchPosition.y > touchOriginalPosition.y
                                            ? MoveDirections.Up
                                            : MoveDirections.Down).Value))
                                {
                                    var movingItemGameObject = pg.Items[touchedItem.X][touchedItem.Y] as GameObject;
                                    var res = touchDirection == MoveDirections.Down && pg is ModeDropsPlayground
                                              && movingItemGameObject != null &&
                                              movingItemGameObject.GetComponent<GameItemMovingScript>()
                                                  .IsMoving;
                                    if (res){ touchDirection = null;
                                        return;
                                    }
                                    if (
                                        !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                            (touchDirection = realTouchPosition.x > touchOriginalPosition.x
                                                ? MoveDirections.Right
                                                : MoveDirections.Left).Value))
                                    {
                                        //if (res) touchDirection = null;
                                        return;
                                    }

                                    deltaXYZ = deltaX;
                                }

                            }
                        }
                        break;
                    case GameItemMovingType.Diagonal:

                        deltaXYZ = deltaX > deltaY ? deltaX : deltaY;

                        if (realTouchPosition.y > touchOriginalPosition.y)
                        {
                            if (realTouchPosition.x > touchOriginalPosition.x)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                       (touchDirection = MoveDirections.UL).Value))
                                    return;

                            }
                            else
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                       (touchDirection = MoveDirections.UR).Value))
                                    return;
                            }
                        }
                        else
                        {
                            if (realTouchPosition.x > touchOriginalPosition.x)
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                       (touchDirection = MoveDirections.DL).Value))
                                    return;

                            }
                            else
                            {
                                if (
                                    !pg.IsItemMovingAvailable(touchedItem.X, touchedItem.Y,
                                       (touchDirection = MoveDirections.DR).Value))
                                    return;
                            }
                        }
                        break;
                    case GameItemMovingType.Free:
                        deltaXYZ = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                        break;
                    case GameItemMovingType.Static:
                        return;
                }

                if (!touchDirection.HasValue) return;

                var secondX = touchedItem.X + (int)pg.AvailableMoveDirections[touchDirection.Value].x;
                var secondY = touchedItem.Y - (int)pg.AvailableMoveDirections[touchDirection.Value].y;
                if (!pg.isDisabledItemActive && pg.Items[secondX][secondY] != pg.DisabledItem)
                {
                    var gobjCHck = pg.Items[secondX][secondY] as GameObject;
                    if (gobjCHck == null) return;
                    var gims2 = gobjCHck.GetComponent<GameItemMovingScript>();
                    var gi = gobjCHck.GetComponent<GameItem>();
                    if (gims2 == null || gi == null || gims2.IsMoving) return;
                }
                //LogFile.Message("dELTA: " + deltaXYZ + "D2E: " + pg.DeltaToExchange);
                if (deltaXYZ > pg.DeltaToExchange)
                {
                    //here we check touchDirection , select GameItem for exchange and call pg.GameItemsExchange(...)

                    exchangeSpeedMultiple += 1.7f;
                    LogFile.Message("From:" + firstX + " " + firstY);
                    LogFile.Message("To:" + secondX + " " + secondY);

                    var result = pg.TryMakeMove(firstX, firstY, secondX, secondY);
                    LogFile.Message("Result:" + result);

                    if (result)
                    {
                        if (!pg.GameItemsExchange(firstX, firstY, ref secondX, ref secondY, 10 * exchangeSpeedMultiple, false)) return;
                        //var o = pg.Items[secondX][secondY] as GameObject;

                        //if (o != null)
                        //{
                        //    var cd = o.GetComponent<GameItemMovingScript>().CurrentDestination;
                        //    touchedItem = cd != null && o.GetComponent<GameItemMovingScript>().ChangingDirection//cd.ChangingDirection
                        //        ? new Point {X = secondX, Y = secondY}
                        //        : null;
                        //}
                        //else
                        LogFile.Message("After exchange From:" + firstX + " " + firstY);
                        LogFile.Message("After exchange To:" + secondX + " " + secondY);

                    }
                    else
                    {
                        pg.GameItemsExchange(firstX, firstY, ref secondX, ref secondY, 10 * exchangeSpeedMultiple, true);
                        //var gi = gobj1.GetComponent<GameItem>();
                        
                    }
                    touchDirection = null;
                    touchedItem = null;
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
                if (touchedItem != null)
                {
                    if (pg.Items == null || pg.Items[touchedItem.X][touchedItem.Y] == null ||
                        pg.Items[touchedItem.X][touchedItem.Y] == pg.DisabledItem)
                    {
                        touchedItem = null;
                        break;
                    };
                    var gobj = pg.Items[touchedItem.X][touchedItem.Y] as GameObject;
                    if (gobj != null)
                    {
                        if (touchDirection == null)
                        {
                            var gims = gobj.GetComponent<GameItemMovingScript>();
                            if (gims.IsMoving)
                            {
                                gims.ChangeSpeed(gims.CurrentDestination.Speed.x + 16);
                                touchedItem = null;
                                return;
                            }
                        }

                        //var gi = gobj.GetComponent<GameItem>();
                        //if (gi != null && gi.IsDraggableWhileMoving)
                        touchDirection = null;

                        pg.RevertMovedItem(touchedItem.X, touchedItem.Y);
                    }
                    //LogFile.Message("TouchPhase.Ended");
                    //if (Math.Abs(realTouchPosition.x - touchedItemOriginalPosition.x) > 0.000000000000000001 &&
                    // Math.Abs(realTouchPosition.y - touchedItemOriginalPosition.y) > 0.000000000000000001)  
                }
                touchedItem = null;
                break;
        }
    }
}
