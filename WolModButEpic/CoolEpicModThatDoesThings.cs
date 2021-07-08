using BepInEx;
using UnityEngine;

namespace WolModButEpic {


    /// <summary>
    /// This attribute in [brackets] is (as can be seen) parameters for the beipn plugin
    /// </summary>
    /// <param name="GUID">
    /// The GUID is the identifier for your mod. Follows the convention of "com.YourUsername.YourModName"
    /// </param>
    /// <param name="Name">
    /// The Name is the full name of your mod.
    /// </param>
    /// <param name="Version">
    /// The Version, as usually, is useful for differing between updates.
    /// If you have two pulgins of the same GUID, bepin will automatically load the newest one and not load older ones.
    /// Custmoary to follow Semantic Versioning (major.minor.patch). You don't have to, but you'll just look silly in front of everyone. It's ok. I won't make fun of you.
    /// </param>
    [BepInPlugin("com.TheTimeSweeper.AlwaysPvpCameraMod", "Always PvP Camera", "1.0.1")]
    public class CoolEpicModThatDoesThings : BaseUnityPlugin {

        private bool zoomin;

        void Awake() { 
            //this was the just first little tester code to see if bepin was actually running on WoL
            //  it was a rousing success c:
            Debug.Log("I belive in you c:");

            //these are hooking on to the functions in the game. when the method is called, our functon will be called
            On.CameraController.Awake += CameraController_Awake;
            On.CameraController.Update += CameraController_Update;
        }

        private void CameraController_Awake(On.CameraController.orig_Awake orig, CameraController self) {
            //this orig() line is very important. this is executing the original function we've hooked to.
            //if you don't have this orig() function, the game's original function will not run 
            orig(self);

            self.maxHorizontalDistBetweenPlayers = 100;
            self.maxVerticalDistBetweenPlayers = 100;
            self.teleportToOtherPlayerRange = 120;

            if (!zoomin) {
                zoomin = true;
                CameraController.originalCameraSize *= 1.2f;
                Debug.LogWarning("came");
            }
        }


        private void CameraController_Update(On.CameraController.orig_Update orig, CameraController self) {

            orig(self);

            if (self.overrideCameraUpdate)
                return;

            if (GameController.playerScripts[0] == null || GameController.playerScripts[1] == null)
                return;
            //Debug.LogWarning("I'm not racist I swear");
            //if (GameController.pvpOn || SceneManager.GetActiveScene().name.Contains("PvP"))
            //this the bit of code ordinarly runs after the 'if' statement above ("are we pvp right now").
            //so for this hook i'm simply doing it again, but without the check, so it always does it.
            self.distanceBetweenPlayers = self.playerDiff.magnitude * 0.42f;
            Camera.main.orthographicSize = ((self.distanceBetweenPlayers <= CameraController.originalCameraSize * 0.95f) ? CameraController.originalCameraSize * 0.95f : self.distanceBetweenPlayers);
        
            //ordinarily, simply copypasting code from the game is kinda bad practice, but that's a subject for a different time
            //copypasting their code and tweaking things is kind of a cop-out, also a lot of the time ain't even gonna cut it
            //what you want is to really understand what's happening in their code, then figure out what you want to do by adding your additional code.
            //it's a pretty interesting unique problem to solve. gotta think outside the box almost literally
        }
    }
}
