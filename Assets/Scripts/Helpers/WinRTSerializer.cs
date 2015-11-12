//#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using FullSerializer;

public class WinRTSerializer {

    //private static readonly FullSerializer.fsSerializer _serializer = new FullSerializer.fsSerializer();

    public void Serialize(Stream fileStream, object graph)
    {
        fsData data;
        var serializer = new fsSerializer();
        serializer.TrySerialize(graph, out data);
        var jsonString = fsJsonPrinter.CompressedJson(data);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes(jsonString), 0, jsonString.Length);
    }

    public object Deserialize(Stream fileStream, Type type)
    {
        var bytes = new byte[fileStream.Length];
        var count = fileStream.Read(bytes, 0, bytes.Length);
       
        var jsonString = System.Text.Encoding.UTF8.GetString(bytes, 0, count);
        var data = fsJsonParser.Parse(jsonString);

        var serializer = new fsSerializer();
        object refObject = null;

        serializer.TryDeserialize(data, type, ref refObject);
        return refObject;
    }
}

public class SquarePlaygroundSavedataConverter : fsDirectConverter
{
    public override Type ModelType { get { return typeof(SquarePlaygroundSavedata); } }

    public override object CreateInstance(fsData data, Type storageType)
    {
        var obj = (SquarePlaygroundSavedata) storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        obj.ProgressBarStateData = new ProgressBarState();
        return obj;
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
        var myType = (SquarePlaygroundSavedata)instance;
        serialized = new fsData(new List<fsData>
        {
            new fsData(myType.CurrentPlaygroundTime),
            new fsData(myType.Score),
            new fsData((Int32)myType.Difficulty),
            new fsData((Int32)myType.MaxInitialElementType),
            new fsData(
                myType.Items != null ?
                myType.Items.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(
                myType.MovingTypes != null ?
                myType.MovingTypes.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(new List<fsData>
            {
                new fsData(myType.ProgressBarStateData.Multiplier),
                new fsData(myType.ProgressBarStateData.Upper),
                new fsData(myType.ProgressBarStateData.State)
            })
        });
        return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
        if (instance == null) 
            instance = storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        
        var myType = (SquarePlaygroundSavedata)instance;

        var dataItems = data.AsList;

        myType.CurrentPlaygroundTime = (float)dataItems[0].AsDouble;
        myType.Score = (Int32)dataItems[1].AsInt64; 
        myType.Difficulty = (DifficultyLevel)dataItems[2].AsInt64;
        myType.MaxInitialElementType = (GameItemType)dataItems[3].AsInt64;
        if (dataItems[4].IsList)
        {
            var list = dataItems[4].AsList;
            if (list != null && list.Count != 0)
            {
                myType.Items = new GameItemType[list.Count][];
                for (var i = 0; i < list.Count; i++)
                {
                    var localList = list[i].AsList;
                    myType.Items[i] = localList.Select(lli => (GameItemType) lli.AsInt64).ToArray();
                }
            }
        }
        if (dataItems[5].IsList)
        {
            var list2 = dataItems[5].AsList;
            if (list2 != null && list2.Count != 0)
            {
                myType.MovingTypes = new GameItemMovingType[list2.Count][];
                for (var i = 0; i < list2.Count; i++)
                {
                    var localList2 = list2[i].AsList;
                    myType.MovingTypes[i] = localList2.Select(lli => (GameItemMovingType)lli.AsInt64).ToArray();
                }
            }
        }

        if (!dataItems[6].IsList) return fsResult.Success;
        var progressBarData = dataItems[6].AsList;
        if (myType.ProgressBarStateData == null)
            myType.ProgressBarStateData = new ProgressBarState();
        myType.ProgressBarStateData.Multiplier = (float)progressBarData[0].AsDouble;
        myType.ProgressBarStateData.Upper = (float)progressBarData[1].AsDouble;
        myType.ProgressBarStateData.State = (float)progressBarData[2].AsDouble;

        return fsResult.Success;
    }
}

public class ModeMatch3PlaygroundSavedataConverter : fsDirectConverter
{
    public override Type ModelType { get { return typeof(ModeMatch3PlaygroundSavedata); } }

    public override object CreateInstance(fsData data, Type storageType)
    {
        var obj = (ModeMatch3PlaygroundSavedata)storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        obj.ProgressBarStateData = new ProgressBarState();
        return obj;
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
        var myType = (ModeMatch3PlaygroundSavedata)instance;
        serialized = new fsData(new List<fsData>
        {
            new fsData(myType.CurrentPlaygroundTime),
            new fsData(myType.Score),
            new fsData((Int32)myType.Difficulty),

            new fsData(
                myType.Items != null ?
                myType.Items.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(
                myType.MovingTypes != null ?
                myType.MovingTypes.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(new List<fsData>
            {
                new fsData(myType.ProgressBarStateData.Multiplier),
                new fsData(myType.ProgressBarStateData.Upper),
                new fsData(myType.ProgressBarStateData.State)
            })
        });
        return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
        if (instance == null) 
            instance = storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        
        var myType = (ModeMatch3PlaygroundSavedata)instance;

        var dataItems = data.AsList;

        myType.CurrentPlaygroundTime = (float)dataItems[0].AsDouble;
        myType.Score = (Int32)dataItems[1].AsInt64; 
        myType.Difficulty = (DifficultyLevel)dataItems[2].AsInt64;

        if (dataItems[3].IsList)
        {
            var list = dataItems[3].AsList;
            if (list != null && list.Count != 0)
            {
                myType.Items = new GameItemType[list.Count][];
                for (var i = 0; i < list.Count; i++)
                {
                    var localList = list[i].AsList;
                    myType.Items[i] = localList.Select(lli => (GameItemType) lli.AsInt64).ToArray();
                }
            }
        }
        if (dataItems[4].IsList)
        {
            var list2 = dataItems[4].AsList;
            if (list2 != null && list2.Count != 0)
            {
                myType.MovingTypes = new GameItemMovingType[list2.Count][];
                for (var i = 0; i < list2.Count; i++)
                {
                    var localList2 = list2[i].AsList;
                    myType.MovingTypes[i] = localList2.Select(lli => (GameItemMovingType)lli.AsInt64).ToArray();
                }
            }
        }

        if (!dataItems[5].IsList) return fsResult.Success;
        var progressBarData = dataItems[5].AsList;
        if (myType.ProgressBarStateData == null)
            myType.ProgressBarStateData = new ProgressBarState();
        myType.ProgressBarStateData.Multiplier = (float)progressBarData[0].AsDouble;
        myType.ProgressBarStateData.Upper = (float)progressBarData[1].AsDouble;
        myType.ProgressBarStateData.State = (float)progressBarData[2].AsDouble;

        return fsResult.Success;
    }
}

public class RhombusPlaygroundSavedataConverter : fsDirectConverter
{
    public override Type ModelType { get { return typeof(SquarePlaygroundSavedata); } }

    public override object CreateInstance(fsData data, Type storageType)
    {
        var obj = (RhombusPlaygroundSavedata)storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        obj.ProgressBarStateData = new ProgressBarState();
        return obj;
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
        var myType = (RhombusPlaygroundSavedata)instance;
        serialized = new fsData(new List<fsData>
        {
            new fsData(myType.CurrentPlaygroundTime),
            new fsData(myType.Score),
            new fsData((Int32)myType.Difficulty),
            new fsData((Int32)myType.MaxInitialElementType),
            new fsData(
                myType.Items != null ?
                myType.Items.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(
                myType.MovingTypes != null ?
                myType.MovingTypes.Select(i => 
                    new fsData(i.Select(ii => new fsData((Int32)ii)).ToList())
                ).ToList() : null
            ),
            new fsData(new List<fsData>
            {
                new fsData(myType.ProgressBarStateData.Multiplier),
                new fsData(myType.ProgressBarStateData.Upper),
                new fsData(myType.ProgressBarStateData.State)
            })
        });
        return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
        if (instance == null)
            instance = storageType.GetConstructor(Type.EmptyTypes).Invoke(null);

        var myType = (RhombusPlaygroundSavedata)instance;

        var dataItems = data.AsList;

        myType.CurrentPlaygroundTime = (float)dataItems[0].AsDouble;
        myType.Score = (Int32)dataItems[1].AsInt64;
        myType.Difficulty = (DifficultyLevel)dataItems[2].AsInt64;
        myType.MaxInitialElementType = (GameItemType)dataItems[3].AsInt64;
        if (dataItems[4].IsList)
        {
            var list = dataItems[4].AsList;
            if (list != null && list.Count != 0)
            {
                myType.Items = new GameItemType[list.Count][];
                for (var i = 0; i < list.Count; i++)
                {
                    var localList = list[i].AsList;
                    myType.Items[i] = localList.Select(lli => (GameItemType)lli.AsInt64).ToArray();
                }
            }
        }
        if (dataItems[5].IsList)
        {
            var list2 = dataItems[5].AsList;
            if (list2 != null && list2.Count != 0)
            {
                myType.MovingTypes = new GameItemMovingType[list2.Count][];
                for (var i = 0; i < list2.Count; i++)
                {
                    var localList2 = list2[i].AsList;
                    myType.MovingTypes[i] = localList2.Select(lli => (GameItemMovingType)lli.AsInt64).ToArray();
                }
            }
        }

        if (!dataItems[6].IsList) return fsResult.Success;
        var progressBarData = dataItems[6].AsList;
        if (myType.ProgressBarStateData == null)
            myType.ProgressBarStateData = new ProgressBarState();
        myType.ProgressBarStateData.Multiplier = (float)progressBarData[0].AsDouble;
        myType.ProgressBarStateData.Upper = (float)progressBarData[1].AsDouble;
        myType.ProgressBarStateData.State = (float)progressBarData[2].AsDouble;

        return fsResult.Success;
    }
}
//#endif