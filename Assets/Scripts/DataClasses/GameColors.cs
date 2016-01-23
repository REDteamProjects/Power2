using System.Collections.Generic;
using Assets.Scripts.Enums;
using UnityEngine;
using System;

namespace Assets.Scripts.DataClasses
{
    public class GameColors
    {
        public static Dictionary<GameItemType, Color> ItemsColors = new Dictionary<GameItemType, Color>();
        public static Dictionary<GameItemType, Color> Match3Colors = new Dictionary<GameItemType, Color>();
        public static Dictionary<DifficultyLevel, Color> DifficultyLevelsColors = new Dictionary<DifficultyLevel, Color>();
        public static Dictionary<String, Color> ModesColors = new Dictionary<String, Color>();
        public static readonly Color DefaultLabelColor = new Color(0.67f, 0.67f, 0.67f);
        public static readonly Color DefaultLight = new Color(1f, 1f, 1f);//new Color(1f, 0.95f, 0.85f);
        public static readonly Color DefaultDark = new Color(0.16f, 0.16f, 0.18f);
        public static readonly Color Additional1 = new Color(0.2f, 0.8f, 1f);//new Color(0.6f, 0.1f, 0f);
        public static readonly Color Additional2 = new Color(0.65f, 0.45f, 1f);
        public static readonly Color Additional3 = new Color(0.4f, 1f, 0.7f);
        public static readonly Color Additional4 = new Color(1f, 0.9f, 0.4f);

        public static Color BackgroundColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return DefaultDark;
                    case GameTheme.light:
                        return DefaultLight; 
                    case GameTheme.additional_1:
                        return Additional1;
                    case GameTheme.additional_2:
                        return Additional2;
                    case GameTheme.additional_3:
                        return Additional3;
                    case GameTheme.additional_4:
                        return Additional4;
                }
                return DefaultLight;
            }
        }
        public static Color ForegroundColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return DefaultLight;//new Color(0.6f, 0.416f, 0.231f);
                    case GameTheme.light:
                        return DefaultDark;//new Color(0.16f, 0.16f, 0.16f);
                }
                return DefaultLight;//new Color(0.6f, 0.416f, 0.231f);
            }
        }

        public static Color ForegroundButtonsColor
        {
            get
            {
                switch (Game.Theme)
                {
                    case GameTheme.dark:
                        return DefaultLight;//new Color(1f, 1f, 1f);
                    case GameTheme.light:
                        return DefaultDark;//new Color(0.16f, 0.16f, 0.16f);
                }
                return DefaultLight;// new Color(1f, 1f, 1f);
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
            ItemsColors.Add(GameItemType._7, new Color(0.9f, 0.51f, 0.12f));
            ItemsColors.Add(GameItemType._8, new Color(0.82f, 0.35f, 0.16f));
            ItemsColors.Add(GameItemType._9, new Color(0.78f, 0f, 0.2f));
            ItemsColors.Add(GameItemType._10, new Color(0.78f, 0.12f, 0.55f));
            ItemsColors.Add(GameItemType._11, new Color(0.75f, 0.27f, 0.2f));
            ItemsColors.Add(GameItemType._12, new Color(0.86f, 0.86f, 0.78f));
            ItemsColors.Add(GameItemType._13, new Color(0.94f, 0.9f, 0.55f));
            ItemsColors.Add(GameItemType._14, new Color(0.94f, 0.9f, 0.2f));
            ItemsColors.Add(GameItemType._15, new Color(0.71f, 1f, 0.82f));
            ItemsColors.Add(GameItemType._16, new Color(0.47f, 0.67f, 1f));
            ItemsColors.Add(GameItemType._2x, new Color(0.43f, 0.74f, 0.43f));

            Match3Colors.Add(GameItemType._1, new Color(0.96f, 0.9f, 0.55f));
            Match3Colors.Add(GameItemType._2, new Color(0.94f, 0.9f, 0.2f));
            Match3Colors.Add(GameItemType._3, new Color(0.78f, 0f, 0.2f));
            Match3Colors.Add(GameItemType._4, new Color(0.78f, 0.12f, 0.55f));
            Match3Colors.Add(GameItemType._5, new Color(0.71f, 1f, 0.82f));
            Match3Colors.Add(GameItemType._6, new Color(0.47f, 0.67f, 0.98f));
            Match3Colors.Add(GameItemType._7, new Color(0.43f, 0.74f, 0.43f));

            DifficultyLevelsColors.Add(DifficultyLevel._easy, new Color(0.94f, 0.75f, 0.27f));//240 190 70
            DifficultyLevelsColors.Add(DifficultyLevel._medium, new Color(0.63f, 0.63f, 0.63f));//160 160 160
            DifficultyLevelsColors.Add(DifficultyLevel._hard, new Color(0.94f, 0.75f, 0.16f));//240 190 40
            DifficultyLevelsColors.Add(DifficultyLevel._veryhard, new Color(0.16f, 0.55f, 0.78f));//4 140 200

            ModesColors.Add("6x6", new Color(0.86f, 0.82f, 0.59f));
            ModesColors.Add("8x8", new Color(0.94f, 0.78f, 0.2f));
            ModesColors.Add("Match3", new Color(0.86f, 0.86f, 0.78f));
            ModesColors.Add("Rhombus", new Color(0.9f, 0.86f, 0.35f));
        }
    }
}
