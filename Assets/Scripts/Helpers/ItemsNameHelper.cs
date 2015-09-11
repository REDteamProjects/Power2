using System;
using Assets.Scripts;
using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Collections;

public class ItemsNameHelper  {

    public static String GetPrefabPath<T>() where T:IPlayground
    {
        var typeT = typeof (T);
        if (typeT == typeof(ModeDropsPlayground))
            return "Prefabs/SD/Drops/DropsGameItem";

        if (typeT == typeof(ModeMatch3Playground))
            return "Prefabs/SD/Match3/Match3GameItem";

        if (typeT == typeof(RhombusPlayground) || typeT.BaseType == typeof(RhombusPlayground))
            return "Prefabs/SD/Rhombus/RhombusGameItem";

        if (typeT == typeof(SquarePlayground) || typeT.BaseType == typeof(SquarePlayground))
            return "Prefabs/SD/Standard/GameItem";

        return null;
    }

    public static String GetBackgroundTexturePrefix<T>() where T : IPlayground
    {
        var typeT = typeof(T);
        if (typeT == typeof(ModeDropsPlayground))
            return "SD/6x6Atlas";

        if (typeT == typeof(ModeMatch3Playground))
            return "SD/8x8Atlas";

        if (typeT == typeof(RhombusPlayground) || typeT.BaseType == typeof(RhombusPlayground))
            return "SD/RhombusAtlas";

        if (typeT == typeof(Mode8x8SquarePlayground))
            return "SD/8x8Atlas";

        if (typeT == typeof(Mode6x6SquarePlayground) || typeT.BaseType == typeof(SquarePlayground))
            return "SD/6x6Atlas";

        return null;
    }
}
