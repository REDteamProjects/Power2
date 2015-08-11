using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ProgressBarState
    {
        public float State;
        public float Upper;
        public float Multiplier;
    }
}
