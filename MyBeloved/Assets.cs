namespace MyBeloved {
    public class Assets {
        //this is getting out of hand isnt it

        public static AttackInfo AttackInfoChannel;
        public static AttackInfo AttackInfoWindBurst;

        public static void Ass() {
            AttackInfoChannel = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            AttackInfoWindBurst = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo2.json");
        }
    }
}
