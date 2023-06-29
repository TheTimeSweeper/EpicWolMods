using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillsButEpic
{
    [BepInPlugin("TheTimeSweeper.SkillsButEpic", "SkillsButEpic", "0.2.0")]
    public class SkillsButEpicPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Log.Init(Logger);

            Skills.Awake();

            ExampleSkillMod.Awake(Info);
        }
    }

    public class Skills
    {
        public static Dictionary<string, SkillInfo> SkillsDict = new Dictionary<string, SkillInfo>();

        private static List<string> badentrypointsignal = new List<string>();

        public static void Register(SkillInfo skill)
        {
            SkillsDict[skill.ID] = skill;
        }
        
        public static void Awake()
        {
            On.GameController.Awake += delegate (On.GameController.orig_Awake orig, GameController self)
            {
                orig.Invoke(self);
                On.LootManager.ResetAvailableSkills += LootManager_ResetAvailableSkills;
            };

            On.StatManager.LoadData += StatManager_LoadData_AddSkillsToLoadedData;

            On.Player.InitFSM += Player_InitFSM_AddSkills;
        }

        private static void LootManager_ResetAvailableSkills(On.LootManager.orig_ResetAvailableSkills orig)
        {
            if (badentrypointsignal.Contains("Loot")){
                //Log.Warning("already entered LootManager_ResetAvailableSkills. skipping");
                orig();
                return; 
            }

            foreach (SkillInfo info in SkillsDict.Values)
            {
                if (!LootManager.completeSkillList.Contains(info.ID))
                {
                    LootManager.completeSkillList.Add(info.ID);
                }
                //add tiers if this skill is higher than max
                if (info.tier >= LootManager.maxSkillTier)
                {
                    for (int i = LootManager.maxSkillTier; i <= info.tier; i++)
                    {
                        LootManager.skillTierDict.Add(i, new List<string>());
                    }
                    LootManager.maxSkillTier = info.tier + 1;
                }
                //if this skill already exists, remove it from an existing tier
                foreach (List<string> list in LootManager.skillTierDict.Values)
                {
                    if (list.Contains(info.ID))
                    {
                        list.Remove(info.ID);
                    }
                }
                //add this skill to tier
                LootManager.skillTierDict[info.tier].Add(info.ID);
            }
            badentrypointsignal.Add("Loot");
            orig();
        }


        private static void StatManager_LoadData_AddSkillsToLoadedData(On.StatManager.orig_LoadData orig, string assetPath, string statID, string category, string categoryModifier)
        {
            orig(assetPath, statID, category, categoryModifier);

            //Null checks to make sure errors don't occur
            if (IconManager.skillIcons == null)
            {
                IconManager.skillIcons = IconManager.SkillIcons;
            }
            if (TextManager.skillInfoDict == null)
            {
                TextManager.skillInfoDict = new Dictionary<string, TextManager.SkillInfo>();
            }

            foreach (SkillInfo skillInfo in SkillsDict.Values)
            {
                if (skillInfo.Sprite != null)
                {
                    IconManager.skillIcons[skillInfo.ID] = skillInfo.Sprite;
                }

                //Putting the skill text stuff here since idk where else it should go
                TextManager.SkillInfo skillText = new TextManager.SkillInfo();
                skillText.skillID = skillInfo.ID;
                skillText.displayName = skillInfo.DisplayName;
                skillText.description = skillInfo.Description;
                skillText.empowered = skillInfo.EnhancedDescription;
                TextManager.skillInfoDict[skillInfo.ID] = skillText;

                SkillStats stats = skillInfo.SkillStats;
                stats.Initialize();
                string fullCategory = category + categoryModifier;
                StatData statData = new StatData(stats, fullCategory);
                List<string> targetNames = statData.GetValue<List<string>>("targetNames", -1);

                if (targetNames.Contains(Globals.allyHBStr) || targetNames.Contains(Globals.enemyHBStr))
                {
                    targetNames.Add(Globals.ffaHBStr);
                }
                if (targetNames.Contains(Globals.allyFCStr) || targetNames.Contains(Globals.enemyFCStr))
                {
                    targetNames.Add(Globals.ffaFCStr);
                }

                Dictionary<string, StatData> dictionary = StatManager.data[statID][fullCategory];
                dictionary[statData.GetValue<string>("ID", -1)] = statData;
                StatManager.globalSkillData[statData.GetValue<string>("ID", -1)] = statData;
            }
        }

        private static void Player_InitFSM_AddSkills(On.Player.orig_InitFSM orig, Player self)
        {

            orig(self);

            foreach (SkillInfo info in SkillsDict.Values)
            {
                Player.SkillState state = (Player.SkillState)Activator.CreateInstance(info.StateType, self.fsm, self);
                //self.skillsDict[info.ID] = state;

                //no Idea what this was
                //info.StateDict[self.playerID] = state;
                self.fsm.AddState(state);
            }
            //new skills have been added to the skilldict. check GameDataManager.gameData.SkillDataDictionary if they're unlocked
            GameDataManager.gameData.PullSkillData();
        }
    }

    public class SkillInfo
    {
        public string ID = "NewArcanaIDDefaultChangeThisPls<3";
        public string DisplayName = "Spell Display Name";
        public string Description = "Description Goes Here";
        public string EnhancedDescription = "Enhanced Description Goes Here";
        public int tier = 0;
        public SkillStats SkillStats;
        public Type StateType;
        public ElementType Element = ElementType.Fire;
        public Sprite Sprite = null;

        public Dictionary<int, Player.SkillState> StateDict = new Dictionary<int, Player.SkillState>();
    }
}
