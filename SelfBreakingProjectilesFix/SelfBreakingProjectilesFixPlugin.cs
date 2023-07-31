using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace SelfBreakingProjectilesFix {
    [BepInPlugin("TheTimeSweeper.SelfBreakingProjectilesFix", "SelfBreakingProjectilesFix", "0.1.0")]
    public class SelfBreakingProjectilesFixPlugin : BaseUnityPlugin {
        void Awake() {
            IL.ShatterStone.Shatter += ShatterStone_Shatter;

            IL.DragonGrade.CreateCircle += DragonGrade_CreateCircle;


            //On.Attack.CheckCollision += Attack_CheckCollision;

            //On.Projectile.HandleProjectileCollision += Projectile_HandleProjectileCollision;
            //On.Projectile.OnProjectileCollision += Projectile_OnProjectileCollision;
            //On.Attack.CheckCollision += Attack_CheckCollision;

            NegateProjectiles.globalNegateEventHandlers += nip;

            On.NegateProjectiles.OnTriggerEnter2D += NegateProjectiles_OnTriggerEnter2D;

            On.NegateProjectiles.Start += NegateProjectiles_Start;

            On.NegateProjectiles.Awake += NegateProjectiles_Awake;

            #region just incase
            On.DragonGrade.CreateExplosion += DragonGrade_CreateExplosion;
            On.DragonGrade.CreatePillar += DragonGrade_CreatePillar;
            On.DragonGrade.CreateCircle += DragonGrade_CreateCircle;
            #endregion just incase
        }

        private void DragonGrade_CreateCircle(ILContext il)
        {
            BindingFlags AllFlags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance;

            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchStloc(0),
                instruction => instruction.MatchLdloc(0),
                instruction => instruction.MatchLdcI4(1)
                );
            cursor.Index -= 2;
            Logger.LogWarning(cursor);
            cursor.Emit(OpCodes.Ldarg_0);
            Logger.LogWarning(cursor);
            cursor.Emit(OpCodes.Ldfld, typeof(DragonGrade).GetField("skillCat", AllFlags));
            Logger.LogWarning(cursor);
            cursor.Emit(OpCodes.Ldloc_0);
            Logger.LogWarning(cursor);
            cursor.EmitDelegate<Action<string, ChaosCircle>>((cat, circle) =>
            {
                circle.GetComponentInChildren<NegateProjectiles>().skillCategory = cat;
            });
            Logger.LogWarning(cursor);
            //cursor.Emit(OpCodes.Ldloc_0);
            //Logger.LogWarning(cursor);
            cursor.Index ++;
            Logger.LogWarning(cursor);
        }

        private void DragonGrade_CreateCircle1(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchStloc(0),
                instruction => instruction.MatchLdloc(0),
                instruction => instruction.MatchLdcI4(1)
                );
            cursor.Index -= 1;
            Logger.LogWarning(cursor);
            cursor.EmitDelegate<Action<ChaosCircle>>(circle =>
            {
                circle.GetComponentInChildren<NegateProjectiles>().skillCategory = "Player0";
            });
            Logger.LogWarning(cursor);
            cursor.Emit(OpCodes.Ldloc_0);
            Logger.LogWarning(cursor);
        }

        private void ShatterStone_Shatter(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchLdloc(0),
                instruction => instruction.MatchCallvirt<Component>("get_gameObject"),
                instruction => instruction.MatchCallvirt<GameObject>("AddComponent")
                );
            cursor.Index-=1;
            cursor.EmitDelegate<Func<GameObject, GameObject>> ((gob) => {
                return gob.transform.Find("Attack").gameObject;
            });
        }


        private void NegateProjectiles_Awake(On.NegateProjectiles.orig_Awake orig, NegateProjectiles self)
        {
            Logger.LogWarning($"awake| {self.name}, {self.GetComponent<Attack>()}");
            orig(self);
        }

        private void NegateProjectiles_Start(On.NegateProjectiles.orig_Start orig, NegateProjectiles self)
        {
            Logger.LogWarning($"start| cat {self.skillCategory == string.Empty} && ak {self.attack != null} && akinf {self.attack?.atkInfo != null}");
            string category = self.attack?.atkInfo?.skillCategory;
            Logger.LogWarning($"start| cat {category}");
            orig(self);
        }

        /*

   if (log += "self.otherScript == null || 
       log += "self.otherScript.ignoreNegate ||
       log += "self.otherScript.forcedReturn || 
       log += "(self.otherScript.parentObject != null && self.otherScript.parentObject == self.parentObject) || 
       log += "self.otherScript.attackBox == null || 
       log += "self.otherScript.attackBox.atkInfo == null)
*/
        private void NegateProjectiles_OnTriggerEnter2D(On.NegateProjectiles.orig_OnTriggerEnter2D orig, NegateProjectiles self, Collider2D other)
        {
            Projectile otherScript = other.transform.parent.GetComponent<Projectile>();
            if (otherScript != null)
            {
                string log = "touch proj " + otherScript;
                //log += $"1{otherScript == null                                                                      }, ";
                //log += $"2{otherScript.ignoreNegate                                                                 }, ";
                //log += $"3{otherScript.forcedReturn                                                                 }, ";
                //log += $"4{(otherScript.parentObject != null && otherScript.parentObject == self.parentObject)      }, ";
                //log += $"5{otherScript.attackBox == null                                                            }, ";
                //log += $"6{otherScript.attackBox.atkInfo == null                                                    }, ";
                Attack otherAttack = otherScript.attackBox;
                List<string> incomingTargetList = otherAttack.atkInfo.targetNames;
                log += $"\nskillCategory {self.skillCategory}, otherCategory {otherAttack.atkInfo.skillCategory}";
                log += $"\nincoming target list: ";
                for (int i = 0; i < incomingTargetList.Count; i++)
                {
                    log += $"{incomingTargetList[i]}, ";
                }
                log += $"\n{self.skillCategory.Contains("Player")} && {!incomingTargetList.Contains(Globals.allyHBStr)} && {!incomingTargetList.Contains(Globals.allyFCStr)}";
                Logger.LogWarning(log);

                var gob = otherScript.parentObject != null ? otherScript.parentObject : null;

                Logger.LogWarning($"parent {self.parentObject}, otherscript parent {gob}");
            }

            orig(self, other);
        }

        private void nip(NegateProjectiles negate, Collider2D other, Projectile otherProjectile, string negateSkillCategory)
        {
            //Logger.LogWarning(otherProjectile.name);
        }

        //private void Projectile_OnProjectileCollision(On.Projectile.orig_OnProjectileCollision orig, Projectile self, Collider2D other, Projectile otherProjectile, bool doCalculations)
        //{
        //    orig(self, other, otherProjectile, doCalculations);
        //    Logger.LogWarning($"{self.name}, {other.name}");
        //}

        //private void Projectile_HandleProjectileCollision(On.Projectile.orig_HandleProjectileCollision orig, 
        //    Projectile self, Collider2D other)
        //{
        //    orig(self, other);
        //    Logger.LogWarning($"{self.name}, {other.name}");
        //}

        /*
		return !this.IsAllyCheck(this.atkInfo.skillCategory, this.checkColObjNameStr) && 
               !this.IsSkillOriginator(col.gameObject) && 
               !this.ignoreTargetsList.Contains(targetObjID) && 
               (base.isActiveAndEnabled && this.CheckAttackHistory(targetObjID)) && 
               this.atkInfo.targetNames.Contains(this.checkColObjNameStr);
         */
        //private bool Attack_CheckCollision(On.Attack.orig_CheckCollision orig, Attack self, Collider2D col, int targetObjID)
        //{
        //    bool returnOrig = orig(self, col, targetObjID);
        //    List<string> list = ((List<string>)self.statData?.statDict["ID"]);
        //    string statdictID = list!= null? list[0] : "nig";
        //    Logger.LogWarning($"from {self._attackerObjID}, {self.attackerName}, {statdictID}, obj name {self.checkColObjNameStr}, targetObjID {targetObjID}, hit {returnOrig}");
        //    return returnOrig;
        //}

        #region just in case
        private void DragonGrade_CreateExplosion(On.DragonGrade.orig_CreateExplosion orig, DragonGrade self)
        {
            if (Input.GetKey(KeyCode.V))
                return;

            orig(self);
        }

        private void DragonGrade_CreatePillar(On.DragonGrade.orig_CreatePillar orig, DragonGrade self)
        {
            if (Input.GetKey(KeyCode.B))
                return;

            orig(self);
        }

        private void DragonGrade_CreateCircle(On.DragonGrade.orig_CreateCircle orig, DragonGrade self)
        {
            if (Input.GetKey(KeyCode.N))
                return;

            orig(self);
        }
        #endregion just in case

    }
}
