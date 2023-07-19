using LegendAPI;
using System.Collections.Generic;

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

            AnalOutfits();

            if (!ClothesPlugin.TournamentEditionInstalled)
            {
                foreach (string robeName in ContentLoaderStolen.robeNames)
                {
                    ContentLoaderStolen.palettes.Add(ImgHandlerStolen.LoadTex2D(robeName));
                }
            }
        }

        private static void TutorialOutfits()
        {

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


            #endregion testing tutorial
        }

        private static void SimpleOutfits()
        {
            int desolateOutfitColor = !ClothesPlugin.TournamentEditionInstalled ? ContentLoaderStolen.AssignNewID("Desolate") : 3;
            LegendAPI.OutfitInfo DesolateOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Desolate",
                outfit = new Outfit(
                    "Sweep_Desolate",
                    desolateOutfitColor,
                    new List<OutfitModStat> {
                        new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.069f, 0f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
                    }),
                customDesc = _ => { return "Glow Sticks"; },
            };
            LegendAPI.Outfits.Register(DesolateOutfit);
        }

        private static void JoeOutfit()
        {
            int joeOutfitColor = !ClothesPlugin.TournamentEditionInstalled ? ContentLoaderStolen.AssignNewID("Joe") : 4;
            LegendAPI.OutfitInfo joeOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Joe",
                outfit = new Outfit(
                    "Sweep_Joe",
                    joeOutfitColor,
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, -0.2f, 0f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.2f, 0f, false),
                    }),
                customDesc = (showStats) =>
                {
                    string description = "- Reduces attack end time";

                    if (showStats)
                    {
                        string statString = "+ 10 %";
                        string formattedStats = $"<color=#009999>( </color><color=#00dddd>{statString}</color><color=#009999> )</color>";

                        description = $"- Reduces attack end time {formattedStats}";
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
            //On.Player.SkillState.SetAnimTimes += SkillState_SetAnimTimes;
            //On.Player.SlideState.cctor += SlideState_cctor;
            On.Player.SlideState.OnEnter += SlideState_OnEnter;
        }

        private static void SlideState_OnEnter(On.Player.SlideState.orig_OnEnter orig, Player.SlideState self)
        {
            if (CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe"))
            {
                self.slideStopwatch.SetDelay(0.1f/*0.25f * 0.8f*/);
            }
            else
            {
                self.slideStopwatch.SetDelay(0.25f);
            }

            orig(self);


        }

        private static bool SkillState_ExitToSkillOrRunOrIdle(On.Player.SkillState.orig_ExitToSkillOrRunOrIdle orig, Player.SkillState self)
        {
            if (!CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe"))
                return orig(self);

            float origCancel = self.cancelThreshold;
            float origRun =    self.runThreshold;
            float origExit =   self.exitThreshold;

            self.cancelThreshold *= 0.7f;
            self.runThreshold    *= 0.7f;
            self.exitThreshold *= 0.7f;

            bool returnOrig = orig(self);

            self.cancelThreshold = origCancel;
            self.runThreshold    = origRun;
            self.exitThreshold = origExit;

            return returnOrig;

        }

        private static void SlideState_cctor(On.Player.SlideState.orig_cctor orig)
        {
            orig();
        }

        private static void SkillState_SetAnimTimes(On.Player.SkillState.orig_SetAnimTimes orig, Player.SkillState self, float newStart, float newHold, float newExecute, float newCancel, float newRun, float newExit)
        {
            if (CustomOutfitModManager.PlayerHasMod(self.parent, "Sweep_Joe"))
            {
                Log.Warning("sweepin");
                newStart *= 0.7f;
                newHold *= 0.7f;
                newExecute *= 0.7f;
                newCancel *= 0.7f;
                newRun *= 0.7f;
                newExit *= 0.7f;
            }

            orig(self, newStart, newHold, newExecute, newCancel, newRun, newExit);
        }

        private static void AnalOutfits()
        {
            int analOutfitColor = !ClothesPlugin.TournamentEditionInstalled ? ContentLoaderStolen.AssignNewID("Anal") : 21;
            LegendAPI.OutfitInfo TestOutfit = new LegendAPI.OutfitInfo()
            {
                name = "Analysis",
                outfit = new Outfit(
                    "Sweep_Analysis",
                    analOutfitColor,
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                        new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0f, 0f, -1f, false),
                        new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.9f, 0f, false),
                    }),
                customDesc = _ => { return "- Press G to Empower Arcana"; },
                customMod = (player, onoroff, idontevenknow) =>
                {
                    ClothesPlugin.debugPlayer = onoroff ? player : null;
                }
            };
            LegendAPI.Outfits.Register(TestOutfit);

            int analOutfitColor2 = !ClothesPlugin.TournamentEditionInstalled ? ContentLoaderStolen.AssignNewID("Anal3") : 22;
            LegendAPI.OutfitInfo TestOutfit2 = new LegendAPI.OutfitInfo()
            {
                name = "Appraisal",
                outfit = new Outfit(
                    "Sweep_Analysis2",
                    analOutfitColor2,
                    new List<OutfitModStat> {
                        new OutfitModStat(LegendAPI.Outfits.CustomModType, 0, 0, 0, true),
                        new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0f, 0f, -1f, false),
                    }),
                customDesc = _ => { return "- Press G to Empower Arcana"; },
                customMod = (player, onoroff, idontevenknow) =>
                {
                    ClothesPlugin.debugPlayer = onoroff ? player : null;
                }
            };
            LegendAPI.Outfits.Register(TestOutfit2);
        }
    }
}

/*
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