using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CameraModExample {
    //for the full guide, visit https://github.com/WoL-Modding-Extravaganza/WoLWiki/wiki
    #region BepInPlugin notes
    // summary:
    // This attribute in [brackets] is setting parameters for the bepin plugin

    // GUID: "TheTimeSweeper.CameraModExample"
    // The the identifier for your mod. Follows the convention of "com.YourUsername.YourModName"

    // Name: "CameraModExample"
    // The the full name of your mod.

    // Version:  "0.1.0"
    // The Version, as you'd expect, useful for differing between updates.
    //     Customary to follow Semantic Versioning (major.minor.patch). 
    //         You don't have to, but you'll just look silly in front of everyone. It's ok. I won't make fun of you.
    #endregion

    [BepInPlugin("TheTimeSweeper.CameraModExample", "CameraModExample", "0.1.0")]
    public class CameraModExamplePlugin : BaseUnityPlugin {
        #region BaseUnityPluginNotes
        // BaseUnityPlugin is the main class that gets loaded by bepin.
        // It inherits from MonoBehaviour, so it gains all the familiar Unity callback functions you can use:
        //     Awake, Start, Update, FixedUpdate, etc.
        //     Awake is usually most important. it's where we initialize our modding
        #endregion

        //declaring a variable we'll use later
        private bool hasZoomedCamera;

        void Awake() {
            // this was the just first little tester code to see if bepin was actually running on WoL
            // it was a rousing success c:
            Debug.Log("I belive in you c:");

            // these are hooks
            // they hook on to the functions in the game. when the hooked functon is called, our functon will be called
            On.CameraController.Awake += CameraController_Awake;
            On.CameraController.Update += CameraController_Update;
        }

        // This Update() function will run every frame
        void Update() {

            // Every frame, we'll check if the user is currently pressing f1 on the keyboard
            if (Input.GetKeyDown(KeyCode.F1)) {
                //if they just pressed f1 this frame, let's zoom the camera out
                CameraController.originalCameraSize += 0.5f;
                Logger.LogMessage("zooming the camera out to " + CameraController.originalCameraSize);
            }

            // same as above, but pressing f2 to zoom in
            if (Input.GetKeyDown(KeyCode.F2)) {
                CameraController.originalCameraSize -= 0.5f;
                Logger.LogMessage("zooming the camera in to " + CameraController.originalCameraSize);
            }
        }

        private void CameraController_Awake(On.CameraController.orig_Awake orig, CameraController self) {

            // this orig() line is very important. this is executing the original function we've hooked to.
            // if you don't have this orig() function, the game's original function will not run 
            orig(self);

            // after the camera initalizes, I'm swoocing right in and increasing these values so the camera will follow players up to a huge distance
            self.maxHorizontalDistBetweenPlayers = 100;
            self.maxVerticalDistBetweenPlayers = 80;
            self.teleportToOtherPlayerRange = 120;

            // zooming out the camera a little bit by default cause I want that
            if (!hasZoomedCamera) {
                //but only need to do it once
                hasZoomedCamera = true;

                CameraController.originalCameraSize *= 1.2f; //change this value to whatever you like!
            }
        }

        private void CameraController_Update(On.CameraController.orig_Update orig, CameraController self) {

            orig(self);

            if (self.overrideCameraUpdate)
                return;
            if (GameController.playerScripts[0] == null || GameController.playerScripts[1] == null)
                return;
            if (GameController.coopOn && GameController.PlayerIsDead())
                return;

            //copied from CameraController.Update in dnSpy:

            //if (GameController.pvpOn || SceneManager.GetActiveScene().name.Contains("PvP"))
            self.distanceBetweenPlayers = self.playerDiff.magnitude * 0.42f;
            Camera.main.orthographicSize = (self.distanceBetweenPlayers <= CameraController.originalCameraSize) ? CameraController.originalCameraSize : self.distanceBetweenPlayers;

            // this the bit of code ordinarly runs in the 'if' statement above (checks are we pvp right now).
            // so for this hook i'm simply doing it again, but without the check, so it always does it.

            // and we're done! build this mod, put it in Plugins, and run the game!

            #region end notes
            // optional note: about the above code, ordinarily, simply copypasting code from the game is kinda bad practice, but that's a subject in itself
            //     copypasting their code and tweaking things is kind of a cop-out, also a lot of the time ain't even gonna cut it
            //         what you want is to really understand what's happening in their code, then figure out what you want to do by adding your additional code.
            //         it's a pretty interesting unique problem to solve. gotta think outside the box almost literally

            // that said, for simple things like this, and until you really wrap your head around all this, it's fine. it'll come with experience
            // tl;dr I believ in you c:
            #endregion
        }
    }
}
