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
        private bool printed;

        void Awake() {

            SetUpConfig();
            On.PlatWallet.ctor += PlatWallet_ctor;
        }

        private void SetUpConfig() {

            configWalletSize =
                Config.Bind<int>("Config section",
                                 "WalletSize",
                                 696969,
                                 "Max Gems. go nuts c:");
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
