using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataClasses
{
    public class GameTouch
    {
        public TouchPhase Phase;
        public Vector2 OriginalPosition;
        public Int32 FingerId;
    }
}
