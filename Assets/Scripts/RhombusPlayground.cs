﻿using System;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using System.Collections.Generic;
namespace Assets.Scripts
{
    class RhombusPlayground : SquarePlayground
    {
		public override String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<RhombusPlayground>(); } }

        public override string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<RhombusPlayground>(); } }

        public override bool isBackgroundInSprite { get { return true; } }

        public RhombusPlayground()
            : base(new Dictionary<MoveDirections, Vector2>
            {
                { MoveDirections.Up, new Vector2(0, 2) },
                { MoveDirections.Down, new Vector2(0, -2) },
                { MoveDirections.Left, new Vector2(-2, 0) },
                { MoveDirections.Right, new Vector2(2, 0) },
                { MoveDirections.UL, new Vector2(-2, 2) },
                { MoveDirections.UR, new Vector2(2, 2) },
                { MoveDirections.DL, new Vector2(-2, -2) },
                { MoveDirections.DR, new Vector2(2, -2) },
            })
        {
            //InitialMoveTimerMultiple = 26;
            ScaleMultiplyer = 4.1f;
        }


        public override IEnumerable<Line> LinesWithItem(IEnumerable<Line> lines, int currentX, int currentY)
        {
            var ret = new List<Line>();
            foreach (var l in lines)
            {
                if (l.Y1 > l.Y2)
                {
                      if(l.X1 <= currentX && l.X2 >= currentX && l.Y1 >= currentY && l.Y2 <= currentY )
                          ret.Add(l);
                }
                else
                {
                    if (l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY)
                        ret.Add(l);
                }
            }
            return ret;
        }
        
        public override Vector3 GetCellCoordinates(int col, int row)
        {
            var halfItem = GameItemSize * 0.9325 / 2 + GameItemSize * 0.227;
            var roundedX = Math.Round(InitialGameItem.X + col * halfItem, 2);
            var roundedY = Math.Round(InitialGameItem.Y - row * halfItem, 2);
            return new Vector3((float)roundedX, (float)roundedY, InitialGameItem.Z);
        }
        
        public override bool MatchPattern(GameItemType itemType, Point firstItem, Point secondItem, IEnumerable<Point> pointForCheck)
        {
            var mainType = MatchType(firstItem.X + secondItem.X, firstItem.Y + secondItem.Y, itemType);
            if (!mainType) return false;

            var lineGenerationPoints = pointForCheck.Where(point => MatchType(firstItem.X + point.X, firstItem.Y + point.Y, itemType)).ToList();
            if (!lineGenerationPoints.Any()) return false;

            foreach (var lineGenerationPoint in lineGenerationPoints)
            {
                int sp1X;
                int sp1Y;
                if (secondItem.Y < 0)
                {
                    LogFile.Message("secondItem.Y < 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem "
                        + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: " + lineGenerationPoint.X + " " + lineGenerationPoint.Y, true);
                    if (secondItem.Y < -1)
                        {
                            sp1X = firstItem.X + (secondItem.X > 0 ? 1 : -1);
                            sp1Y = firstItem.Y - 1;
                        }
                    else
                        {
                            sp1X = firstItem.X + (lineGenerationPoint.Y > -2 ? -1 : 2);
                            sp1Y = firstItem.Y + (lineGenerationPoint.Y > -2 ? 1 : -2);
                        }
                }
                else if (secondItem.Y > 0)
                {
                    LogFile.Message("secondItem.Y > 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem " + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: "
                        + lineGenerationPoint.X + " " + lineGenerationPoint.Y, true);
                    if (secondItem.Y > 1)
                        {
                            sp1X = firstItem.X + (secondItem.X > 0 ? 1 : -1);
                            sp1Y = firstItem.Y + 1;
                        }
                    else
                        {
                            sp1X = firstItem.X + (lineGenerationPoint.Y < 2 ? -1 : 2);
                            sp1Y = firstItem.Y + (lineGenerationPoint.Y < 2 ? -1 : 2);
                        }
                }
                else
                    continue;
                if ((sp1X < 0) || (sp1X >= FieldSize) || (sp1Y < 0) || (sp1Y >= FieldSize) || Items[sp1X][sp1Y] == null || Items[sp1X][sp1Y] == DisabledItem)
                    continue;
                SelectedPoint1 = new Point
                {
                    X = sp1X,
                    Y = sp1Y
                };

                var diffX = Math.Abs(SelectedPoint1Coordinate.X - (firstItem.X + lineGenerationPoint.X));
                var diffY = Math.Abs(SelectedPoint1Coordinate.Y - (firstItem.Y + lineGenerationPoint.Y));
                if(!((diffX == 0 && diffY == 2) || (diffX == 2 && diffY == 0)))
                    continue;
                if (Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != null && Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != DisabledItem)
                {
                    var selectedObject = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                    if (selectedObject != null)
                    {
                        var gi = selectedObject.GetComponent<GameItem>();
                        if (gi.MovingType == GameItemMovingType.Static || gi.Type == GameItemType.NullItem)
                            continue;
                    }
                }
                else continue;
                SelectedPoint2 = new Point { X = firstItem.X + lineGenerationPoint.X, Y = firstItem.Y + lineGenerationPoint.Y };
                return true;
            }

            return false;
        }
        
        public override IList<Line> GetAllLines()
        {
            LogFile.Message("Finding lines...", true);
            var list = new List<Line>();
            //Vertical
            var startRow = FieldSize / 2;
            for (var startCol = 0; startRow >= 0; startCol++, startRow--)
            {
                var currentCol = startCol;
                for (var row = startRow; row < FieldSize && currentCol < FieldSize; row++)
                {
                    if (Items[currentCol][row] == null || Items[currentCol][row] == DisabledItem) continue;
                    var match = CheckForLine(currentCol, row, LineOrientation.Vertical);
                    if (match > 2)
                    {
                        var line = new Line
                        {
                            X1 = currentCol,
                            Y1 = row,
                            X2 = currentCol + match - 1,
                            Y2 = row + match - 1,
                            Orientation = LineOrientation.Vertical
                        };
                        list.Add(line);
                        LogFile.Message(match + " " + line, true);
                        currentCol += match - 1;
                        row += match - 1;
                    }
                    currentCol++;
                }
            }
            //Horizontal
            startRow = FieldSize / 2;
            for (var startCol = 0; startRow < FieldSize; startCol++, startRow++)
            {
                var currentCol = startCol;
                for (var row = startRow; row >= 0 && currentCol < FieldSize; row--)
                {
                    if (Items[currentCol][row] == null || Items[currentCol][row] == DisabledItem) continue;
                    var match = CheckForLine(currentCol, row, LineOrientation.Horizontal);
                    if (match > 2)
                    {
                        var line = new Line
                        {
                            X1 = currentCol,
                            Y1 = row,
                            X2 = currentCol + match - 1,
                            Y2 = row - match + 1,
                            Orientation = LineOrientation.Horizontal
                        };
                        list.Add(line);
                        LogFile.Message(match + " " + line, true);
                        currentCol += match;
                        row -= match - 1;
                    }
                    currentCol++;
                }
            }
            LogFile.Message("Return lines: " + list.Count, true);
            return list;
        }

        public override int CheckForLine(int x, int y, LineOrientation orientation, bool includeMovingItemsInLine = true)
        {
            var count = 1;
            if (Items[x][y] == null || Items[x][y] == DisabledItem) return count;
            switch (orientation)
            {
                // ->\\     |
                //  ->\\    V
                //   ->\\
                case LineOrientation.Vertical:
                    for (var i = 1; x + i < FieldSize && y + i < FieldSize; i++)
                    {
                        if (Items[x + i][y + i] == null || Items[x + i][y + i] == DisabledItem) break;
                        if (includeMovingItemsInLine)
                        {
                            var goItem = (Items[x + i][y + i] as GameObject);
                            if (goItem != null && ((goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)
                            || goItem.GetComponent<GameItemScalingScript>().isScaling)) return 1;
                        }
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x + i][y + i] as GameObject;
                            if (gobj2 != null)
                            {
                                var gi2 = gobj2.GetComponent<GameItem>();
                                if (gi1.Type != gi2.Type)
                                    break;
                            }
                        }
                        count++;
                    }
                    break;
                // ->  //
                // -> //   ^
                // ->//    |
                case LineOrientation.Horizontal:
                    for (var i = 1; x + i < FieldSize && y - i >= 0; i++)
                    {
                        if (Items[x + i][y - i] == null || Items[x + i][y - i] == DisabledItem) break;
                        if (includeMovingItemsInLine)
                        {
                            var goItem = (Items[x + i][y - i] as GameObject);
                            if (goItem != null && ((goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)
                            || goItem.GetComponent<GameItemScalingScript>().isScaling)) return 1;
                        }
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x + i][y - i] as GameObject;
                            if (gobj2 != null)
                            {
                                var gi2 = gobj2.GetComponent<GameItem>();
                                if (gi1.Type != gi2.Type)
                                    break;
                            }
                        }
                        count++;
                    }
                    break;
            }
            return count;
        }
        
        public override int ClearChains()
        {
            if (IsGameOver) return -1;

            SelectedPoint1 = null;
            SelectedPoint2 = null;

            var lines = GetAllLines();
            if (lines.Count == 0)
            {
                if (RaiseMaxInitialElement && CallbacksCount == 0)
                {
                    RaiseMaxInitialElement = false;
                    MaxInitialElementTypeRaisedActions();
                }
                ChainCounter = 0;
                if (HintTimeCounter < 0) HintTimeCounter = 0;
                if (!RemoveAdditionalItems() && CallbacksCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);
                    GenerateField(false, true);
                }
                UpdateTime();
                return 0;
            }

            HintTimeCounter = -1;
            _isDropDone = false;
            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            var repeatForLine = -1;
            var raiseMaxIEtype = true;
            int toObjX = 0, toObjY = 0;
            var toCell = Vector3.zero;

            while (l != null && !IsGameOver)
            {
                var pointsMultiple = 1;
                
                //Vertical
                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;
                    if (repeatForLine == -1)
                    {
                        toObjX = l.X2 - (l.X2 - l.X1) / 2;
                        toObjY = l.Y2 - (l.Y2 - l.Y1) / 2;
                        for (int i = l.X2, j = l.Y2; i >= l.X1 && j >= l.Y1; i--, j--)
                        {
                            var lwi = LinesWithItem(lines, i, j);
                            if (lwi.Count() <= 1) continue;
                            
                            toObjX = i;
                            toObjY = j;
                            repeatForLine = lines.IndexOf(lwi.FirstOrDefault(line => line != l));
                        }
                        toCell = GetCellCoordinates(toObjX, toObjY);
                    }
                    else
                    {
                        raiseMaxIEtype = false;
                        repeatForLine = -1;
                    }

                    for (int i = l.X2, j = l.Y2; i >= l.X1 && j >= l.Y1; i--, j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem || (i == toObjX && j == toObjY))
                            continue;

                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();

                        var cX = i;
                        var cY = j;
                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                                {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(toCell.x, toCell.y, Game.StandartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;

                                    DestroyGameItem((GameObject)item);
                                });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, toCell.y, Game.StandartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;

                                DestroyGameItem((GameObject)item);
                            });
                        }
                    }
                }


                //Horizontal
                else
                {
                    pointsMultiple += l.X2 - l.X1 - 2;

                    if (repeatForLine == -1)
                    {
                        toObjX = l.X2 - (l.X2 - l.X1) / 2;
                        toObjY = l.Y2 - (l.Y2 - l.Y1) / 2;
                        for (int i = l.X1, j = l.Y1; i <= l.X2 && j >= l.Y2; i++, j--)
                        {
                            var lwi = LinesWithItem(lines, i, j);
                            if (lwi.Count() <= 1) continue;
                            
                            toObjX = i;
                            toObjY = j;
                            repeatForLine = lines.IndexOf(lwi.FirstOrDefault(line => line != l));
                        }
                        toCell = GetCellCoordinates(toObjX, toObjY);
                    }
                    else
                    {
                        raiseMaxIEtype = false;
                        repeatForLine = -1;
                    }


                    for (int i = l.X1, j = l.Y1; i <= l.X2 && j >= l.Y2; i++, j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem || (i == toObjX && j == toObjY))
                            continue;

                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = 
                            new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();

                        var cX = i;
                        var cY = j;
                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                                {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(toCell.x, toCell.y, Game.StandartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;

                                    DestroyGameItem((GameObject)item); 
                                });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, toCell.y, Game.StandartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;

                                DestroyGameItem((GameObject)item);
                            });
                        }
                    }
                }

                var toGobj = Items[toObjX][toObjY] as GameObject;
                if (toGobj != null)
                {
                    var toGi = toGobj.GetComponent<GameItem>();
                    if (raiseMaxIEtype && toGi.Type > MaxInitialElementType)
                        MaxInitialElementType = toGi.Type;
                    var newgobjtype = toGi.Type != GameItemType._2x ? toGi.Type + 1 : GameItemType._2x;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell, Vector3.one);
                        //new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f));
                    if (toGobj.GetComponent<GameItemMovingScript>().IsMoving)
                        CallbacksCount--;
                    DestroyGameItem(toGobj);
                        Items[toObjX][toObjY] = newgobj;
                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                        if (newgobjtype <= MaxInitialElementType)
                        {
                            pointsBank += points;
                            LabelShowing.ShowScalingLabel(newgobj, true, false,
                                "+" + points, GameColors.ItemsColors[newgobjtype], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, null, 0, newgobjtype);
                        }
                        else
                        {
                            pointsBank += 2 * points;
                            LabelShowing.ShowScalingLabel(newgobj, true, false,
                                "+" + points + "x2", GameColors.ItemsColors[newgobjtype], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, null, 0, newgobjtype);
                        }
                }
                lines.Remove(l);
                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected", true);
                l = repeatForLine == -1 ? lines.FirstOrDefault() : lines[repeatForLine - 1];

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                pointsBank *= linesCount;
                ChainCounter++;
                RaisePoints(pointsBank * ChainCounter);

                pointsBank = 0;
                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected", true);

            return linesCount;
        }

        public override void Drop()
        {
            if (Items == null) return;

            var generateAfterDrop = true;

            var rhombusDropSpeed = Game.StandartItemSpeed - 4;

            for (var row = FieldSize - 1; row > 0; row--)
            {
                for (var col = 0; col < FieldSize; col++)
                {
                    if (Items[col][row] != null || Items[col][row] == DisabledItem)
                        continue;
                    var side = col % 2 == 1 ? 1 : -1;
                    var downItemRow = row - 1;
                    var downItemCol = col + side;
                    if (downItemCol < 0 || downItemCol >= FieldSize || Items[downItemCol][downItemRow] == DisabledItem || Items[downItemCol][downItemRow] == null || 
                        (Items[downItemCol][downItemRow] as GameObject).GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        downItemCol = col - side;
                    if (downItemCol < 0 || downItemCol >= FieldSize)
                        continue;
                    if (row == 0)
                    {
                        break;
                    }
                    while (downItemRow >= 0)
                    {
                        var gobj = Items[downItemCol][downItemRow] as GameObject;
                        if (Items[downItemCol][downItemRow] == null || Items[downItemCol][downItemRow] == DisabledItem)
                            break;
                        if (gobj != null && (!AreStaticItemsDroppable && gobj.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                            downItemRow--;
                        else
                        {
                            generateAfterDrop = false;
                            if (gobj != null)
                            {
                                var gims = gobj.GetComponent<GameItemMovingScript>();

                                if (gims.IsMoving || gobj.GetComponent<GameItem>().IsTouched)
                                    break;

                                Items[col][row] = Items[downItemCol][downItemRow];
                                Items[downItemCol][downItemRow] = null;
                                CallbacksCount++;
                                var coord = GetCellCoordinates(col, row);
                                gims.MoveTo(coord.x, coord.y, rhombusDropSpeed, (item, result) =>
                                {
                                    CallbacksCount--;
                                    if (!result) return;
                                    /*if (_clearAfterDropDone && CallbacksCount == 0)
                                    {
                                        _clearAfterDropDone = false;
                                        ClearChains();
                                    }*/
                                    LogFile.Message("New item droped Items[" + downItemCol + "][" + downItemRow + "] DC: " + DropsCount, true);
                                });
                            }
                            break;
                        }
                    }
                }
            }
            if (!generateAfterDrop) return;
            _isDropDone = true;
            GenerateField(true);
        }

        public override void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false, LabelAnimationFinishedDelegate callback = null)
        {
            LogFile.Message("Generating field...", true);
            if (!mixCurrent)
            {
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnX = i % 2 == 1 ? -i : i;
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        var deniedList = new List<GameItemType>();
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        switch (Game.Difficulty)
                        {
                            case DifficultyLevel._medium:
                                if (ToMoveItemsCount < MaxAdditionalItemsCount && j < 4)
                                {
                                        Items[i][j] = GenerateGameItem(GameItemType._ToMoveItem, i, j, new Vector2(generateOnX, i), null, false, 12 + i * 2);//may be calculate speed or generateOn vector in another way
                                        ToMoveItemsCount++;
                                        generateOnX++;
                                        continue;
                                }
                                break;
                        }
                        if (completeCurrent)
                        {
                            LogFile.Message("New gameItem need to i:" + i + "j: " + j, true);
                            Items[i][j] = GenerateGameItem(i, j, null, new Vector2(generateOnX, i), false, 12 + i * 2);//may be calculate speed or generateOn vector in another way
                            continue;
                        }
                        //Horizontal before
                        if (i >= 2 && j < FieldSize - 2 && CheckForLine(i - 2, j + 2, LineOrientation.Horizontal, false) > 1)
                        {
                            var o = Items[i - 1][j + 1] as GameObject;
                            if (o != null)
                                deniedList.Add(o.GetComponent<GameItem>().Type);
                        }
                        //Horizontal after
                        if (i < FieldSize - 2 && j > 1 && CheckForLine(i + 1, j - 1, LineOrientation.Horizontal, false) > 1)
                        {
                            var gameObject1 = Items[i + 1][j - 1] as GameObject;
                            if (gameObject1 != null)
                                deniedList.Add(gameObject1.GetComponent<GameItem>().Type);
                        }
                        //Vertical before
                        if (i > 1 && j > 1 && CheckForLine(i - 2, j - 2, LineOrientation.Vertical, false) > 1)
                        {
                            var o1 = Items[i - 1][j - 1] as GameObject;
                            if (o1 != null)
                                deniedList.Add(o1.GetComponent<GameItem>().Type);
                        }
                        //Vertical after
                        if (i < FieldSize - 2 && j < FieldSize - 2 && CheckForLine(i + 1, j + 1, LineOrientation.Vertical, false) > 1)
                        {
                            var gameObject2 = Items[i + 1][j + 1] as GameObject;
                            if (gameObject2 != null)
                                deniedList.Add(gameObject2.GetComponent<GameItem>().Type);
                        }
                        Items[i][j] = GenerateGameItem(i, j, deniedList, new Vector2(generateOnX, i));
                    }
                }
                SavedataHelper.SaveData(SavedataObject);
            }
            else
            {
                    LogFile.Message("Mix field...", true);
                    var o = Instantiate(LabelShowing.LabelPrefab) as GameObject;
                    if (o == null) return;
                    if (!onlyNoMovesLabel)
                    {
                        PlaygroundProgressBar.ProgressBarRun = false;
                        callback = () =>
                        {
                            MixField();
                        };
                    }
                    var noMovesLabel = o.GetComponent<LabelShowing>();
                    noMovesLabel.ShowScalingLabel(new Vector3(0, -2, -4),
                             LanguageManager.Instance.GetTextValue("NoMovesTitle"), GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark,
                             LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, callback, true);
                    //if (!onlyNoMovesLabel) MixField();
                
            }
        }

        public override bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            if (!AvailableMoveDirections.ContainsKey(mdir)) return false;
            if (col < 0 || row < 0 || col >= FieldSize || row >= FieldSize)
                return false;
            var gameObject1 = Items[col][row] as GameObject;
            if (gameObject1 == null || gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) return false;
            var direction = AvailableMoveDirections[mdir];
            var newX = col + (int)direction.x;
            var newY = row - (int)direction.y;
            GameObject gobj;
            return newX >= 0 && newX <= (FieldSize - 1) && newY >= 0 && newY <= (FieldSize - 1) && Items[newX][newY] != DisabledItem && (gobj = Items[newX][newY] as GameObject)!= null &&
                                                                           gobj.GetComponent<GameItem>().MovingType != GameItemMovingType.Static && !gobj.GetComponent<GameItemMovingScript>().IsMoving;
        }
        
        public override bool IsPointInLine(int col, int row)
        {
            var verticalCount = 0;
            
            //Vertical before
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            for (int i = col - 1, j = row - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (Items[i][j] == null || Items[i][j] == DisabledItem) continue;
                var gobj2 = Items[i][j] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    verticalCount++;
                else
                    break;
            }
            
            //Vertical after
            if (verticalCount < 3)
            {
                for (int i = col + 1, j = row + 1; i < FieldSize && j < FieldSize; i++, j++)
                {
                    if (Items[i][j] == null || Items[i][j] == DisabledItem) continue;
                    var gobj2 = Items[i][j] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        verticalCount++;
                    else
                        break;
                }
            }
            else
                return true;
            if (verticalCount > 1)
                return true;
            var horizontalCount = 0;
            
            //Horizontal before
            for (int i = col - 1, j = row + 1; i >= 0 && j < FieldSize; i--, j++)
            {
                if (Items[i][j] == null || Items[i][j] == DisabledItem) continue;
                var gobj2 = Items[i][j] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    horizontalCount++;
                else break;
            }
            //Horizontal after
            if (horizontalCount < 3)
            {
                for (int i = col + 1, j = row - 1; i < FieldSize && j >= 0; i++, j--)
                {
                    if (Items[i][j] == null || Items[i][j] == DisabledItem) continue;
                    var gobj2 = Items[i][j] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        horizontalCount++;
                    else break;
                }
            }
            else
                return true;
            return horizontalCount > 1;
        }
      
        public override bool CheckForPossibleMoves()
        {
            for (var col = 0; col < FieldSize; col++)
                for (var row = 0; row < FieldSize; row++)
                {
                    if(Items[col][row] == null)
                        return true;

                    if (Items[col][row] == DisabledItem)
                        continue;

                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;

                    var gi = gobj.GetComponent<GameItem>();

                    var positionPoint = new Point { X = col, Y = row };
                    // возможна горизонтальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = 1 }, new List<Point>
                     {
                         new Point{X = -1, Y = -3}, new Point{X = -3, Y = -1}, new Point{X = -1, Y = 1}, new Point{X = 1, Y = -1 },
                         new Point{X = 2, Y = 4}, new Point{X = 4, Y = 2}, new Point{X = 2, Y = 0},  new Point{X = 0, Y = 2 }
                     }
                        ))
                        return true;

                    // возможна горизонтальная, две по разным сторонам      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = 2 }, new List<Point>
                     {
                         new Point{X = -1, Y = 1}, new Point{X = 1, Y = -1}, new Point{X = 1, Y = 3}, new Point{X = 3, Y = 1}
                     }
                        ))
                        return true;

                    // возможна вертикальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = -1 }, new List<Point>
                     {
                         new Point{X = -1, Y = -1}, new Point{X = 1, Y = 1}, new Point{X = -3, Y = 1}, new Point{X = -1, Y = 3},
                         new Point{X = 2, Y = -4},  new Point{X = 4, Y = -2}, new Point{X = 2, Y = 0}, new Point{X = 0, Y = -2}
                     }
                        ))
                        return true;

                    // возможна вертикальная, две по разным сторонам       
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = -2 }, new List<Point>
                     {
                         new Point{X = -1, Y = -1}, new Point{X = 1, Y = 1}, new Point{X = 3, Y = -1}, new Point{X = 1, Y = -3},
                     }
                        ))
                        return true;
                }
            return false;
        }

        protected override bool RemoveAdditionalItems()
        {
            var result = false;
            for (int row = FieldSize / 2, col = 0; row < FieldSize; row++, col++)
            {
                if (Items[col][row] != null && Items[col][row] != DisabledItem)
                {
                    var gobj = Items[col][row] as GameObject;
                    if (!(gobj == null ||
                        (gobj.GetComponent<GameItem>().Type != GameItemType._ToMoveItem ||
                         gobj.GetComponent<GameItemMovingScript>().IsMoving)))
                    {
                        LabelShowing.ShowScalingLabel(gobj, true, false, "+" + 222, Color.white, Color.white, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, Game.numbersFont, true, null, 0, GameItemType._ToMoveItem);
                        RaisePoints(AdditionalItemCost);
                        if (ProgressBar != null)
                            ProgressBar.AddTime(AdditionalItemCost * 2);
                        RemoveGameItem(col, row);
                        result = true;
                    }
                }
                
                var col2 = FieldSize - col - 1;
                if (col2 != col && Items[col2][row] != null && Items[col2][row] != DisabledItem)
                {
                    var gobj = Items[col2][row] as GameObject;
                    if (gobj == null ||
                        (gobj.GetComponent<GameItem>().Type != GameItemType._ToMoveItem ||
                         gobj.GetComponent<GameItemMovingScript>().IsMoving)) continue;
                    LabelShowing.ShowScalingLabel(gobj, true, false, "+" + 222, Color.white, Color.white, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, Game.numbersFont, true, null, 0, GameItemType._ToMoveItem);
                    RaisePoints(AdditionalItemCost);
                    if (ProgressBar != null)
                        ProgressBar.AddTime(AdditionalItemCost * 2);
                    RemoveGameItem(col2, row);
                    result = true;
                }
            }
            return result;
        }

        protected override String ItemsTextureName(GameItemType type)
        {
             return ItemPrefabName;
        }

        protected override GameItemMovingType GetMovingTypeByItemType(GameItemType type)
        {
            switch (type)
            {
                case GameItemType._XItem:
                    return GameItemMovingType.Static;
                default:
                    return GameItemMovingType.Standart;
            }
        }


        protected override String GetTextureIDByType(GameItemType type)
        {
            switch(type)
            {
                case GameItemType._ToMoveItem:
                    return "_17";
                default:
                    return GameItem.GetStandartTextureIDByType(type);
            }
        }

    }
}