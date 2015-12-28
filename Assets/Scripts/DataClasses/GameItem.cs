using System;
using Assets.Scripts.Enums;
using UnityEngine;
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using FullSerializer;
#endif

namespace Assets.Scripts.DataClasses
{
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject]
#else
    [Serializable]
#endif
    public class GameItem : MonoBehaviour
    {
        private bool _isTouched;
        private Vector3 notTouchedPosition;

        public bool IsTouched
        {
            get { return _isTouched; }
            set
            {
                if (_isTouched == value || Type == GameItemType._XItem) return;
                _isTouched = value;
                if (value)
                {
                    transform.localScale = new Vector3(transform.localScale.x + 0.2f, transform.localScale.y + 0.2f, transform.localScale.z);
                    notTouchedPosition = transform.localPosition;
                    transform.localPosition = new Vector3(notTouchedPosition.x, notTouchedPosition.y, notTouchedPosition.z - 3);
                }
                else
                {
                    transform.localScale = new Vector3(transform.localScale.x - 0.2f, transform.localScale.y - 0.2f, transform.localScale.z);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 3); ;
                } 
            }
        }
        public Vector3 CurrentPosition;
        public bool IsDraggableWhileMoving;
        public GameItemType Type;
        public GameItemMovingType MovingType;

        // Use this for initialization
        void Start () {
	    
        }
	
        // Update is called once per frame
        void Update ()
        {
            CurrentPosition = transform.localPosition;

            //if (Application.platform == RuntimePlatform.WindowsEditor) return;
            //if (Input.touchCount == 0) return;
            //var touch = Input.GetTouch(0);
            //if (Type != GameItemType.DisabledItem && Type != GameItemType.NullItem && Type != GameItemType._StaticItem && touch.phase == TouchPhase.Began)
            //    Vibration.Vibrate();
        }

        public void Reset()
        {
            IsTouched = false;
            //CurrentPosition = transform.localPosition;
            Type = GameItemType.NullItem;
        }

        public static String GetStandartTextureIDByType(GameItemType type)
        {
            switch (type)
            {
                case GameItemType._1:
                    return "_0";
                case GameItemType._2:
                    return "_1";
                case GameItemType._3:
                    return "_2";
                case GameItemType._4:
                    return "_3";
                case GameItemType._5:
                    return "_4";
                case GameItemType._6:
                    return "_5";
                case GameItemType._7:
                    return "_6";
                case GameItemType._8:
                    return "_7";
                case GameItemType._9:
                    return "_8";
                case GameItemType._10:
                    return "_9";
                case GameItemType._11:
                    return "_10";
                case GameItemType._12:
                    return "_11";
                case GameItemType._13:
                    return "_12";
                case GameItemType._14:
                    return "_13";
                case GameItemType._15:
                    return "_14";
                case GameItemType._16:
                    return "_15";
                case GameItemType._2x:
                    return "_16";
                case GameItemType._ToMoveItem:
                    return "_17";
                case GameItemType._XItem:
                    return "_18";
                default:
                    return String.Empty;
            }
        }
    }
}
