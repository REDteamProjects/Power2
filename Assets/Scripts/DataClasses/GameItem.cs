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
                if (_isTouched == value) return;
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
    }
}
