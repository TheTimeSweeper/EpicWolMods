using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIMod {
    [BepInPlugin("TheTimeSweeper.DIMod", "DIMod", "0.1.0")]
    public class DIModPlugin : BaseUnityPlugin {

        public static bool overrideKnockback;

        void Awake() {

            overrideKnockback = Config.Bind(
                "Hello", 
                "Override", 
                false, 
                "By default (false), joystick direction adds to knockback velocity. " +
                "If true, joystick direction will just override knockback probably.")
                .Value;

            On.Player.PlayerHurtState.OnEnter += PlayerHurtState_OnEnter;
        }

        private void PlayerHurtState_OnEnter(On.Player.PlayerHurtState.orig_OnEnter orig, Player.PlayerHurtState self)
        {
            orig(self);

            //SDI: shift position
            Vector2 ipnutVector = self.parent.inputDevice.GetMoveVector();
            self.parent.transform.position += new Vector3(ipnutVector.x, ipnutVector.y, 0);

            //DI: influence velocity
            self.parent.movement.MoveOnInput(self.parent.inputDevice, !overrideKnockback);
        }
    }
}
