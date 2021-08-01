using System;
using BepInEx;
using UnityEngine;

namespace LongPot {

    [BepInPlugin("com.TheTimeSweeper.LongPot", "Teapot Buff Increase", "1.0.0")]
    public class LongPotMod : BaseUnityPlugin {

        void Awake () {
            On.IncreaseBuffDuration.ctor += IncreaseBuffDuration_ctor; ;
        }
        
        private void IncreaseBuffDuration_ctor(On.IncreaseBuffDuration.orig_ctor orig, IncreaseBuffDuration self) {
            orig(self);
            self.durationMod = new NumVarStatMod(self.ID, 4000.0f, 10, VarStatModType.Multiplicative, false);
            Logger.LogMessage("pot is now long");
        }
    }
}
