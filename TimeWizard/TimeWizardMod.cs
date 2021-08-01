using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System;

namespace TimeWizard {

    [BepInPlugin("com.TheTimeSweeper.TimeWizard", "Time Wizard", "1.0.0")]
    public class TimeWizardMod : BaseUnityPlugin {

        private BepInEx.Configuration.ConfigEntry<KeyCode> cfg_increaseKey;
        private BepInEx.Configuration.ConfigEntry<KeyCode> cfg_decreaseKey;
        private BepInEx.Configuration.ConfigEntry<KeyCode> cfg_setto1Key;
        private BepInEx.Configuration.ConfigEntry<KeyCode> cfg_setto0Key;

        private BepInEx.Configuration.ConfigEntry<bool> cfg_useMouseWheel;

        private BepInEx.Configuration.ConfigEntry<KeyCode> cfg_disableKeys;

        private bool keysDisable;

        void Awake() {
            setConfigs();
        }

        private void setConfigs() {
            string configSection = "hope youre having a lovely day";
            
            cfg_increaseKey =
                Config.Bind(configSection,
                            "Hotkey: Increase Timescale",
                            KeyCode.I,
                            "Key to make time go faster by 0.5f");
            cfg_decreaseKey =
                Config.Bind(configSection,
                            "Hotkey: Decrease Timescale",
                            KeyCode.K,
                            "Key to make time go slower by 0.1f");
            cfg_setto1Key =
                Config.Bind(configSection,
                            "Hotkey: Reset to 1",
                            KeyCode.O,
                            "Key to reset timescale to normal time (1)");
            cfg_setto0Key =
                Config.Bind(configSection,
                            "Hotkey: Set to 0 (pause)",
                            KeyCode.L,
                            "Key to stop time (0)");

            cfg_useMouseWheel =
                Config.Bind(configSection,
                            "use mouse wheel",
                            false,
                            "whee");

            cfg_disableKeys =
                Config.Bind(configSection,
                                     "Hotkey: disable hotkeys",
                                     KeyCode.Semicolon,
                                     "I used the stones to destroy the stones\n\n\n\nit almost\nkilled me");
        }

        void Update() {

            //debug hotkeys
            if (Input.GetKeyDown(cfg_disableKeys.Value)) {
                keysDisable = !keysDisable;
                Logger.LogWarning($"Time Wizard powers toggled {!keysDisable}");
                if (keysDisable && Time.timeScale != 1) {
                    setTimeScale(1);
                }
            }

            if (keysDisable)
                return;

            if (Input.GetKeyDown(KeyCode.I)) {
                if (Time.timeScale < 1f) {
                    setTimeScale(Time.timeScale + 0.1f);
                } else {
                    setTimeScale(Time.timeScale + 0.5f);
                }
            }
            if (Input.GetKeyDown(KeyCode.K)) {

                setTimeScale(Time.timeScale - 0.1f);
            }
            if (Input.GetKeyDown(KeyCode.O)) {
                setTimeScale(1);
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                setTimeScale(0);
            }
        }


        private void setTimeScale(float tim) {
            Time.timeScale = tim;

            //simply using warning because yellow text is easier to see in the window :P
            Logger.LogWarning($"set tim: {Time.timeScale}");
        }
    }
}
