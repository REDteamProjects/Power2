using System;
using System.Diagnostics;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Collections;

public class ItemsNameHelper  {

    public static String GetPrefabPath<T>() where T:IPlayground
    {
        var typeT = typeof (T);
        if (typeT == typeof(ModeDropsPlayground))
        {
                    return "Prefabs/Drops/DropsGameItem";
        }

        if (typeT == typeof (ModeMatch3SquarePlayground))
        {
                    return "Prefabs/Match3/Match3GameItem";
        }

        if (typeT == typeof (RhombusPlayground) || typeT.BaseType == typeof (RhombusPlayground))
        {
                    return "Prefabs/Rhombus/RhombusGameItem";
        }
            

        if (typeT == typeof(SquarePlayground) || typeT.BaseType == typeof(SquarePlayground))
        {
                    return "Prefabs/Standard/GameItem";
        }

        return null;
    }

    public static String GetBackgroundTexturePrefix<T>() where T : IPlayground
    {
        var typeT = typeof(T);
        if (typeT == typeof(ModeDropsPlayground))
            return "6x6Atlas";

        if (typeT == typeof(ModeMatch3SquarePlayground))
            return "8x8Atlas";

        if (typeT == typeof(RhombusPlayground) || typeT.BaseType == typeof(RhombusPlayground))
            return "RhombusAtlas";

        if (typeT == typeof(Mode8x8SquarePlayground))
            return "8x8Atlas";

        if (typeT == typeof(Mode6x6SquarePlayground) || typeT.BaseType == typeof(SquarePlayground))
            return "6x6Atlas";

        return null;
    }
}
