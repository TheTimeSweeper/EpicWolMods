using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;
using InputControlType = InControl.InputControlType;

namespace ButtonMutiny {
    [BepInPlugin("com.TheTimeSweeper.ButtonMutiny", "ButtonMutiny", "1.0.0")]
    public class ButtonMutinyMod : BaseUnityPlugin {

        void Awake() {

            On.ChaosInputDevice.InitButtonMapping += ChaosInputDevice_InitButtonMapping;
            On.GameUI.LoadInputButtonSprites += GameUI_LoadInputButtonSprites;

        }

        private void GameUI_LoadInputButtonSprites(On.GameUI.orig_LoadInputButtonSprites orig) {
            orig();

            GameUI.InputButtonSprites["Slot4Switch"] = GameUI.UIButtonIcons["SwitchZL"];
            GameUI.InputButtonSprites["Slot5Switch"] = GameUI.UIButtonIcons["SwitchZR"];

            GameUI.InputButtonSprites["ConfirmSwitch"] = GameUI.UIButtonIcons["SwitchA"];
            GameUI.InputButtonSprites["CancelSwitch"] = GameUI.UIButtonIcons["SwitchB"];

            GameUI.InputButtonSprites["MapSwitch"] = GameUI.UIButtonIcons["SwitchR"];
            GameUI.InputButtonSprites["EquipMenuSwitch"] = GameUI.UIButtonIcons["SwitchL"];
        }

        private void ChaosInputDevice_InitButtonMapping(On.ChaosInputDevice.orig_InitButtonMapping orig, ChaosInputDevice self) {
            orig(self);

            if (self.inputScheme == ChaosInputDevice.InputScheme.Gamepad) {
                self.buttonMapping["Skill0"] = new List<InputControlType> { InputControlType.Action4 };
                self.buttonMapping["Skill1"] = new List<InputControlType> { InputControlType.Action2 };

                self.buttonMapping["Skill2"] = new List<InputControlType> { InputControlType.Action3 };
                self.buttonMapping["Skill3"] = new List<InputControlType> { InputControlType.Action1 };

                self.buttonMapping["Skill4"] = new List<InputControlType> { InputControlType.LeftTrigger };
                self.buttonMapping["Skill5"] = new List<InputControlType> { InputControlType.RightTrigger };

                self.buttonMapping["EquipMenu"] = new List<InputControlType> { InputControlType.LeftBumper };
                self.buttonMapping["Map"] = new List<InputControlType> { InputControlType.RightBumper, InputControlType.Select };

                self.buttonMapping["Overdrive"] = new List<InputControlType> { InputControlType.RightStickButton };

                // on switch y button interact
                //TODO: detect if it's a switch gamepad (though idk if it'll work for joycons)

                self.buttonMapping["Interact"] = new List<InputControlType> { InputControlType.Action4 };
            }
        }
    }
}
