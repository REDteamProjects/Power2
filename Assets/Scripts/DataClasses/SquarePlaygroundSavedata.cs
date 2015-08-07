using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.DataClasses
{
    [Serializable]
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
    [Serializable]
    public class Mode6x6SquarePlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/SquarePlayground6x6.sdf"; }
        }
    }

    [Serializable]
    public class Mode8x8SquarePlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/SquarePlayground8x8.sdf"; }
        }
    }

    [Serializable]
    public class ModeDropsPlaygroundSavedata : SquarePlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/DropsPlaygroundDefault.sdf"; }
        }
    }
}
