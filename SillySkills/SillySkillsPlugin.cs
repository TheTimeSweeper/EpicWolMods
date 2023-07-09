using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using LegendAPI;
using SillySkills.States;

namespace SillySkills {

    [BepInPlugin("TheTimeSweeper.SillySkills", "SillySkills", "0.1.0")]
    public class SillySkillsPlugin : BaseUnityPlugin {

        void Awake()
        {
            Log.Init(Logger);
            AssetLoading.pluginInfo = Info;

            InitSkills();
        }

        private static void InitSkills()
        {
            #region airchanneldashairchanneldashairchanneldashairchanneldashairchanneldash
            SkillInfo AirChannelDashGoodSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                displayName = "Gust Burst",
                description = "Dash forward with such force that enemies in the area are pulled into your wake! (My beloved returns!)",
                enhancedDescription = "Creates a secondary burst on landing!",
                icon = AssetLoading.LoadSprite("AirChannelDashGood"),
                tier = 1,
                stateType = typeof(AirChannelDashGoodState),
                skillStats = AssetLoading.LoadJsonFromFile<SkillStats>("AirChannelDashGoodSkillStats.json")
            };
            #endregion
            Skills.Register(AirChannelDashGoodSkill);

            #region gustburstbutbig
            SkillInfo GustBurstButBigSkill = new SkillInfo()
            {
                ID = "GustBurstButBig",
                displayName = "Gale Burst",
                description = "Create large pulling wind bursts at your position!",
                enhancedDescription = "Create a third, larger wind burst!",
                icon = AssetLoading.LoadSprite("GustBurstButBig"),
                tier = 3,
                stateType = typeof(GustBurstButBigState),
                skillStats = new SkillStats
                {
                    ID = new string[] { "GustBurstButBig" },
                    elementType = new string[] { "Air" },
                    subElementType = new string[] { "Air" },
                    targetNames = new string[] { "EnemyHurtBox", "DestructibleHurtBox" },
                    damage = new int[] { 17 },
                    cooldown = new float[] { 5f },
                    knockbackMultiplier = new float[] { -20f, -17f },
                    hitStunDurationModifier = new float[] { 1.2f },
                    sameAttackImmunityTime = new float[] { 0.25f }
                },
            };
            #endregion gustburstbutbig
            Skills.Register(GustBurstButBigSkill);
        }
    }
}
