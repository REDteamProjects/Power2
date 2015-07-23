using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.DataClasses
{
    [Serializable]
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
    }

    [Serializable]
    public class Mode11RhombusPlaygroundSavedata : RhombusPlaygroundSavedata
    {
        public override string FileName
        {
            get { return "/Mode11RhombusPlayground.sdf"; }
        }
    }
}
