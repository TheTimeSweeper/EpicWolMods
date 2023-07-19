using System;
using System.Collections.Generic;

namespace Clothes
{
    public static class CustomOutfitModManager
    {
        public static List<CustomOutfitMod> CustomMods = new List<CustomOutfitMod>();
        internal static void EvaluateMod<T>(string id, Player player, bool isEquipping) where T : CustomOutfitMod, new()
        {
            CustomOutfitMod customMod = CustomMods.Find(mod => mod.ID == id);

            if (isEquipping)
            {
                if (customMod == null)
                {
                    customMod = new T().PoorMansConstructor(player, id);
                    CustomMods.Add(customMod);
                }
            }

            customMod.OnEquip(player, isEquipping);

            if (!isEquipping)
            {
                if(customMod != null && CustomMods.Contains(customMod))
                {
                    CustomMods.Remove(customMod);
                }
            }
        

        }
        public static bool PlayerHasMod(Player player, string ID)
        {
            return CustomMods.Find(outfitMod => outfitMod.player == player && outfitMod.ID == ID) != null;
        }
    }
}