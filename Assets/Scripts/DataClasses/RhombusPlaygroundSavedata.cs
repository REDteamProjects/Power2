using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using FullSerializer;
#endif

namespace Assets.Scripts.DataClasses
{
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject(Converter = typeof(SquarePlaygroundSavedataConverter))]
#else
    [Serializable]
#endif
    public class RhombusPlaygroundSavedata  : IPlaygroundSavedata
    {
        public virtual string FileName
        {
            get { return "/RhombusPlaygroundDefault.sdf"; }
        }

        public GameItemType[][] Items
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }

        public GameItemType MaxInitialElementType
        {
            get;
            set;
        }

        public float CurrentPlaygroundTime
        {
            get;
            set;
        }

        public void ResetDynamicPart()
        {
            Items = null;
            MaxInitialElementType = GameItemType.NullItem;
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

#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject(Converter = typeof(SquarePlaygroundSavedataConverter))]
#else
    [Serializable]
#endif
    public class Mode11RhombusPlaygroundSavedata : RhombusPlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/Mode11RhombusPlayground.sdf"; }
        }
    }
}
