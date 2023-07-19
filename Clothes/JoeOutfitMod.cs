namespace Clothes
{
    public class JoeOutfitMod : CustomOutfitMod
    {
        //private NumVarStatMod animSpeedMod = new NumVarStatMod("JoeAnimSpeed", 0.30f, 10, VarStatModType.Additive, false);

        public override void OnEquip(Player player, bool equipStatus)
        {
            //player.animSpeedHandler.Modify(animSpeedMod, equipStatus);
        }
    }
}
