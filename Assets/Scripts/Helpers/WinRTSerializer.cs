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

public class SquarePlaygroundSavedataConverter : fsConverter
{

    public override bool CanProcess(Type type)
    {
        return type == typeof(SquarePlaygroundSavedata);
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
        if (data.Type != fsDataType.String)
        {
            return fsResult.Fail("Expected string fsData type but got " + data.Type);
        }
        if (instance == null) 
            instance = storageType.GetConstructor(Type.EmptyTypes).Invoke(null);
        
        var myType = (SquarePlaygroundSavedata)instance;

        var dataItems = data.AsList;

        myType.CurrentPlaygroundTime = (float)dataItems[0].AsDouble;
        myType.Score = (Int32)dataItems[1].AsInt64; 
        myType.Difficulty = (DifficultyLevel)dataItems[2].AsInt64;
        myType.MaxInitialElementType = (GameItemType)dataItems[3].AsInt64;
        if (dataItems[3].IsList && dataItems[3].AsList != null)
        {
            var list = dataItems[3].AsList;
            myType.Items = new GameItemType[list.Count][];
            for (var i = 0; i < list.Count; i++)
            {
                var localList = list[i].AsList;
                myType.Items[i] = localList.Select(lli => (GameItemType)lli.AsInt64).ToArray();
            }
        }

        if (!dataItems[4].IsList || dataItems[4].AsList == null) return fsResult.Success;
        var progressBarData = dataItems[4].AsList;
        myType.ProgressBarStateData.Multiplier = (float)progressBarData[0].AsDouble;
        myType.ProgressBarStateData.State = (float)progressBarData[0].AsDouble;
        myType.ProgressBarStateData.Multiplier = (float)progressBarData[0].AsDouble;

        return fsResult.Success;
    }
}
//#endif