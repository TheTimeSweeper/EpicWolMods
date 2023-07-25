using BepInEx;
using System.IO;
using UnityEngine;

namespace Clothes
{
    public class Assets
    {
        public static AssetBundle bundle;

        public static void Init()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(ClothesPlugin.PluginInfo.Location), Path.Combine("Assets", "clothes")));
        }
    }
}