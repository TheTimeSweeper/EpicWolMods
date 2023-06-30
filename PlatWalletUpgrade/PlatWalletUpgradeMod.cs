using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;


namespace PlatWalletUpgrade {

    [BepInPlugin("TheTimeSweeper.PlatWalletUpgrade", "Gems Gems Gems", "1.0.0")]
    public class PlatWalletUpgradeMod : BaseUnityPlugin {

        private BepInEx.Configuration.ConfigEntry<int> configWalletSize;
        private BepInEx.Configuration.ConfigEntry<int> configItemMax;
        private bool printed;

        void Awake() {

            SetUpConfig();
            On.PlatWallet.ctor += PlatWallet_ctor;
            On.DamageUpWithPlatCount.CalculateStatValue += DamageUpWithPlatCount_CalculateStatValue;
            On.ArmorUpWithPlatCount.CalculateStatValue += ArmorUpWithPlatCount_CalculateStatValue;
        }

        private void ArmorUpWithPlatCount_CalculateStatValue(On.ArmorUpWithPlatCount.orig_CalculateStatValue orig, ArmorUpWithPlatCount self)
        {
            orig(self);
            self.armorMod.modValue = self.armorIncBase + Mathf.Min(configItemMax.Value, (float)Player.platWallet.balance) * self.armorIncPerPlat;
            self.parentEntity.health.armorStat.Modify(self.armorMod, true);
        }

        private void DamageUpWithPlatCount_CalculateStatValue(On.DamageUpWithPlatCount.orig_CalculateStatValue orig, DamageUpWithPlatCount self)
        {
            orig(self);
            self.damageMod.modValue = self.damageIncBase + Mathf.Min(configItemMax.Value, (float)Player.platWallet.balance) * self.damageIncPerPlat;
        }

        private void SetUpConfig() {

            configWalletSize =
                Config.Bind<int>("Config section",
                                 "WalletSize",
                                 696969,
                                 "Max Gems. go nuts c:");

            configItemMax =
                Config.Bind<int>("Config section",
                                 "Item Max",
                                 999,
                                 "Max gems that a 'current chaos gems held' item will consider.");
        }

        private void PlatWallet_ctor(On.PlatWallet.orig_ctor orig, PlatWallet self, int startBalance) {

            orig(self, startBalance);
            self.maxBalance = configWalletSize.Value;

            if (!printed) {
                //only really need to say this the first time
                printed = true;
                Logger.LogMessage($"starting platwallet with a maximum of {configWalletSize.Value}");
            }
        }
    }
}
