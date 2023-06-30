using BepInEx;
using System;
using System.Collections.Generic;

namespace SkillsButEpic
{
    public class ExampleSkillMod
    {
        public static PluginInfo pluginInfo;

        public static void Awake(PluginInfo info)
        {
            pluginInfo = info;

            SkillInfo AirChannelDashGoodSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                DisplayName = "Gust Burst",
                Description = "Dash forward with such force that enemies in the area are pulled into your wake! (Revived through SpellsAPI)",
                EnhancedDescription = "Creates a secondary burst on landing!",
                tier = 1,
                StateType = typeof(AirChannelDashGoodState),
                SkillStats = Utils.LoadFromFileJson<SkillStats>("AirChannelDashGoodSkillStats.json"),
                //no longer used?
                Element = ElementType.Air,
                Sprite = null
            };
            Skills.Register(AirChannelDashGoodSkill);

            SkillInfo GustBurstButBigSkill = new SkillInfo()
            {
                ID = "GustBurstButBig",
                DisplayName = "Gust Burst but Big",
                Description = "Create large pulling wind bursts at your position!",
                EnhancedDescription = "Create a third, larger wind burst!",
                tier = 1,
                StateType = typeof(GustBurstButBigState),
                SkillStats = GustBurstbutBigSKillStats(),
                Element = ElementType.Air,
                Sprite = null
            };
            Skills.Register(GustBurstButBigSkill);
        }

        private static SkillStats GustBurstbutBigSKillStats()
        {
            return new SkillStats
            {
                ID = new string[1] { "GustBurstButBig" },
                damage = new int[1] { 20 },
                targetNames = new string[1] { "EnemyHurtBox" },
                elementType = new string[1] { "Air" },
                subElementType = new string[1] { "Air" },
                knockbackMultiplier = new float[1] { -30f },
                cooldown = new float[1] { 4f },
            };
        }
    }
}