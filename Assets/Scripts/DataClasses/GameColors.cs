﻿using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.DataClasses
{
    public class GameColors
    {
        public static Dictionary<GameItemType, Color> ItemsColors = new Dictionary<GameItemType, Color>();
        public static Dictionary<DifficultyLevel, Color> DifficultyLevelsColors = new Dictionary<DifficultyLevel, Color>();
        public static readonly Color Default = new Color(0.94f, 0.9f, 0.82f);

        public static Color BackgroundColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return new Color(0.157f, 0.157f, 0.157f);
                    case GameTheme.light:
                        return Default; 
                }
                return Default;
            }
        }
        public static Color ForegroundColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return new Color(0.6f, 0.416f, 0.231f);
                    case GameTheme.light:
                        return new Color(0.16f, 0.16f, 0.16f);
                }
                return new Color(0.6f, 0.416f, 0.231f);
            }
        }

        public static Color ForegroundButtonsColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return new Color(1f, 1f, 1f);
                    case GameTheme.light:
                        return new Color(0.16f, 0.16f, 0.16f);
                }
                return new Color(1f, 1f, 1f);
            }
        }

        static GameColors()
        {
            ItemsColors.Add(GameItemType._1, new Color(0.882f, 0.863f, 0,784f));
            ItemsColors.Add(GameItemType._2, new Color(0.902f, 0.824f, 0.588f));
            ItemsColors.Add(GameItemType._3, new Color(0.98f, 0.824f, 0.43f));
            ItemsColors.Add(GameItemType._4, new Color(0.902f, 0.863f, 0.275f));
            ItemsColors.Add(GameItemType._5, new Color(0.96f, 0.804f, 0.04f));
            ItemsColors.Add(GameItemType._6, new Color(1f, 0.686f, 0.02f));
            ItemsColors.Add(GameItemType._7, new Color(0.902f, 0.51f, 0.12f));
            ItemsColors.Add(GameItemType._8, new Color(0.84f, 0.35f, 0.18f));
            ItemsColors.Add(GameItemType._9, new Color(0.75f, 0.18f, 0.04f));
            ItemsColors.Add(GameItemType._10, new Color(0.94f, 0.16f, 0f));
            ItemsColors.Add(GameItemType._11, new Color(0.608f, 0f, 0f));
            ItemsColors.Add(GameItemType._12, new Color(0.88f, 0.65f, 0.2f));
            ItemsColors.Add(GameItemType._13, new Color(0.765f, 0.7f, 0.51f));
            ItemsColors.Add(GameItemType._14, new Color(0.65f, 0.63f, 0.2f));
            ItemsColors.Add(GameItemType._15, new Color(0.84f, 0.75f, 0.18f));
            ItemsColors.Add(GameItemType._16, new Color(0.78f, 0.59f, 0.14f));

            DifficultyLevelsColors.Add(DifficultyLevel.easy, new Color(0.82f, 0.63f, 0.35f));//210 160 89
            DifficultyLevelsColors.Add(DifficultyLevel.medium, new Color(0.59f, 0.59f, 0.59f));//150 150 150
            DifficultyLevelsColors.Add(DifficultyLevel.hard, new Color(0.75f, 0.63f, 0.15f));//190 160 39
            DifficultyLevelsColors.Add(DifficultyLevel.veryhard, new Color(0.16f, 0.55f, 0.78f));//4 140 200
        }
    }
}
