using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SkillsButEpic
{
    public class Utils
    {
        private static int savedJsonCount;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(ExampleSkillMod.pluginInfo.Location);
            }
        }

        internal static string GetPluginFilePath(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, Path.Combine(folderName, fileName));
        }

        public static void SaveJson<T>(T obj, string filename = "")
        {
            string json = JsonUtility.ToJson(obj, true);
            string jsonPath = Application.persistentDataPath + $"/attacks/{filename}{obj}{savedJsonCount}.json";
            savedJsonCount++;

            File.WriteAllText(jsonPath, json);
            Log.Warning("printedjson to " + jsonPath);
        }

        public static T LoadFromFileJson<T>(string json)
        {
            string jsonString;

            string path = GetPluginFilePath("Assets", json);

            using (StreamReader reader = new StreamReader(path))
            {
                jsonString = reader.ReadToEnd();
            }

            return JsonUtility.FromJson<T>(jsonString);
        }

        public static T LoadFromEmbeddedJson<T>(string json)
        {
            string jsonString;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"SkillsButEpic.Assets.{json}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }

            return JsonUtility.FromJson<T>(jsonString);
        }

        public static void printAllFields<T>(T obj, bool saveJson = false)
        {
            string log = "printing " + obj.GetType();

            FieldInfo[] property_infos = obj.GetType().GetFields();

            int padLength = 0;
            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                if (info.Name.Length > padLength)
                {
                    padLength = info.Name.Length;
                }
            }


            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                log += ($"\n{info.Name.PadLeft(padLength, '-')}| {info.GetValue(obj)}");
            }

            Log.Message(log);

            if (saveJson)
                SaveJson(obj);
        }


        public static void PrintStatData(StatData self)
        {
            Log.Warning("Printing all stat data");

            foreach (KeyValuePair<string, object> item in self.statDict)
            {
                Log.Warning($"{item.Key}:");

                if (item.Value is List<float>)
                {
                    PrintList<float>(item.Value as List<float>);
                }
                if (item.Value is List<int>)
                {
                    PrintList<int>(item.Value as List<int>);
                }
                if (item.Value is List<bool>)
                {
                    PrintList<bool>(item.Value as List<bool>);
                }
                if (item.Value is List<string>)
                {
                    PrintList<string>(item.Value as List<string>);
                }
            }
        }

        public static void PrintList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Log.Warning(list[i]);
            }
        }
    }
}
