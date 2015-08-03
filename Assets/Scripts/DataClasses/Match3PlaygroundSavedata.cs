using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.DataClasses
{
    [Serializable]
    public class ModeMatch3PlaygroundSavedata : IPlaygroundSavedata
    {
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
