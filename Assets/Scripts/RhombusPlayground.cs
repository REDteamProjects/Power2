using System;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using System.Collections.Generic;
namespace Assets.Scripts
{
    class RhombusPlayground : SquarePlayground
    {
        //private const float HintDelayTime = 4;
        //protected GameItemType MaxType = GameItemType._3;
        //protected const int maxAdditionalItemsCount = 2;
        //private static readonly System.Object _disabledItem = new object();
        //private static readonly System.Random RandomObject = new System.Random();
        //private readonly AutoResetEvent _callbackReady = new AutoResetEvent(false);
        //protected readonly DifficultyLevel Difficulty;
        //protected int DropDownItemsCount;
        //protected int StaticItemsCount;
        //private volatile int _callbacksCount;
        //private int _dropsCount;
        //private RealPoint _item00;
        //private float _timeCounter = -1;
        //private GameObject _selectedPoint1;
        //private GameObject _selectedPoint2;
        //private Point _selectedPoint1Coordinate;
        //private Point _selectedPoint2Coordinate;
        //private int _chainCounter;
		//private int _score;
        //private float _currentTime;

		public override String ItemPrefabName { get { return ItemPrefabNameHelper.GetPrefabPath<RhombusPlayground>(); } }

        public override float ScaleMultiplyer
        {
            get { return 7.12f; }
        }

        //public virtual bool isDisabledItemActive { get { return false; } }
        //public System.Object DisabledItem { get { return _disabledItem; } }
        /* public Point SelectedPoint1
        {
            set
            {
                if (_selectedPoint1 == null && value == null)
                    return;
                if (_selectedPoint1 != null)
                {
                    Destroy(_selectedPoint1);
                    _selectedPoint1 = null;
                }
                if (value != null) LogFile.Message("SelectedPoint1 X = " + value.X + " Y= " + value.Y);
                if (value == null || Items[value.X][value.Y] == null) return;
                _selectedPoint1Coordinate = value;
            }
        }  */
        /* public Point SelectedPoint2
        {
            set
            {
                if (_selectedPoint2 == null && value == null)
                    return;
                if (_selectedPoint2 != null)
                {
                    Destroy(_selectedPoint2);
                    _selectedPoint2 = null;
                }
                if (value != null) LogFile.Message("SelectedPoint2 X = " + value.X + " Y= " + value.Y);
                if (value == null || Items[value.X][value.Y] == null) return;
                _selectedPoint2Coordinate = value;
            }
        } */
		/* public virtual GameItemType MaxInitialElementType 
        { 
            get { return MaxType; }
            set
            {
                if (MaxType == value) return;
                MaxType = value;
                var stat = GetComponent<Game>().Stats;
                if (stat != null && stat.CurrentItemType < MaxType)
                {
                    stat.CurrentItemType = MaxType;
                    LogFile.Message("MaxType rhombus is " + MaxType);
                }
            } 
        } */
		/* protected void ShowMaxInitialElement()
        {
            var fg = GameObject.Find("/Foreground");
            var cmi = GameObject.Find("/Foreground/MaximumItem");
            var gobj = Instantiate(Resources.Load(ItemPrefabName + MaxType)) as GameObject;
            if (gobj == null) return;
            gobj.transform.SetParent(fg.transform);
            gobj.transform.localPosition = new Vector3(0, 360f + GameItemSize * 6, 0);
            gobj.transform.localScale = new Vector3(15, 15);
            gobj.name = "MaximumItem";
            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y));
            c.MoveTo(null, gobj.transform.localPosition.y - GameItemSize * 6, 2f, (item, result) =>
            {
                if (!result) return;
                if (cmi != null)
                    Destroy(cmi);
            });
        } */
/*         protected int CallbacksCount
        {
            get
            {
                return _callbacksCount;
            }
            set
            {
                if (value == _callbacksCount)
                {
                    LogFile.Message("value == _callbacksCount: " + value);
                    return;
                }
                _callbacksCount = value;
                LogFile.Message("CallbacksCount = " + value);
                if (value > 0)
                {
                    _callbackReady.Reset();
                }
                else
                {
                    _callbackReady.Set();
                }
            }
        }
        public virtual IPlaygroundSavedata SavedataObject
        {
            get { return null; }
        } */
        /* public virtual int FieldSize { get { return 0; } }
        public float DeltaToExchange { get { return GameItemSize / 2f; } }
        public float DeltaToMove { get { return GameItemSize / 4f; } }
        public object[][] Items { get; set; }
        public virtual float GameItemSize { get { return 0f; } }
        public virtual RealPoint InitialGameItemPosition { get { return null; } } */
        //public Dictionary<MoveDirections, Vector2> AvailableMoveDirections { get; private set; }
		/* public int CurrentScore
        {
            get { return _score; }
            set
            {
                if (_score == value) return;
                _score = value;
                var stat = GetComponent<Game>().Stats;
                if (stat != null && stat.ScoreRecord < _score)
                    stat.ScoreRecord = _score;
            }
        }
        public float CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime == value) return;
                _currentTime = value;
                var stat = GetComponent<Game>().Stats;
                if (stat != null && stat.LongestSession < _currentTime)
                    stat.LongestSession = _currentTime;
            }
        } */
        /* protected int DropsCount
        {
            get { return _dropsCount; }
            set
            {
                if (value == _dropsCount)
                {
                    LogFile.Message("value == _dropsCount: " + value);
                    return;
                }
                _dropsCount = value;
                LogFile.Message("DropsCount = " + value);
            }
        } */
		/* public int ChainCounter
        {
            get { return _chainCounter; }
            set
            {
                if (_chainCounter == value) return;
                _chainCounter = value;
                var stat = GetComponent<Game>().Stats;
                if (stat != null && stat.MaxMultiplier < _chainCounter)
                    stat.MaxMultiplier = _chainCounter;
            }
        } */
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
        {}

        //void Update()
        //{
        //     if (CallbacksCount ==0)
        //        Drop();
        //    if (!(_timeCounter >= 0)) return;
        //    if (_timeCounter > HintDelayTime && _selectedPoint1 == null && _selectedPoint2 == null)
        //    {
        //        var parentGobj = Items[_selectedPoint1Coordinate.X][_selectedPoint1Coordinate.Y] as GameObject;
        //        if (parentGobj == null) return;
        //        _selectedPoint1 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
        //        if (_selectedPoint1 == null)
        //        {
        //            LogFile.Message("SelectedPoint1 initialization failed");
        //            return;
        //        }
        //        _selectedPoint1.transform.SetParent(parentGobj.transform);
        //        _selectedPoint1.transform.localScale = Vector3.one;
        //        _selectedPoint1.transform.localPosition = Vector3.zero;
        //        parentGobj = Items[_selectedPoint2Coordinate.X][_selectedPoint2Coordinate.Y] as GameObject;
        //        if (parentGobj == null) return;
        //        _selectedPoint2 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
        //        if (_selectedPoint2 == null)
        //        {
        //            LogFile.Message("SelectedPoint2 initialization failed");
        //            return;
        //        }
        //        _selectedPoint2.transform.SetParent(parentGobj.transform);
        //        _selectedPoint2.transform.localScale = Vector3.one;
        //        _selectedPoint2.transform.localPosition = Vector3.zero;
        //    }
        //    _timeCounter += Time.deltaTime;
        //    CurrentTime += Time.deltaTime; 
        //}
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
        /* private bool MatchType(int x, int y, GameItemType itemType)
        {
            // убедимся, что фишка не выходит за пределы поля    
            if ((x < 0) || (x >= FieldSize) || (y < 0) || (y >= FieldSize) || Items[x][y] == null || Items[x][y] == DisabledItem) return false;
            var gobj = Items[x][y] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            return gi != null && gi.Type == itemType;
        } */
        public override Vector3 GetCellCoordinates(int col, int row)
        {
            var halfItem = GameItemSize * 0.9325 / 2 + GameItemSize * 0.0625;
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
                if (secondItem.Y < 0)
                {
                    LogFile.Message("secondItem.Y < 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem "
                        + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: " + lineGenerationPoint.X + " " + lineGenerationPoint.Y);
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
                    LogFile.Message("secondItem.Y > 0 and firstItem: " + firstItem.X + " " + firstItem.Y + " secondItem " + secondItem.X + " " + secondItem.Y + "lineGenerationPoint: " + lineGenerationPoint.X + " " + lineGenerationPoint.Y);
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
            LogFile.Message("Finding lines...");
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
                        LogFile.Message(match + " " + line);
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
                        LogFile.Message(match + " " + line);
                        currentCol += match;
                        row -= match - 1;
                    }
                    currentCol++;
                }
            }
            LogFile.Message("Return lines: " + list.Count);
            return list;
        }
        public override int CheckForLine(int x, int y, LineOrientation orientation)
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
        /* public GameObject InstantiateGameItem(GameItemType itemType, Vector3 localPosition, Vector3 localScale)
        {
            var newgobj = Instantiate(Resources.Load(ItemPrefabName + itemType)) as GameObject;
            if (newgobj != null)
            {
                var rectTransform = newgobj.transform as RectTransform;
                if (rectTransform != null)
                    rectTransform.sizeDelta = new Vector2(GameItemSize * 0.9375f, GameItemSize * 0.9375f);
                newgobj.transform.SetParent(transform);
                newgobj.transform.localPosition = localPosition;
                newgobj.transform.localScale = localScale;  
            }
            return newgobj;
        } */
/*         public GameObject GenerateGameItem(GameItemType itemType, int i, int j, Vector2? generateOn = null, bool isItemDirectionChangable = false)
        {
            if (!generateOn.HasValue)
                generateOn = new Vector2(0, FieldSize - j);
            if (_item00 == null)
                _item00 = InitialGameItemPosition;
            var cell = GetCellCoordinates(i, j);
            var gobj = InstantiateGameItem(itemType, new Vector3(
                (float)Math.Round(cell.x + generateOn.Value.x * GameItemSize, 2),
                (float)Math.Round(_item00.Y + generateOn.Value.y * GameItemSize, 2),
                _item00.Z), Vector3.zero);
            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y - 6 * GameItemSize));
            CallbacksCount++;
            LogFile.Message("c.MoveTo(" + cell.x + ", " + cell.y + ", " + (10 - i % 2 + j * 1.5f + ", ..."));
            c.MoveTo(cell.x, cell.y, 10 - i % 2 + j * 1.5f, (item, result) =>
            {
                CallbacksCount--;
                if (!result) return;
                if (_callbackReady.WaitOne(1))
                    ClearChains();
            }, new Vector2(_item00.X, _item00.Y + GameItemSize / 2), new Vector3(GameItemSize / 7.12f, GameItemSize / 7.12f, 1f), isItemDirectionChangable);
            return gobj;
        } */
        /* public GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null)
        {
            var newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            if (deniedTypes == null || deniedTypes.Count <= 0)
                return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable);
            while (deniedTypes.Contains((GameItemType)newType))
                newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable);
        } */
        /* void RemoveAdditionalItems()
        {
            for (var i = 0; i < FieldSize; i++)
            {
                var onY = i <= FieldSize/2 ? FieldSize/2 + i : FieldSize*2 - FieldSize/2 - i;
                var gobj = Items[i][onY] as GameObject;
                if (gobj != null && (Items[i][onY] != null && Items[i][onY] != DisabledItem && gobj.GetComponent<GameItem>().Type == GameItemType._DropDownItem))
                {
                    var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var pointsLabel = o.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);
                        pointsLabel.ShowScalingLabel(new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y + GameItemSize / 2, gobj.transform.localPosition.z - 1),
                            "+" + 222, Color.white, Color.gray, 60, 90, null, true);
                    }
                    RisePoints(222);
                    Destroy(gobj);
                    Items[i][onY] = null;
                    DropDownItemsCount--;
                }
            }
        } */
        public override int ClearChains()
        {
            if (IsGameOver) return -1;
            //var gameOver = false;
            SelectedPoint1 = null;
            SelectedPoint2 = null;
            var lines = GetAllLines();
            if (lines.Count == 0)
            {
                ChainCounter = 0;
                if (TimeCounter < 0) TimeCounter = 0;
                if (!CheckForPossibleMoves() && DropsCount == 0)
                {
                    LogFile.Message("No moves");
                    GenerateField(false, true);
                    //ClearField();
                }
                UpdateTime();
                SavedataHelper.SaveData(SavedataObject);
                return 0;
            }
            TimeCounter = -1;
            LogFile.Message("Start clear chaines. Lines: " + lines.Count);
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            while (l != null && !IsGameOver)
            {
                GameObject toObj;
                Vector3 toCell;
                var pointsMultiple = 1;
                //Vertical
                if (l.Orientation == LineOrientation.Vertical)
                {
                    toObj = Items[l.X2][l.Y2] as GameObject;
                    toCell = GetCellCoordinates(l.X2, l.Y2);
                    pointsMultiple += l.Y2 - l.Y1 - 2;
                    for (int i = l.X2 - 1, j = l.Y2 - 1; i >= l.X1 && j >= l.Y1; i--, j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                        {
                            LogFile.Message("Items[i][j] == null || Items[i][j] == DisabledItem");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, j))
                        {
                            LogFile.Message("Items[" + i + "][" + j + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        var cX = i;
                        var cY = j;
                        CallbacksCount++;
                        c.MoveTo(toCell.x, toCell.y, 14, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            Items[cX][cY] = null;
                            Destroy(item);
                        });
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
                            LogFile.Message("Items[i][j] == null || Items[i][j] == DisabledItem");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, j))
                        {
                            LogFile.Message("Items[" + i + "][" + j + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        var cX = i;
                        var cY = j;
                        CallbacksCount++;
                        c.MoveTo(toCell.x, toCell.y, 14, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            Items[cX][cY] = null;
                            Destroy(item);
                        });
                    }
                }
                if (toObj != null)
                {
                    var toGi = toObj.GetComponent<GameItem>();
                    if (toGi.Type == MaxInitialElementType + 1)
                    {
                        MaxInitialElementType++;
                    }
                    var newgobjtype = toGi.Type + 1;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell,
                        new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f));
                    
                    Destroy(toObj);
                    if (l.Orientation == LineOrientation.Vertical) Items[l.X2][l.Y2] = newgobj; else Items[l.X1][l.Y1] = newgobj;
                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                    var gameObject1 = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (gameObject1 != null)
                    {
                        var pointsLabel = gameObject1.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);
                        if (newgobjtype <= MaxInitialElementType)
                        {
                            pointsBank += points;
                            pointsLabel.ShowScalingLabel(new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, newgobj.transform.localPosition.z - 1),
                                "+" + points, GameColors.ItemsColors[newgobjtype], Color.gray, 60, 90, null, true);
                        }
                        else
                        {
                            pointsBank += 2 * points;
                            pointsLabel.ShowScalingLabel(new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, newgobj.transform.localPosition.z - 1),
                                "+" + points + "x2", GameColors.ItemsColors[newgobjtype], Color.gray, 60, 90, null, true);
                        }
                    }
                    IsGameOver = newgobjtype == GameItemType._Gameover;
                }
                lines.Remove(l);
                LogFile.Message("line collected");
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected");
            RemoveAdditionalItems();
            if (linesCount > 1)
            {
                var labelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                if (labelObject != null)
                {
                    var comboLabel = labelObject.GetComponent<LabelShowing>();
                    comboLabel.transform.SetParent(transform);
                    comboLabel.ShowScalingLabel(new Vector3(0, Item00.Y + GameItemSize, -3),
                                "Combo x" + linesCount + " lines!", GameColors.BackgroundColor, Color.gray, 60, 90, null, true);
                }
            }

            pointsBank *= linesCount;
            ChainCounter++;
            RisePoints(pointsBank * ChainCounter);

            if (!IsGameOver) return linesCount;
            var gameOverLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (gameOverLabelObject == null) return linesCount;
            var gameOverLabel = gameOverLabelObject.GetComponent<LabelShowing>();
            gameOverLabel.transform.SetParent(transform);
            gameOverLabel.ShowScalingLabel(new Vector3(0, 0, -3),
                "Game over", Color.white, Color.gray, 60, 90);
            //TODO: stop the game

            return linesCount;
        }


        public override void Drop()
        {
            var generateAfterDrop = true;
            var counter = 0;
            //DropsCount += FieldSize * (FieldSize - 1);
            for (var row = 0; row < FieldSize - 1; row++)
            {
                for (var col = 1; col < FieldSize; col++)
                {
                    var downItemSide = (col + 1) % 2 == 1 ? 1 : -1;
                    if (Items[col][row] == null)
                        continue;
                    counter++;
                    if (Items[col][row] == DisabledItem || col + downItemSide >= FieldSize ||
                        Items[col + downItemSide][row + 1] != null)
                    {
                        if (col - downItemSide >= 0)
                        {
                            downItemSide = -downItemSide;
                            if (Items[col + downItemSide][row + 1] != null)
                                continue;
                        }
                    }
                    var o = Items[col][row] as GameObject;
                    if (o != null && (Items[col][row] != null && Items[col][row] != DisabledItem && o.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                        continue;
                    var gameObject1 = Items[col + downItemSide][row + 1] as GameObject;
                    if (gameObject1 != null && (Items[col + downItemSide][row + 1] != null && Items[col + downItemSide][row + 1] != DisabledItem && gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                    {
                        var rowStaticCounter = 1;
                        GameObject o1;
                        while ((row + rowStaticCounter) < FieldSize && (o1 = Items[col + downItemSide * (rowStaticCounter)][row + rowStaticCounter] as GameObject) != null && o1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                            rowStaticCounter++;
                        if ((row + rowStaticCounter) >= FieldSize || Items[col + downItemSide * rowStaticCounter][row + rowStaticCounter] != null)
                            continue;
                        var gobjS = Items[col][row] as GameObject;
                        if (gobjS == null) continue;
                        var cS = gobjS.GetComponent<GameItemMovingScript>();
                        if (cS.IsMoving) continue;
                        Items[col + downItemSide * rowStaticCounter][row + rowStaticCounter] = Items[col][row];
                        Items[col][row] = null;
                        if (!cS.IsMoving) DropsCount++;
                        var colS = col;
                        var rowS = row;
                        cS.MoveTo(null, GetCellCoordinates(col + downItemSide * rowStaticCounter, row + rowStaticCounter).y, 10, (item, result) =>
                        {
                            if (!cS.IsMoving)
                                DropsCount--;
                            if (!result) return;
                            LogFile.Message("New item droped Items[" + colS + "][" + rowS + "] cc: " + CallbacksCount);
                        });
                        if (row + 2 < FieldSize && Items[col][row + 2] == null)
                            generateAfterDrop = false;
                        continue;
                    }
                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;
                    var c = gobj.GetComponent<GameItemMovingScript>();
                    if (c.IsMoving) continue;
                    var toCell = GetCellCoordinates(col + downItemSide, row + 1);
                    var col1 = col;
                    var row1 = row;
                    if (!c.IsMoving) DropsCount++;
                    c.MoveTo(toCell.x, toCell.y, 10, (item, result) =>
                    {
                        if (!c.IsMoving)
                            DropsCount--;
                        if (!result) return;
                        LogFile.Message("New item droped Items[" + col1 + "][" + row1 + "] cc: " + CallbacksCount);
                    });
                    Items[col + downItemSide][row + 1] = Items[col][row];
                    Items[col][row] = null;
                    if (row + 2 < FieldSize && Items[col][row + 2] == null) 
                        generateAfterDrop = false;//DropsCount++;
                }
            }
            //DropsCount -= FieldSize * (FieldSize - 1);
            if (DropsCount == 0 && generateAfterDrop 
                && counter < (FieldSize - 1) * (FieldSize - 1))
                GenerateField(true);
        }
        public override void GenerateField(bool completeCurrent = false, bool mixCurrent = false)
        {
            LogFile.Message("Generating field...");
            if (!mixCurrent)
            {
                switch (Difficulty)
                {
                    case DifficultyLevel.hard:
                    case DifficultyLevel.veryhard:
                        while (XItemsCount < maxAdditionalItemsCount)
                        {
                            int col;
                            int row;
                            while (Items[(col = RandomObject.Next(1, FieldSize-1))][(row = RandomObject.Next(1, FieldSize-1))] != null) { }
                            Items[col][row] = GenerateGameItem(GameItemType._XItem, col, row, new Vector2((col % 2 == 1 ? -col : col), col), false, null, null, GameItemMovingType.Static);
                            XItemsCount++;
                        }
                        break;
                }
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnX = i % 2 == 1 ? -i : i;
                    var resCol = RandomObject.Next(0, FieldSize);
                    //var generateOnY = Math.Abs(generateOnX);
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        var deniedList = new List<GameItemType>();
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        switch (Difficulty)
                        {
                            case DifficultyLevel.medium:
                            case DifficultyLevel.veryhard:
                                if (DropDownItemsCount < maxAdditionalItemsCount && j <= FieldSize / 2)
                                {
                                    var resRow = RandomObject.Next(0, FieldSize);
                                    if (resCol == resRow)
                                    {
                                        Items[i][j] = GenerateGameItem(GameItemType._DropDownItem, i, j, new Vector2(generateOnX, i));
                                        DropDownItemsCount++;
                                        generateOnX++;
                                        continue;
                                    }
                                }
                                break;
                        }
                        if (completeCurrent)
                        {
                            LogFile.Message("New gameItem need to i:" + i + "j: " + j);
                            Items[i][j] = GenerateGameItem(i, j, null, new Vector2(generateOnX, i), false, 8 + i*3);//may be calculate speed or generateOn vector in another way
                            continue;
                        }
                        //Horizontal before
                        if (i >= 2 && j < FieldSize - 2 && CheckForLine(i - 2, j + 2, LineOrientation.Horizontal) > 1)
                        {
                            var o = Items[i - 1][j + 1] as GameObject;
                            if (o != null)
                                deniedList.Add(o.GetComponent<GameItem>().Type);
                        }
                        //Horizontal after
                        if (i < FieldSize - 2 && j > 1 && CheckForLine(i + 1, j - 1, LineOrientation.Horizontal) > 1)
                        {
                            var gameObject1 = Items[i + 1][j - 1] as GameObject;
                            if (gameObject1 != null)
                                deniedList.Add(gameObject1.GetComponent<GameItem>().Type);
                        }
                        //Vertical before
                        if (i > 1 && j > 1 && CheckForLine(i - 2, j - 2, LineOrientation.Vertical) > 1)
                        {
                            var o1 = Items[i - 1][j - 1] as GameObject;
                            if (o1 != null)
                                deniedList.Add(o1.GetComponent<GameItem>().Type);
                        }
                        //Vertical after
                        if (i < FieldSize - 2 && j < FieldSize - 2 && CheckForLine(i + 1, j + 1, LineOrientation.Vertical) > 1)
                        {
                            var gameObject2 = Items[i + 1][j + 1] as GameObject;
                            if (gameObject2 != null)
                                deniedList.Add(gameObject2.GetComponent<GameItem>().Type);
                        }
                        Items[i][j] = GenerateGameItem(i, j, deniedList, new Vector2(generateOnX, i));
                    }
                }
            }
            else
            {
                var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                if (o != null)
                {
                    var noMovesLabel = o.GetComponent<LabelShowing>();
                    noMovesLabel.transform.SetParent(transform);
                    noMovesLabel.ShowScalingLabel(new Vector3(0, 0, -4), "No moves", Color.white, GameColors.BackgroundColor, 60, 90, null, true);
                }
                while (!CheckForPossibleMoves())
                {
                    var toMixList = new List<object>();
                    for (var i = FieldSize - 1; i >= 0; i--)
                    {
                        for (var j = FieldSize - 1; j >= 0; j--)
                        {
                            if (Items[i][j] == null || Items[i][j] == DisabledItem)
                                continue;
                            var go = Items[i][j] as GameObject;
                            if (go == null || go.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;
                            toMixList.Add(Items[i][j]);
                        }
                    }
                    for (var i = FieldSize - 1; i >= 0; i--)
                    {
                        for (var j = FieldSize - 1; j >= 0; j--)
                        {
                            if (Items[i][j] == null || Items[i][j] == DisabledItem)
                                continue;
                            var go = Items[i][j] as GameObject;
                            if (go == null || go.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;
                            var index = RandomObject.Next(0, toMixList.Count);
                            Items[i][j] = toMixList[index];
                            toMixList.RemoveAt(index);
                        }
                    }
                }
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                            continue;
                        var gameObject1 = Items[i][j] as GameObject;
                        if (gameObject1 == null || gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;
                        var moving = gameObject1.GetComponent<GameItemMovingScript>();
                        var to = GetCellCoordinates(i, j);
                        CallbacksCount++;
                        moving.MoveTo(to.x, to.y, 10, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            if (CallbacksCount == 0)
                                ClearChains();
                        });
                    }
                }
            }
        }
        /* public bool GameItemsExchange(int x1, int y1, ref int x2, ref int y2, float speed, bool isReverse)
        {
            var item1 = Items[x1][y1] as GameObject;
            var item2 = Items[x2][y2] as GameObject;
            if (!isReverse)
            {
                Items[x1][y1] = item2;
                Items[x2][y2] = item1;
            }
            var position1 = GetCellCoordinates(x1, y1);
            var position2 = GetCellCoordinates(x2, y2);
			LogFile.Message("Exchange items: " + position1.x + position1.y + " " + position2.x + " " + position2.y);
            CallbacksCount++;
			if (item1 != null)
                item1.GetComponent<GameItemMovingScript>()
                    .MoveTo((float?)position1.x == (float?)position2.x ? null : (float?)position2.x,
                        (float?)position1.y == (float?)position2.y ? null : (float?)position2.y,
                        15, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo((float?)position1.x == (float?)position2.x ? null : (float?)position1.x,
                                        (float?)position1.y == (float?)position2.y ? null : (float?)position1.y,
                                        14, null);
                            }
                            else
                            {
                                if (_callbackReady.WaitOne(1))
                                    ClearChains();
                            }
                        });
            CallbacksCount++;
            if (item2 != null)
            {
                item2.GetComponent<GameItemMovingScript>()
                    .MoveTo((float?)position1.x == (float?)position2.x ? null : (float?)position1.x,
                        (float?)position1.y == (float?)position2.y ? null : (float?)position1.y,
                        15, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo((float?)position1.x == (float?)position2.x ? null : (float?)position2.x,
                                        (float?)position1.y == (float?)position2.y ? null : (float?)position2.y,
                                        14, null);
                            }
                            else
                            {
                                if (_callbackReady.WaitOne(1))
                                    ClearChains();
                            }
                        });
                return true;
            }
            return false;
        } */
        public override bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            if (!AvailableMoveDirections.ContainsKey(mdir)) return false;
            if (col < 0 || row < 0 || col > FieldSize - 1 || row > FieldSize - 1)
                return false;
            var direction = AvailableMoveDirections[mdir];
            var newX = col + (int)direction.x;
            var newY = row - (int)direction.y;
            var o = Items[newX][newY] as GameObject;
            var gameObject1 = Items[col][row] as GameObject;
            return gameObject1 != null && (o != null && (newX >= 0 && newX <= FieldSize - 1 && newY >= 0 && newY <= FieldSize - 1 && Items[newX][newY] != null && Items[newX][newY] != DisabledItem &&
                                                                           o.GetComponent<GameItem>().MovingType != GameItemMovingType.Static && Items[newX][newY] != DisabledItem && gameObject1.GetComponent<GameItem>().MovingType != GameItemMovingType.Static));
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
        /* public bool TryMakeMove(int x1, int y1, int x2, int y2)
        {
            if (Items[x1][y1] == null || Items[x1][y1] == DisabledItem ||
                Items[x2][y2] == null || Items[x2][y2] == DisabledItem)
                return false;
            var tItem = Items[x1][y1];
            Items[x1][y1] = Items[x2][y2];
            Items[x2][y2] = tItem;
            var ret = IsPointInLine(x1, y1) || IsPointInLine(x2, y2);
            tItem = Items[x2][y2];
            Items[x2][y2] = Items[x1][y1];
            Items[x1][y1] = tItem;
            return ret;
        } */
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
        /* void RisePoints(int points)
        {
			CurrentScore += points;
            var plabel = GetComponentInChildren<Text>();
            plabel.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
        } */
        /* public void RevertMovedItem(int col, int row)
        {
            var gobj = Items[col][row] as GameObject;
            var pos = GetCellCoordinates(col, row);
            LogFile.Message("Revert item to place: " + pos.x + " " + pos.y);
            if (gobj == null) return;
            var gims = gobj.GetComponent<GameItemMovingScript>();
            gims.MoveTo(pos.x, pos.y, 14, null);
        } */
    }
}