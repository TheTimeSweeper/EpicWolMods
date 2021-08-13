using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace MyBeloved {

    [BepInPlugin("TheTimeSweeper.MyBeloved", "mybeloved", "0.0")]
    public class WhyDidYouLeaveMe : BaseUnityPlugin {

        void Awake() {

            Utils.loge = Logger;

            On.Player.VinePullDash.ctor += VinePullDash_ctor;

            //On.Player.SkillState.InitChargeSkillSettings += SkillState_InitChargeSkillSettings;

            return;

            Assets.Ass();

            On.FSM.AddState += FSM_AddState;

            On.Attack.SetAttackInfo_string_string_int_bool += Attack_SetAttackInfo_string_string_int_bool;

            //On.StatManager.GetSkillData += StatManager_GetSkillData;
        }

        private void SkillState_InitChargeSkillSettings(On.Player.SkillState.orig_InitChargeSkillSettings orig, Player.SkillState self, int maxCharges, float delayBetweenCharges, StatData statData, Player.SkillState skillState) {
            orig(self, maxCharges, delayBetweenCharges, statData, skillState);

            if(self.skillID == "VinePullDash") {
                self.cooldownRef.chargeCount = 2;
                Utils.printAllFields(self.cooldownRef, true);
                //self.cooldownRef.statData.numVarStatDict[StatData.cdStr].BaseValue *= 0.2f;
            }
        }

        private void VinePullDash_ctor(On.Player.VinePullDash.orig_ctor orig, Player.VinePullDash self, FSM fsm, Player parentPlayer) {
            orig(self, fsm, parentPlayer);

            self.InitChargeSkillSettings(2, 0f, self.skillData, self);
        }

        private AttackInfo Attack_SetAttackInfo_string_string_int_bool( 
            On.Attack.orig_SetAttackInfo_string_string_int_bool orig, Attack self, string newSkillCat, string newSkillID, int newSkillLevel, bool newIsUltimate) {

            AttackInfo stinkyAttackInfo = orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);

            if (newSkillID == "AirChannelDash") {

                AttackInfo sexyAttackInfo = newSkillLevel == 1 ? Assets.AttackInfoWindBurst : Assets.AttackInfoChannel;
                if(sexyAttackInfo == null) {
                    Utils.loge.LogError("couldn't load attackinfo json");
                    return stinkyAttackInfo;
                }
                    
                replaceAttackInfo(stinkyAttackInfo, sexyAttackInfo);

                //Utils.SaveJson(stinkyAttackInfo, "stink");
                //Utils.SaveJson(sexyAttackInfo, "sexy");

                return sexyAttackInfo;
            }

            return stinkyAttackInfo;
        }

        private StatData StatManager_GetSkillData(On.StatManager.orig_GetSkillData orig, string category, string skillID) {

            StatData origStatData = orig(category, skillID);

            if (skillID == "AirChannelDash") {

                #region don'tworryboutthat
                for (int i = 0; i < 20; i++) {

                    Logger.LogMessage("AirChannelDash");
                }
                #endregion

            }

            return orig(category, skillID);

        }

        private void FSM_AddState(On.FSM.orig_AddState orig, FSM self, IState newState) {

            if(newState is Player.AirChannelDash) {

                Player.BaseDashState airchanneldashpoopoo = ((Player.BaseDashState)newState);
                newState = new AirChannelDashGood(airchanneldashpoopoo.fsm, airchanneldashpoopoo.parent);
            }

            orig(self, newState);
        }



        private void replaceAttackInfo(AttackInfo stinky, AttackInfo sexy) {

            sexy.entity = stinky.entity;
            sexy.gameObject = stinky.gameObject;
            sexy.skillCategory = stinky.skillCategory;
            sexy.attackInfoKey = stinky.attackInfoKey;
            sexy.atkObjID = stinky.atkObjID;
            sexy.attacker = stinky.attacker;
        }
    }
}
