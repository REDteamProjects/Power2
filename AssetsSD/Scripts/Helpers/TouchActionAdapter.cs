using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataClasses;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class TouchActionAdapter : MonoBehaviour
    {
        private static bool MouseActive;

        public static IList<GameTouch> GetTouch()
        {
            var result = new List<GameTouch>();

            if (Input.touchCount != 0 && !PauseButtonScript.PauseMenuActive) 
            {
                result.AddRange(Input.touches.Select(t => new GameTouch{FingerId = t.fingerId, OriginalPosition = t.position, Phase = t.phase}));
                return result;
            }
            #if UNITY_EDITOR || UNITY_STANDALONE

            if (Input.GetMouseButtonDown(0) && !PauseButtonScript.PauseMenuActive)
            {
                result.Add(new GameTouch
                {
                    Phase = TouchPhase.Began,
                    FingerId = 0,
                    OriginalPosition = Input.mousePosition
                });

                MouseActive = true;
                //LogFile.Message("MouseButtonDown");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!MouseActive) return result;
                result.Add(new GameTouch
                {
                    Phase = TouchPhase.Ended,
                    FingerId = 0,
                    OriginalPosition = Input.mousePosition
                });
                MouseActive = false;

            }
            else if (Input.GetMouseButton(0))
            {
                result.Add(new GameTouch
                {
                    Phase = TouchPhase.Moved,
                    FingerId = 0,
                    OriginalPosition = Input.mousePosition
                });
            }

            #endif

            return result;
        }
    }
}
