using LegendAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine;

namespace Clothes
{
    public class Clothes {
        public static string StyleStatString(string statString)
        {
            return $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";
        }
        //public static Dictionary<CustomColor, Texture2D> Palettes = new Dictionary<CustomColor, Texture2D>();

        public static void Init()
        {
            TutorialOutfits();

            SimpleOutfits();

            JoeOutfit();

            if (Configger.anal)
            {
                AnalOutfits();
            }

            PandemoniumCloak.Init();
        }

        public static int GetCustomColor(Texture2D texture, int fallback)
        {
            if (ClothesPlugin.palettesPluginInstalled)
            {
                return TryGetCustomPalette(texture);
            }

            if (!ClothesPlugin.tournamentEditionInstalled)
            {
                return Palettes.AssignNewID(texture);
            }

            return fallback;
        }
        public static int GetCustomColor(string fileName, int fallback)
        {
            if (ClothesPlugin.palettesPluginInstalled)
            {
                return TryGetCustomPalette(fileName);
            }

            if (!ClothesPlugin.tournamentEditionInstalled)
            {
                return Palettes.AssignNewID(fileName);
            }

            return fallback;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static int TryGetCustomPalette(string fileName)
        {
            return CustomPalettes.Palettes.AddPalette(Path.GetDirectoryName(ClothesPlugin.PluginInfo.Location), "Assets", fileName);
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static int TryGetCustomPalette(Texture2D texture)
        {
            return CustomPalettes.Palettes.AddPalette(texture);
        }

        #region testing tutorial
        private static void TutorialOutfits()
        {
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

            OutfitInfo coolOutfit = new OutfitInfo()
            {
                name = "Cool Guy",
                outfit = new Outfit(
                    "ModName::OutfitID", //newID
                    1, //newColorIndex
                    new List<OutfitModStat>
                    {
            new OutfitModStat(OutfitModStat.OutfitModType.Health,
                              100, //addValue
                              0, //multiValue
                              0, //override value
                              false), //bool value
                                      // more OutfitModStats here
                    },
                    false, //isUnlocked
                    false) //isLeveling
            };

            //Outfits.Register(coolOutfit);


            OutfitInfo coolerOutfit = new OutfitInfo()
            {
                name = "Cooler Guy",
                outfit = new Outfit(
                    "ModName::OutfitID", //newID
                    1, //newColorIndex
                    new List<OutfitModStat>
                    {
            new OutfitModStat(OutfitModStat.OutfitModType.Health,100,0,0,false),
            new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                        // more OutfitModStats here
                    },
                    true, //isUnlocked
                    false), //isLeveling
                customDesc = (showStats) =>
                {
                    string description = "- Gives Cool Effect";

                    if (showStats)
                    {
                        string statString = "+ 10 %";
                        string formattedStats = $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";

                        description = $"- Gives Cool Effect {formattedStats}";
                    }

                    return description;
                },
                customMod = (player, isEquipping, onEquip) =>
                {
                    if (isEquipping)
                    {
                        player.transform.localScale = new UnityEngine.Vector3(2f, 2f, 2f);
                    }
                    else
                    {
                        player.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
                    }
                },
                unlockCondition = () =>
                {
                    return true;
                }
            };

            //LegendAPI.Outfits.Register(coolerOutfit);
        }
        #endregion testing tutorial

        private static void SimpleOutfits()
        {
            List<OutfitModStat> desoModStats = new List<OutfitModStat>
            {
                new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.069f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.09f, 0f, false)
            };

            if (Configger.funny)
            {
                desoModStats.Insert(0, new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false));
            }

            LegendAPI.OutfitInfo DesolateOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Desolate",
                outfit = new Outfit(
                    "Sweep_Desolate",
                    GetCustomColor("Desolate.png", 3),
                    desoModStats),
                customDesc = _ => { return "Glow Sticks"; },
            };
            LegendAPI.Outfits.Register(DesolateOutfit);
 
            LegendAPI.OutfitInfo BoosOutfit = new LegendAPI.OutfitInfo()
            {
                name = Configger.funny? "Boos": "Credence",
                outfit = new Outfit(
                    $"Sweep_Boos",
                    GetCustomColor("Boos1.png", 19),
                    new List<OutfitModStat>
                    {
                        new OutfitModStat(OutfitModStat.OutfitModType.Evade, 0f, 0.1f, 0f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, 0.1f, 0f, false)
                    }),
            };
            LegendAPI.Outfits.Register(BoosOutfit);
            
            LegendAPI.OutfitInfo AquaOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Aqua",
                outfit = new Outfit(
                    $"Sweep_Aqua",
                    GetCustomColor("Aqua.png", 5),
                    new List<OutfitModStat>
                    {
                        new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.HealCrit, 0.1f, 0.0f, 0f, false)
                    }),
            };
            LegendAPI.Outfits.Register(AquaOutfit);
        }

        #region joe
        private static void JoeOutfit()
        {
            int joeOutfitColor = GetCustomColor("Joe.png", 4);
            LegendAPI.OutfitInfo joeOutfit = new LegendAPI.OutfitInfo()
            {
                name = Configger.funny ? "Joe" : "Impatience",
                outfit = new Outfit(
                    "Sweep_Joe",
                    joeOutfitColor,
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.2f, 0f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.2f, 0f, false),
                        //#region tesuto
                        //new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0, -0.9f, 0f, false),
                        //new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0, -1, 0, false)
                        //#endregion
                    }),
                customDesc = (showStats) =>
                {
                    string description = "- Reduces arcana end time";
                    
                    if (showStats)
                    {
                        float modValue = 15;
                        if(CustomOutfitModManager.PlayerHasMod("Sweep_Joe", out bool upgraded))
                        {
                            if (upgraded) modValue *= 1.5f;
                        }
                        string statString = $"+ {modValue} %";
                        string formattedStats = $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";

                        description = $"- Reduces arcana end time {formattedStats}";
                    }

                    return description;
                },
                customMod = (player, isEquipping, onEquip) =>
                {
                    CustomOutfitModManager.EvaluateMod<JoeOutfitMod>("Sweep_Joe", player, isEquipping);
                },
            };
            LegendAPI.Outfits.Register(joeOutfit);

            #region test joes
            //LegendAPI.OutfitInfo joeOutfit2 = new LegendAPI.OutfitInfo()
            //{
            //    name = "Joe both",
            //    outfit = new Outfit(
            //        "Sweep_Joe2",
            //        joeOutfitColor,
            //        new List<OutfitModStat> {
            //            new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.2f, 0f, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.2f, 0f, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.9f, 0f, false),
            //        }),
            //    customDesc = (showStats) =>
            //    {
            //        string description = "- Increases Attack Speed";

            //        if (showStats)
            //        {
            //            string statString = "+ 30 %";
            //            string formattedStats = $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";

            //            description = $"- Increases Attack Speed {formattedStats}";
            //        }

            //        return description;
            //    },
            //    customMod = (player, isEquipping, onEquip) =>
            //    {
            //        NumVarStatMod animSpeedMod = new NumVarStatMod("JoeAnimSpeed", 0.30f, 10, VarStatModType.Additive, false);
            //        player.animSpeedHandler.Modify(animSpeedMod, isEquipping);

            //        CustomOutfitModManager.EvaluateMod<JoeOutfitMod>("Sweep_Joe", player, isEquipping);
            //    },
            //};
            //LegendAPI.Outfits.Register(joeOutfit2);

            //LegendAPI.OutfitInfo joeOutfit3 = new LegendAPI.OutfitInfo()
            //{
            //    name = "Joe just anim",
            //    outfit = new Outfit(
            //        "Sweep_Joe3",
            //        joeOutfitColor,
            //        new List<OutfitModStat> {
            //            new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.2f, 0f, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.2f, 0f, false),
            //            new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.9f, 0f, false),
            //        }),
            //    customDesc = (showStats) =>
            //    {
            //        string description = "- Increases Attack Speed";

            //        if (showStats)
            //        {
            //            string statString = "+ 30 %";
            //            string formattedStats = $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";

            //            description = $"- Increases Attack Speed {formattedStats}";
            //        }

            //        return description;
            //    },
            //    customMod = (player, isEquipping, onEquip) =>
            //    {
            //        NumVarStatMod animSpeedMod = new NumVarStatMod("JoeAnimSpeed", 0.30f, 10, VarStatModType.Additive, false);
            //        player.animSpeedHandler.Modify(animSpeedMod, isEquipping);
            //    },
            //};
            //LegendAPI.Outfits.Register(joeOutfit3);
            #endregion test joes
            
            On.Player.SkillState.ExitToSkillOrRunOrIdle += SkillState_ExitToSkillOrRunOrIdle;
            On.Player.SlideState.OnEnter += SlideState_OnEnter;
            //On.Player.SkillState.SetAnimTimes += SkillState_SetAnimTimes;
        }

        private static bool SkillState_ExitToSkillOrRunOrIdle(On.Player.SkillState.orig_ExitToSkillOrRunOrIdle orig, Player.SkillState self)
        {
            if (!CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe", out bool upgraded))
                return orig(self);

            float reduction = 0.15f;

            //don't fully reduce end duration of basic arcana combos
                //because they're already kinda redeuced
            Player.MeleeAttackState basicState = self as Player.MeleeAttackState;
            if (basicState != null)
            {
                if (basicState.HitsRemaining != 0)
                {
                    reduction = 0.05f;
                }
            }

            float multiplier = 1 - (upgraded ? reduction * 1.5f : reduction);

            float origCancel = self.cancelThreshold;
            float origRun =    self.runThreshold;
            float origExit =   self.exitThreshold;


            self.cancelThreshold *= multiplier;
            self.runThreshold *= multiplier;
            self.exitThreshold *= multiplier;

            bool returnOrig = orig(self);

            self.cancelThreshold = origCancel   ;
            self.runThreshold    = origRun      ;
            self.exitThreshold = origExit;

            return returnOrig;

        }

        private static void SlideState_OnEnter(On.Player.SlideState.orig_OnEnter orig, Player.SlideState self)
        {
            if (CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe", out bool upgraded))
            {
                float reduction = 0.2f;
                float multiplier = 1 - (upgraded ? reduction * 1.5f : reduction);

                self.slideStopwatch.SetDelay(0.25f * multiplier);
            }
            else if (CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Anal"))
            {
                self.slideStopwatch.SetDelay(0);
            }
            else
            {
                self.slideStopwatch.SetDelay(0.25f);
            }

            orig(self);
        }

        private static void SkillState_SetAnimTimes(On.Player.SkillState.orig_SetAnimTimes orig, Player.SkillState self, float newStart, float newHold, float newExecute, float newCancel, float newRun, float newExit)
        {
            if (CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe"))
            {
                newStart *= 0.7f;
                newHold *= 0.7f;
                newExecute *= 0.7f;
                newCancel *= 0.7f;
                newRun *= 0.7f;
                newExit *= 0.7f;
            }

            orig(self, newStart, newHold, newExecute, newCancel, newRun, newExit);
        }
        #endregion joe
        
        #region anal
        private static void AnalOutfits()
        {
            LegendAPI.OutfitInfo TestOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Analysis",
                outfit = new Outfit(
                    "Sweep_Analysis",
                    GetCustomColor("Anal.png", 21),
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                        new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0f, 0f, -1f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.9f, 0f, false),
                    }),
                customDesc = _ => { return "- Press G to Empower Arcana"; },
                customMod = (player, onoroff, idontevenknow) =>
                {
                    CustomOutfitModManager.EvaluateMod("Sweep_Anal", player, onoroff);
                }
            };
            LegendAPI.Outfits.Register(TestOutfit);

            LegendAPI.OutfitInfo TestOutfit2 = new LegendAPI.OutfitInfo()
            {
                name = "Appraisal",
                outfit = new Outfit(
                    "Sweep_Analysis2",
                    GetCustomColor("Anal3.png", 22),
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                        new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0f, 0f, -1f, false),
                    }),
                customDesc = _ => { return "- Press G to Empower Arcana"; },
                customMod = (player, onoroff, idontevenknow) =>
                {
                    CustomOutfitModManager.EvaluateMod("Sweep_Anal", player, onoroff);
                }
            };
            LegendAPI.Outfits.Register(TestOutfit2);
        }
        #endregion anal
    }
}

/*
				Outfit.baseHope,
				Outfit.basePatience,
				new Outfit("Vigor", 20, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, 0.1f, 0f, false),
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
					new OutfitModStat(OutfitModStat.OutfitModType.Speed, 0f, 0.15f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Run, 0f, -0.3f, 0f, false)
				}, false, false),
				new Outfit("Tempo", 5, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.15f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Speed, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Switch", 4, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Evade, 0.1f, 0f, 0f, false),
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
					new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, -0.5f, 0f, false)
				}, false, true),
				new Outfit("Venture", 2, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.4f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Speed, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Fall", 9, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, 0f, 100f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Fall, 0f, 0f, 0f, true),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pride", 15, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, 0f, 1f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Pride, 0f, 0f, -1f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false)

	public enum OutfitModType
	{
		None,
		Health,
		Speed,
		Run,
		Evade,
		Armor,
		CritChance,
		CritDamage,
		Damage,
		Cooldown,
		Pride,
		Fall,
		ODRate,
		ODDamage,
		HealAmount,
		HealCrit,
		Gold,
		Platinum,
		AllowUpgrade
	}
 */