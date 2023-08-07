using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SillySkills
{
    public class Assets
    {
        public static BepInEx.PluginInfo pluginInfo;

        public static AssetBundle funnyBundle;
        public static AssetBundle unfunnyBundle;
        public static GameObject FloorSpikeSmall;
        public static GameObject FloorSpikeLarge;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(pluginInfo.Location);
            }
        }

        internal static void Init(PluginInfo info)
        {
            pluginInfo = info;
            funnyBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(pluginInfo.Location), Path.Combine("Assets", "skillsbundle")));
            unfunnyBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(pluginInfo.Location), Path.Combine("Assets", "skillsunfunny")));
        }

        internal static void LateInit()
        {
            FloorSpikeSmall = funnyBundle.LoadAsset<GameObject>("FloorSpikeSmall");
            SetupFloorSpikeIn2023(FloorSpikeSmall, "Small");
            FloorSpikeLarge = funnyBundle.LoadAsset<GameObject>("FloorSpikeLarge");
            SetupFloorSpikeIn2023(FloorSpikeLarge, "Large");
        }
        
        private static void SetupFloorSpikeIn2023(GameObject spikeObject, string bigorsmall)
        {
            FloorSpike spike = spikeObject.GetComponent<FloorSpike>();
            spike.spikeRendererRenderer = spikeObject.transform.GetChild(0).GetChild(0).gameObject.AddComponent<SpriteRenderer>();
            spike.spikeRendererRenderer.sortingLayerName = "Actor";
            Material loadedMaterial = ChaosBundle.Get<Material>("Assets/materials/basic/Sprite-Diffuse.mat");
            spike.spikeRendererRenderer.material = loadedMaterial;

            spike.TopSprite = unfunnyBundle.LoadAsset<Sprite>($"Floorspike{bigorsmall}Top");
            spike.SideSprite = unfunnyBundle.LoadAsset<Sprite>($"Floorspike{bigorsmall}Side");
            spike.BottomSprite = unfunnyBundle.LoadAsset<Sprite>($"Floorspike{bigorsmall}Bottom");
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

        #region load sprite
        public static Sprite LoadSprite(string path)
        {
            return unfunnyBundle.LoadAsset<Sprite>(path);
        }
        #endregion load sprite

    }
}
