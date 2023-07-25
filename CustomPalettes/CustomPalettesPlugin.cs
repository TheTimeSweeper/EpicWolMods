﻿using BepInEx;
using BepInEx.Configuration;
using System;

namespace CustomPalettes
{
    [BepInPlugin("TheTimeSweeper.CustomPalettes", "CustomPalettes", "0.1.0")]
    public class CustomPalettesPlugin : BaseUnityPlugin {
        void Awake() {
            Log.Init(Logger);
            Palettes.Init();
        }
        void Start()
        {
            //Palettes.Start();
        }
    }
}
