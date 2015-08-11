//#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using System;
using System.IO;
using Assets.Scripts.DataClasses;
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
//#endif