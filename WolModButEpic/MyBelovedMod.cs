using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBeloved {
    //[BepInPlugin("com.TheTimeSweeper.MyBeloved", "Gust Burst Come Back", "1.0.0")]
    public class MyBelovedMod : BaseUnityPlugin {

        void Awake() {

            On.Player.InitFSM += Player_InitFSM;
            On.StatManager.LoadPlayerSkills += StatManager_LoadPlayerSkills;
        }

        private void StatManager_LoadPlayerSkills(On.StatManager.orig_LoadPlayerSkills orig, string categoryModifier) {
            orig(categoryModifier);

            Debug.Log("loadingnewkills");

            string category = StatManager.playerBaseCategory + categoryModifier;
            Dictionary<string, StatData> statDataDictionary = StatManager.data[StatManager.statFieldStr][category];

            SkillStatContainer skillStatContainer = JsonUtility.FromJson<SkillStatContainer>(ChaosBundle.Get<TextAsset>(StatManager.playerSkillAssetPath).text);

            StatData newStatData = new StatData(new SkillStats(), category);

            statDataDictionary[newStatData.GetValue<string>("ID", -1)] = newStatData;
        }

        private void Player_InitFSM(On.Player.orig_InitFSM orig, Player self) {
            orig(self);

            Debug.LogWarning("initsfm");
            return;

            try {
                self.fsm.AddState(new Player.UseAirTrailState(self.fsm, self));
            }
            catch (System.Exception e) {
                Debug.LogWarning($"NImrod\n{e}");
            }
        }

    }
}
