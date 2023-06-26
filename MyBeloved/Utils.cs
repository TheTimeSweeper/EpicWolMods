using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MyBeloved {

    public class Utils {

        static int count;

        public static void printAllFields<T>(T obj, bool json = false) {


            string log = "printing " + obj.GetType();

            FieldInfo[] property_infos = obj.GetType().GetFields();

            int padLength = 0;
            for (int i = 0; i < property_infos.Length; i++) {
                FieldInfo info = property_infos[i];

                if (info.Name.Length > padLength) {
                    padLength = info.Name.Length;
                }
            }


            for (int i = 0; i < property_infos.Length; i++) {
                FieldInfo info = property_infos[i];

                log += ($"\n{info.Name.PadLeft(padLength, '-')}| {info.GetValue(obj)}");
            }

            Log.Message(log);


            if (json)
                SaveJson(obj);
        }

        public static void SaveJson<T>(T obj, string filename = "") {
            string jsn = JsonUtility.ToJson(obj, true);
            string jsnPath = Application.persistentDataPath + $"/attacks/{filename}{obj}{count}.json";
            count++;

            File.WriteAllText(jsnPath, jsn);
            Log.Warning("printedjson to " + jsnPath);
        }

        public static T LoadFromEmbeddedJson<T>(string jsn) {

            string jsnstring;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MyBeloved.{jsn}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream)) {
                jsnstring = reader.ReadToEnd();
            }

            return JsonUtility.FromJson<T>(jsnstring);
        }


        public static void PrintStatData(StatData self) {

            Log.Warning("Printing all stat data");

            foreach (KeyValuePair<string, object> item in self.statDict) {
                Log.Warning($"{item.Key}:");

                if (item.Value is List<float>) {
                    PrintList<float>(item.Value as List<float>);
                }
                if (item.Value is List<int>) {
                    PrintList<int>(item.Value as List<int>);
                }
                if (item.Value is List<bool>) {
                    PrintList<bool>(item.Value as List<bool>);
                }
                if (item.Value is List<string>) {
                    PrintList<string>(item.Value as List<string>);
                }
            }
        }

        public static void PrintList<T>(List<T> list) {
            for (int i = 0; i < list.Count; i++) {
                Log.Warning(list[i]);
            }
        }
    }
}
