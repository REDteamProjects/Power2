using System;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPlaygroundSavedata
    {
        DifficultyLevel Difficulty { get; set; }

        ProgressBarState ProgressBarStateData { get; set; }

        String FileName { get; }

        GameItemType[][] Items { get; set; }

        GameItemMovingType[][] MovingTypes { get; set; }

        Int32 Score { get; set; }

        Int32 MovesCount { get; set; }

        float CurrentPlaygroundTime { get; set; }

        void ResetDynamicPart();
    }
}
