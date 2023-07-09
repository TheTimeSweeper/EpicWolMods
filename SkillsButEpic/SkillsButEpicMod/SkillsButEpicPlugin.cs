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

            TestSkillMod.Awake(this, Info);
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

            On.Player.InitFSM += Player_InitFSM_AddSkillStates;
        }

        private static void LootManager_ResetAvailableSkills(On.LootManager.orig_ResetAvailableSkills orig)
        {
            if (badentrypointsignal.Contains("Loot")){
                orig();
                return; 
            }

            foreach (SkillInfo skillInfo in SkillsDict.Values)
            {
                if (!LootManager.completeSkillList.Contains(skillInfo.ID))
                {
                    LootManager.completeSkillList.Add(skillInfo.ID);
                }
                //add new tiers if this skill is higher than max
                if (skillInfo.tier >= LootManager.maxSkillTier)
                {
                    for (int i = LootManager.maxSkillTier; i <= skillInfo.tier; i++)
                    {
                        LootManager.skillTierDict.Add(i, new List<string>());
                    }
                    LootManager.maxSkillTier = skillInfo.tier + 1;
                }
                //if this skill already exists, remove it from its existing tier
                foreach (List<string> list in LootManager.skillTierDict.Values)
                {
                    if (list.Contains(skillInfo.ID))
                    {
                        list.Remove(skillInfo.ID);
                    }
                }
                //add this skill to tier
                LootManager.skillTierDict[skillInfo.tier].Add(skillInfo.ID);

                //Putting the skill text and icon stuff here since idk where else it should go
                //Null checks to make sure errors don't occur
                if (IconManager.skillIcons == null)
                {
                    IconManager.skillIcons = IconManager.SkillIcons;
                }
                if (TextManager.skillInfoDict == null)
                {
                    TextManager.skillInfoDict = new Dictionary<string, TextManager.SkillInfo>();
                }

                if (skillInfo.Sprite != null)
                {
                    IconManager.skillIcons[skillInfo.ID] = skillInfo.Sprite;
                }

                TextManager.SkillInfo skillText = new TextManager.SkillInfo();
                skillText.skillID = skillInfo.ID;
                skillText.displayName = skillInfo.DisplayName;
                skillText.description = skillInfo.Description;
                skillText.empowered = skillInfo.EnhancedDescription;
                TextManager.skillInfoDict[skillInfo.ID] = skillText;
            }

            badentrypointsignal.Add("Loot");
            orig();
        }


        private static void StatManager_LoadData_AddSkillsToLoadedData(On.StatManager.orig_LoadData orig, string assetPath, string statID, string category, string categoryModifier)
        {
            orig(assetPath, statID, category, categoryModifier);

            foreach (SkillInfo skillInfo in SkillsDict.Values)
            {
                //generate StatData from our skills' SkillStats, which values for damage, knockback, etc
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

                //add our StatData to the StatManager
                Dictionary<string, StatData> dictionary = StatManager.data[statID][fullCategory];
                dictionary[statData.GetValue<string>("ID", -1)] = statData;
                StatManager.globalSkillData[statData.GetValue<string>("ID", -1)] = statData;
            }
        }

        private static void Player_InitFSM_AddSkillStates(On.Player.orig_InitFSM orig, Player self)
        {
            orig(self);

            //init our skills and add them to the player state machine
            foreach (SkillInfo info in SkillsDict.Values)
            {
                Player.SkillState state = (Player.SkillState)Activator.CreateInstance(info.StateType, self.fsm, self);
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
        public Sprite Sprite = null;
        public int tier = 0;
        public Type StateType;
        public SkillStats SkillStats;
    }

    //helper class to construct SkillStats through more streamlined/intuitive code
    public class SkillStatsInfo
    {
        public string ID;
        public ElementType elementType;
        public ElementType subElementType;
        public string[] targetNames;
        //bread and butter of the attack data
        //streamlined to be multiple fields as the game treats them as different stages of the attack
        public SkillStatsLevelInfo[] attackInfos;

        public SkillStatsInfo() { }
        public SkillStatsInfo(string ID, ElementType elementType, ElementType subElementType, string[] targetNames, params SkillStatsLevelInfo[] attackInfos)
        {
            this.ID = ID;
            this.elementType = elementType;
            this.subElementType = subElementType;
            this.targetNames = targetNames;
            this.attackInfos = attackInfos;
        }

        public SkillStats GetSkillStats()
        {
            SkillStats skillStats = new SkillStats();

            skillStats.ID = new string[] { ID };

            skillStats.elementType = new string[] { elementType.ToString() };
            skillStats.subElementType = new string[] { subElementType.ToString() };

            //only one that the game doesn't read each entry by skill level (as far as I'm aware)
            skillStats.targetNames = targetNames;

            //wew lad
            skillStats.showDamageNumber = new bool[attackInfos.Length];
            skillStats.showDamageEffect = new bool[attackInfos.Length];
            skillStats.shakeCamera = new bool[attackInfos.Length];
            skillStats.sameTargetImmunityTime = new float[attackInfos.Length];
            skillStats.sameAttackImmunityTime = new float[attackInfos.Length];
            skillStats.canHitStun = new bool[attackInfos.Length];
            skillStats.hitStunDurationModifier = new float[attackInfos.Length];
            skillStats.knockbackMultiplier = new float[attackInfos.Length];
            skillStats.knockbackOverwrite = new bool[attackInfos.Length];
            skillStats.damage = new int[attackInfos.Length];
            skillStats.cooldown = new float[attackInfos.Length];
            skillStats.duration = new float[attackInfos.Length];
            skillStats.hitCount = new int[attackInfos.Length];
            skillStats.damageInterval = new float[attackInfos.Length];
            skillStats.spawnCount = new int[attackInfos.Length];
            skillStats.baseHealth = new int[attackInfos.Length];
            skillStats.criticalHitChance = new float[attackInfos.Length];
            skillStats.criticalDamageModifier = new float[attackInfos.Length];
            skillStats.isStatusEffect = new bool[attackInfos.Length];
            skillStats.rootChance = new float[attackInfos.Length];
            skillStats.rootDuration = new float[attackInfos.Length];
            skillStats.chaosChance = new float[attackInfos.Length];
            skillStats.chaosLevel = new int[attackInfos.Length];
            skillStats.burnChance = new float[attackInfos.Length];
            skillStats.burnLevel = new int[attackInfos.Length];
            skillStats.slowChance = new float[attackInfos.Length];
            skillStats.slowLevel = new int[attackInfos.Length];
            skillStats.poisonChance = new float[attackInfos.Length];
            skillStats.poisonLevel = new int[attackInfos.Length];
            skillStats.shockChance = new float[attackInfos.Length];
            skillStats.shockLevel = new int[attackInfos.Length];
            skillStats.freezeChance = new float[attackInfos.Length];
            skillStats.freezeDuration = new float[attackInfos.Length];
            skillStats.overdriveDamageMultiplier = new float[attackInfos.Length];
            skillStats.overdriveProgressMultiplier = new float[attackInfos.Length];
            skillStats.overdriveSingleIncrease = new bool[attackInfos.Length];

            for (int i = 0; i < attackInfos.Length; i++)
            {
                SkillStatsLevelInfo attackInfo = attackInfos[i];
                //wew lad 2
                skillStats.showDamageNumber[i] = attackInfo.showDamageNumber;
                skillStats.showDamageEffect[i] = attackInfo.showDamageEffect;
                skillStats.shakeCamera[i] = attackInfo.shakeCamera;
                skillStats.sameTargetImmunityTime[i] = attackInfo.sameTargetImmunityTime;
                skillStats.sameAttackImmunityTime[i] = attackInfo.sameAttackImmunityTime;
                skillStats.canHitStun[i] = attackInfo.canHitStun;
                skillStats.hitStunDurationModifier[i] = attackInfo.hitStunDurationModifier;
                skillStats.knockbackMultiplier[i] = attackInfo.knockbackMultiplier;
                skillStats.knockbackOverwrite[i] = attackInfo.knockbackOverwrite;
                skillStats.damage[i] = attackInfo.damage;
                skillStats.cooldown[i] = attackInfo.cooldown;
                skillStats.duration[i] = attackInfo.duration;
                skillStats.hitCount[i] = attackInfo.hitCount;
                skillStats.damageInterval[i] = attackInfo.damageInterval;
                skillStats.spawnCount[i] = attackInfo.spawnCount;
                skillStats.baseHealth[i] = attackInfo.baseHealth;
                skillStats.criticalHitChance[i] = attackInfo.criticalHitChance;
                skillStats.criticalDamageModifier[i] = attackInfo.criticalDamageModifier;
                skillStats.isStatusEffect[i] = attackInfo.isStatusEffect;
                skillStats.rootChance[i] = attackInfo.rootChance;
                skillStats.rootDuration[i] = attackInfo.rootDuration;
                skillStats.chaosChance[i] = attackInfo.chaosChance;
                skillStats.chaosLevel[i] = attackInfo.chaosLevel;
                skillStats.burnChance[i] = attackInfo.burnChance;
                skillStats.burnLevel[i] = attackInfo.burnLevel;
                skillStats.slowChance[i] = attackInfo.slowChance;
                skillStats.slowLevel[i] = attackInfo.slowLevel;
                skillStats.poisonChance[i] = attackInfo.poisonChance;
                skillStats.poisonLevel[i] = attackInfo.poisonLevel;
                skillStats.shockChance[i] = attackInfo.shockChance;
                skillStats.shockLevel[i] = attackInfo.shockLevel;
                skillStats.freezeChance[i] = attackInfo.freezeChance;
                skillStats.freezeDuration[i] = attackInfo.freezeDuration;
                skillStats.overdriveDamageMultiplier[i] = attackInfo.overdriveDamageMultiplier;
                skillStats.overdriveProgressMultiplier[i] = attackInfo.overdriveProgressMultiplier;
                skillStats.overdriveSingleIncrease[i] = attackInfo.overdriveSingleIncrease;
            }
            return skillStats;
        }
    }

    public class SkillStatsLevelInfo
    {
        //bread and butter of this skill's level's attack
        public int damage;
        public float cooldown;

        public bool knockbackOverwrite = false;
        public float knockbackMultiplier = 0.0f;

        public bool canHitStun = true;
        public float hitStunDurationModifier = 1.0f;

        public float sameTargetImmunityTime = 0.0f;
        public float sameAttackImmunityTime = 0.0f;

        //used by items to increase your crit values. usually not changed (I think. I need to print all the game's skills' skillstats);
        public float criticalHitChance = 0.05f;
        public float criticalDamageModifier = 1.5f;

        //used by items to increase your signature damage
        public float overdriveDamageMultiplier = 1;
        public float overdriveProgressMultiplier = 1;
        public bool overdriveSingleIncrease = true;

        //situational based on your spell
        public int spawnCount = 0;
        public int baseHealth = 0;
        public float duration = 0.0f;
        public int hitCount = 0;
        public float damageInterval = 0.0f;

        //situational based on your spell
        public bool isStatusEffect = false;
        public float rootChance = 0.0f;
        public float rootDuration = 0.0f;
        public float chaosChance = 0.0f;
        public int chaosLevel = 0;
        public float burnChance = 0.0f;
        public int burnLevel = 0;
        public float slowChance = 0.0f;
        public int slowLevel = 0;
        public float shockChance = 0.0f;
        public int shockLevel = 0;
        public float freezeChance = 0.0f;
        public float freezeDuration = 0.0f;
        public float poisonChance = 0.0f;
        public int poisonLevel = 0;

        //misc
        public bool showDamageNumber = true;
        public bool showDamageEffect = true;
        public bool shakeCamera = true;
    }
}
