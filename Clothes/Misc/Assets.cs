using BepInEx;
using System.IO;
using UnityEngine;

namespace Clothes
{
    public class Assets
    {
        public static AssetBundle funnyBundle;
        public static AssetBundle safeBundle;

        public static GameObject pandemoniumHUDPrefab;

        public static void Init()
        {
            funnyBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(ClothesPlugin.PluginInfo.Location), Path.Combine("Assets", "clothes")));
            safeBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(ClothesPlugin.PluginInfo.Location), Path.Combine("Assets", "clothes2")));

            pandemoniumHUDPrefab = funnyBundle.LoadAsset<GameObject>("PandemoniumHUD");
            Animator anim = pandemoniumHUDPrefab.AddComponent<Animator>();
            anim.runtimeAnimatorController = safeBundle.LoadAsset<RuntimeAnimatorController>("PandemoniumHUD.controller");

            pandemoniumHUDPrefab.GetComponent<PandemoniumHUD>()._animator = anim;
        }
    }
}