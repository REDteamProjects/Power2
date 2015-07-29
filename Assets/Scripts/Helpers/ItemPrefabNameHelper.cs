using System;
using Assets.Scripts;
using Assets.Scripts.Interfaces;
using UnityEngine;
using System.Collections;

public class ItemPrefabNameHelper  {

    public static String GetPrefabPath<T>() where T:IPlayground
    {
        var typeT = typeof (T);
        if (typeT == typeof(ModeDropsPlayground))
            return "Prefabs/Drops/DropsGameItem";

        if (typeT == typeof(ModeMatch3Playground))
            return "Prefabs/Match3/Match3GameItem";

        if (typeT == typeof(RhombusPlayground) || typeT.BaseType == typeof(RhombusPlayground))
            return "Prefabs/Rhombus/RhombusGameItem";

        if (typeT == typeof(SquarePlayground) || typeT.BaseType == typeof(SquarePlayground))
            return "Prefabs/SD/Standard/GameItem";

        

        return null;
    }
}
