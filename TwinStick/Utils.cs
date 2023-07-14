using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TwinStick
{
    public class AssetLoading
    {
        public static BepInEx.PluginInfo pluginInfo;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(pluginInfo.Location);
            }
        }

        internal static string GetFilePathFromPlugin(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, Path.Combine(folderName, fileName));
        }
        #region json stuff

        public static T LoadJsonFromFile<T>(string json)
        {
            string jsonString;

            string path = GetFilePathFromPlugin("Assets", json);

            using (StreamReader reader = new StreamReader(path))
            {
                jsonString = reader.ReadToEnd();
            }

            return JsonUtility.FromJson<T>(jsonString);
        }

        public static T LoadJsonFromEmbedded<T>(string json)
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
        #endregion json stuff

        #region load sprite (stolen from tournamentedition)
        public static Sprite LoadSprite(string path)
        {
            Texture2D texture2D = LoadTex2D(path, true);
            texture2D.name = path;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = path;
            return sprite;
        }

        public static Texture2D LoadTex2D(string spriteFileName, bool pointFilter = false, Texture2D T2D = null, bool overrideFullPath = false)
        {
            string path = "Assets/" + spriteFileName + ".png";
            if (overrideFullPath)
            {
                path = spriteFileName;
            }
            string text = Path.Combine(assemblyDir, path);
            Texture2D texture2D = T2D == null ? LoadPNG(text, pointFilter) : T2D;
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }
            return texture2D;
        }

        public static Texture2D LoadPNG(string filePath, bool pointFilter = false)
        {
            Texture2D texture2D = new Texture2D(2, 2);
            byte[] data = File.ReadAllBytes(filePath);
            texture2D.LoadImage(data);
            texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
            texture2D.Apply();

            return texture2D;
        }
        #endregion load sprite

    }

    public class Utils
    {
        private static int savedJsonCount;

        public static void SaveJson<T>(T obj, string filename = "")
        {
            string json = JsonUtility.ToJson(obj, true);
            string jsonPath = Application.persistentDataPath + $"/ALLTHETHINGS2/{filename}{obj}.json";
            savedJsonCount++;

            File.WriteAllText(jsonPath, json);
            Log.Warning("printedjson to " + jsonPath);
        }

        public static void PrintMyCodePlease()
        {
            var skillStats = AssetLoading.LoadJsonFromFile<SkillStats>("AirChannelDashGoodSkillStats.json");

            string log = "printing " + skillStats.GetType();

            FieldInfo[] property_infos = skillStats.GetType().GetFields();

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

                //log += ($"\nskillStats.{info.Name} = new {GetCodeTypeString(info.GetValue(skillStats))}nig[attackInfos.Length];").Replace("[]nig", "");
                log += ($"\nskillStats.{info.Name}[i] = attackInfo.{info.Name};");
            }

            Log.Message(log);
        }

        private static object GetCodeTypeString(object infoValue)
        {
            string infoString = infoValue.GetType().ToString();
            infoString = infoString.Replace("System.String", "string");
            infoString = infoString.Replace("System.Boolean", "bool");
            infoString = infoString.Replace("System.Int32", "int");
            infoString = infoString.Replace("System.Single", "float");
            return infoString;
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
            Log.Warning("Printing list. Count: " + list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Log.Warning(list[i]);
            }
        }
    }
}
