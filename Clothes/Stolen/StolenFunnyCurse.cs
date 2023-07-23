namespace Clothes
{
    public static class StolenFunnyCurse
    {
        private static bool isDebugPlayerReal
        {
            get
            {
                for (int i = 0; i < GameController.activePlayers.Length; i++)
                {
                    if (CustomOutfitModManager.PlayerHasMod(GameController.activePlayers[i], "Sweep_Anal"))
                        return true;
                }
                return false;
            }
        }
        public static void Init()
        {
            On.RelicChestUI.LoadPlayerRelics += RelicChestUI_LoadPlayerRelics_MimiHook;
            On.Item.IsUnlocked += Item_IsUnlocked;// += hook_IsUnlocked;
            //On.Item.hook_IsUnlocked hook_IsUnlocked = (On.Item.orig_IsUnlocked orig, string s, bool b) => orig(s, b) || global::LootManager.completeItemDict[s].isCursed;
            On.Player.GiveDesignatedItem += Player_GiveDesignatedItem;// += hook_GiveDesignatedItem;
            //On.Player.hook_GiveDesignatedItem hook_GiveDesignatedItem = delegate (On.Player.orig_GiveDesignatedItem orig, global::Player self, string s)
            //{
            //    bool flag2 = s != null && s != string.Empty && global::LootManager.completeItemDict[s].isCursed;
            //    if (flag2)
            //    {
            //        self.inventory.AddItem(s, false, false);
            //    }
            //    else
            //    {
            //        orig(self, s);
            //    }
            //};

            On.Inventory.RemoveItem += Inventory_RemoveItem;// += hook_RemoveItem;
            //On.Inventory.hook_RemoveItem hook_RemoveItem = delegate (On.Inventory.orig_RemoveItem orig, global::Inventory self, string s, bool b3, bool b4)
            //{
            //    //bool flag2 = !b3 && global::LootManager.completeItemDict[s].isCursed && self.parentEntity is global::Player;
            //    //if (flag2)
            //    {
            //        b3 = true;
            //    }
            //    return orig(self, s, b3, b4);
            //};
        }

        private static void RelicChestUI_LoadPlayerRelics_MimiHook(On.RelicChestUI.orig_LoadPlayerRelics orig, global::RelicChestUI self)
        {
            orig(self);
            if (!isDebugPlayerReal)
                return;
            foreach (string text in global::LootManager.cursedItemIDList)
            {
                global::Item.Category category = global::LootManager.completeItemDict[text].category;
                self.categoryInfoDict[category].idList.Add(text);
                self.categoryInfoDict[category].unlockedCount++;
            }
            foreach (string text2 in global::LootManager.cursedHubOnlyItemIDList)
            {
                global::Item.Category category2 = global::LootManager.completeItemDict[text2].category;
                self.categoryInfoDict[category2].idList.Add(text2);
                self.categoryInfoDict[category2].unlockedCount++;
            }
        }

        private static bool Item_IsUnlocked(On.Item.orig_IsUnlocked orig, string givenID, bool setUnlocked)
        {
            return orig(givenID, setUnlocked) || (LootManager.completeItemDict[givenID].isCursed && isDebugPlayerReal);
        }

        private static void Player_GiveDesignatedItem(On.Player.orig_GiveDesignatedItem orig, Player self, string givenID)
        {
            bool givenIDIsCursed = givenID != null && givenID != string.Empty && global::LootManager.completeItemDict[givenID].isCursed;
            if (givenIDIsCursed && isDebugPlayerReal)
            {
                self.inventory.AddItem(givenID, false, false);
            }
            else
            {
                orig(self, givenID);
            }
        }

        private static bool Inventory_RemoveItem(On.Inventory.orig_RemoveItem orig, Inventory self, string givenItemID, bool forceOverride, bool showNotice)
        {
            //bool flag2 = !b3 && global::LootManager.completeItemDict[s].isCursed && self.parentEntity is global::Player;
            //if (flag2)
            {
                forceOverride |= isDebugPlayerReal;
            }
            return orig(self, givenItemID, forceOverride, showNotice);
        }
    }
}


/*
 * 
				Outfit.baseHope,
				Outfit.basePatience,
				new Outfit("Vigor", 20, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Grit", 17, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Greed", 18, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Platinum, 1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Gold, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Pink", 14, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.HealAmount, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.HealCrit, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pace", 3, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)3, 0f, -0.3f, 0f, false)
				}, false, false),
				new Outfit("Tempo", 5, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.15f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false)
				}, false, false),
				new Outfit("Switch", 4, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)4, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.05f, 0f, 0f, false)
				}, false, false),
				new Outfit("Awe", 6, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.CritChance, 0.1f, 0f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.CritDamage, 0.25f, 0f, 0f, false)
				}, false, false),
				new Outfit("Fury", 8, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, 0.2f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODDamage, 0f, 0.25f, 0f, false)
				}, false, false),
				new Outfit("Rule", 19, new List<OutfitModStat>
				{
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Armor, 0.1f, 0f, 0f, false)
				}, false, false),
				new Outfit(Outfit.lvlStr, 7, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, -0.5f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.ODRate, 0f, -0.5f, 0f, false)
				}, false, true),
				new Outfit("Venture", 2, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, -0.4f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)2, 0f, 0.1f, 0f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
				}, false, false),
				new Outfit("Fall", 9, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 100f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)11, 0f, 0f, 0f, true),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false),
				new Outfit("Pride", 15, new List<OutfitModStat>
				{
					new OutfitModStat((OutfitModStat.OutfitModType)1, 0f, 0f, 1f, false),
					new OutfitModStat((OutfitModStat.OutfitModType)10, 0f, 0f, -1f, false),
					new OutfitModStat(OutfitModStat.OutfitModType.AllowUpgrade, 0f, 0f, 0f, false)
				}, false, false)
 */