using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;
using System.Collections.ObjectModel;
using System.Reflection;
using Rewired.Data;
using Rewired.Data.Mapping;
using System.IO;

namespace TwinStick
{
    [BepInPlugin("TheTimeSweeper.TwinStick", "TwinStick", "0.1.0")]
    public class TwinStickPlugin : BaseUnityPlugin
    {
        public static List<ControllerMap> joystickMaps = new List<ControllerMap>();

        public static InputAction actionHori = new InputAction
        {
            _id = 15,
            _name = "LookHorizontal",
            _descriptiveName = "Look Horizontal",
            _type = InputActionType.Axis,
            _userAssignable = true,
        };
        public static InputAction actionVeri = new InputAction
        {
            _id = 16,
            _name = "LookVertical",
            _descriptiveName = "Look Vertical",
            _type = InputActionType.Axis,
            _userAssignable = true
        };

        private Vector2 _lastValidLookVector;

        void Awake()
        {
            Log.Init(Logger);

            On.Rewired.InputManager_Base.Initialize += InputManager_Base_Initialize;
            On.InputController.AssignControllerToPlayer += InputController_AssignControllerToPlayer;
            On.ChaosInputDevice.GetAimVector += ChaosInputDevice_GetAimVector;
            //affects dashes woops
            //innovation from 1996 is too much
            //On.ChaosInputDevice.GetMoveVector += ChaosInputDevice_GetMoveVector;

            On.Player.MeleeAttackState.MovementCheck += MeleeAttackState_MovementCheck;

            On.GameUI.LoadInputButtonSprites += GameUI_LoadInputButtonSprites;
        }

        private bool MeleeAttackState_MovementCheck(On.Player.MeleeAttackState.orig_MovementCheck orig, Player.MeleeAttackState self)
        {
            bool origReturn = orig(self);
            if (!self.parent.inputDevice.IsMouseAim && GetLookVectorRaw(self.parent.inputDevice).magnitude < 0.9f)
                return false;
            return origReturn;
        }

        private Vector2 ChaosInputDevice_GetMoveVector(On.ChaosInputDevice.orig_GetMoveVector orig, ChaosInputDevice self)
        {
            if(self.rewiredPlayer == null)
            {
                return orig(self);
            }
            self.HandleInputSchemeSwitch();
            Vector2 axis2D = self.rewiredPlayer.GetAxis2D("MoveHorizontal", "MoveVertical") * 2;
            if(axis2D.sqrMagnitude > 1)
            {
                return orig(self);
            }
            return axis2D;
        }

        private void GameUI_LoadInputButtonSprites(On.GameUI.orig_LoadInputButtonSprites orig)
        {
            orig();

            GameUI.InputButtonSprites["Slot0XBox"] = GameUI.UIButtonIcons["LeftTrigger"];
            GameUI.InputButtonSprites["Slot1XBox"] = GameUI.UIButtonIcons["LeftButton"];
            GameUI.InputButtonSprites["Slot2XBox"] = GameUI.UIButtonIcons["R3"];
            GameUI.InputButtonSprites["Slot3XBox"] = GameUI.UIButtonIcons["R3"];
            GameUI.InputButtonSprites["Slot4XBox"] = GameUI.UIButtonIcons["RightButton"];
            GameUI.InputButtonSprites["Slot5XBox"] = GameUI.UIButtonIcons["RightTrigger"];

            GameUI.InputButtonSprites["EquipMenuXBox"] = GameUI.UIButtonIcons["GamepadButtonYellow"];
            GameUI.InputButtonSprites["MapXBox"] = GameUI.UIButtonIcons["GamepadButtonBlue"];

            GameUI.InputButtonSprites["InteractXBox"] = GameUI.UIButtonIcons["LeftTrigger"];
            //GameUI.InputButtonSprites["ConfirmXBox"] =  GameUI.UIButtonIcons["LeftButton"];
            //GameUI.InputButtonSprites["CancelXBox"] =   GameUI.UIButtonIcons["R3"];
        }

        private void InputManager_Base_Initialize(On.Rewired.InputManager_Base.orig_Initialize orig, InputManager_Base self)
        {
            AddActionsToUserData(self._userData);
            orig(self);
        }

        private static void AddActionsToUserData(UserData userData)
        {
            if(userData.actions.Find(action =>
            {
                return action.descriptiveName == actionHori.descriptiveName;
            }) != null)
            {
                Log.Warning("already added actions");
                return;
            }
            Log.Warning("addinga ctions");
            userData.actions.Add(actionHori);
            userData.actionCategoryMap.AddAction(0, actionHori.id);
            userData.actions.Add(actionVeri);
            userData.actionCategoryMap.AddAction(0, actionVeri.id);

            userData.actionIdCounter += 2;
        }

        private void InputController_AssignControllerToPlayer(On.InputController.orig_AssignControllerToPlayer orig, Rewired.Player player, Controller controller, bool removeFromOtherPlayers, bool ignoreKBMouseCheck)
        {
            orig(player, controller, removeFromOtherPlayers, ignoreKBMouseCheck);
            AddJoysticksAndAddRightStick();
        }

        private static void AddJoysticksAndAddRightStick()
        {
            for (int i = joystickMaps.Count - 1; i >= 0; i--)
            {
                if (joystickMaps[i].controller == null)
                {
                    Log.Warning("removing joystick " + i);
                    joystickMaps.RemoveAt(i);
                }
            }

            foreach (Rewired.Player player in InputController.Players)
            {
                InputMapHelper.tempMaps.Clear();
                player.controllers.maps.GetAllMaps(ControllerType.Joystick, InputMapHelper.tempMaps);

                Log.Warning($"found {InputMapHelper.tempMaps.Count} maps");
                foreach (ControllerMap map in InputMapHelper.tempMaps)
                {
                    if (!joystickMaps.Contains(map))
                    {
                        //PrintJoystickMap(map);
                        Log.Debug("adding new joystick " + map.name);
                        joystickMaps.Add(map);
                        AddFuckinRightStick(map);

                    } else
                    {
                        Log.Warning("already had joystick " + map.name);
                    }
                }
            }
        }

        private static void AddFuckinRightStick(ControllerMap map)
        {
            //the key to victory. thank you
                //tells me what the element ids are of the physical inputs
            //for (int i = 0; i < map.controller.ElementIdentifiers.Count; i++)
            //{
            //    var item = map.controller.ElementIdentifiers[i];
            //    Log.Warning($"id {item.id}, name {item.name}");
            //}            

            map.CreateElementMap(new ElementAssignment(2,
                                                       actionHori.id,
                                                       false));

            map.CreateElementMap(new ElementAssignment(3,
                                                       actionVeri.id,
                                                       false));

            //int mapID = -1;
            //for (int i = 0; i < map.AllMaps.Count; i++)
            //{
            //    if(map.AllMaps[i].actionId == 5)
            //    {
            //        mapID = map.AllMaps[i].id;
            //    }
            //}
            //if (mapID != -1)
            //{
            //    ActionElementMap tempmap = new ActionElementMap(ReInput.mapping.GetAction("Menu").id, ControllerElementType.Button, 9);

            //    map.RemoveElementAssignmentConflicts(tempmap);

            //    map.CreateElementMap(new ElementAssignment(9,
            //                                               5,
            //                                               Pole.Positive,
            //                                               mapID));
            //}

        }


        private Vector2 ChaosInputDevice_GetAimVector(On.ChaosInputDevice.orig_GetAimVector orig, ChaosInputDevice self)
        {
            Vector2 originalAimVector = orig(self);

            if(self.rewiredPlayer == null || self.IsMouseAim)
            {
                return originalAimVector;
            }

            Vector2 lookVector = GetLookVector(self);
            Vector2 moveVector = self.GetMoveVector();

            if (lookVector == Vector2.zero && moveVector != Vector2.zero)
            {
                _lastValidLookVector = moveVector;
                return moveVector;
            }

            if (lookVector != Vector2.zero)
            {
                _lastValidLookVector = lookVector;
            }

            if (ChaosInputDevice.lockControllerAim && moveVector == Vector2.zero &&
               lookVector == Vector2.zero && _lastValidLookVector != Vector2.zero)
            {
                return _lastValidLookVector;
            }
            
            return lookVector;
        }

        private Vector2 GetLookVector(ChaosInputDevice self)
        {
            return self.rewiredPlayer.GetAxis2D("LookHorizontal", "LookVertical").normalized;
        }

        private Vector2 GetLookVectorRaw(ChaosInputDevice self)
        {
            Vector2 raw = self.rewiredPlayer.GetAxis2D("LookHorizontal", "LookVertical");
            return raw.sqrMagnitude > 1 ? raw.normalized : raw;
        }

        private static void PrintAction(InputAction actin)
        {
            Log.Warning($" _id {actin.id}" +
                        $"\n _name {actin._name}" +
                        $"\n _type {actin._type}" +
                        $"\n _descriptiveName {actin._descriptiveName}" +
                        $"\n _positiveDescriptiveName {actin._positiveDescriptiveName}" +
                        $"\n _negativeDescriptiveName {actin._negativeDescriptiveName}" +
                        $"\n _behaviorId {actin._behaviorId}" +
                        $"\n _userAssignable {actin._userAssignable}" +
                        $"\n _categoryId {actin._categoryId}" +
                        $"");
        }

        private static void PrintJoystickMap(ControllerMap map)
        {
            for (int i = 0; i < map.AllMaps.Count; i++)
            {
                var actionMap = map.AllMaps[i];

                Log.Message($"printing action id: {actionMap.actionId}, " +
                    $"action name: {actionMap.actionDescriptiveName}, " +
                    $"element: {actionMap.elementIdentifierName}, " +
                    $"mapID: {actionMap.id}");
            }
        }

        private static void PrintAllJoystickMaps()
        {
            for (int i = 0; i < joystickMaps.Count; i++)
            {
                ControllerMap map = joystickMaps[i];
                PrintJoystickMap(map);
            }
        }

        private static void PrintAllJoystickActions()
        {
            for (int i = 0; i < joystickMaps.Count; i++)
            {
                ControllerMap map = joystickMaps[i];
                Log.Message(map.name);
                //for (int i = 0; i < map.DXgLWByQeIcZLsfqgBFQNjhclxc; i++)
                //{

                //}
            }
        }
    }
}

/*
actions I think
[Warning: TwinStick] 0 printing axis Move Horizontal, Left Stick X
[Warning: TwinStick] 0 printing axis Move Horizontal, Left Stick X
[Warning: TwinStick] 0 printing butn Left, +Control Pad Left
[Warning: TwinStick] 0 printing butn Right, +Control Pad Right
[Warning: TwinStick] 0 printing butn Left, D-Pad Left
[Warning: TwinStick] 0 printing butn Right, D-Pad Right
[Warning: TwinStick] 0 printing butn Left, Left Arrow
[Warning: TwinStick] 0 printing butn Right, Right Arrow
[Warning: TwinStick] 0 printing butn Left, A
[Warning: TwinStick] 0 printing butn Right, D
[Warning: TwinStick] 1 printing axis Move Vertical, Left Stick Y
[Warning: TwinStick] 1 printing axis Move Vertical, Left Stick Y
[Warning: TwinStick] 1 printing butn Up, +Control Pad Up
[Warning: TwinStick] 1 printing butn Down, +Control Pad Down
[Warning: TwinStick] 1 printing butn Up, D-Pad Up
[Warning: TwinStick] 1 printing butn Down, D-Pad Down
[Warning: TwinStick] 1 printing butn Up, Up Arrow
[Warning: TwinStick] 1 printing butn Down, Down Arrow
[Warning: TwinStick] 1 printing butn Up, W
[Warning: TwinStick] 1 printing butn Down, S
[Warning: TwinStick] 2 printing butn Confirm, B
[Warning: TwinStick] 2 printing butn Confirm, A
[Warning: TwinStick] 2 printing butn Confirm, Return
[Warning: TwinStick] 2 printing butn Confirm, Space
[Warning: TwinStick] 3 printing butn Cancel, A
[Warning: TwinStick] 3 printing butn Cancel, B
[Warning: TwinStick] 3 printing butn Cancel, ESC
[Warning: TwinStick] 4 printing butn Interact, Y
[Warning: TwinStick] 4 printing butn Interact, X
[Warning: TwinStick] 4 printing butn Interact, F
[Warning: TwinStick] 5 printing butn Menu, + Button
[Warning: TwinStick] 5 printing butn Menu, Start
[Warning: TwinStick] 5 printing butn Menu, ESC
[Warning: TwinStick] 6 printing butn Map, Right Stick
[Warning: TwinStick] 6 printing butn Map, Right Stick Button
[Warning: TwinStick] 6 printing butn Map, C
[Warning: TwinStick] 7 printing butn Skill 1, Y
[Warning: TwinStick] 7 printing butn Skill 1, X
[Warning: TwinStick] 7 printing butn Skill 1, Left Mouse Button
[Warning: TwinStick] 8 printing butn Skill 2, B
[Warning: TwinStick] 8 printing butn Skill 2, A
[Warning: TwinStick] 8 printing butn Skill 2, Space
[Warning: TwinStick] 9 printing butn Skill 3, X
[Warning: TwinStick] 9 printing butn Skill 3, Y
[Warning: TwinStick] 9 printing butn Skill 3, Right Mouse Button
[Warning: TwinStick] 10 printing butn Skill 4, A
[Warning: TwinStick] 10 printing butn Skill 4, B
[Warning: TwinStick] 10 printing butn Skill 4, Left Shift
[Warning: TwinStick] 11 printing butn Skill 5, L
[Warning: TwinStick] 11 printing butn Skill 5, Left Shoulder
[Warning: TwinStick] 11 printing butn Skill 5, E
[Warning: TwinStick] 12 printing butn Skill 6, R
[Warning: TwinStick] 12 printing butn Skill 6, Right Shoulder
[Warning: TwinStick] 12 printing butn Skill 6, R
[Warning: TwinStick] 13 printing butn Debug, - Button
[Warning: TwinStick] 13 printing butn Debug, Back
[Warning: TwinStick] 13 printing butn Debug, Backspace
[Warning: TwinStick] 14 printing axis Equip Menu, Left Trigger
[Warning: TwinStick] 14 printing butn Equip Menu, ZL
[Warning: TwinStick] 14 printing butn Equip Menu, Tab

//element ids
[Warning: TwinStick] id 0, name Left Stick X
[Warning: TwinStick] id 1, name Left Stick Y
[Warning: TwinStick] id 2, name Right Stick X
[Warning: TwinStick] id 3, name Right Stick Y
[Warning: TwinStick] id 4, name Left Trigger
[Warning: TwinStick] id 5, name Right Trigger
[Warning: TwinStick] id 6, name A
[Warning: TwinStick] id 7, name B
[Warning: TwinStick] id 8, name X
[Warning: TwinStick] id 9, name Y
[Warning: TwinStick] id 10, name Left Shoulder
[Warning: TwinStick] id 11, name Right Shoulder
[Warning: TwinStick] id 12, name Back
[Warning: TwinStick] id 13, name Start
[Warning: TwinStick] id 22, name Guide
[Warning: TwinStick] id 14, name Left Stick Button
[Warning: TwinStick] id 15, name Right Stick Button
[Warning: TwinStick] id 16, name D-Pad Up
[Warning: TwinStick] id 17, name D-Pad Right
[Warning: TwinStick] id 18, name D-Pad Down
[Warning: TwinStick] id 19, name D-Pad Left
[Warning: TwinStick] id 20, name Left Stick
[Warning: TwinStick] id 21, name Right Stick
*/