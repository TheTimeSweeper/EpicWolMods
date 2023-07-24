using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPalettes
{
    public class Palettes
    {
        public static List<Texture2D> palettes = new List<Texture2D>();

        public static int lastAssignableID = 31;

        public static int AddPalette(string fullPath)
        {
            return AddPalette(ImgHandler.LoadPNG(fullPath));
        }

        public static int AddPalette(string assemblyDir, string folderName, string fileName)
        {
            return AddPalette(ImgHandler.LoadPNG(assemblyDir, folderName, fileName));
        }
        public static int AddPalette(Texture2D palleteTexture)
        {
            palettes.Add(palleteTexture);

            lastAssignableID++;
            return lastAssignableID;
        }


        internal static void Start()
        {
            Log.Warning("start");
            CreateAndApplyPalettes();
        }
        internal static void Init()
        {
            Log.Warning("init");
            On.ChaosBundle.Get += ChaosBundle_Get;
            On.ChaosBundle.LoadBundle += ChaosBundle_LoadBundle;
            On.UnlockNotifier.Awake += UnlockNotifier_Awake;
            On.GameController.InitializeScene += GameController_InitializeScene;
        }

        private static void GameController_InitializeScene(On.GameController.orig_InitializeScene orig, GameController self)
        {
            orig(self);
            Log.Warning("GameController_InitializeScene");
        }

        private static void UnlockNotifier_Awake(On.UnlockNotifier.orig_Awake orig, UnlockNotifier self)
        {
            orig(self);
            Log.Warning("helo wolrd");
        }

        private static void ChaosBundle_LoadBundle(On.ChaosBundle.orig_LoadBundle orig)
        {
            Log.Warning("lodabundle");
            On.ChaosBundle.LoadBundle -= ChaosBundle_LoadBundle;
            orig();
            CreateAndApplyPalettes();
        }

        private static GameObject ChaosBundle_Get(On.ChaosBundle.orig_Get orig, string assetPath)
        {
            Log.Warning("bundle get");
            On.ChaosBundle.Get -= ChaosBundle_Get;

            CreateAndApplyPalettes();

            return orig(assetPath);
        }

        private static void CreateAndApplyPalettes()
        {
            CreatePaletteTexture();

            Material playerMaterial = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwap.mat");
            playerMaterial.SetFloat("_PaletteCount", 32 + palettes.Count);

            Material playerMaterial2 = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwapUnlit.mat");
            playerMaterial2.SetFloat("_PaletteCount", 32 + palettes.Count);
        }

        private static void CreatePaletteTexture()
        {
            Texture2D baseTexture = ChaosBundle.Get<Texture2D>("Assets/sprites/player/WizardPalette.png");

            List<Color32> colors = new List<Color32>();
            colors.AddRange(baseTexture.GetPixels32());
            foreach (Texture2D paletteTexture in palettes)
            {
                colors.AddRange(paletteTexture.GetPixels32());
            }
            baseTexture.Resize(baseTexture.width, baseTexture.height + palettes.Count * 2);
            baseTexture.SetPixels32(colors.ToArray());
            baseTexture.Apply();
        }
    }
}
