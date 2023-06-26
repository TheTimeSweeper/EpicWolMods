using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Clothes
{
    public class StatusBarMod : MonoBehaviour
    {
        public PlayerStatusBar self;
        public void Update()
        {
            if (self.playerPortrait != null && ContentLoaderStolen.newPalette != null)
            {
                Material material = UnityEngine.Object.Instantiate<Material>(self.playerPortrait.material);
                material.SetFloat("_PaletteCount", 32 + ContentLoaderStolen.palettes.Count/*Clothes.Palettes.Count*/);
                material.SetTexture("_Palette", ContentLoaderStolen.newPalette);
                self.playerPortrait.material = material;
                Destroy(this);
            }
        }
    }
}
