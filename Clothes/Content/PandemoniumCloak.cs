using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Clothes
{
    public class PandemoniumCloak
    {
        private static StripContainer _randomStrips;
        private static List<Color32> _cachedStripColorsList;
        private static List<Color32> _cachedStripPixelsList;
        
        public static int RandomStartIndex;
        public static int RandomEndIndex;

        public static int ShadowStartIndex;
        public static int ShadowEndIndex;

        internal static void Init()
        {
            GenerateRandomOutfits();

            CreatePandemoniumOutfit();
        }

        private static void CreatePandemoniumOutfit()
        {
            LegendAPI.OutfitInfo pandemoniumOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Pandemonium",
                outfit = new Outfit(
                    "Sweep_Pandemonium",
                    Clothes.GetCustomColor("RGB.png"),
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false),
                    }),
                customDesc = (showStats, outfitModStat) =>
                {
                    return "- <i>HEE HEE HEE</i>";
                },
                customMod = (player, isEquipping, onEquip, outfitModStat) =>
                {
                    CustomOutfitModManager.EvaluateMod<PandemoniumMod>("Sweep_Pandemonium", player, isEquipping);
                },
            };
            LegendAPI.Outfits.Register(pandemoniumOutfit);
        }

        private static void GenerateRandomOutfits()
        {
            initCachedVariables();

            List<int[]> indicesList = GenerateRandomOutfitIndices();

            for (int i = 0; i < indicesList.Count; i++)
            {
                Texture2D stripTexture = CreateStrip(indicesList[i]);

                //CreateTestOutfit(indicesList[i][0], indicesList[i][1], indicesList[i][2], indicesList[i][3], indicesList[i][4]);
                int palette = Clothes.GetCustomColor(stripTexture);
                if (RandomStartIndex == 0)
                {
                    RandomStartIndex = palette;
                }
                if (i == indicesList.Count - 1)
                {
                    RandomEndIndex = palette;
                }
            }

            for (int i = 0; i < indicesList.Count; i++)
            {
                Texture2D stripTexture = CreateShadowStrip(indicesList[i]);

                int palette = Clothes.GetCustomColor(stripTexture);
                if (ShadowStartIndex == 0)
                {
                    ShadowStartIndex = palette;
                }
                if (i == indicesList.Count - 1)
                {
                    ShadowEndIndex = palette;
                }
            }
        }

        #region random outfit generation
        private static void initCachedVariables()
        {
            _randomStrips = Assets.funnyBundle.LoadAsset<StripContainer>("RandomStrips");

            _cachedStripColorsList = new List<Color32>();
            for (int i = 0; i < 28; i++)
            {
                _cachedStripColorsList.Add(new Color32(0, 0, 0, 0));
            }

            _cachedStripPixelsList = new List<Color32>();
            for (int i = 0; i < 112; i++)
            {
                _cachedStripPixelsList.Add(new Color32(0, 0, 0, 0));
            }
        }

        private static List<int[]> GenerateRandomOutfitIndices()
        {
            List<int[]> indicesList = new List<int[]>();

            List<int> capeStrips = new List<int>();
            List<int> capeUnderStrips = new List<int>();
            List<int> underStrips = new List<int>();
            List<int> shinyStrips = new List<int>();
            List<int> skinStrips = new List<int>();

            for (int i = 0; i < Configger.pandemoniumColors; i++)
            {
                int[] indices = new int[]
                {
                    GetRandomIndexFromList(capeStrips, _randomStrips.CapeTextures.Count),
                    GetRandomIndexFromList(capeUnderStrips, _randomStrips.CapeUnderTextures.Count),
                    GetRandomIndexFromList(underStrips, _randomStrips.UnderGarmentTextures.Count),
                    GetRandomIndexFromList(shinyStrips, _randomStrips.ShinyTextures.Count),
                    GetRandomIndexFromList(skinStrips, _randomStrips.SkinTextures.Count),
                };

                if (indicesList.Find(existingIndices => 
                    existingIndices[0] == indices[0] &&
                    existingIndices[1] == indices[1] &&
                    existingIndices[2] == indices[2] &&
                    existingIndices[3] == indices[3] &&
                    existingIndices[4] == indices[4]) != null)
                {
                    Log.Warning("skipping duplicate robe colors");
                    continue;
                }

                indicesList.Add(indices);
            }

            return indicesList;
        }

        private static int GetRandomIndexFromList(List<int> stripIndices, int count)
        {
            if(stripIndices.Count == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    stripIndices.Add(i);
                }
            }
            int randomIndex = stripIndices[Random.Range(0, stripIndices.Count)];
            stripIndices.Remove(randomIndex);
            return randomIndex;
        }

        private static void CreateTestOutfit(int cape, int capeUnder, int under, int eyes, int skin)
        {
            Texture2D stripTexture = CreateStrip(cape, capeUnder, under, eyes, skin);
            //Palettes.AssignNewID(_cachedStripTexture);

            LegendAPI.OutfitInfo testOutfit = new LegendAPI.OutfitInfo()
            {
                name = $"Pandemonium{cape}{capeUnder}{under}{eyes}{skin}",
                outfit = new Outfit(
                    $"Sweep_Random{cape}{capeUnder}{under}{eyes}{skin}",
                    Clothes.GetCustomColor(stripTexture),
                    new List<OutfitModStat>
                    {
                    },
                    false),
            };
            LegendAPI.Outfits.Register(testOutfit);
        }

        private static Texture2D CreateShadowStrip(params int[] stripsIndices)
        {
            Texture2D stripTexture = new Texture2D(56, 2);

            Color32[] capeColors = _randomStrips.CapeTextures[stripsIndices[0]].GetPixels32();
            Color32[] capeUnderColors = _randomStrips.CapeUnderTextures[stripsIndices[1]].GetPixels32();
            Color32[] underColors = _randomStrips.UnderGarmentTextures[stripsIndices[2]].GetPixels32();
            Color32[] shinyColors = _randomStrips.ShinyTextures[stripsIndices[3]].GetPixels32();
            Color32[] skinColors = _randomStrips.SkinTextures[stripsIndices[4]].GetPixels32();

            CreateStripColors(capeColors, capeUnderColors, underColors, shinyColors, skinColors);
            for (int i = 0; i < _cachedStripPixelsList.Count; i++)
            {
                Color32 stripPixel = _cachedStripPixelsList[i];
                stripPixel.a = stripPixel.a == (byte)0 ? (byte)0 : (byte)220;
                stripPixel.r /= 2;
                stripPixel.g /= 2;
                stripPixel.b /= 2;
                _cachedStripPixelsList[i] = stripPixel;
            }
            stripTexture.SetPixels32(_cachedStripPixelsList.ToArray());
            stripTexture.Apply();

            return stripTexture;
        }

        private static Texture2D CreateStrip(params int[] stripsIndices)
        {
            Texture2D stripTexture = new Texture2D(56, 2);

            Color32[] capeColors = _randomStrips.CapeTextures[stripsIndices[0]].GetPixels32();
            Color32[] capeUnderColors = _randomStrips.CapeUnderTextures[stripsIndices[1]].GetPixels32();
            Color32[] underColors = _randomStrips.UnderGarmentTextures[stripsIndices[2]].GetPixels32();
            Color32[] shinyColors = _randomStrips.ShinyTextures[stripsIndices[3]].GetPixels32();
            Color32[] skinColors = _randomStrips.SkinTextures[stripsIndices[4]].GetPixels32();

            CreateStripColors(capeColors, capeUnderColors, underColors, shinyColors, skinColors);

            stripTexture.SetPixels32(_cachedStripPixelsList.ToArray());
            stripTexture.Apply();

            return stripTexture;
        }

        private static List<Color32> CreateStripColors(params Color32[][] capeColors)
        {
            for (int i = 0; i < capeColors.Length; i++)
            {
                ApplyStripColors(capeColors[i]);
            }

            for (int i = 0; i < _cachedStripColorsList.Count * 2 - 1; i++)
            {
                int j = Mathf.FloorToInt(i * 0.5f);
                _cachedStripPixelsList[i] = _cachedStripColorsList[j];
            }

            for (int i = 56; i < _cachedStripPixelsList.Count; i++)
            {
                _cachedStripPixelsList[i] = _cachedStripPixelsList[i - 56];
            }

            return _cachedStripPixelsList;
        }

        private static void ApplyStripColors(Color32[] sourceColors)
        {
            for (int i = 0; i < sourceColors.Length; i++)
            {
                //skip 6th pixel because walter uses it for outline
                if (ClothesPlugin.palettesPluginInstalled && i == 6)
                    continue;

                if (sourceColors[i].a > 0)
                {
                    _cachedStripColorsList[i] = sourceColors[i];
                }
            }
        }

        #endregion random outfit generation
    }
}