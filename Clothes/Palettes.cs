
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Clothes
{
    internal class Palettes
    {
        public static List<Texture2D> palettes = new List<Texture2D>();

        public static int nextAssignableID = 32;

        public static int AssignNewID(string file) => AssignNewID(ImgHandlerStolen.LoadTex2D(file));
        public static int AssignNewID(Texture2D texture)
        {
            palettes.Add(texture);

            nextAssignableID += 1;
            return nextAssignableID - 1;
        }

        public static void Init()
        {
            On.ChaosBundle.Get += ChaosBundle_Get;
        }

        private static GameObject ChaosBundle_Get(On.ChaosBundle.orig_Get orig, string assetPath)
        {
            On.ChaosBundle.Get -= ChaosBundle_Get;

            CreatePaletteTexture();
            
            Material playerMaterial = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwap.mat");
            playerMaterial.SetFloat("_PaletteCount", 32 + palettes.Count);

            Material playerMaterial2 = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwapUnlit.mat");
            playerMaterial2.SetFloat("_PaletteCount", 32 + palettes.Count);

            return orig(assetPath);
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

            if (Configger.anal)
            {
                File.WriteAllBytes("FunnyPalette.png", baseTexture.EncodeToPNG());
            }
        }
    }
}
