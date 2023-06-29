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

            SkillInfo NewSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                DisplayName = "Gust Burst",
                Description = "Dash forward with such force that enemies in the area are pulled into your wake! (Revived through SpellsAPI)",
                EnhancedDescription = "Creates a secondary burst on landing!",
                tier = 1,
                StateType = typeof(AirChannelDashGoodState),
                SkillStats = Utils.LoadFromFileJson<SkillStats>("AirChannelDashGoodSkillStats.json"),
                Element = ElementType.Air,
                Sprite = null
            };
            Skills.Register(NewSkill);
        }
    }
}