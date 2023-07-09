using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillsButEpic
{
    public class TestSkillMod
    {
        public static PluginInfo pluginInfo;

        public static void Awake(SkillsButEpicPlugin skillsButEpicPlugin, PluginInfo Info)
        {
            skillsButEpicPlugin.gameObject.AddComponent<TestValueManager>();

            pluginInfo = Info;
            //loading straight from a json. not recommended at all to do manually,
            //but if you're pulling an existing data from the game to json here ya go
            #region airchanneldashairchanneldashairchanneldashairchanneldashairchanneldash
            SkillInfo AirChannelDashGoodSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                DisplayName = "Gust Burst",
                Description = "Dash forward with such force that enemies in the area are pulled into your wake! (My beloved returns!)",
                EnhancedDescription = "Creates a secondary burst on landing!",
                Sprite = Utils.LoadSprite("AirChannelDashGood"),
                tier = 1,
                StateType = typeof(AirChannelDashGoodState),
                SkillStats = Utils.LoadJsonFromFile<SkillStats>("AirChannelDashGoodSkillStats.json")
            };
            #endregion
            Skills.Register(AirChannelDashGoodSkill);

            //somewhat unintuitive collection way of doing skillstats,
                //but it's still technically correct, and quite compact if you know what you're doing
            //ok you don't need to duplicate fields as I thought
                //this is the clear winner
            #region gustburstbutbig
            SkillInfo GustBurstButBigSkill = new SkillInfo()
            {
                ID = "GustBurstButBig",
                DisplayName = "Gust Burst but Big",
                Description = "Create large pulling wind bursts at your position!",
                EnhancedDescription = "Create a third, larger wind burst!",
                Sprite = Utils.LoadSprite("GustBurstButBig"),
                tier = 3,
                StateType = typeof(GustBurstButBigState),
                SkillStats = new SkillStats
                {
                    ID = new string[] { "GustBurstButBig" },
                    elementType = new string[] { "Air" },
                    subElementType = new string[] { "Air" },
                    targetNames = new string[] { "EnemyHurtBox", "DestructibleHurtBox" },
                    damage = new int[] { 17, 17 },
                    cooldown = new float[] { 5f },
                    knockbackMultiplier = new float[] { -20f, -20f },
                    hitStunDurationModifier = new float[] { 1.2f, 1.2f },
                    sameAttackImmunityTime = new float[] { 0.25f }
                },
            };
            #endregion gustburstbutbig
            Skills.Register(GustBurstButBigSkill);

            //more streamlined way of doing skillstats
            //much more verbose, but more intuitive to the concept of multiple attacks at different levels
            #region gustburstbutbig api-ified
            SkillInfo GustBurstButBigSkill2 = new SkillInfo()
            {
                ID = "GustBurstButBig",
                DisplayName = "Gust Burst but Big 2",
                Description = "Create large pulling wind bursts at your position!",
                EnhancedDescription = "Create a third, larger wind burst!",
                Sprite = null,
                tier = 3,
                StateType = typeof(GustBurstButBigState),
                SkillStats = new SkillStatsInfo()
                {
                    ID = "GustBurstButBig",
                    elementType = ElementType.Air,
                    subElementType = ElementType.Air,
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    attackInfos = new SkillStatsLevelInfo[]
                    {
                        new SkillStatsLevelInfo
                        {
                            damage = 20,
                            cooldown = 5f,
                            knockbackMultiplier = -20f,
                            hitStunDurationModifier = 1.2f,
                            sameAttackImmunityTime = 0.25f,
                        },
                        new SkillStatsLevelInfo
                        {
                            damage = 10,
                            cooldown = 5f,
                            knockbackMultiplier = 10f,
                            hitStunDurationModifier = 1.2f,
                            sameAttackImmunityTime = 0.25f,
                        }
                    }
                }.GetSkillStats(),
            };
            #endregion gustburstbutbig api-ified
            //don't register the same skillID multiple times
            //Skills.Register(GustBurstButBigSkill2);

            //above but a few lines saved with a constructor and params
            #region gustburstbutbig api-ified 2
            SkillInfo GustBurstButBigSkill3 = new SkillInfo()
            {
                ID = "GustBurstButBig",
                DisplayName = "Gust Burst but Big 3",
                Description = "Create large pulling wind bursts at your position!",
                EnhancedDescription = "Create a third, larger wind burst!",
                Sprite = null,
                tier = 3,
                StateType = typeof(GustBurstButBigState),
                SkillStats = new SkillStatsInfo(
                    ID: "GustBurstButBig",
                    elementType: ElementType.Air,
                    subElementType: ElementType.Air,
                    targetNames: new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    new SkillStatsLevelInfo
                    {
                        damage = 20,
                        cooldown = 5f,
                        knockbackMultiplier = -20f,
                        hitStunDurationModifier = 1.2f,
                        sameAttackImmunityTime = 0.25f,
                    },
                    new SkillStatsLevelInfo
                    {
                        damage = 10,
                        cooldown = 5f,
                        knockbackMultiplier = 10f,
                        hitStunDurationModifier = 1.2f,
                        sameAttackImmunityTime = 0.25f,
                    }
                ).GetSkillStats(),
            };
            #endregion gustburstbutbig api-ified 2
            //don't register the same skillID multiple times
            //Skills.Register(GustBurstButBigSkill3);
        }
    }
}