#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using UnityEngine;
using System.IO;
using System.Text;
public class WinRTSerializer {

    private static readonly FullSerializer.fsSerializer _serializer = new FullSerializer.fsSerializer();

    public void Serialize(Stream fileStream, object graph)
    {
        FullSerializer.fsData data;
        _serializer.TrySerialize(graph, out data);
        var jsonString = FullSerializer.fsJsonPrinter.CompressedJson(data);
        fileStream.Write(Encoding.UTF8.GetBytes(jsonString), 0, jsonString.Length);
    }

    public object Deserialize(Stream fileStream, object result)
    {
        var bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);

        var data = FullSerializer.fsJsonParser.Parse(Encoding.UTF8.GetString(bytes));
        _serializer.TryDeserialize(data, ref result);
        return result;
    }
}
#endif