using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Clothes
{
    //personal mod for fun and to learn about robes.
    //credit to only_going_up_fr0m_here with tournamentedition
    [BepInPlugin("TheTimeSweeper.Clothes", "Clothes", "0.3.0")]
    public class ClothesPlugin : BaseUnityPlugin {

        public static PluginInfo PluginInfo;

        public static bool isDebugPlayerReal => debugPlayer != null;
        public static Player debugPlayer = null;
        public static bool empowered;

        void Awake() {
            PluginInfo = Info;

			Clothes.Init();

			ContentLoaderStolen.Init();

            On.GameController.Start += GameController_Start_LateInit;
		}

        private void GameController_Start_LateInit(On.GameController.orig_Start orig, GameController self)
        {
            orig(self);
            //StolenFunnyCurse.Init();
            //broken when not wearing cloak
            //beat sura with infinite halo lol
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (debugPlayer != null)
                {
                    empowered = !empowered;
                    foreach (Player.SkillState skillState in debugPlayer.skillsDict.Values)
                    {
                        skillState.SetEmpowered(empowered, EmpowerStatMods.DefaultEmpowerMod);
                    }
                }
            }
        }
    }

	public class Clothes {

		//public static Dictionary<CustomColor, Texture2D> Palettes = new Dictionary<CustomColor, Texture2D>();

		public static void Init() {

            #region testing tutorial
            //OutfitInfo outfit = new OutfitInfo();
            //outfit.name = "Cool Guy";
            //outfit.outfit = new Outfit(
            //    "ModName::OutfitID",
            //    1,
            //    new List<OutfitModStat>()
            //    {
            //        new OutfitModStat(OutfitModStat.OutfitModType.Health,100,0,0,false),
            //        // more OutfitModStats here
            //    });

            //LegendAPI.Outfits.Register(outfit);

            //OutfitInfo coolOutfit = new OutfitInfo()
            //{
            //    name = "Cool Guy",
            //    outfit = new Outfit(
            //        "ModName::OutfitID", 
            //        1, 
            //        new List<OutfitModStat>
            //        {
            //            new OutfitModStat(OutfitModStat.OutfitModType.Health,100,0,0,false),
            //            // more OutfitModStats here
            //        })
            //};

            //Outfits.Register(coolOutfit);
            #endregion testing tutorial

            LegendAPI.OutfitInfo DesolateOutfit = new LegendAPI.OutfitInfo() {
				name = "Desolate",
				customDesc = _ => { return "Glow Sticks"; },
				outfit = new Outfit("Sweep_Desolate", ContentLoaderStolen.AssignNewID("Desolate")/*(int)CustomColor.Desolate*/, new List<OutfitModStat> {
					new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.069f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				})
			};

			//Palettes[CustomColor.Desolate] = ImgHandlerStolen.LoadTex2D(DesolateOutfit.name);

			LegendAPI.Outfits.Register(DesolateOutfit);

            LegendAPI.OutfitInfo TestOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Analysis",
                customDesc = _ => { return "- Press G to Empower Arcana"; },
                outfit = new Outfit("Sweep_Analysis", ContentLoaderStolen.AssignNewID("Anal"), new List<OutfitModStat> {
                    new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                    new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.9f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0f, 0f, -1f, false),
                }),
                customMod = (player, onoroff, idontevenknow)=>
                {
                    ClothesPlugin.debugPlayer = onoroff ? player : null;
                }
            };

            LegendAPI.Outfits.Register(TestOutfit);

            foreach (string robeName in ContentLoaderStolen.robeNames)
            {
                ContentLoaderStolen.palettes.Add(ImgHandlerStolen.LoadTex2D(robeName));
            }
        }

        public enum CustomColor {
			Desolate = 33
		}
	}
}


/*
 * 
				Outfit.baseHope,
				Outfit.basePatience,
				new Outfit("Vigor", 20, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Grit", 17, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Greed", 18, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Platinum, 1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Pink", 14, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealCrit, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pace", 3, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)3, 0f, -0.3f, 0f, false)
				}, false, false),
				new Outfit("Tempo", 5, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Switch", 4, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)4, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.05f, 0f, 0f, false)
				}, false, false),
				new Outfit("Awe", 6, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritDamage, 0.25f, 0f, 0f, false)
				}, false, false),
				new Outfit("Fury", 8, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODDamage, 0f, 0.25f, 0f, false)
				}, false, false),
				new Outfit("Rule", 19, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit(Outfit.lvlStr, 7, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, -0.5f, 0f, false)
				}, false, true),
				new Outfit("Venture", 2, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.4f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Fall", 9, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 100f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)11, 0f, 0f, 0f, true),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pride", 15, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 1f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)10, 0f, 0f, -1f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false)
 */