namespace Clothes
{
    public abstract class CustomOutfitMod
    {
        public Player player;
        public string ID;
        public CustomOutfitMod PoorMansConstructor(Player player, string ID)
        {
            this.player = player;
            this.ID = ID;
            return this;
        }

        public abstract void OnEquip(Player player, bool equipStatus);
    }
}