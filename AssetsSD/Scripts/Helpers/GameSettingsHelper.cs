using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class GeneralSettings
    {
        public static SoundState SoundEnabled
        {
            get
            {
                if (PlayerPrefs.HasKey("General_SoundEnabled"))
                    return (SoundState)PlayerPrefs.GetInt("General_SoundEnabled");
                PlayerPrefs.SetInt("General_SoundEnabled", 0);
                return SoundState.on;
            }
            set
            {
                PlayerPrefs.SetInt("General_SoundEnabled", (int)(value > SoundState.off ? SoundState.on : value));
            }
        }

        public static Sprite SoundButtonSprite
        {
            get
            {
                switch (SoundEnabled)
                {
                    case SoundState.on:
                        return Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("SignsAtlas_4"));
                    case SoundState.vibrate:
                        return Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("SignsAtlas_6"));
                    case SoundState.off:
                        return Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("SignsAtlas_5"));
                    default:
                        return null;
                }
            }
        }

        public static GameTheme ActiveTheme
        {
            get
            {
                if (PlayerPrefs.HasKey("General_Theme"))
                    return (GameTheme)PlayerPrefs.GetInt("General_Theme");
                PlayerPrefs.SetInt("General_Theme", (Int32)GameTheme.light);
                return GameTheme.light;
            }
            set
            {
                PlayerPrefs.SetInt("General_Theme", (Int32)value);
            }
        }

        public static void RemoveAllPrefsExceptGeneral()
        {
            var theme = ActiveTheme;
            var sound = SoundEnabled;
            var pressedLogoLabel = PlayerPrefs.HasKey("PressLogoLabel");

            PlayerPrefs.DeleteAll();

            ActiveTheme = theme;
            SoundEnabled = sound;
            if (pressedLogoLabel)
                PlayerPrefs.SetInt("PressLogoLabel",1);
        }
    }

    public interface IGameSettingsHelper
    {
        bool SoundEnabled { get; set; }

        //bool VibrationEnabled { get; set; }

        DifficultyLevel MaximumOpenedLevel { get; set; }

        int ScoreRecord { get; set; }

        int GamesPlayed { get; set; }

        GameItemType CurrentItemType { get; set; }

        float LongestSession { get; set; }

        int MaxMultiplier { get; set; }

        int MovesRecord { get; set; }

        void RemovePrefs();
    }

    public class GameSettingsHelper<T> : IGameSettingsHelper where T:IPlayground
    {
        private static readonly GameSettingsHelper<T> Instance = new GameSettingsHelper<T>();

        private GameSettingsHelper(){}

        public static GameSettingsHelper<T> Preferenses
        {
            get 
            {
                return Instance; 
            }
        }

        public bool SoundEnabled
        {
            get
            {
                if (PlayerPrefs.HasKey("General_SoundEnabled"))
                    return PlayerPrefs.GetInt("General_SoundEnabled") != 0;
                PlayerPrefs.SetInt("General_SoundEnabled", 1);
                return true;
            }
            set
            {
                PlayerPrefs.SetInt("General_SoundEnabled", value ? 1 : 0);
            }
        }

        //public bool VibrationEnabled
        //{
        //    get
        //    {
        //        if (PlayerPrefs.HasKey(GetType().FullName + "_VibrationEnabled"))
        //            return PlayerPrefs.GetInt(GetType().FullName + "_VibrationEnabled") != 0;
        //        PlayerPrefs.SetInt(GetType().FullName + "_VibrationEnabled", 1);
        //        return true;
        //    }
        //    set
        //    {
        //        PlayerPrefs.SetInt(GetType().FullName + "_VibrationEnabled", value ? 1 : 0);
        //    }
        //}

        public DifficultyLevel MaximumOpenedLevel
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_MaximumOpenedLevel"))
                    return (DifficultyLevel)PlayerPrefs.GetInt(GetType().FullName + "_MaximumOpenedLevel");
                PlayerPrefs.SetInt(GetType().FullName + "_MaximumOpenedLevel", (int)DifficultyLevel._easy);
                return DifficultyLevel._easy;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_MaximumOpenedLevel", (int)value);
            }
        }

        public int ScoreRecord
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_ScoreRecord"))
                    return PlayerPrefs.GetInt(GetType().FullName + "_ScoreRecord");
                PlayerPrefs.SetInt(GetType().FullName + "_ScoreRecord", 0);
                return 0;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_ScoreRecord", value);
            }
        }

        public int GamesPlayed
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_GamesPlayed"))
                    return PlayerPrefs.GetInt(GetType().FullName + "_GamesPlayed");
                PlayerPrefs.SetInt(GetType().FullName + "_GamesPlayed", 0);
                return 0;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_GamesPlayed", value);
            }
        }

        public GameItemType CurrentItemType
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_CurrentItemType"))
                    return (GameItemType)PlayerPrefs.GetInt(GetType().FullName + "_CurrentItemType");
                PlayerPrefs.SetInt(GetType().FullName + "_CurrentItemType", (int)GameItemType.NullItem);
                return GameItemType.NullItem;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_CurrentItemType", (int)value);
            }
        }

        public float LongestSession
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_LongestSession"))
                    return PlayerPrefs.GetFloat(GetType().FullName + "_LongestSession");
                PlayerPrefs.SetFloat(GetType().FullName + "_LongestSession", 0f);
                return 0;
            }
            set
            {
                PlayerPrefs.SetFloat(GetType().FullName + "_LongestSession", value);
            }
        }

        public int MaxMultiplier
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_MaxMultiplier"))
                    return PlayerPrefs.GetInt(GetType().FullName + "_MaxMultiplier");
                PlayerPrefs.SetInt(GetType().FullName + "_MaxMultiplier", 0);
                return 0;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_MaxMultiplier", value);
            }
        }

        public int MovesRecord
        {
            get
            {
                if (PlayerPrefs.HasKey(GetType().FullName + "_MovesRecord"))
                    return PlayerPrefs.GetInt(GetType().FullName + "_MovesRecord");
                PlayerPrefs.SetInt(GetType().FullName + "_MovesRecord", 0);
                return 0;
            }
            set
            {
                PlayerPrefs.SetInt(GetType().FullName + "_MovesRecord", value);
            }
        }

        public void RemovePrefs()
        {
            var typeName = GetType().FullName;
            if (PlayerPrefs.HasKey(typeName + "_MaximumOpenedLevel"))
                PlayerPrefs.DeleteKey(typeName + "_MaximumOpenedLevel");

            if (PlayerPrefs.HasKey(typeName + "_ScoreRecord"))
                PlayerPrefs.DeleteKey(typeName + "_ScoreRecord");

            if (PlayerPrefs.HasKey(typeName + "_GamesPlayed"))
                PlayerPrefs.DeleteKey(typeName + "_GamesPlayed");

            if (PlayerPrefs.HasKey(typeName + "_CurrentItemType"))
                PlayerPrefs.DeleteKey(typeName + "_CurrentItemType");

            if (PlayerPrefs.HasKey(typeName + "_LongestSession"))
                PlayerPrefs.DeleteKey(typeName + "_LongestSession");

            if (PlayerPrefs.HasKey(typeName + "_MaxMultiplier"))
                PlayerPrefs.DeleteKey(typeName + "_MaxMultiplier"); 
        }

        //public static void RemoveAllPrefs()
        //{
        //    var tpgs = GetPlaygroundTypes();
        //    foreach (var tpg in tpgs)
        //    {
        //        var type = typeof(GameSettingsHelper<>).MakeGenericType(tpg);
        //        var prefsField = type.GetProperty("Preferenses", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        //        var prefsValue = prefsField.GetValue(null, null);

        //        var gameSettingsHelper = prefsValue as IGameSettingsHelper;
        //        if (gameSettingsHelper != null) 
        //            gameSettingsHelper.RemovePrefs();
        //    }
        //}

        //private static IEnumerable<Type> GetPlaygroundTypes()
        //{
        //    return Assembly.GetAssembly(typeof(SquarePlayground)).GetTypes().Where(type => type.IsSubclassOf(typeof(SquarePlayground))).ToList();
        //}
    }
}
