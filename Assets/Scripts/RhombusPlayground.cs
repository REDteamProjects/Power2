using System;
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

        public override float ScaleMultiplyer
        {
            get { return 4.1f; }// 4.95f; }
            //get { return 1; }
        }

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
            _initialMoveTimerMultiple = 28;
        }

        protected override void VeryHardLevelAction(object sender, EventArgs e)
        {
            var actionIndex = RandomObject.Next(0, 2);
            switch (actionIndex)
            {
                case 0:
                    int col = RandomObject.Next(0, FieldSize - 1);
                    for (int row = 0; row < FieldSize; row++)
                    if (Items[col][row] != DisabledItem)
                    {
                        var gobj = Items[col][row] as GameObject;
                        if (gobj == null) continue;
                        var gi = gobj.GetComponent<GameItem>();
                        if (gi.MovingType != GameItemMovingType.Static)
                            RemoveGameItem(col, row);
                    }
                    return;
                case 1:
                    int row1 = RandomObject.Next(0, FieldSize - 1);
                    for (int col1 = 0; col1 < FieldSize; col1++)
                    if (Items[col1][row1] != DisabledItem)
                    {
                        var gobj = Items[col1][row1] as GameObject;
                        if (gobj == null) continue;
                        var gi = gobj.GetComponent<GameItem>();
                        if (gi.MovingType != GameItemMovingType.Static)
                            RemoveGameItem(col1, row1);
                    }
                    return;
            }
        }

        public override bool IsInAnotherLine(IEnumerable<Line> lines, int currentX, int currentY)
        {
            var count = 0;
            foreach (var l in lines)
            {
                if (l.Y1 > l.Y2)
                {
                    count += l.X1 <= currentX && l.X2 >= currentX && l.Y1 >= currentY && l.Y2 <= currentY ? 1 : 0;
                }
                else
                {
                    count += l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY ? 1 : 0;
                }
            }
            return count > 1;//lines.Count(l => (l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY)) > 1;
        }
        
        public override Vector3 GetCellCoordinates(int col, int row)
        {
            //var halfItem = GameItemSize * 0.9325 / 2 + GameItemSize * 0.0625;
            var halfItem = GameItemSize * 0.9325 / 2 + GameItemSize * 0.220;
            var roundedX = Math.Round(Item00.X + col * halfItem, 2);
            var roundedY = Math.Round(Item00.Y - row * halfItem, 2);
            return new Vector3((float)roundedX, (float)roundedY, Item00.Z);
        }
        
        public override bool MatchPattern(GameItemType itemType, Point firstItem, Point secondItem, IEnumerable<Point> pointForCheck)
        {
            var mainType = MatchType(firstItem.X + secondItem.X, firstItem.Y + secondItem.Y, itemType);
            if (!mainType) return false;

            var lineGenerationPoints = pointForCheck.Where(point => MatchType(firstItem.X + point.X, firstItem.Y + point.Y, itemType)).ToList();
            if (!lineGenerationPoints.Any()) return false;

            foreach (var lineGenerationPoint in lineGenerationPoints)
            {
                if (secondItem.Y < 0)//TODO: Recalculate directions, error somewhere
                {
                    LogFile.Message("secondItem.Y < 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem "
                        + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: " + lineGenerationPoint.X + " " + lineGenerationPoint.Y, true);
                    if (Math.Abs(secondItem.Y) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + 1,
                            Y = firstItem.Y - 1
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (lineGenerationPoint.X > 0 ? 2 : -1),
                            Y = firstItem.Y + (lineGenerationPoint.X > 0 ? -2 : 1)
                        };
                    }
                }
                else if (secondItem.Y > 0)
                {
                    LogFile.Message("secondItem.Y > 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem " + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: "
                        + lineGenerationPoint.X + " " + lineGenerationPoint.Y, true);
                    if (Math.Abs(secondItem.X) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + 1,
                            Y = firstItem.Y + 1
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (lineGenerationPoint.X > 0 ? 2 : -1),
                            Y = firstItem.Y + (lineGenerationPoint.X > 0 ? 2 : -1)
                        };
                    }
                }

                if (Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != null)
                {
                    var selectedObject = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                    if (selectedObject != null)
                    {
                        var gi = selectedObject.GetComponent<GameItem>();
                        if (gi.MovingType == GameItemMovingType.Static || gi.Type == GameItemType.NullItem || gi.Type == GameItemType.DisabledItem)
                            continue;
                    }
                }

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
                if (_raiseMaxInitialElement && CallbacksCount == 0)
                {
                    _raiseMaxInitialElement = false;
                    MaxInitialElementTypeRaisedActions();
                }
                ChainCounter = 0;
                if (HintTimeCounter < 0) HintTimeCounter = 0;
                if (!RemoveAdditionalItems() && CallbacksCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);
                    GenerateField(false, true);
                    //ClearField();
                }
                UpdateTime();
                return 0;
            }
            HintTimeCounter = -1;
            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            while (l != null && !IsGameOver)
            {
                var toObj = Items[l.X2][l.Y2] as GameObject;
                var toCell = GetCellCoordinates(l.X2, l.Y2);
                var pointsMultiple = 1;
                //Vertical
                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;

                    for (int i = l.X2 - 1, j = l.Y2 - 1; i >= l.X1 && j >= l.Y1; i--, j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                        {
                            //LogFile.Message("Items[i][j] == null || Items[i][j] == DisabledItem");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, j))
                        {
                            LogFile.Message("Items[" + i + "][" + j + "] on another line", true);
                            continue;
                        }
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        if (c.IsMoving) continue;
                        var cX = i;
                        var cY = j;

                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                                {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(toCell.x, toCell.y, Game.standartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;

                                    Destroy(item);
                                });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, toCell.y, Game.standartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;

                                Destroy(item);
                            });
                        }
                    }
                }
                //Horizontal
                else
                {
                    toObj = Items[l.X1][l.Y1] as GameObject;
                    toCell = GetCellCoordinates(l.X1, l.Y1);

                    pointsMultiple += l.X2 - l.X1 - 2;

                    for (int i = l.X1 + 1, j = l.Y1 - 1; i <= l.X2 && j >= l.Y2; i++, j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                        {
                            //LogFile.Message("Items[i][j] == null || Items[i][j] == DisabledItem");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, j))
                        {
                            //LogFile.Message("Items[" + i + "][" + j + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = 
                            new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        if (c.IsMoving) continue;

                        var cX = i;
                        var cY = j;
                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                                {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(toCell.x, toCell.y, Game.standartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;

                                    Destroy(item); 
                                });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, toCell.y, Game.standartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;

                                Destroy(item);
                            });
                        }
                    }
                }
                if (toObj != null)
                {
                    var toGi = toObj.GetComponent<GameItem>();
                    if (toGi.Type == MaxInitialElementType + 1)
                        MaxInitialElementType++;
                    var newgobjtype = toGi.Type + 1;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell,
                        new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f));
                    if (toObj.GetComponent<GameItemMovingScript>().IsMoving)
                        CallbacksCount--;
                    Destroy(toObj);
                    if (l.Orientation == LineOrientation.Vertical) 
                        Items[l.X2][l.Y2] = newgobj;
                    if (l.Orientation == LineOrientation.Horizontal)
                        Items[l.X1][l.Y1] = newgobj;

                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                        if (newgobjtype <= MaxInitialElementType)
                        {
                            pointsBank += points;
                            LabelShowing.ShowScalingLabel(newgobj,//new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, newgobj.transform.localPosition.z - 1),
                                "+" + points, GameColors.ItemsColors[newgobjtype], Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, null, true, null, 0, newgobjtype);
                        }
                        else
                        {
                            pointsBank += 2 * points;
                            LabelShowing.ShowScalingLabel(newgobj, //new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, newgobj.transform.localPosition.z - 1),
                                "+" + points + "x2", GameColors.ItemsColors[newgobjtype], Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, null, true, null, 0, newgobjtype);
                        }
                }
                lines.Remove(l);
                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected", true);
                l = lines.FirstOrDefault();

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                pointsBank *= linesCount;
                ChainCounter++;
                RaisePoints(pointsBank * ChainCounter * (int)Game.Difficulty);

                pointsBank = 0;
                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected", true);
            RemoveAdditionalItems();
            
            return linesCount;
        }

        public override void Drop()
        {
            if (Items == null) return;

            var generateAfterDrop = true;

            var rhombusDropSpeed = Game.standartItemSpeed - 4;

            for (var row = FieldSize - 1; row > 0; row--)
            {
                for (var col = 0; col < FieldSize; col++)
                {
                    if (Items[col][row] != null || Items[col][row] == DisabledItem)
                        continue;
                    var side = col % 2 == 1 ? 1 : -1;
                    var downItemRow = row - 1;
                    var downItemCol = col + side;
                    if (downItemCol < 0 || downItemCol >= FieldSize || Items[downItemCol][downItemRow] == DisabledItem || Items[downItemCol][downItemRow] == null)
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
                        if (!AreStaticItemsDroppable && gobj.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                            downItemRow--;
                        else
                        {
                            generateAfterDrop = false;
                            var gims = gobj.GetComponent<GameItemMovingScript>();
                            //var dis = GetComponent<DragItemScript>();
                            if (gims.IsMoving || gobj.GetComponent<GameItem>().IsTouched)
                                continue;
                            //DropsCount++;
                            Items[col][row] = Items[downItemCol][downItemRow];
                            Items[downItemCol][downItemRow] = null;
                            CallbacksCount++;
                            var coord = GetCellCoordinates(col, row);
                            gims.MoveTo(coord.x, coord.y, rhombusDropSpeed, (item, result) =>
                            {
                                //DropsCount--;
                                CallbacksCount--;
                                if (!result) return;
                                LogFile.Message("New item droped Items[" + downItemCol + "][" + downItemRow + "] DC: " + DropsCount, true);
                            });
                            break;
                        }
                    }
                }
            }
            if (generateAfterDrop)
                GenerateField(true);
        }

        public override void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false)
        {
            LogFile.Message("Generating field...", true);
            if (!mixCurrent)
            {
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnX = i % 2 == 1 ? -i : i;
                    var resCol = RandomObject.Next(0, FieldSize);

                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        var deniedList = new List<GameItemType>();
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        switch (Game.Difficulty)
                        {
                            case DifficultyLevel._medium:
							case DifficultyLevel._hard:
                            //case DifficultyLevel._veryhard:
                                if (DropDownItemsCount < MaxAdditionalItemsCount && j <= FieldSize / 2)
                                {
                                    var resRow = RandomObject.Next(0, FieldSize);
                                    if (resCol == resRow)
                                    {
                                        Items[i][j] = GenerateGameItem(GameItemType._DropDownItem, i, j, new Vector2(generateOnX, i), false, Game.standartItemSpeed/2 + i * 2);//may be calculate speed or generateOn vector in another way
                                        //(Items[i][j] as GameObject).transform.localScale = new Vector3(4,4);
                                        DropDownItemsCount++;
                                        generateOnX++;
                                        continue;
                                    }
                                }
                                break;
                        }
                        if (completeCurrent)
                        {
                            LogFile.Message("New gameItem need to i:" + i + "j: " + j, true);
                            Items[i][j] = GenerateGameItem(i, j, null, new Vector2(generateOnX, i), false, Game.standartItemSpeed/2 + i * 2);//may be calculate speed or generateOn vector in another way
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
                    var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var noMovesLabel = o.GetComponent<LabelShowing>();
                        LabelAnimationFinishedDelegate callback = null;
                        if (!onlyNoMovesLabel)
                            callback = MixField;
                        noMovesLabel.ShowScalingLabel(new Vector3(0, -2, -4),
                             LanguageManager.Instance.GetTextValue("NoMovesTitle"), GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, callback, true);
                        //noMovesLabel.ShowScalingLabel(new Vector3(0, Item00.Y + GameItemSize * 2.5f, -4), 
                        //    "No moves", new Color(240, 223, 206), new Color(240, 223, 206), 60, 90, null, true, null, true);
                    }
                
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
            var o = Items[newX][newY] as GameObject;
            return o != null && (newX >= 0 && newX <= (FieldSize - 1) && newY >= 0 && newY <= (FieldSize - 1) && Items[newX][newY] != null && Items[newX][newY] != DisabledItem &&
                                                                           o.GetComponent<GameItem>().MovingType != GameItemMovingType.Static && !o.GetComponent<GameItemMovingScript>().IsMoving);
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
                    if (Items[col][row] == null || Items[col][row] == DisabledItem)
                    {
                        if (Items[col][row] == null) Debug.LogError("Items[col][row] null in checkForPossibleMoves");
                        continue;
                    }
                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;
                    var gi = gobj.GetComponent<GameItem>();
                    var positionPoint = new Point { X = col, Y = row };
                    // возможна горизонтальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = 1 }, new List<Point>
                     {
                         new Point{X = -1, Y = -3}, new Point{X = -1, Y = 1}, new Point{X = -3, Y = -1}, new Point{X = 2, Y = 4},
                         new Point{X = 4, Y = 2}, new Point{X = 2, Y = 0}
                     }
                        ))
                        return true;
                    // возможна горизонтальная, две по разным сторонам      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = 2 }, new List<Point>
                     {
                         new Point{X = 1, Y = -1}, new Point{X = 1, Y = 3}
                     }
                        ))
                        return true;
                    // возможна вертикальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = -1 }, new List<Point>
                     {
                         new Point{X = -1, Y = -1}, new Point{X = -3, Y = 1}, new Point{X = -1, Y = 3}, new Point{X = 2, Y = -4},
                         new Point{X = 4, Y = -2}, new Point{X = 2, Y = 0}
                     }
                        ))
                        return true;
                    // возможна вертикальная, две по разным сторонам       
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = -2 }, new List<Point>
                     {
                         new Point{X = 1, Y = 1}, new Point{X = 1, Y = -3}
                     }
                        ))
                        return true;
                }
            return false;
        }
    }
}