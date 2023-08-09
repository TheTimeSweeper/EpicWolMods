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
            Assets.Init(Info);
            if (TestValueManager.testingEnabled)
            {
                gameObject.AddComponent<TestValueManager>();
            }

            InitSkills();
        }

        void Start()
        {
            Assets.LateInit();
        }

        private static void InitSkills()
        {
            #region airchanneldashairchanneldashairchanneldashairchanneldashairchanneldash
            SkillInfo AirChannelDashGoodSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                displayName = "Gust Burst",
                description = "Dash forward with such force that enemies in the area are pulled into your wake!",
                enhancedDescription = "Creates a secondary burst on landing!",
                icon = Assets.LoadSprite("AirChannelDashGood"),
                tier = 1,
                stateType = typeof(AirChannelDashGoodState),
                skillStats = Assets.LoadJsonFromFile<SkillStats>("AirChannelDashGoodSkillStats.json"),
                priceMultiplier = 4
            };
            #endregion
            Skills.Register(AirChannelDashGoodSkill);

            #region gustburstbutbig
            SkillInfo GustBurstButBigSkill = new SkillInfo()
            {
                ID = GustBurstButBigState.staticID,
                displayName = "Gale Burst",
                description = "Create large pulling wind bursts at your position!",
                enhancedDescription = "Create a third, larger wind burst!",
                icon = Assets.LoadSprite("GustBurstButBig"),
                tier = 3,
                stateType = typeof(GustBurstButBigState),
                skillStats = new SkillStats
                {
                    ID = new string[] { GustBurstButBigState.staticID },
                    elementType = new string[] { "Air" },
                    subElementType = new string[] { "Air" },
                    targetNames = new string[] { "EnemyHurtBox", "DestructibleHurtBox" },
                    damage = new int[] { 17 },
                    cooldown = new float[] { 5f },
                    knockbackMultiplier = new float[] { -20f, -17f },
                    hitStunDurationModifier = new float[] { 1.2f },
                    sameAttackImmunityTime = new float[] { 0.25f }
                },  
                priceMultiplier = 6
            };
            #endregion gustburstbutbig
            Skills.Register(GustBurstButBigSkill);


            #region gustburstbutert
            SkillInfo GustBurstButEarthSkill = new SkillInfo()
            {
                ID = GustBurstButEarthState.staticID,
                displayName = "Stone Outburst",
                description = "Create an impassible ring of stones, pushing enemies towards you!",
                enhancedDescription = "Stones are larger and deal more damage!",
                icon = Assets.LoadSprite("GustBurstButEarth"),
                tier = 3,
                stateType = typeof(GustBurstButEarthState),
                skillStats = new SkillStats
                {
                    ID = new string[] { GustBurstButEarthState.staticID },
                    elementType = new string[] { "Earth" },
                    subElementType = new string[] { "Earth" },
                    targetNames = new string[] { "EnemyFloorContact", "DestructibleHurtBox" },
                    damage = new int[] { 40, 50, 0 },
                    cooldown = new float[] { 7f },
                    knockbackMultiplier = new float[] { 0, 0, 1 },
                    knockbackOverwrite = new bool[] { false },
                    hitStunDurationModifier = new float[] { 3.0f, 3.0f, 1.0f },
                    sameAttackImmunityTime = new float[] { 0.0f },
                    sameTargetImmunityTime = new float[] { 0.5f },
                    showDamageNumber = new bool[] {true, true, false}
                },
                priceMultiplier = 6
            };
            #endregion gustburstbutert
            Skills.Register(GustBurstButEarthSkill);
        }
    }
}
