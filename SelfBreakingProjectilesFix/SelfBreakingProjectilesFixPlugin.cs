using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace SelfBreakingProjectilesFix {
    [BepInPlugin("TheTimeSweeper.SelfBreakingProjectilesFix", "SelfBreakingProjectilesFix", "1.0.0")]
    public class SelfBreakingProjectilesFixPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            IL.ShatterStone.Shatter += ShatterStone_Shatter;

            IL.DragonGrade.CreateCircle += DragonGrade_CreateCircle;
        }

        private void ShatterStone_Shatter(ILContext il)
        {
            //Issue comes on this line
            //earthBurst.gameObject.AddComponent<NegateProjectiles>();
            //NegateProjectiles should be on the same object as the Attack object but instead it's on the root object
            //I'll hijack the earthBurst.gameObject reference before the AddComponent and make it reference the Attack gameobject instead

            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchLdloc(0),
                instruction => instruction.MatchCallvirt<Component>("get_gameObject"),
                instruction => instruction.MatchCallvirt<GameObject>("AddComponent")
                );
            cursor.Index -= 1;
            cursor.EmitDelegate<Func<GameObject, GameObject>>((gob) =>
            {
                return gob.transform.Find("Attack").gameObject;
            });
        }

        private void DragonGrade_CreateCircle(ILContext il)
        {
            //issue is that the circle is being created without initializing its attackinfo
                //so the NegateProjectiles component never properly gets its skillCategory field set
            //but that was probably on purpose so this move doesn't do 150 damage
            //I'll just swooce in and populate the skillCategoryfield itself

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
            //load skillCat field from the class
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, typeof(DragonGrade).GetField("skillCat", AllFlags));
            //load local variable 0 (the ChaosCircle that was created in this function)
            cursor.Emit(OpCodes.Ldloc_0);
            //do magic with these two loaded
            cursor.EmitDelegate<Action<string, ChaosCircle>>((cat, circle) =>
            {
                circle.GetComponentInChildren<NegateProjectiles>().skillCategory = cat;
            });
        }
    }
}
