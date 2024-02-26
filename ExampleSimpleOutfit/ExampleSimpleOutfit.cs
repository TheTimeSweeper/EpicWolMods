using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleSimpleOutfit {

    [BepInDependency("TheTimeSweeper.CustomPalettes", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("TheTimeSweeper.ExampleSimpleOutfit", "ExampleSimpleOutfit", "0.1.0")]
    public class ExampleSimpleOutfit : BaseUnityPlugin {
        void Awake() {

            int newColorIndex = CustomPalettes.Palettes.AddPalette(System.IO.Path.GetDirectoryName(Info.Location), "example.png");

            OutfitInfo coolOutfit = new OutfitInfo()
            {
                name = "Cool Guy Colored",
                outfit = new Outfit(
                    "ModName::ExampleOutfitID", //newID
                    newColorIndex, //newColorIndex
                    new List<OutfitModStat>
                    {
                        new OutfitModStat(OutfitModStat.OutfitModType.Health,
                              100, //addValue
                              0, //multiValue
                              0, //override value
                              false), //bool value
                        // more OutfitModStats here
                    },
                    false, //isUnlocked
                    false) //isLeveling
            };
            LegendAPI.Outfits.Register(coolOutfit);
        }
    }
}
