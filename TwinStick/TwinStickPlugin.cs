using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;
using System.Collections.ObjectModel;

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
        private static bool _addedActions;

        void Awake()
        {
            Log.Init(Logger);
            On.InputController.AssignControllerToPlayer += InputController_AssignControllerToPlayer;
            On.ChaosInputDevice.GetAimVector += ChaosInputDevice_GetAimVector;
        }

        private void InputController_AssignControllerToPlayer(On.InputController.orig_AssignControllerToPlayer orig, Rewired.Player player, Controller controller, bool removeFromOtherPlayers, bool ignoreKBMouseCheck)
        {
            orig(player, controller, removeFromOtherPlayers, ignoreKBMouseCheck);
            Log.Message("assign to " + player.name);
            GrabJoystickMaps();
            //printAllJoystickMaps();
        }

        private static void GrabJoystickMaps()
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
                        Log.Warning("adding new joystick " + map.name);
                        joystickMaps.Add(map);
                        PrintJoystickMap(map);
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
            if (!_addedActions)
            {
                AddToActions();
            }

            //the key to victory. thank you
                //tells me what the element ids are of the physical inputs
            //for (int i = 0; i < map.controller.ElementIdentifiers.Count; i++)
            //{
            //    var item = map.controller.ElementIdentifiers[i];
            //    Log.Warning($"id {item.id}, name {item.name}");
            //}            

            //map.CreateElementMap(actionHori.id, Pole.Positive, 2, ControllerElementType.Axis, AxisRange.Full, false);
            //map.CreateElementMap(actionVeri.id, Pole.Positive, 3, ControllerElementType.Axis, AxisRange.Full, false);

            map.CreateElementMap(new ElementAssignment(2,
                                                       actionHori.id,
                                                       false));

            map.CreateElementMap(new ElementAssignment(3,
                                                       actionVeri.id,
                                                       false));

            Log.Warning("created element maps");
            PrintJoystickMap(map);
        }

        private static void AddToActions()
        {
            _addedActions = true;

            Log.Warning("adding actions");

            //jIqeCvHHKwvAbZSxWMpGqMRrbVY inputActionMapOrSomething = ReInput.EqUBFyqgFrEprcmzfEsQfMRcSahQ;
            //List<InputAction> newActions = new List<InputAction>(inputActionMapOrSomething.eGEdOfZqjUCXxPJfHFlonVkbGYMe);
            //newActions.Add(actionHori);
            //newActions.Add(actionVeri);

            //inputActionMapOrSomething.eGEdOfZqjUCXxPJfHFlonVkbGYMe = newActions.ToArray();
            //inputActionMapOrSomething.VwYoFyUXRIkHTzfqpHHjNGdLEME = new ReadOnlyCollection<InputAction>(newActions);

            List<InputAction> newActions = new List<InputAction>(ReInput.mapping.Actions);
            newActions.Add(actionHori);
            newActions.Add(actionVeri);

            ReInput.EqUBFyqgFrEprcmzfEsQfMRcSahQ = new jIqeCvHHKwvAbZSxWMpGqMRrbVY(newActions);


            Rewired.Data.UserData userData = ReInput.auGwiZGGibyrKvBDxGgbhehskLDE;

            userData.actions.Add(actionHori);
            userData.actionCategoryMap.AddAction(0, actionHori.id);
            userData.actions.Add(actionVeri);
            userData.actionCategoryMap.AddAction(0, actionVeri.id);

            userData.actionIdCounter += 2;

            //for (int i = 0; i < ReInput.mapping.Actions.Count; i++)
            //{
            //    var actin = ReInput.mapping.Actions[i];

            //    PrintAction(actin);
            //}

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

        private static void PrintAllJoystickMaps()
        {
            for (int i = 0; i < joystickMaps.Count; i++)
            {
                ControllerMap map = joystickMaps[i];
                PrintJoystickMap(map);
            }
        }

        private static void PrintJoystickMap(ControllerMap map)
        {
            for (int i = 0; i < map.AllMaps.Count; i++)
            {
                var item = map.AllMaps[i];

                Log.Message($"printing map id: {item.actionId}, action: {item.actionDescriptiveName}, element: {item.elementIdentifierName}");
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

        private Vector2 ChaosInputDevice_GetAimVector(On.ChaosInputDevice.orig_GetAimVector orig, ChaosInputDevice self)
        {
            //GameUI.BroadcastNoticeMessage($"{self.rewiredPlayer.GetAxis("LookHorizontal")}, {self.rewiredPlayer.GetAxis("LookVertical")}", 0f);
            return self.rewiredPlayer.GetAxis2D("LookHorizontal", "LookVertical").normalized;

            return orig(self);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {

                Log.Message("update");
                GrabJoystickMaps();
                //PrintAllJoystickMaps();
            }
        }
    }
}

/*
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
*/