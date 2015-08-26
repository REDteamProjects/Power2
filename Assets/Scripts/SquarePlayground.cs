using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public delegate void LabelAnimationFinishedDelegate();
    class SquarePlayground : MonoBehaviour, IPlayground
    {
        private static readonly System.Object _disabledItem = new object();
        private readonly AutoResetEvent _callbackReady = new AutoResetEvent(false);
        //private readonly ManualResetEvent _dropsReady = new ManualResetEvent(false);
        private const float HintDelayTime = 3;

        private volatile int _callbacksCount;
        private int _dropsCount;
        private RealPoint _item00;
        private float _timeCounter = -1;
        private float _mixTimeCounter = -1;
        private const float _mixTimeCounterSize = 16; //every 16 seconds field mixes in veryhard difficulty mode
        //private float _moveTimer = 8;
        //private float _moveTimerMultiple = 10;
        private GameObject _selectedPoint1;
        private GameObject _selectedPoint2;
        private Point _selectedPoint1Coordinate;
        private Point _selectedPoint2Coordinate;
        private int _chainCounter;
        private int _score;
        private float _currentTime;

        protected static readonly System.Random RandomObject = new System.Random();
        protected GameItemType MaxType = GameItemType._3;
        
        protected const int maxAdditionalItemsCount = 2;
        protected int DropDownItemsCount;
        protected int XItemsCount;
        private bool _isGameOver;

        public virtual IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<SquarePlayground>.Preferenses; }
        }

        public bool IsGameOver
        {
            get { return _isGameOver; }
            set
            {
                if (value == _isGameOver) return;
                _isGameOver = value;
                if (value)
                    SavedataHelper.SaveData(SavedataObject);
            }
        }

        public int AdditionalItemCost { get { return 222; } }

        protected GameItemType MinType
        {
            get { return MaxType - FieldSize > GameItemType._1 ? MaxType - FieldSize : GameItemType._1; }
        }

        protected int CallbacksCount
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
                    CallbackReady.Reset();
                    //LogFile.Message("_callbackReady.Reset()");
                }
                else
                {
                    CallbackReady.Set();
                    LogFile.Message("_callbackReady.Set()");
                }
            }
        }

        public virtual float ScaleMultiplyer
        {
            //get { return 0.512f; }
            get { return 1; }
        }

        public virtual String ItemPrefabName { get { return "Prefabs/Standard/GameItem"; } }
        
        public virtual bool AreStaticItemsDroppable { get { return false; } }
        
        public virtual bool isDisabledItemActive { get { return false; } }
        
        public System.Object DisabledItem { get { return _disabledItem; } }
        
        public Point SelectedPoint1
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
        }
        
        public Point SelectedPoint2
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
        }
        
        public GameItemType MaxInitialElementType
        {
            get { return MaxType; }
            set
            {
                if (MaxType == value) return;
                MaxType = value;
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                //var stat = GetComponent<Game>().Stats;
                //if (stat != null && stat.CurrentItemType < MaxType)
                if (Preferenses.CurrentItemType < MaxType)
                    Preferenses.CurrentItemType = MaxType;
				switch(MaxType)
                {
                    case GameItemType._7:
                        Game.Difficulty = DifficultyLevel.medium;
                        var mediumlabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                        var mediumlabel = mediumlabelObject.GetComponent<LabelShowing>();
                        mediumlabel.transform.SetParent(transform);
                        mediumlabel.ShowScalingLabel(new Vector3(0, 0, -4), "Difficulty raised!", Color.white, GameColors.BackgroundColor, 60, 90, null, true);
                        break;
                    case GameItemType._10:
                        Game.Difficulty = DifficultyLevel.hard;
                        var hardlabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                        var hardlabel = hardlabelObject.GetComponent<LabelShowing>();
                        hardlabel.transform.SetParent(transform);
                        hardlabel.ShowScalingLabel(new Vector3(0, 0, -4), "Difficulty raised!", Color.white, Color.gray, 60, 90, null, true);
                        while (XItemsCount < maxAdditionalItemsCount)
                            {
                              int col;
                              int row;
                              while ((col = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1) > (row = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1)) { }
                              RemoveGameItem(col, row, (item, r) =>
                              {
                                  Items[col][row] = GenerateGameItem(GameItemType._XItem, col, row, Vector2.zero, false, null, null, GameItemMovingType.Static);
                              });
                              XItemsCount++;
                            }
                        break;
                    case GameItemType._13:
                        Game.Difficulty = DifficultyLevel.veryhard;
                        var veryhardlabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                        var veryhardlabel = veryhardlabelObject.GetComponent<LabelShowing>();
                        veryhardlabel.transform.SetParent(transform);
                        veryhardlabel.ShowScalingLabel(new Vector3(0, 0, -4), "Difficulty raised!", Color.white, Color.gray, 60, 90, null, true);
                        MixTimeCounter = _mixTimeCounterSize;
                        break;
                }
                if ((int) MaxType > FieldSize)
                {
                    DestroyElements(MinType);
                    //GenerateField();
                }
                ShowMaxInitialElement();
            }
        }
        
        public void ShowMaxInitialElement()
        {
            var fg = GameObject.Find("/Foreground");
            var cmi = GameObject.Find("/Foreground/MaximumItem");
            var gobj = Instantiate(Resources.Load(ItemPrefabName + MaxType)) as GameObject;
            if (gobj == null) return;
            gobj.transform.SetParent(fg.transform);
            gobj.transform.localPosition = new Vector3(0, 350f + GameItemSize * 5f, 0);
            gobj.transform.localScale = new Vector3(80, 80);
            gobj.name = "MaximumItem";
            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y));
            c.MoveTo(null, gobj.transform.localPosition.y - GameItemSize * 6, 2f, (item, result) =>
            {
                if (!result) return;
                if (cmi != null)
                    Destroy(cmi);
            });
        }
        
        public void DestroyElements(GameItemType withType)
        {
            LogFile.Message("Destroy elements above " + withType);
            var pointsBank = 0;
            for (var i = 0; i < FieldSize; i++)
            {
                for (var j = 0; j < FieldSize; j++)
                    if (Items[i][j] != null && Items[i][j] != DisabledItem)
                    {
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        var item = gobj.GetComponent<GameItem>();
                        if (item.Type == GameItemType.DisabledItem || item.Type == GameItemType.NullItem || item.Type > withType) continue;
                        LogFile.Message("Item destroied: " + item.Type);
                        pointsBank += (int) Math.Pow(2, (double) item.Type);
						RemoveGameItem(i, j);
                        //Items[i][j] = null;
                        //Destroy(gobj);
                    }
            }
           
            RisePoints(pointsBank);
            if (ProgressBar != null)
                ProgressBar.AddTime(pointsBank * 2);
        }
        
        public virtual IPlaygroundSavedata SavedataObject
        {
            get { return null; }
        }
        
        public virtual int FieldSize { get { return 0; } }
        
        public float DeltaToExchange { get { return GameItemSize / 2f; } }
        
        public float DeltaToMove { get { return GameItemSize / 4f; } }
        
        public object[][] Items { get; set; }
        
        public virtual float GameItemSize { get { return 0f; } }
        
        public virtual RealPoint InitialGameItemPosition { get { return null; } }
        
        public Dictionary<MoveDirections, Vector2> AvailableMoveDirections { get; protected set; }
        
        public int CurrentScore
        {
            get { return GetComponent<PointsUpdater>().CurrentScore; }
            set
            {
                var c = GetComponent<PointsUpdater>();
                c.RisePoints(value);

                if (Preferenses.ScoreRecord < c.CurrentScore)
                    Preferenses.ScoreRecord = c.CurrentScore;
            }
        }

        public void UpdateTime()
        {
            //var stat = GetComponent<Game>().Stats;
            //if (stat != null && stat.LongestSession < CurrentTime + Time.time)
            if (Preferenses.LongestSession < CurrentTime + Time.timeSinceLevelLoad)
                Preferenses.LongestSession = CurrentTime + Time.timeSinceLevelLoad;
        }

        public float CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime == value) return;
                    _currentTime = value;  
            }
        }
        
        public int DropsCount
        {
            get
            {
                return _dropsCount;
            }
            set
            {
                if (value == _dropsCount)
                {
                    LogFile.Message("value == _dropsCount: " + value);
                    return;
                }
                _dropsCount = value;
                LogFile.Message("DropsCount = " + value);
                //if (value > 0)
                //{
                //    DropsReady.Reset();
                //    //LogFile.Message("_callbackReady.Reset()");
                //}
                //else
                //{
                //    DropsReady.Set();
                //    //LogFile.Message("DropsReady.Set()");
                //}
            }
        }
        
        public int ChainCounter
        {
            get { return _chainCounter; }
            set
            {
                if (_chainCounter == value) return;
                _chainCounter = value;
                //var stat = GetComponent<Game>().Stats;
                //if (stat != null && stat.MaxMultiplier < _chainCounter)
                if (Preferenses.MaxMultiplier < _chainCounter)
                    Preferenses.MaxMultiplier = _chainCounter;
            }
        }
        
        public float TimeCounter
        {
            get { return _timeCounter; }
            set { _timeCounter = value; }
        }

		public float MixTimeCounter
        {
            get { return _mixTimeCounter; }
            set { _mixTimeCounter = value; }
        }
        //public float MoveTimer
        //{
        //    get { return _moveTimer; }
        //    set { _moveTimer = value; }
        //}

        public RealPoint Item00
        {
            get { return _item00; }
            set { _item00 = value; }
        }

        protected AutoResetEvent CallbackReady
        {
            get { return _callbackReady; }
        }

        //protected ManualResetEvent DropsReady
        //{
        //    get { return _dropsReady; }
        //}

        protected Point SelectedPoint1Coordinate
        {
            get { return _selectedPoint1Coordinate; }
        }

        protected Point SelectedPoint2Coordinate
        {
            get { return _selectedPoint2Coordinate; }
        }

        public PlaygroundProgressBar ProgressBar
        {
            get
            {
                PlaygroundProgressBar pb = null;
                try
                {
                    pb = GetComponent<PlaygroundProgressBar>();
                }
                catch (Exception ex)
                {
                    LogFile.Message(ex.Message + " " + ex.StackTrace);
                }
                return pb;
            }
        }

        public SquarePlayground(Dictionary<MoveDirections, Vector2> dictionary)
        {
            AvailableMoveDirections = dictionary;
        }

        public SquarePlayground() : this(new Dictionary<MoveDirections, Vector2>
            {
                {MoveDirections.Up, new Vector2(0, 1)},
                {MoveDirections.Down, new Vector2(0, -1)},
                {MoveDirections.Left, new Vector2(-1, 0)},
                {MoveDirections.Right, new Vector2(1, 0)},
                {MoveDirections.UL, new Vector2(-1, 1)},
                {MoveDirections.UR, new Vector2(1, 1)},
                {MoveDirections.DL, new Vector2(-1, -1)},
                {MoveDirections.DR, new Vector2(1, -1)},
            }){}


        protected void RemoveAdditionalItems()
        {
            for (var i = 0; i < FieldSize; i++)
                if (Items[i][FieldSize - 1] != null && Items[i][FieldSize - 1] != DisabledItem)
                {
                    var gobj = Items[i][FieldSize - 1] as GameObject;
                    if (gobj == null ||
                        (gobj.GetComponent<GameItem>().Type != GameItemType._DropDownItem ||
                         gobj.GetComponent<GameItemMovingScript>().IsMoving)) continue;
                    var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var pointsLabel = o.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);
                        pointsLabel.ShowScalingLabel(new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y + GameItemSize / 2, gobj.transform.localPosition.z - 1),
                            "+" + 222, Color.white, Color.gray, 60, 90, null, true);
                    }
                    RisePoints(AdditionalItemCost);
                    if (ProgressBar != null)
                        ProgressBar.AddTime(AdditionalItemCost * 2);

                    Destroy(gobj);
                    Items[i][FieldSize - 1] = null;
                    DropDownItemsCount--;
                }
        }

        protected virtual void Update()
        {
            if (IsGameOver) return;

            if (CallbacksCount == 0)
                Drop();

            if (!(TimeCounter >= 0)) return;
            if (TimeCounter > HintDelayTime && _selectedPoint1 == null && _selectedPoint2 == null)
            {
                var parentGobj = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                if (parentGobj == null) return;
                _selectedPoint1 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint1 == null)
                {
                    LogFile.Message("SelectedPoint1 initialization failed");
                    return;
                }
                _selectedPoint1.transform.SetParent(parentGobj.transform);
                _selectedPoint1.transform.localScale = new Vector3(1.3707f, 1.3707f);
                _selectedPoint1.transform.localPosition = new Vector3(0, 0, -1);
                parentGobj = Items[SelectedPoint2Coordinate.X][SelectedPoint2Coordinate.Y] as GameObject;
                if (parentGobj == null) return;
                _selectedPoint2 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint2 == null)
                {
                    LogFile.Message("SelectedPoint2 initialization failed");
                    return;
                }
                _selectedPoint2.transform.SetParent(parentGobj.transform);
                _selectedPoint2.transform.localScale = new Vector3(1.3707f, 1.3707f);
                _selectedPoint2.transform.localPosition = new Vector3(0, 0, -1);
            }
            TimeCounter += Time.deltaTime;
			if (Game.Difficulty >= DifficultyLevel.veryhard)
            {
                MixTimeCounter -= Time.deltaTime;
                if(MixTimeCounter <= 0)
                {
                    GenerateField(false, true);
                    MixTimeCounter = _mixTimeCounterSize;
                }
            }
		}

        public GameObject InstantiateGameItem(GameItemType itemType, Vector3 localPosition, Vector3 localScale, GameItemMovingType movingType = GameItemMovingType.Standart)
        {
            var newgobj = Instantiate(Resources.Load(ItemPrefabName + itemType)) as GameObject;
            if (newgobj == null) return null;
            var rectTransform = newgobj.transform as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = new Vector2(GameItemSize * 0.9375f, GameItemSize * 0.9375f);
            newgobj.transform.SetParent(transform);
            newgobj.transform.localPosition = localPosition;
            newgobj.transform.localScale = localScale;
            newgobj.GetComponent<GameItem>().MovingType = movingType;
            return newgobj;
        }
        
        public GameObject GenerateGameItem(GameItemType itemType, int i, int j, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, 
            GameItemMovingType movingType = GameItemMovingType.Standart)
        {
            if (!generateOn.HasValue)
                generateOn = new Vector2(0, FieldSize - j);
            if (Item00 == null) 
                Item00 = InitialGameItemPosition;
            var cell = GetCellCoordinates(i, j);
            var gobj = InstantiateGameItem(itemType, new Vector3(
                (float)Math.Round(cell.x + generateOn.Value.x * GameItemSize, 2),
                (float)Math.Round(Item00.Y + generateOn.Value.y * GameItemSize, 2),
                Item00.Z), Vector3.zero, movingType);
            //if (generateOn == Vector2.zero) //TODO: WHAT?
            //{
            //    var c = gobj.GetComponent<GameItemScalingScript>();
            //    c.ScaleTo(new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f), 8, null);
            //}
            //else
            //{
                var c = gobj.GetComponent<GameItemMovingScript>();
                LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y - 6 * GameItemSize));
                CallbacksCount++;
                c.MoveTo(cell.x, cell.y, dropSpeed.HasValue ? dropSpeed.Value : 10 - i % 2 + j * 1.5f, (item, result) =>
                {
                    CallbacksCount--;
                    if (movingCallback != null)
                        movingCallback(item, result);
                    else
                    {
                        if (!result) return;
                        if (CallbacksCount == 0)
                            ClearChains();
                    }
                }, new Vector2(Item00.X, Item00.Y + GameItemSize / 2), new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f), isItemDirectionChangable, 
                null, Power2Sounds.Drop);
            //}
            return gobj;
        }
        
        public GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType movingType = GameItemMovingType.Standart)
        {
            //var minType = MaxType - FieldSize;
            var newType = RandomObject.Next((int)MaxType > FieldSize ? (int)MinType + 1 : (int)GameItemType._1, (int)MaxInitialElementType + 1);
            if (deniedTypes == null || deniedTypes.Count <= 0)
                return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
            while (deniedTypes.Contains((GameItemType)newType))
                newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
        }
        
        //private bool IsEndPoint(IEnumerable<Line> lines, int currentX, int currentY)
        //{
        //    return lines.Any(l => (l.X2 == currentX && l.Y2 == currentY));
        //}
        
        public virtual bool IsInAnotherLine(IEnumerable<Line> lines, int currentX, int currentY)
        {
            return lines.Count(l => (l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY)) > 1;
        }
        
        public bool MatchType(int x, int y, GameItemType itemType)
        {
            // убедимся, что фишка не выходит за пределы поля    
            if ((x < 0) || (x >= FieldSize) || (y < 0) || (y >= FieldSize) || Items[x][y] == null || Items[x][y] == DisabledItem) return false;
            var gobj = Items[x][y] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            return gi != null && gi.Type == itemType;
        }
        
        public virtual Vector3 GetCellCoordinates(int col, int row)
        {
            var roundedX = Math.Round(Item00.X + Convert.ToSingle(col) * GameItemSize, 2);
            var roundedY = Math.Round(Item00.Y - GameItemSize * Convert.ToSingle(row), 2);
            return new Vector3((float)roundedX, (float)roundedY, Item00.Z);
        }
        
        public virtual bool MatchPattern(GameItemType itemType, Point firstItem, Point secondItem, IEnumerable<Point> pointForCheck)
        {
            var mainType = MatchType(firstItem.X + secondItem.X, firstItem.Y + secondItem.Y, itemType);
            if (!mainType) return false;

            var lineGenerationPoints = pointForCheck.Where(point => MatchType(firstItem.X + point.X, firstItem.Y + point.Y, itemType)).ToList();
            if (!lineGenerationPoints.Any()) return false;

            foreach (var lineGenerationPoint in lineGenerationPoints)
            {
                if (secondItem.X == 0)
                {
                    LogFile.Message("secondItem.X == 0 and firstItem.X: " + firstItem.X + " firstItem.Y: " + firstItem.Y +
                                    " secondItem.Y: " + secondItem.Y + "lineGenerationPoint.Y: " + lineGenerationPoint.Y);
                    if (Math.Abs(secondItem.Y) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X,
                            Y = firstItem.Y + (secondItem.Y > 0 ? 1 : -1)
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X,
                            Y = firstItem.Y + (lineGenerationPoint.Y > 0 ? 2 : -1)
                        };
                    }
                }
                else if (secondItem.Y == 0)
                {
                    LogFile.Message("secondItem.Y == 0 and firstItem.X: " + firstItem.X + " firstItem.Y: " + firstItem.Y +
                                    " secondItem.Y: " + secondItem.Y + "lineGenerationPoint.Y: " + lineGenerationPoint.Y);
                    if (Math.Abs(secondItem.X) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (secondItem.X > 0 ? 1 : -1),
                            Y = firstItem.Y
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (lineGenerationPoint.X > 0 ? 2 : -1),
                            Y = firstItem.Y
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
        
        public virtual IList<Line> GetAllLines()
        {
            //LogFile.Message("Finding lines...");
            var list = new List<Line>();
            for (var row = 0; row < FieldSize; row++)
                for (var col = 0; col < FieldSize - 2; col++)
                    if (Items[col][row] != null && Items[col][row] != DisabledItem && (!(Items[col][row] as GameObject).GetComponent<GameItemMovingScript>().IsMoving
                        || (Items[col][row] as GameObject).GetComponent<GameItem>().IsDraggableWhileMoving))//TODO: Attention
                    {
                        var match = CheckForLine(col, row, LineOrientation.Horizontal);
                        if (match <= 2) continue;
                        list.Add(new Line { X1 = col, Y1 = row, X2 = col + match - 1, Y2 = row, Orientation = LineOrientation.Horizontal });
                        col += match - 1;
                    }
            for (var col = 0; col < FieldSize; col++)
                for (var row = 0; row < FieldSize - 2; row++)
                    if (Items[col][row] != null && Items[col][row] != DisabledItem && (!(Items[col][row] as GameObject).GetComponent<GameItemMovingScript>().IsMoving
                        || (Items[col][row] as GameObject).GetComponent<GameItem>().IsDraggableWhileMoving))
                    {
                        var match = CheckForLine(col, row, LineOrientation.Vertical);
                        if (match <= 2) continue;
                        list.Add(new Line { X1 = col, Y1 = row, X2 = col, Y2 = row + match - 1, Orientation = LineOrientation.Vertical });
                        row += match - 1;
                    }
            if (list.Count > 0)
                LogFile.Message("Find "+ list.Count + " lines");
            return list;
        }
        
        public virtual int CheckForLine(int x, int y, LineOrientation orientation)
        {
            var count = 1;
            if (Items[x][y] == null || Items[x][y] == DisabledItem) return count;
            switch (orientation)
            {
                case LineOrientation.Horizontal:
                    for (var i = 1; x + i < FieldSize; i++)
                    {
                        //var goItem = (Items[x + i][y] as GameObject);
                        if (Items[x + i][y] == null || Items[x + i][y] == DisabledItem) break; //|| (
                            //goItem != null && goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)) break;
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x + i][y] as GameObject;
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
                case LineOrientation.Vertical:
                    for (var i = 1; y + i < FieldSize; i++)
                    {
                        //var goItem = (Items[x][y + i] as GameObject);
                        if (Items[x][y + i] == null || Items[x][y + i] == DisabledItem) break; // || (
                            //goItem != null && goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)) break;
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x][y + i] as GameObject;
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
        
        public virtual int ClearChains()
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
                var toObj = Items[l.X2][l.Y2] as GameObject;
                var toCell = GetCellCoordinates(l.X2, l.Y2);
                var pointsMultiple = 1;
                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;

                    for (var j = l.Y2 - 1; j >= l.Y1; j--)
                    {
                        if (Items[l.X1][j] == null || Items[l.X1][j] == DisabledItem)
                        {
                            //LogFile.Message("Items[i][l.Y1] == null");
                            continue;
                        }
                        if (IsInAnotherLine(lines, l.X1, j))
                        {
                            //LogFile.Message("Items[" + l.X1 + "][" + j + "] = null;");
                            continue;
                        }
                        var gobj = Items[l.X1][j] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        if (c.IsMoving) continue;
                        var cX = l.X1;
                        var cY = j;

                        CallbacksCount++;
                        Items[cX][cY] = null;
                        c.MoveTo(null, toCell.y, 14, (item, result) =>
                        {
                            LogFile.Message(cX + " " + cY);
                            CallbacksCount--;
                            if (!result) return;
                           
                            Destroy(item);
                        });

                    }
                }
                else
                {
                    pointsMultiple += l.X2 - l.X1 - 2;

                    for (var i = l.X2 - 1; i >= l.X1; i--)
                    {
                        if (Items[i][l.Y1] == null || Items[i][l.Y1] == DisabledItem)
                        {
                            //LogFile.Message("Items[i][l.Y1] == null");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, l.Y1))
                        {
                            //LogFile.Message("Items[" + i + "][" + l.Y1 + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][l.Y1] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        if (c.IsMoving) continue;

                        var cX = i;
                        var cY = l.Y1;

                        CallbacksCount++;
                        Items[cX][cY] = null;
                        c.MoveTo(toCell.x, null, 14, (item, result) =>
                        {
                            LogFile.Message(cX + " " + cY);
                            CallbacksCount--;
                            if (!result) return;

                            Destroy(item);                            
                        });
                    }
                }
                if (toObj != null)
                {
                    var toGi = toObj.GetComponent<GameItem>();
                    if (toGi.Type == MaxInitialElementType + 1)
                        MaxInitialElementType++;
                    var newgobjtype = toGi.Type + 1;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell,
                        new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 0f));

                    Items[l.X2][l.Y2] = null;
                    Destroy(toObj);
                    Items[l.X2][l.Y2] = newgobj;
                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                    var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var pointsLabel = o.GetComponent<LabelShowing>();
                        
                        if (newgobjtype <= MaxInitialElementType)
                        {
                            pointsBank += points;
                            pointsLabel.ShowScalingLabel(newgobj,//new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, -3),
                                "+" + points, GameColors.ItemsColors[newgobjtype], Color.gray, 60, 90, null, true);
                        }
                        else
                        {
                            pointsBank += 2 * points;
                            pointsLabel.ShowScalingLabel(newgobj,//new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, -3),
                                "+" + points + "x2", GameColors.ItemsColors[newgobjtype], Color.gray, 60, 90, null, true);
                        }
                    }
                    IsGameOver = newgobjtype == GameItemType._Gameover;
                }
                lines.Remove(l);
                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected");
                l = lines.FirstOrDefault();

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected");
            RemoveAdditionalItems();

            pointsBank *= linesCount;
            ChainCounter++;
            RisePoints(pointsBank * ChainCounter);
            
            if (!IsGameOver) return linesCount;
            GenerateGameOverMenu();

			return linesCount;
        }

        public void GenerateGameOverMenu()
        {   
            var fg = GameObject.Find("/Foreground");

            var pausebackground = Instantiate(Resources.Load("Prefabs/PauseBackground")) as GameObject;
            var gameOverLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
 
            if (fg != null)
            {
                pausebackground.transform.SetParent(fg.transform);
                gameOverLabelObject.transform.SetParent(fg.transform); 
            }

            pausebackground.transform.localPosition = Vector3.zero;
            (pausebackground.transform as RectTransform).sizeDelta = Vector2.zero; 

            var gameOverLabel = gameOverLabelObject.GetComponent<LabelShowing>();
            gameOverLabel.ShowScalingLabel(new Vector3(0, 100, -3),
                "Game over", Color.white, Color.gray, 60, 90, null, false, () =>
                {
                    var gameOverMenu = Instantiate(Resources.Load("Prefabs/GameOverMenu")) as GameObject;

                    if (fg != null)
                    {
                        gameOverMenu.transform.SetParent(fg.transform);

                    }
                    gameOverMenu.transform.localScale = Vector3.one;
                    gameOverMenu.transform.localPosition = new Vector3(0, 0, 0);
                });
            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.GameOver, false, true);
        }

        public virtual void Drop()
        {
            if (Items == null) return;

            var generateAfterDrop = true;
            //if (DropsCount == 0) GenerateField(true);

            var counter = 0;

            for (var col = 0; col < FieldSize; col++)
            {
                for (var row = 0; row < FieldSize - 1; row++)
                {
                    if (Items[col][row] == null || Items[col][row] == DisabledItem)
                        continue;
                    
                    var o = Items[col][row] as GameObject;
                    if (o != null && (!AreStaticItemsDroppable && o.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                        continue;

                    //if (DropsCount == 0)
                    //    DropsCount = FieldSize * (FieldSize - 1);

                    if (Items[col][row + 1] != null)
                    {
                        var gameObject1 = Items[col][row + 1] as GameObject;
                        if (gameObject1 != null && gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        {
                            var rowStaticCounter = 1;
                            GameObject o1;
                            while ((row + rowStaticCounter) < FieldSize && (o1 = Items[col][row + rowStaticCounter] as GameObject) != null &&
                                o1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                                rowStaticCounter++;
                            if ((row + rowStaticCounter) >= FieldSize || Items[col][row + rowStaticCounter] != null)
                                continue;
                            var gobjS = Items[col][row] as GameObject;
                            if (gobjS == null) continue;
                            var cS = gobjS.GetComponent<GameItemMovingScript>();
                            if (cS.IsMoving) continue;
                            counter++;
                            Items[col][row + rowStaticCounter] = Items[col][row];
                            Items[col][row] = null;
                            if (!cS.IsMoving) DropsCount++;
                            var colS = col;
                            var rowS = row;
                            cS.MoveTo(null, GetCellCoordinates(col, row + rowStaticCounter).y, 14, (item, result) =>
                            {
                                if (!cS.IsMoving)
                                    DropsCount--;
                                if (!result) return;
                                LogFile.Message("New item droped Items[" + colS + "][" + rowS + "] DC: " + DropsCount);
                            });
                        }
                        if (row + 2 < FieldSize && Items[col][row + 2] == null) generateAfterDrop = false;
                        continue;
                    }
                    
                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;
                    var c = gobj.GetComponent<GameItemMovingScript>();
                    if (c.IsMoving) continue;
                    counter++;
                    Items[col][row + 1] = Items[col][row];
                    Items[col][row] = null;
                    if (row + 2 < FieldSize && Items[col][row + 2] == null) generateAfterDrop = false;//DropsCount++;
                    if (!c.IsMoving) DropsCount++;
                    var col1 = col;
                    var row1 = row;
                    c.MoveTo(null, GetCellCoordinates(col, row + 1).y, 14, (item, result) =>
                    {
                        if (!c.IsMoving) 
                            DropsCount--;
                        if (!result) return;
                        LogFile.Message("New item droped Items[" + col1 + "][" + row1 + "] DC: " + DropsCount);

                        //GenerateField(true);
                    });
                }
            }
            //LogFile.Message("counter " + counter + " DropsCount " + DropsCount);

            if (counter == 0 && DropsCount == 0 && generateAfterDrop)
                GenerateField(true);
        }
        
        public virtual void GenerateField(bool completeCurrent = false, bool mixCurrent = false)
        {        
            if (!mixCurrent)
            {
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnY = 1;
                    var resCol = RandomObject.Next(0, FieldSize);
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        var deniedList = new List<GameItemType>();
                        //var itemsJ = FieldSize - 1 - j;
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        switch (Game.Difficulty)
                        {
                            case DifficultyLevel.medium:
							case DifficultyLevel.hard:
                            case DifficultyLevel.veryhard:
                                if (DropDownItemsCount < maxAdditionalItemsCount && j <= FieldSize / 2)
                                {
                                    var resRow = RandomObject.Next(0, FieldSize);
                                    if (resCol == resRow)
                                    {
                                        Items[i][j] = GenerateGameItem(GameItemType._DropDownItem, i, j, new Vector2(0, generateOnY));
                                        DropDownItemsCount++;
                                        generateOnY++;
                                        continue;
                                    }
                                }
                                break;
                        }
                        if (completeCurrent)
                        {
                            LogFile.Message("New gameItem need to i:" + i + "j: " + j);
                            Items[i][j] = GenerateGameItem(i, j, null, new Vector2(0, generateOnY));
                            generateOnY++;
                            continue;
                        }
                        //Horizontal before
                        if (i > 1 && CheckForLine(i - 2, j, LineOrientation.Horizontal) > 1)
                        {
                            var o = Items[i - 1][j] as GameObject;
                            if (o != null)
                                deniedList.Add(o.GetComponent<GameItem>().Type);
                        }
                        //Horizontal after
                        if (i < FieldSize - 2 && CheckForLine(i + 1, j, LineOrientation.Horizontal) > 1)
                        {
                            var gameObject1 = Items[i + 1][j] as GameObject;
                            if (gameObject1 != null)
                                deniedList.Add(gameObject1.GetComponent<GameItem>().Type);
                        }
                        //Vertical before
                        if (j > 1 && CheckForLine(i, j - 2, LineOrientation.Vertical) > 1)
                        {
                            var o1 = Items[i][j - 1] as GameObject;
                            if (o1 != null)
                                deniedList.Add(o1.GetComponent<GameItem>().Type);
                        }
                        //Vertical after
                        if (j < FieldSize - 2 && CheckForLine(i, j + 1, LineOrientation.Vertical) > 1)
                        {
                            var gameObject2 = Items[i][j + 1] as GameObject;
                            if (gameObject2 != null)
                                deniedList.Add(gameObject2.GetComponent<GameItem>().Type);
                        }
                        Items[i][j] = GenerateGameItem(i, j, deniedList);
                    }
                }
            }
            else
            {
                LogFile.Message("Mix field...");
                var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                if (o != null)
                {
                    var noMovesLabel = o.GetComponent<LabelShowing>();
                    noMovesLabel.transform.SetParent(transform);
                    noMovesLabel.ShowScalingLabel(new Vector3(0, Item00.Y + GameItemSize * 2.5f, -1), "No moves", Color.white, GameColors.BackgroundColor, 60, 90, null, true);
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
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.MixField, false);
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
            
            //if (!isNoLines && _callbackReady.WaitOne(1))
            // ClearChains();
        }
        
        public virtual bool GameItemsExchange(int x1, int y1, ref int x2, ref int y2, float speed, bool isReverse, MovingFinishedDelegate exchangeCallback = null)
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

            if (item1 != null)
            {
                CallbacksCount++;
                item1.GetComponent<GameItemMovingScript>()
                    .MoveTo(position2.x,
                        position2.y,
                        15, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position1.x,
                                        position1.y,
                                        14, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;

                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            else if (CallbacksCount == 0)
                                                ClearChains();
                                        });
                                return;
                            }

                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            else if (CallbacksCount == 0)
                                ClearChains();

                        });
            }

            if (item2 != null)
            {
                CallbacksCount++;
                item2.GetComponent<GameItemMovingScript>()
                    .MoveTo(position1.x,
                        position1.y,
                        15, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position2.x,
                                        position2.y,
                                        14, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;

                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            else if (CallbacksCount == 0)
                                                ClearChains();
                                        });
                                return;
                            }

                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            else if (CallbacksCount == 0)
                                ClearChains();

                        });
            }
            return true;
        }

        public virtual bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            if (!AvailableMoveDirections.ContainsKey(mdir)) return false;
            if (col < 0 || row < 0 || col > FieldSize - 1 || row > Items[col].Length)
                return false;

            var gameObject1 = Items[col][row] as GameObject;
            if (gameObject1 == null || gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) return false;

            switch (mdir)
            {
                case MoveDirections.Up:
                    if (row == 0 || Items[col][row - 1] == DisabledItem)
                        return false;
                    var objectUp = Items[col][row - 1] as GameObject;
                    
                    if (objectUp == null || objectUp.GetComponent<GameItemMovingScript>().IsMoving || objectUp.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Down:
                    if (row == (FieldSize - 1) || Items[col][row + 1] == DisabledItem )
                        return false;
                    var objectDown = Items[col][row + 1] as GameObject;

                    if (objectDown == null || objectDown.GetComponent<GameItemMovingScript>().IsMoving || objectDown.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Left:
                    if (col == 0 || Items[col - 1][row] == DisabledItem)
                        return false;
                    var objectLeft = Items[col - 1][row] as GameObject;
                    
                    if (objectLeft == null || objectLeft.GetComponent<GameItemMovingScript>().IsMoving || objectLeft.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Right:
                    if (col == (FieldSize - 1) || Items[col + 1][row] == DisabledItem)
                        return false;
                    var objectRight = Items[col + 1][row] as GameObject;
                    
                    if (objectRight == null || objectRight.GetComponent<GameItemMovingScript>().IsMoving || objectRight.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
            }
            return true;
        }
        
        public virtual bool IsPointInLine(int col, int row)
        {
            var hor = 0;
            //Horizontal before
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            for (var i = col - 1; i >= 0; i--)
            {
                var gobj2 = Items[i][row] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break; 
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    hor++;
                else
                    break;
            }
            if (hor < 3)
            {
                for (var i = col + 1; i < FieldSize; i++)
                {
                    var gobj2 = Items[i][row] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break; 
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        hor++;
                    else
                        break;
                }
            }
            else return true;
            if (hor > 1) return true;
            var vert = 0;
            //Vertical before
            for (var j = row - 1; j >= 0; j--)
            {
                var gobj2 = Items[col][j] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break; 
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    vert++;
                else break;
            }
            if (vert < 3)
            {
                for (var j = row + 1; j < FieldSize; j++)
                {
                    var gobj2 = Items[col][j] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break; 
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        vert++;
                    else break;
                }
            }
            else return true;
            return vert > 1;
        }
        
        public virtual bool TryMakeMove(int x1, int y1, int x2, int y2)
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
        }
        
        public virtual bool CheckForPossibleMoves()
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
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = 0 }, new List<Point>
                    {
                        new Point{X = -2, Y = 0}, new Point{X = -1, Y = -1}, new Point{X = -1, Y = 1}, new Point{X = 2, Y = -1},
                        new Point{X = 2, Y = 1}, new Point{X = 3, Y = 0}
                    }
                        ))
                        return true;
                    // возможна горизонтальная, две по разным сторонам      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = 0 }, new List<Point>
                    {
                        new Point{X = 1, Y = -1}, new Point{X = 1, Y = 1}
                    }
                        ))
                        return true;
                    // возможна вертикальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 0, Y = 1 }, new List<Point>
                    {
                        new Point{X = 0, Y = -2}, new Point{X = -1, Y = -1}, new Point{X = 1, Y = -1}, new Point{X = -1, Y = 2},
                        new Point{X = 1, Y = 2}, new Point{X = 0, Y = 3}
                    }
                        ))
                        return true;
                    // возможна вертикальная, две по разным сторонам       
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 0, Y = 2 }, new List<Point>
                    {
                        new Point{X = -1, Y = 1}, new Point{X = 1, Y = 1}
                    }
                        ))
                        return true;
                }
            return false;
        }
        
        protected void RisePoints(int points)
        {
            if (points == 0)
            {
                LogFile.Message("Rised 0 ponts");
                return;
            }
            points *= (int)Game.Difficulty;
            LogFile.Message("Points " + points);

            CurrentScore += points;
        }

        public void ShowComboLabel(int count)
        {
            var labelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (labelObject == null) return;
            var comboLabel = labelObject.GetComponent<LabelShowing>();
            comboLabel.name = "ComboLabel";
            comboLabel.transform.SetParent(transform);

            comboLabel.transform.RotateAround(Vector3.zero, Vector3.forward, count%2 == 0 ? 30 : -30);
            comboLabel.ShowScalingLabel(new Vector3(count%2 == 0 ? -9 : 9, Item00.Y + GameItemSize * 2.5f, -1),
                "Combo x" + count + " lines!", new Color(240, 223, 206), new Color(240, 223, 206), 10, 50, null, true);
            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Combo, false);
        }

        public virtual void RevertMovedItem(int col, int row)
        {
            var gobj = Items[col][row] as GameObject;
            var pos = GetCellCoordinates(col, row);
            LogFile.Message("Revert item to place: " + pos.x + " " + pos.y);
            if (gobj == null) return;
            var gims = gobj.GetComponent<GameItemMovingScript>();
            gims.MoveTo(pos.x, pos.y, 14, null);
        }

        public virtual void ResetPlayground()
        {
            for (var i = 0; i < FieldSize; i++)
                for (var j = 0; j < FieldSize; j++)
                {
                    var item = Items[i][j] as GameObject;
                    if (item != null)
                    {
                        var gims = item.GetComponent<GameItemMovingScript>();
                        if (gims != null) gims.CancelMoving(true);
                    }
                    Items[i][j] = null;
                    Destroy(item);
                }
            Items = null;
            MaxType = GameItemType.NullItem;
            _score = 0;
        }

        public void RemoveGameItem(int i, int j, MovingFinishedDelegate removingCallback = null)
        {
            var giss = (Items[i][j] as GameObject).GetComponent<GameItemScalingScript>();
            var toSize = GameItemSize / ScaleMultiplyer / 4;
			CallbacksCount++;
            giss.ScaleTo(new Vector3(toSize, toSize, 0), 8, (item, r) =>
            {
                Items[i][j] = null;
                Destroy(item);
                if (removingCallback != null)
                    removingCallback(item, r);
                CallbacksCount--;
            });
        }
    }
}