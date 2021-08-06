using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WolModButEpic {

    [BepInPlugin("TheTimeSweeper.AlwaysPvpCameraMod", "Always PvP Camera", "1.0.5")]
    public class CoolEpicModThatDoesThings : BaseUnityPlugin {

        private float camSizeMultiplierCoop = 1.2f;
        private float camSizeMultiplierSolo = 1.1f;
        private float playerCameraBuffer = 0.25f;
        private float maxPlayerDistance = 120;

        private float playerDistCheckMult { get => 1 + (playerCameraBuffer * 2); }
        private static float screenRatio { get { return (float)Screen.width / (float)Screen.height; } }

        private bool _zoomin;
        private float _originalOriginalCameraSize;

        void Awake() { 

            Logger.LogMessage("I belive in you c:");

            Configuratinator();

            On.CameraController.Awake += CameraController_Awake;
            On.CameraController.Update += CameraController_Update;
        }

        private void Configuratinator() {

            string configSection = "youre cool dont forget that";

            camSizeMultiplierCoop =
                Config.Bind(configSection,
                            "Camera Size Multiplier Coop",
                            1.2f,
                            "Zoom out camera for multiplayer")
                .Value;

            camSizeMultiplierSolo =
                Config.Bind(configSection,
                            "Camera Size Multiplier Solo",
                            1.2f,
                            "Zoom out camera for solo")
                .Value;

            playerCameraBuffer =
                Config.Bind(configSection,
                            "Camera Border Buffer",
                            0.25f,
                            "The buffer zone between players and the camera border (so players stay on screen)\nbuffer of 0.1 keeps players right at the border\nbuffer of 0.25 gives a little bit of room to see past the players")
                .Value;

            maxPlayerDistance =
                Config.Bind(configSection,
                            "Max Player Distance",
                            120,
                            "Max distance Coop players are allowed to run from each other.")
                .Value;
        }

        private void CameraController_Awake(On.CameraController.orig_Awake orig, CameraController self) {

            orig(self);

            //should a gameplay mechanic (albeit non=canon) be affected by screen ratio?
            self.maxVerticalDistBetweenPlayers = maxPlayerDistance;
            self.maxHorizontalDistBetweenPlayers = maxPlayerDistance;

            self.teleportToOtherPlayerRange = Mathf.Max(self.maxVerticalDistBetweenPlayers, self.maxHorizontalDistBetweenPlayers) * 1.2f;

            if (!_zoomin) {
                _zoomin = true;

                _originalOriginalCameraSize = CameraController.originalCameraSize;
            }

            CameraController.originalCameraSize = _originalOriginalCameraSize * (GameController.coopOn ? camSizeMultiplierCoop : camSizeMultiplierSolo);
        }

        private void CameraController_Update(On.CameraController.orig_Update orig, CameraController self) {

            orig(self);

            if (self.overrideCameraUpdate)
                return;

            if (GameController.playerScripts[0] == null || GameController.playerScripts[1] == null)
                return;

            if (GameController.coopOn && GameController.PlayerIsDead())
                return;

            // copied from CameraController.Update in dnSpy:
            // if (GameController.pvpOn || SceneManager.GetActiveScene().name.Contains("PvP"))

            float playY = Mathf.Abs(self.playerDiff.y) * 0.42f;
            float playX = Mathf.Abs(self.playerDiff.x) / screenRatio * 0.42f;

            float playerDistance = Mathf.Max(playY * playerDistCheckMult, playX * playerDistCheckMult);

            Camera.main.orthographicSize = playerDistance > CameraController.originalCameraSize ? playerDistance : CameraController.originalCameraSize;
            
        }

        void Update() {

            //if (Input.GetKeyDown(KeyCode.U)) {
            //    camSizeMultiplierCoop += 0.05f;
            //    CameraController.originalCameraSize = _originalOriginalCameraSize * camSizeMultiplierCoop;
            //}
            //if (Input.GetKeyDown(KeyCode.J)) {
            //    camSizeMultiplierCoop -= 0.05f;
            //    CameraController.originalCameraSize = _originalOriginalCameraSize * camSizeMultiplierCoop;
            //}

            //if (Input.GetKeyDown(KeyCode.I)) {

            //    playerCameraBuffer += 0.02f;
            //    Logger.LogWarning($"cam buffer {playerCameraBuffer.ToString("0.00")} | dist multi {playerDistCheckMult.ToString("0.00")} ");
            //}
            //if (Input.GetKeyDown(KeyCode.K)) {

            //    playerCameraBuffer -= 0.02f;
            //    Logger.LogWarning($"cam buffer {playerCameraBuffer.ToString("0.00")} | dist multi {playerDistCheckMult.ToString("0.00")} ");
            //}
        }
    }
}
