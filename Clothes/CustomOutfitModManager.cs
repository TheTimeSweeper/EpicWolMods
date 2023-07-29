using System;
using System.Collections.Generic;

namespace Clothes
{
    public static class CustomOutfitModManager
    {
        public static List<CustomOutfitMod> CustomMods = new List<CustomOutfitMod>();

        public static void UpdateMods()
        {
            for (int i = CustomMods.Count - 1; i >= 0; i--)
            {
                if (CustomMods[i].player == null)
                {
                    CustomMods[i].OnPlayerDestroyed();
                    CustomMods.RemoveAt(i);
                    continue;
                }
                CustomMods[i].Update();
            }
        }

        public static void EvaluateMod(string id, Player player, bool isEquipping) => EvaluateMod<CustomOutfitMod>(id, player, isEquipping);
        public static void EvaluateMod<T>(string id, Player player, bool isEquipping) where T : CustomOutfitMod, new()
        {
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
                if (has) 
                    return true;
            }
            upgraded = false;
            return false;
        }

        public static bool PlayerHasMod(Player player, string ID) => PlayerHasMod(player, ID, out _);
        public static bool PlayerHasMod(Player player, string ID, out bool upgraded)
        {
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