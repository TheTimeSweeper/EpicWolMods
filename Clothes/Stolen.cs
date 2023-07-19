
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Clothes
{
    //stolen from tournamentEdition
    public static class ImgHandlerStolen
    {
        public static Sprite LoadSprite(string path)
        {
            Texture2D texture2D = LoadTex2D(path, true);
            texture2D.name = path;
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Rect rect = new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height);
            Sprite sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 16f);
            sprite.name = path;
            return sprite;
        }

        public static Texture2D LoadTex2D(string spriteFileName, bool pointFilter = false, Texture2D T2D = null, bool overrideFullPath = false)
        {
            string path = "Assets/" + spriteFileName + ".png";
            if (overrideFullPath)
            {
                path = spriteFileName;
            }
            string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            Texture2D texture2D = T2D == null ? ImgHandlerStolen.LoadPNG(text, pointFilter) : T2D;
            if (pointFilter)
            {
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
            }
            return texture2D;
        }

        public static Texture2D LoadPNG(string filePath, bool pointFilter = false)
        {
            Texture2D texture2D = new Texture2D(2, 2);
            byte[] data = File.ReadAllBytes(filePath);
            texture2D.LoadImage(data);
            texture2D.filterMode = (pointFilter ? FilterMode.Point : FilterMode.Bilinear);
            texture2D.Apply();

            return texture2D;
        }
    }

    //stolen from tournamentEdition
    public class ContentLoaderStolen
    {
        public static List<Texture2D> palettes = new List<Texture2D>();

        public static bool hasAddedPalettes = false;
        public static Texture2D newPalette = null;
        public static Sprite basePalette;


        public static int nextAssignableID = 32;

        public static List<string> robeNames = new List<string>();

        public static int AssignNewID(string file)
        {
            robeNames.Add(file);

            nextAssignableID += 1;
            return nextAssignableID - 1;
        }
        
        public static void Init()
        {
            basePalette = ImgHandlerStolen.LoadSprite("Base");

            On.ChaosBundle.Get += ChaosBundle_Get;

            //HookAllTheThings();
        }

        private static GameObject ChaosBundle_Get(On.ChaosBundle.orig_Get orig, string assetPath)
        {
            if (hasAddedPalettes)
                return orig(assetPath);

            CreatePaletteTexture();

            Material playerMaterial = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwap.mat");

            playerMaterial.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
            playerMaterial.SetTexture("_Palette", newPalette);

            playerMaterial = ChaosBundle.Get<Material>("Assets/materials/WizardPaletteSwapUnlit.mat");

            playerMaterial.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
            playerMaterial.SetTexture("_Palette", newPalette);

            //unlock notifier

            return orig(assetPath);
        }

        private static void ChaosBundle_LoadBundle(On.ChaosBundle.orig_LoadBundle orig)
        {
            orig();
        }

        private static void CreatePaletteTexture()
        {
            Debug.LogWarning("palette");
            Texture2D baseTexture = basePalette.texture;// (Texture2D) self.spriteMaterial.GetTexture("_Palette");
            if (newPalette == null)
            {
                //Debug.Log("1");
                newPalette = baseTexture;
                if (!hasAddedPalettes)
                {
                    //Debug.Log("2");
                    hasAddedPalettes = true;
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
                        Debug.LogWarning("palette added");
                        h += 2;
                    }
                }
            }
        }

        public static Texture2D FillColorAlpha(Texture2D tex2D, Color32? fillColor = null)
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
