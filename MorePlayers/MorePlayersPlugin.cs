using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Rewired;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MorePlayers {
    [BepInPlugin("TheTimeSweeper.MorePlayers", "MorePlayers", "0.1.0")]
    public class MorePlayersPlugin : BaseUnityPlugin {

        PlayerCharacterSelectUI P3CharacterSelectUI;

        public static Rewired.Player Player2
        {
            get
            {
                //if (_Player2 == null)
                //{
                //    _Player2 = ReInput.players.GetPlayer("Player2");
                //}
                return _Player2;
            }
        }
        private static Rewired.Player _Player2;

        void Awake() {
            On.CharacterSelectUI.Awake += CharacterSelectUI_Awake;
            On.InputController.Start += InputController_Start;
            IL.InputController.ClaimController += InputController_ClaimController;
        }

        private void InputController_ClaimController(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchCall("InputController", "get_Player1")
                );
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<Rewired.Player, int, Rewired.Player>>((player1, playerID) =>
            {
                return playerID != 2? player1 : Player2;
            });
        }

        private void InputController_Start(On.InputController.orig_Start orig, InputController self)
        {
            orig(self);

            Logger.LogWarning("nip");
            for (int i = 0; i < ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF.Length; i++)
            {
                Logger.LogWarning(ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF[i].id);
            }
            Logger.LogWarning("nip2");
            for (int i = 0; i < InputController._Players.Length; i++)
            {
                Logger.LogWarning(InputController._Players[i].descriptiveName);
            }

            Rewired.Player ninja = ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF[1];
            _Player2 = new Rewired.Player(false,
                ninja.id + 1,
                "Player2",
                "Player2",
                ninja.controllers.maps.DLPSGhFqsLysMzYqTBpnYTeocLO,
                ninja.controllers.maps.tjDTjqXeaoFrZfwVolflziGBOFV._startingSettings,
                ninja.controllers.maps.JrWHmlicXNFsdSNErAKUHNNzLJK.DvjFaRDewKkWzywTEXrTPFyuwIve);
            ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF = 
                ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF.Concat(new Rewired.Player[] { _Player2 }).ToArray();
            
            //ref Rewired.Player[] array = ref ReInput.dwyRFFofXhsfFGtIUTwMfsGKpDF.UJGuxhfcVIgvlKKSgAhEaGnoEDZF;
            //Array.Resize(ref array, array.Length + 1);
            //array[array.Length - 1] = player2;

            InputController._Players = new Rewired.Player[]
                   {
                    InputController.Player0,
                    InputController.Player1,
                    Player2,
                    InputController.SysPlayer
                   };
            Logger.LogWarning("nip2");
        }

        private void CharacterSelectUI_Awake(On.CharacterSelectUI.orig_Awake orig, CharacterSelectUI self)
        {
            orig(self);

            if (P3CharacterSelectUI == null)
            {
                P3CharacterSelectUI = UnityEngine.Object.Instantiate(CharacterSelectUI.P2CharacterSelectUI, CharacterSelectUI.P2CharacterSelectUI.transform.parent);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                P3CharacterSelectUI.gameObject.SetActive(true);
                P3CharacterSelectUI.playerID = 2;
                P3CharacterSelectUI.Activate();
            }
        }
    }
}
