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
}
