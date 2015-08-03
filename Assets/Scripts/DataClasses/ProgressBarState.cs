using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataClasses
{
    [Serializable]
    public class ProgressBarState
    {
        public float State;
        public float Upper;
        public float Multiplier;
    }
}
