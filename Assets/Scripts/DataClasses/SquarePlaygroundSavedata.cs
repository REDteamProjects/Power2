using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
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
    public abstract class SquarePlaygroundSavedata : IPlaygroundSavedata
    {
        public virtual string FileName
        {
            get { return "/SquarePlaygroundDefault.sdf"; }
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
    [fsObject]
#else
    [Serializable]
#endif
    public class Mode6x6SquarePlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/SquarePlayground6x6.sdf"; }
        }
    }

#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject]
#else
    [Serializable]
#endif
    public class Mode8x8SquarePlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/SquarePlayground8x8.sdf"; }
        }
    }

#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
    [fsObject]
#else
    [Serializable]
#endif
    public class ModeDropsPlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/DropsPlaygroundDefault.sdf"; }
        }
    }
}
