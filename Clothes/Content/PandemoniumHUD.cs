using System.Collections.Generic;
using UnityEngine;
using OutfitModType = OutfitModStat.OutfitModType;
using UnityEngine.UI;
using System;

namespace Clothes
{
    public class PandemoniumHUD : MonoBehaviour
    {
        [SerializeField]
        public Animator _animator;
        [SerializeField]
        private Transform _textGrid;
        [SerializeField]
        private List<Text> _statTexts;

        public void DisplayStats(List<OutfitModStat> modStats)
        {
            for (int i = 0; i < modStats.Count; i++)
            {
                if(_statTexts.Count <= i)
                {
                    _statTexts.Add(Instantiate(_statTexts[0], _textGrid));
                }

                Text statText = _statTexts[i];
                statText.gameObject.SetActive(true);

                statText.text = GetModTypeText(modStats[i].modType);
               
                float value = GetModValue(modStats[i], out bool positive);
                string sign = value > 0 ? "+" : "";
                statText.text += $" {sign}{value * 100}%";
                statText.color = positive ? Color.green : Color.red;
            }

            for (int i = modStats.Count; i < _statTexts.Count; i++)
            {
                _statTexts[i].gameObject.SetActive(false);
            }

            _animator.Play("PingStats");
        }

        private float GetModValue(OutfitModStat outfitModStat, out bool positive)
        {
            float value = 0;
            if (outfitModStat.hasAddValue) 
            {
                value = outfitModStat.addModifier.modValue;
            }
            if (outfitModStat.hasMultiValue)
            {
                value = outfitModStat.multiModifier.modValue;
            }

            if(outfitModStat.modType == OutfitModType.Cooldown)
            {
                positive = value <= 0;
            } else
            {
                positive = value >= 0;
            }

            return value;
        }

        private string GetModTypeText(OutfitModType outfitModType)
        {
            switch (outfitModType)
            {
                case OutfitModType.Health:
                case OutfitModType.Speed:
                case OutfitModType.Evade:
                case OutfitModType.Armor:
                case OutfitModType.Damage:
                case OutfitModType.Cooldown:
                case OutfitModType.Gold:
                    return outfitModType.ToString();
                case OutfitModType.CritChance:
                    return "Crit Chance";
                case OutfitModType.CritDamage:
                    return "Crit Damage";
                case OutfitModType.ODRate:
                    return "Sig Charge";
                case OutfitModType.ODDamage:
                    return "Sig Damage";
                case OutfitModType.HealAmount:
                    return "Heal Amount";
                default:
                    return "Stat Not Found";
            }
        }
        
        public void Init(Font font)
        {
            _statTexts[0].font = font;
        }
    }
}