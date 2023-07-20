using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShutUpTomi {
    [BepInPlugin("TheTimeSweeper.ShutUpTomi", "ShutUpTomi", "0.1.0")]
    public class ShutUpTomiPlugin : BaseUnityPlugin {

        bool allScenes = true;

        void Awake() {
            
            allScenes = Config.Bind("hello", "all Scenes", false, "set true to skip all scenes. set false to only skip long pvp lobby dialog, keeping fun dialog in other scenes").Value;

            On.SpellBookNpc.Interacted += SpellBookNpc_Interacted;
        }

        private void SpellBookNpc_Interacted(On.SpellBookNpc.orig_Interacted orig, SpellBookNpc self)
        {
            if (Globals.IsPvPScene || allScenes)
            {
                self.dialogCompleted = true;
            }
            orig(self);
        }
    }
}
