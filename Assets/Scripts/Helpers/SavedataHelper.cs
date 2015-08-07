using System.IO;
#if !NETFX_CORE
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

#if NETFX_CORE
                var bf = new WindowsPhoneSerializer();
#else
            var bf = new BinaryFormatter();
#endif
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
#if NETFX_CORE
                var bf = new WindowsPhoneSerializer();
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
