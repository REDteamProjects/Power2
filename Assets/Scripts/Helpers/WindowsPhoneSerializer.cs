using System;
using System.IO;
using System.Text;
using Assets.Scripts.Interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.Helpers
{
    public class WindowsPhoneSerializer
    {
        public void Serialize(Stream fileStream, object graph)
        {
            var jsonString = JsonConvert.SerializeObject(graph);
            fileStream.Write(Encoding.UTF8.GetBytes(jsonString), 0, jsonString.Length);
        }

        public object Deserialize(Stream fileStream)
        {
            var bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
        }
    }
}
