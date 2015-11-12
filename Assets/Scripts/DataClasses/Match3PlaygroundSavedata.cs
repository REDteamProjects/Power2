using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using FullSerializer;
#endif

namespace Assets.Scripts.DataClasses
{
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject(Converter = typeof(ModeMatch3PlaygroundSavedataConverter))]
#else
    [Serializable]
#endif
    public class ModeMatch3PlaygroundSavedata : IPlaygroundSavedata
    {
        public GameItemType[][] Items
        {
            get;
            set;
        }

        public GameItemMovingType[][] MovingTypes
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }

        public string FileName
        {
            get { return "/Match3PlaygroundDefault.sdf"; }
        }

        public float CurrentPlaygroundTime
        {
            get;
            set;
        }

        public void ResetDynamicPart()
        {
            Items = null;
            Score = 0;
        }

        public DifficultyLevel Difficulty
        {
            get;
            set;
        }

        public ProgressBarState ProgressBarStateData
        {
            get;
            set;
        }
    }
}
