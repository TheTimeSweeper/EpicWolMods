using System;
using System.Collections.Generic;

namespace Clothes
{
    public static class CustomOutfitModManager
    {
        public static List<CustomOutfitMod> CustomMods = new List<CustomOutfitMod>();
        internal static void EvaluateMod<T>(string id, Player player, bool isEquipping) where T : CustomOutfitMod, new()
        {
            id = $"{id}{player.playerID}";
            CustomOutfitMod customMod = CustomMods.Find(mod => mod.ID == id);

            if (isEquipping)
            {
                if (customMod == null || customMod.player != player)
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

        public static bool PlayerHasMod(string ID, out bool upgraded)
        {
            for (int i = 0; i < GameController.activePlayers.Length; i++)
            {
                bool has = PlayerHasMod(GameController.activePlayers[i], ID, out upgraded);
                if (has) return true;
            }
            upgraded = false;
            return false;
        }

        public static bool PlayerHasMod(Player player, string ID)
        {
            return PlayerHasMod(player, ID, out _);
        }

        public static bool PlayerHasMod(Player player, string ID, out bool upgraded)
        {
            ID = $"{ID}{player.playerID}";
            bool hasMod = CustomMods.Find(outfitMod => outfitMod.player == player && outfitMod.ID == ID) != null;

            if (hasMod)
            {
                upgraded = TailorNpc.playerBuffed[player.playerID];
            }
            else
            {
                upgraded = false;
            }

            return hasMod;
        }
    }
}