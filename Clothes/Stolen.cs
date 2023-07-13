
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

            On.Player.SetPlayerOutfitColor += Us_AddOutfit;

            On.GameProgressBoard.SetPlayerColors += (On.GameProgressBoard.orig_SetPlayerColors orig, GameProgressBoard self) =>
            {
                if (newPalette != null)
                {
                    self.p1PieceImage.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                    self.p1PieceImage.material.SetTexture("_Palette", newPalette);
                }
                orig(self);

            };

            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig, OutfitMenu self, Player p) => {
                self.outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                self.outfitImage.material.SetTexture("_Palette", newPalette);
                orig(self, p);
            };

            On.DeathSummaryUI.Activate += (On.DeathSummaryUI.orig_Activate orig, DeathSummaryUI self, float f) => {
                orig(self, f);
                if (newPalette != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        self.playerRefs[i].outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                        self.playerRefs[i].outfitImage.material.SetTexture("_Palette", newPalette);
                    }
                }
            };

            On.WardrobeUI.LoadOutfits += (On.WardrobeUI.orig_LoadOutfits orig, WardrobeUI self) => {
                orig(self);
                if (newPalette != null)
                {
                    for (int j = 0; j < self.totalOutfitCount; j++)
                    {

                        self.wrRef.outfitImageArray[j].material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                        self.wrRef.outfitImageArray[j].material.SetTexture("_Palette", newPalette);
                    }
                }
            };
            On.DialogManager.Activate += (On.DialogManager.orig_Activate orig, DialogManager self, DialogMessage m, bool b, bool s) => {
                orig(self, m, b, s);
                if (m.rightActive && m.RightSpeaker != null && newPalette != null)
                {
                    self.rightPlayerImage.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                    self.rightPlayerImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.UnlockNotifier.SetNotice += (On.UnlockNotifier.orig_SetNotice orig, UnlockNotifier self, UnlockNotifier.NoticeVars vars) => {
                orig(self, vars);
                if (newPalette != null)
                {
                    self.outfitIconImage.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                    self.outfitIconImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.WardrobeUI.UpdateOutfits += (On.WardrobeUI.orig_UpdateOutfits orig, WardrobeUI self, int givenIndex) => {
                orig(self, givenIndex);
                if (newPalette != null)
                {
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetTexture("_Palette", newPalette);
                }
            };
            On.OutfitStoreItem.Start += (On.OutfitStoreItem.orig_Start orig, OutfitStoreItem self) => {
                orig(self);
                self.itemSpriteRenderer.material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                self.itemSpriteRenderer.material.SetTexture("_Palette", newPalette);
            };
            On.WardrobeUI.AssignOutfit += (On.WardrobeUI.orig_AssignOutfit orig, WardrobeUI self, Outfit o, int i) => {
                self.wrRef.outfitImageArray[i].material.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                self.wrRef.outfitImageArray[i].material.SetTexture("_Palette", newPalette);
                orig(self, o, i);
            };
            On.PlayerStatusBar.Awake += (On.PlayerStatusBar.orig_Awake orig, PlayerStatusBar self) => {
                orig(self);
                self.gameObject.AddComponent<StatusBarMod>().self = self;
            };

            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig, OutfitMenu self, Player p) => {
                orig(self, p);
                if (hasAddedPalettes)
                {
                    self.outfitImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.OutfitMenu.SwapFocus += (On.OutfitMenu.orig_SwapFocus orig, OutfitMenu self, bool n) => {
                orig(self, n);
                if (hasAddedPalettes)
                {
                    self.outfitImage.material.SetTexture("_Palette", newPalette);
                }
            };

            /*On.Player.InitComponents += (On.Player.orig_InitComponents orig, Player self) =>
            {
                orig(self);
                Debug.Log("Old Sprite Name: " + self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
                self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite = newPlayerSprite;
                Debug.Log("Set new palette: "+ self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
            };*/

        }


        public static void Us_AddOutfit(On.Player.orig_SetPlayerOutfitColor orig, Player self, NumVarStatMod mod, bool givenStatus)
        {
            orig(self, mod, givenStatus);

            //if (!loadedWizSprites) {
            //	loadedWizSprites = true;

            //	Texture2D text = new Texture2D(1, 1); text.LoadImage(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites/Walter2.png")));
            //	Texture2D texture2 = self.spriteRenderer.sprite.texture;
            //	texture2.SetPixels32(text.GetPixels32());
            //	texture2.Apply();
            //	EXPOSED = texture2;
            //	StartCoroutine("BootUpCredits");
            //}

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
                        h += 2;
                    }
                }
            }

            if (hasAddedPalettes)
            {
                self.spriteMaterial.SetFloat("_PaletteCount", 32 + palettes.Count /*Clothes.Palettes.Count*/);
                self.spriteMaterial.SetTexture("_Palette", newPalette);
            }

            //orig(self, mod, givenStatus);
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
