using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyBlob2 {
    [BepInPlugin("com.swepersecreet.sillyblobs", "SillyBlob2", "0.1.0")]
    public class SillyBlob2Plugin : BaseUnityPlugin {

        float sped;

        void Awake() {

            sped = Config.Bind("hello", "blobsped", 69, "sped of blob").Value;

            On.BlobRoller.BlobRollState.FixedUpdate += this.BlobRollState_FixedUpdate;
        }

        private void BlobRollState_FixedUpdate(On.BlobRoller.BlobRollState.orig_FixedUpdate orig, BlobRoller.BlobRollState self) {
            orig(self);
            if (!self.rollStarted || self.targetHit || self.targetTrans == null) {
                return;
            }
            self.parent.movement.MoveToMoveVector(sped, false);
        }
    }
}
