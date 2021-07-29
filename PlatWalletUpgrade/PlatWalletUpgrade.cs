using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;


namespace PlatWalletUpgrade {

    [BepInPlugin("com.TheTimeSweeper.AlwaysPvpCameraMod", "Always PvP Camera", "1.0.1")]
    public class PlatWalletUpgrade : BaseUnityPlugin {

        BepInEx.Configuration.ConfigEntry<int> configWalletSize;

        void Awake() {

            SetUpConfig();

            On.PlatWallet.ctor += PlatWallet_ctor;
        }

        private void SetUpConfig() {
            configWalletSize =
                Config.Bind<int>("Config section",
                                 "WalletSize",
                                 999999,
                                 "Max Gems. go nuts c:");
        }

        private void PlatWallet_ctor(On.PlatWallet.orig_ctor orig, PlatWallet self, int startBalance) {

            orig(self, startBalance);
            self.maxBalance = configWalletSize.Value;
        }
    }
}
