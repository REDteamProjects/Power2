using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class SavedataHelper
    {
        public static void LoadData(ref IPlaygroundSavedata data)
        {
            if (!IsSaveDataExist(data)) return;

            var bf = new BinaryFormatter();
            using (var file = File.Open(Application.persistentDataPath + data.FileName, FileMode.Open))
            {
                var dl = data.Difficulty;
                data = (IPlaygroundSavedata)bf.Deserialize(file);
                if (data.Difficulty != dl)
                    data.ResetDynamicPart();
            }
        }

        public static void SaveData(IPlaygroundSavedata data)
        {
            var bf = new BinaryFormatter();
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
