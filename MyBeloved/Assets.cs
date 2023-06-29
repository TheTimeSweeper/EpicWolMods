namespace MyBeloved {
    public class Assets {
        //this is getting out of hand isnt it

        public static AttackInfo AttackInfoAirChannel;
        public static AttackInfo AttackInfoWindBurst;

        public static SkillStats AirChannelDashGoodSkillStats;

        public static void Ass() {
            AttackInfoAirChannel = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfoAirChannel.json");
            AttackInfoWindBurst = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfoWindBurst.json");

            AirChannelDashGoodSkillStats = Utils.LoadFromEmbeddedJson<SkillStats>("AirChannelDashGoodSkillStats.json");
        }
    }


    public class SkillStatsStepOrSomething {
        public string ID;

        public string elementType;

        public string subElementType;

        public bool showDamageNumber;

        public bool showDamageEffect;

        public bool shakeCamera;

        public string targetNames;

        public float sameTargetImmunityTime;

        //etc
    }

    public class SkillStatsInfo {
        public SkillStatsStepOrSomething[] CollectionOfstats;

        public SkillStats Parse() {
            SkillStats skillStats = new SkillStats();
            //fill all skillstat fields with the collections of skillstats steps
            return skillStats;
        }
    }

}
