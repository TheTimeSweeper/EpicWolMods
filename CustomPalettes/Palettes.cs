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
        public static int AddPalette(params string[] pathDirectories)
        {
            return AddPalette(ImgHandler.LoadPNG(pathDirectories));
        }
        public static int AddPalette(Texture2D palleteTexture)
        {
            palettes.Add(palleteTexture);

            lastAssignableID++;
            return lastAssignableID;
        }

        internal static void Init()
        {
            On.ChaosBundle.Get += ChaosBundle_Get;
        }

        private static GameObject ChaosBundle_Get(On.ChaosBundle.orig_Get orig, string assetPath)
        {
            On.ChaosBundle.Get -= ChaosBundle_Get;

            ApplyWalterToPlayerSprite();

            CreateAndApplyPalettes();

            return orig(assetPath);
        }

        private static void ApplyWalterToPlayerSprite()
        {
            Texture2D baseTexture = ChaosBundle.Get<Texture2D>("Assets/sprites/player/NewWizard.png");
            Texture2D newTexture = ImgHandler.LoadTex2DFromAssets("Walter2.png");
            Color32[] Pixels = newTexture.GetPixels32();
            baseTexture.SetPixels32(Pixels);
            baseTexture.Apply();
        }

        private static void CreateAndApplyPalettes()
        {
            CreatePaletteTexture();

            Material playerMaterial = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwap.mat");
            playerMaterial.SetFloat("_PaletteCount", 32 + palettes.Count);

            Material playerMaterial2 = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwapUnlit.mat");
            playerMaterial2.SetFloat("_PaletteCount", 32 + palettes.Count);

            //the only one that escaped
            On.UnlockNotifier.SetNotice += (On.UnlockNotifier.orig_SetNotice orig, UnlockNotifier self, UnlockNotifier.NoticeVars vars) =>
            {
                self.outfitIconImage.material = playerMaterial2;
                orig(self, vars);
            };
        }

        private static void CreatePaletteTexture()
        {
            Texture2D newTexture = ImgHandler.LoadTex2DFromAssets("Base.png");
            List<Color32> colors = new List<Color32>();
            colors.AddRange(newTexture.GetPixels32());
            
            Color32 transparent = new Color32(0, 0, 0, 0);
            foreach (Texture2D paletteTexture in palettes)
            {
                //ugly fix to some strips having black pixels at the end
                Color32[] palettePixels = paletteTexture.GetPixels32();
                palettePixels[54] = transparent;
                palettePixels[55] = transparent;
                palettePixels[110] = transparent;
                palettePixels[111] = transparent;

                colors.AddRange(palettePixels);
            }

            Texture2D baseTexture = ChaosBundle.Get<Texture2D>("Assets/sprites/player/WizardPalette.png");
            baseTexture.Resize(baseTexture.width, baseTexture.height + palettes.Count * 2);
            baseTexture.SetPixels32(colors.ToArray());
            baseTexture.Apply();
        }
    }
}
