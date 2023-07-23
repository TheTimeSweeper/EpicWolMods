using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPalettes
{
    public class CustomPalettes
    {
        public static List<Texture2D> palettes = new List<Texture2D>();

        public static int lastAssignableID = 31;

        //public static Texture2D newPalette = null;

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

        internal static void Init()
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

            printPixel0(baseTexture, "base");

            int height = baseTexture.height;
            List<Color32> colors = new List<Color32>();
            colors.AddRange(baseTexture.GetPixels32());
            foreach (Texture2D paletteTexture in palettes)
            {
                colors.AddRange(paletteTexture.GetPixels32());
            }
            baseTexture.Resize(baseTexture.width, baseTexture.height + palettes.Count * 2);
            baseTexture.SetPixels32(colors.ToArray());
            baseTexture.Apply();

            printPixel0(baseTexture, "damn kvad");

            //Texture2D originalBaseTexture = new Texture2D(baseTexture.height, baseTexture.width, TextureFormat.RGBA32, false);
            //Graphics.CopyTexture(baseTexture, originalBaseTexture);

            //printPixel0(originalBaseTexture, "original base");

            //int additionalHeight = palettes.Count * 2;
            //baseTexture.Resize(baseTexture.width, baseTexture.height + additionalHeight);

            //printPixel0(baseTexture, "base after resize");

            //int paletteHeight = baseTexture.height;

            //for (int i = 0; i < palettes.Count; i++)
            //{
            //    Texture2D paletteTexture = palettes[i];

            //    for (int x = 1; x < baseTexture.width; x++)
            //    {
            //        baseTexture.SetPixel(x, paletteHeight, paletteTexture.GetPixel(x, 0));
            //    }
            //    for (int x = 1; x < baseTexture.width; x++)
            //    {
            //        baseTexture.SetPixel(x, paletteHeight + 1, paletteTexture.GetPixel(x, 1));
            //    }

            //    baseTexture.Apply();

            //    paletteHeight += 2;

            //    printPixel0(baseTexture, i.ToString());
            //}
        }

        private static void printPixel0(Texture2D baseTexture, string log)
        {
            for (int i = 0; i < baseTexture.height; i++)
            {
                log += $"\n{baseTexture.GetPixel(0, i)}";
            }
            
            Debug.LogWarning(log);
        }
    }
}
