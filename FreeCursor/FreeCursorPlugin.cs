using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FreeCursor {
    [BepInPlugin("TheTimeSweeper.FreeCursor", "FreeCursor", "0.1.0")]
    public class FreeCursorPlugin : BaseUnityPlugin {
        void Awake()
        {
            On.OptionsMenu.SetCursorLock += OptionsMenu_SetCursorLock;
            On.ChaosMouseCursor.Toggle += ChaosMouseCursor_Toggle;
            On.GameUI.TogglePause += GameUI_TogglePause;
            SetConfined();
        }

        private void GameUI_TogglePause(On.GameUI.orig_TogglePause orig)
        {
            orig();
            SetConfined();
        }

        private void ChaosMouseCursor_Toggle(On.ChaosMouseCursor.orig_Toggle orig, ChaosMouseCursor self, bool enable)
        {
            orig(self, enable);
            SetConfined();
        }

        private System.Collections.IEnumerator OptionsMenu_SetCursorLock(On.OptionsMenu.orig_SetCursorLock orig)
        {
            yield return null;
            yield return null;
            SetConfined();
            yield break;
        }

        private static void SetConfined()
        {
            Cursor.lockState = CursorLockMode.None;
            return;
            bool paused = GameUI.pauseMenu ? GameUI.pauseMenu.paused : false;
            if (Application.loadedLevelName == "TitleScreen")
                paused = true;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }
}
