using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NetWizarding
{
    [BepInPlugin("TheTimesweeper.NetWizarding", "NetWizarding", "0.1.0")]
    public class NetWizardingPlugin : BaseUnityPlugin
    {
        public static NetWizardingPlugin Instance;

        public static int config_hostPort;
        public static int config_clientPort;

        private Rewired.Data.UserData userDataButEpic;
        private CustomController netWizardFakeController;

        public static ConfigFile staticConfig;

        public Vector2 networkPlayer2MoveInput;
        public Vector2 networkPlayer2LookInput;

        public string[] stateList;

        public delegate void Player1StateChanged(int stateIndex);
        public static event Player1StateChanged OnPlayer1StateChanged;

        private bool funnyHookBool_StateChangeAllowed;

        void Awake()
        {
            Instance = this;
            staticConfig = base.Config;
            Log.Init(Logger);
            Harmony.CreateAndPatchAll(typeof(NetWizardingPlugin));
            Debug.Log("I belive in you c:");
            config_hostPort = NetWizardingPlugin.staticConfig.Bind("uh", "hostport", 6969).Value;
            config_clientPort = NetWizardingPlugin.staticConfig.Bind("uh", "clientport", 9696).Value;

            //gameObject.AddComponent<NetworkTransportVersion>();
            gameObject.AddComponent<HLAPIVersion>();

            LoadFromBundle();

            On.Health.TakeDamage += Health_TakeDamage;
            On.Rewired.InputManager_Base.Initialize += InputManager_Base_Initialize;
            On.Movement.MoveToMoveVector += Movement_MoveToMoveVector;
            On.Player.InitFSM += Player_InitFSM;
            On.FSM.ChangeState += FSM_ChangeState;
            On.ChaosInputDevice.GetAimVector += ChaosInputDevice_GetAimVector;
            On.ChaosInputDevice.GetMoveVector += ChaosInputDevice_GetMoveVector;
            //On.Attack.CheckCollision += Attack_CheckCollision;
        }

        private Vector2 ChaosInputDevice_GetMoveVector(On.ChaosInputDevice.orig_GetMoveVector orig, ChaosInputDevice self)
        {
            if (TryGetPlayer2(out var player2) && self == player2.inputDevice)
            {
                return networkPlayer2MoveInput;
            }

            return orig(self);
        }

        private Vector2 ChaosInputDevice_GetAimVector(On.ChaosInputDevice.orig_GetAimVector orig, ChaosInputDevice self)
        {
            if(TryGetPlayer2(out var player2) && self == player2.inputDevice)
            {
                return networkPlayer2LookInput;
            }

            return orig(self);
        }

        //this is causing stack overflows???
        private void FSM_ChangeState(On.FSM.orig_ChangeState orig, FSM self, string targetStateName, bool allowSelfTransition)
        {
            if (self.currentState != null && !funnyHookBool_StateChangeAllowed && TryGetPlayer2(out var player2) && self == player2.fsm)
            {
                return;
            }
            funnyHookBool_StateChangeAllowed = false;
            if (stateList != null && TryGetPlayer1(out var player1) && self == player1.fsm)
            {
                OnPlayer1StateChanged?.Invoke(GetStateIndex(targetStateName));
            }
            if(TryGetPlayer1(out var player22) && self == player22.fsm)
            {
                Log.Warning($"player 2 entering state {targetStateName}");
            }
            orig(self, targetStateName, allowSelfTransition);
        }

        private int GetStateIndex(string targetStateName)
        {
            for (int i = 0; i < stateList.Length; i++)
            {
                if (stateList[i] == targetStateName)
                {
                    return i;
                }
            }
            return -1;
        }

        private void Player_InitFSM(On.Player.orig_InitFSM orig, Player self)
        {
            orig(self);
            if (stateList == null)
            {
                stateList = self.fsm.states.Keys.ToArray();
            }
        }

        private bool Health_TakeDamage(On.Health.orig_TakeDamage orig, Health self, AttackInfo givenAttackInfo, Entity attackEntity, bool critPreCalculated)
        {
            if (givenAttackInfo.entity == GameController.playerScripts[1])
            {
                return false;
            }
            return orig(self, givenAttackInfo, attackEntity, critPreCalculated);
        }

        //no. attacks need to know when they hit for effects to go through.
        //private bool Attack_CheckCollision(On.Attack.orig_CheckCollision orig, Attack self, Collider2D col, int targetObjID)
        //{
        //    if(self.atkInfo.entity == GameController.playerScripts[1])
        //    {
        //        return false;
        //    }
        //    return orig(self, col, targetObjID);
        //}

        private bool TryGetPlayer1(out Player playerScript)
        {
            if (GameController.playerScripts.Length > 0 && GameController.playerScripts[0])
            {
                playerScript = GameController.playerScripts[0];
                return true;
            }
            playerScript = null;
            return false;
        }


        private bool TryGetPlayer2(out Player playerScript)
        {
            if (GameController.playerScripts.Length > 1 && GameController.playerScripts[1])
            {
                playerScript = GameController.playerScripts[1];
                return true;
            }
            playerScript = null;
            return false;
        }

        private void Movement_MoveToMoveVector(On.Movement.orig_MoveToMoveVector orig, Movement self, float speed, bool useAddForce)
        {
            if(TryGetPlayer2(out var player2) && self == player2.movement)
            {
                self.UpdateZeroedVelocity(self.moveVector);
                return;
            }
            orig(self, speed, useAddForce);
        }

        void Update()
        {
            #region testFakeController
            //if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    Player2ClaimFakeInput();
            //}
            //if (Input.GetKey(KeyCode.Alpha1))
            //{
            //    positionDifference.x = 1;
            //}
            //if (Input.GetKey(KeyCode.Alpha2))
            //{
            //    positionDifference.x = -1;
            //}
            //if (Input.GetKey(KeyCode.Alpha3))
            //{
            //    positionDifference.y = 1;
            //}
            //if (Input.GetKey(KeyCode.Alpha4))
            //{
            //    positionDifference.y = -1;
            //}
            //if (Input.GetKey(KeyCode.Alpha6))
            //{
            //    positionDifference = Vector3.zero;
            //}
            #endregion testFakeController

            if (Input.GetKey(KeyCode.G)) debugG = !debugG;
            if (debugG)
            {
                GameController.playerScripts[0].movement.moveVector = Vector3.right;
                GameController.playerScripts[0].movement.MoveToMoveVector(0, false);
            }
        }

        [HarmonyPatch(typeof(ChaosInputDevice), "controllerCount", MethodType.Getter)]
        [HarmonyPostfix]
        static void Postfix(ref int __result, ChaosInputDevice __instance)
        {
            __result += __instance.rewiredPlayer.controllers.customControllerCount;
        }

        private void LoadFromBundle()
        {
            var bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), Path.Combine("Assets", "netwizarding")));
            userDataButEpic = bundle.LoadAsset<GameObject>("RewiredInputButEpic").GetComponent<InputManager>().userData;
        }

        private void InputManager_Base_Initialize(On.Rewired.InputManager_Base.orig_Initialize orig, InputManager_Base self)
        {
            self._userData.customControllers = userDataButEpic.customControllers;
            self._userData.customControllerMaps = userDataButEpic.customControllerMaps;
            self._userData.players = userDataButEpic.players;
            orig(self);
            netWizardFakeController = ReInput.ControllerHelper.Instance.CreateCustomController(0);
            //ReInput.InputSourceUpdateEvent += ReInput_InputSourceUpdateEvent;
        }
        private bool debugG;
        private void ReInput_InputSourceUpdateEvent()
        {
            netWizardFakeController.SetAxisValue(0, networkPlayer2MoveInput.y);
            netWizardFakeController.SetAxisValue(1, networkPlayer2MoveInput.x);
        }

        public void Player2ClaimFakeInput()
        {
            CharacterSelectUI.P2CharacterSelectUI.currentController = netWizardFakeController;
            CharacterSelectUI.P2CharacterSelectUI.ClaimInputDevice();
        }

        public void RecieveState(int stateIndex)
        {
            if(stateIndex == -1)
            {
                return;
            }
            if(TryGetPlayer2(out var player2))
            {
                funnyHookBool_StateChangeAllowed = true;
                player2.fsm.ChangeState(stateList[stateIndex]);
                funnyHookBool_StateChangeAllowed = false;
            }
        }
    }
}
