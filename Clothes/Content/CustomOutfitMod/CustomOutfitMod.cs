using System;

namespace Clothes
{
    public class CustomOutfitMod
    {
        public Player player;
        public string ID;
        public OutfitModStat outfitModStat;
        public CustomOutfitMod PoorMansConstructor(Player player, string ID, OutfitModStat outfitModStat)
        {
            this.player = player;
            this.ID = ID;
            this.outfitModStat = outfitModStat;
            return this;
        }

        public virtual void OnEquip(Player player, bool equipStatus) { }

        public virtual void Update() { }

        public virtual void OnPlayerDestroyed() { }
    }
}