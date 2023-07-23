namespace Clothes
{
    public class CustomOutfitMod
    {
        public Player player;
        public string ID;
        public CustomOutfitMod PoorMansConstructor(Player player, string ID)
        {
            this.player = player;
            this.ID = ID;
            return this;
        }

        public virtual void OnEquip(Player player, bool equipStatus) { }
    }
}