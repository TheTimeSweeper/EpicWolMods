using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BossDialogReplacer
{

    public class Utils {

        private static int savedJsonCount;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(BossDialogReplacerPlugin.pluginInfo.Location);
            }
        }

        internal static string GetPluginFilePath(string folderName, string fileName)
        {
            return Path.Combine(assemblyDir, Path.Combine(folderName, fileName));
        }

        #region json stuff
        public static void SaveJson<T>(T obj, string filename = "")
        {
            string json = JsonUtility.ToJson(obj, true);
            string jsonPath = GetPluginFilePath("Assets", filename);
            savedJsonCount++;

            File.WriteAllText(jsonPath, json);
            Debug.LogWarning("printedjson to " + jsonPath);
        }

        public static T LoadJsonFromFile<T>(string json)
        {
            string jsonString;

            string path = GetPluginFilePath("Assets", json);

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

    }
}
