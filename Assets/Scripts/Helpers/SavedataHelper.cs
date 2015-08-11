using System;
using System.IO;
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
using FullSerializer;
#else
using System.Runtime.Serialization.Formatters.Binary;
#endif

using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class SavedataHelper
    {
        public static void LoadData(ref IPlaygroundSavedata data)
        {
            if (!IsSaveDataExist(data)) return;

#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
            var bf = new WinRTSerializer();
#else
            var bf = new BinaryFormatter();
#endif
            using (var file = File.Open(Application.persistentDataPath + data.FileName, FileMode.Open))
            {
                var dl = data.Difficulty;
                
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
                data = (IPlaygroundSavedata)bf.Deserialize(file, data.GetType());
#else
                data = (IPlaygroundSavedata)bf.Deserialize(file);
#endif
                if (data.Difficulty != dl)
                    data.ResetDynamicPart();
            }
        }

        public static void SaveData(IPlaygroundSavedata data)
        {
#if !UNITY_EDITOR && (UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1)
            var bf = new WinRTSerializer();
#else
            var bf = new BinaryFormatter();
#endif
            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
            using (var file = File.Create(Application.persistentDataPath + data.FileName))
            //you can call it anything you want
            {
                bf.Serialize(file, data);
            }
        }

        public static void RemoveData(IPlaygroundSavedata data)
        {
            if (!IsSaveDataExist(data)) return;
            File.Delete(Application.persistentDataPath + data.FileName);
        }

        public static bool IsSaveDataExist(IPlaygroundSavedata data)
        {
            return File.Exists(Application.persistentDataPath + data.FileName);
        }
    }
}
