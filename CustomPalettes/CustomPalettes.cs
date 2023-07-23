using System.Collections.Generic;
using UnityEngine;

namespace CustomPalettes
{
    public class CustomPalettes
    {
        public static List<Texture2D> palettes = new List<Texture2D>();

        public static int lastAssignableID = 31;

        public static Texture2D newPalette = null;

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
            playerMaterial.SetTexture("_Palette", newPalette);

            Material playerMaterial2 = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwapUnlit.mat");

            playerMaterial2.SetFloat("_PaletteCount", 32 + palettes.Count);
            playerMaterial2.SetTexture("_Palette", newPalette);

            //unlock notifier

            return orig(assetPath);
        }

        private static void CreatePaletteTexture()
        {
            //basePalette = ImgHandler.LoadSpriteFromAssets("Base.png");
            //Debug.LogWarning("palette");
            Texture2D baseTexture = ChaosBundle.Get<Texture2D>("Assets/sprites/player/WizardPalette.png");//basePalette.texture;// (Texture2D) self.spriteMaterial.GetTexture("_Palette");
            if (newPalette == null)
            {
                //Debug.Log("1");
                newPalette = baseTexture;
                //Debug.Log("2");
                Texture2D t = newPalette;
                Texture2D newT;
                int h = t.height;
                //Debug.Log("3");
                foreach (Texture2D texture in palettes)
                {
                    //Debug.Log("Iterating over " + te.name);
                    newT = new Texture2D(newPalette.width, newPalette.height + 2, TextureFormat.RGBA32, false);
                    newT = FillColorAlpha(newT);
                    for (int x = 1; x < newT.width; x++)
                    {
                        for (int y = 0; y < newPalette.height; y++)
                        {
                            newT.SetPixel(x, y, newPalette.GetPixel(x, y));
                        }
                    }
                    // Debug.Log("Out of loop for " + te.name);
                    for (int x = 1; x < newT.width; x++)
                    {
                        newT.SetPixel(x, h, texture.GetPixel(x, h));
                    }
                    for (int x = 1; x < newT.width; x++)
                    {
                        newT.SetPixel(x, h + 1, texture.GetPixel(x, h + 1));
                    }

                    //Debug.Log("Out of loop 2 for " + te.name);
                    newT.filterMode = FilterMode.Point;
                    newT.Apply();

                    newPalette = newT;
                    //Debug.LogWarning("palette added");
                    h += 2;
                }

            }
        }

        private static Texture2D FillColorAlpha(Texture2D tex2D, Color32? fillColor = null)
        {
            if (fillColor == null)
            {
                fillColor = UnityEngine.Color.clear;
            }
            Color32[] fillPixels = new Color32[tex2D.width * tex2D.height];
            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = (Color32)fillColor;
            }
            tex2D.SetPixels32(fillPixels);
            return tex2D;
        }

    }
}
