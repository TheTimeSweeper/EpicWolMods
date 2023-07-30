using BepInEx;
using BepInEx.Configuration;
using System;

namespace CustomPalettes
{
    [BepInPlugin("TheTimeSweeper.CustomPalettes", "CustomPalettes", "1.0.0")]
    public class CustomPalettesPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Log.Init(Logger);
            Palettes.Init();
        }
    }
}
