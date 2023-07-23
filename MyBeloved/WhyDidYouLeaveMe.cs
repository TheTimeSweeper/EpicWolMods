using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBeloved {

    [BepInDependency("TheTimeSweeper.SkillsButEpic", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TheTimeSweeper.SillySkills", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("TheTimeSweeper.MyBeloved", "mybeloved", "0.1.0")]
    public class WhyDidYouLeaveMe : BaseUnityPlugin {

        static bool fuckinRan;

        void Start() {

            //On.GameController.Awake += GameController_Awake_PrintAllTheThings;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TheTimeSweeper.SkillsButEpic") ||
                BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("TheTimeSweeper.SillySkills"))
            {
                Logger.LogWarning("I'ma let you finish");
                return;
            }

            Log.Init(Logger);
            Assets.Ass();

            On.StatManager.LoadData += StatManager_LoadData_AddAirChannelDashToLoadedData;

            On.Player.InitFSM += Player_InitFSM_AddAirChannelDash;
            On.Player.InitSkills += Player_InitSkills_AssignSkill;

            //old way
            //On.FSM.AddState += FSM_AddState;
            //On.Attack.SetAttackInfo_string_string_int_bool += Attack_SetAttackInfo_string_string_int_bool_OldReplaceMethod;
        }

        private static void GameController_Awake_PrintAllTheThings(On.GameController.orig_Awake orig, GameController self)
        {
            orig(self);

            if (fuckinRan)
                return;
            fuckinRan = true;
            string statID = StatManager.statFieldStr;
            string category = StatManager.playerBaseCategory;

            string text = category;
            if (!StatManager.data.ContainsKey(statID))
            {
                StatManager.data[statID] = new Dictionary<string, Dictionary<string, StatData>>();
            }
            StatManager.data[statID][text] = new Dictionary<string, StatData>();
            Dictionary<string, StatData> dictionary = StatManager.data[statID][text];
            SkillStatContainer skillStatContainer = JsonUtility.FromJson<SkillStatContainer>(ChaosBundle.Get<TextAsset>(StatManager.playerSkillAssetPath).text);
            int num = skillStatContainer.skillStats.Length;
            for (int i = 0; i < num; i++)
            {
                SkillStats skillStats = skillStatContainer.skillStats[i];
                skillStats.Initialize();

                string prettyName = TextManager.skillInfoDict.ContainsKey(skillStats.ID[0]) ? TextManager.skillInfoDict[skillStats.ID[0]].displayName : "z_noname";

                Utils.SaveJson(skillStats, prettyName + "_" + skillStats.ID[0]);
            }
        }
        #region add the man
        private void StatManager_LoadData_AddAirChannelDashToLoadedData(On.StatManager.orig_LoadData orig, string assetPath, string statID, string category, string categoryModifier) {
            orig(assetPath, statID, category, categoryModifier);

            SkillStats stats = Assets.AirChannelDashGoodSkillStats;
            stats.Initialize();
            string fullCategory = category + categoryModifier;
            StatData statData = new StatData(stats, fullCategory);
            List<string> targetNames = statData.GetValue<List<string>>("targetNames", -1);
            if (targetNames.Contains(Globals.allyHBStr) || targetNames.Contains(Globals.enemyHBStr)) {
                targetNames.Add(Globals.ffaHBStr);
            }
            if (targetNames.Contains(Globals.allyFCStr) || targetNames.Contains(Globals.enemyFCStr)) {
                targetNames.Add(Globals.ffaFCStr);
            }

            Dictionary<string, StatData> dictionary = StatManager.data[statID][fullCategory];
            dictionary[statData.GetValue<string>("ID", -1)] = statData;
            StatManager.globalSkillData[statData.GetValue<string>("ID", -1)] = statData;
        }
        #endregion

        private void Player_InitFSM_AddAirChannelDash(On.Player.orig_InitFSM orig, Player self) {

            orig(self);
            //AirChannelDashGood newState = new AirChannelDashGood(self.fsm, self);
            Player.SkillState newState = (Player.SkillState)Activator.CreateInstance(typeof(AirChannelDashGood), self.fsm, self);
            self.fsm.AddState(newState);
            GameDataManager.gameData.PullSkillData();
        }

        private void Player_InitSkills_AssignSkill(On.Player.orig_InitSkills orig, Player self) {
            if (Input.GetKey(KeyCode.G)) {
                self.playerData.skills[1] = "AirChannelDashGood0";
            }
            orig(self);
        }

        #region old replace way of doing it
        private void FSM_AddState(On.FSM.orig_AddState orig, FSM self, IState newState) {

            if (newState is Player.AirChannelDash) {

                Player.BaseDashState airchanneldashpoopoo = ((Player.BaseDashState)newState);
                newState = new AirChannelDashGood(airchanneldashpoopoo.fsm, airchanneldashpoopoo.parent);
            }

            orig(self, newState);
        }

        private AttackInfo Attack_SetAttackInfo_string_string_int_bool_OldReplaceMethod( 
            On.Attack.orig_SetAttackInfo_string_string_int_bool orig, Attack self, string newSkillCat, string newSkillID, int newSkillLevel, bool newIsUltimate) {

            //statdict has been added method
            if(newSkillID == "AirChannelDash") {
                newSkillID = "AirChannelDashGood";
            }

            return orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);


            //replace attackinfo method
            AttackInfo stinkyAttackInfo = orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);

            if (newSkillID == "AirChannelDash") {

                AttackInfo sexyAttackInfo = newSkillLevel == 1 ? Assets.AttackInfoAirChannel : Assets.AttackInfoWindBurst;
                if (sexyAttackInfo == null) {
                    Log.Error("couldn't load attackinfo json");
                    return stinkyAttackInfo;
                }
                
                replaceAttackInfo(stinkyAttackInfo, sexyAttackInfo);

                //Utils.SaveJson(stinkyAttackInfo, "stink " + newSkillLevel);
                //Utils.SaveJson(sexyAttackInfo, "sexy");

                return stinkyAttackInfo;
            }

            return stinkyAttackInfo;
        }

        private void replaceAttackInfo(AttackInfo stinky, AttackInfo sexy) {

            //stinky.entity = sexy.entity;
            //stinky.gameObject = sexy.gameObject;
            //stinky.skillCategory = sexy.skillCategory;
            //stinky.attackInfoKey = sexy.attackInfoKey;
            //stinky.atkObjID = sexy.atkObjID;
            //stinky.attacker = sexy.attacker;
            //why did I think this would suffice lol
            //maybe I was right though idk
            //new way
            stinky.sameAttackImmunityTime = sexy.sameAttackImmunityTime;
            stinky.hitStunDurationModifier = sexy.hitStunDurationModifier;
            stinky.knockbackMultiplier = sexy.knockbackMultiplier;
            stinky.knockbackOverwrite = sexy.knockbackOverwrite;
            stinky.damage = sexy.damage;
            stinky.odSingleIncrease = sexy.odSingleIncrease;
        }
        #endregion
    }
}
