using System.IO;
using System.Reflection;
using UnityEngine;

namespace MyBeloved {

    public class Utils {

        public static BepInEx.Logging.ManualLogSource loge;

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

            loge.LogMessage(log);


            if (json)
                SaveJson(obj);
        }

        public static void SaveJson<T>(T obj, string filename = "") {
            string jsn = JsonUtility.ToJson(obj, true);
            string jsnPath = Application.persistentDataPath + $"/attacks/{filename}{obj}{count}.json";
            count++;

            File.WriteAllText(jsnPath, jsn);
            loge.LogWarning("printedjson to " + jsnPath);
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
    }
}
