using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using OutfitModType = OutfitModStat.OutfitModType;
using System;
using UnityEngine.UI;

namespace Clothes
{
    public class PandemoniumMod : CustomOutfitMod
    {
        #region constant values

        private static Dictionary<OutfitModType, float> statModScoreMultipliers = new Dictionary<OutfitModType, float>
        {
            //health causing some funny edge cases
                //changing health randomly (unless it heals you I suppose) doesn't make sense anyways
                //maybe revisit
            //{ OutfitModType.Health, 0.2f },
            { OutfitModType.Speed, 0.3f },
            { OutfitModType.Evade, 0.2f },
            { OutfitModType.Armor, 0.2f },
            { OutfitModType.CritChance, 0.2f },
            { OutfitModType.CritDamage, 0.5f },
            { OutfitModType.Damage, 0.2f },
            { OutfitModType.Cooldown, -0.2f },
            { OutfitModType.ODRate, 0.4f },
            { OutfitModType.ODDamage, 0.5f },
            { OutfitModType.HealAmount, 0.4f },
            { OutfitModType.Gold, 0.15f },
        };

        private static List<OutfitModType> statModTypeList = new List<OutfitModType>(statModScoreMultipliers.Keys);

        public const float RAINBOW_INTERVAL = 0.09f;
        private const float RANDOMIZE_INTERVAL = 3f;
        private const float START_RAINBOW_INTERVAL = 1.2f;
        #endregion constant values

        private PandemoniumHUD _pandemoniumHUD;

        private NumVarStatMod _paletteMod;

        private OutfitModStat _customModStat;
        private List<OutfitModType> _randomModTypes;

        private int startIndex => (!ClothesPlugin.tournamentEditionInstalled && player.outfitID == "Chaos") ? PandemoniumCloak.ShadowStartIndex : PandemoniumCloak.RandomStartIndex;

        private int _colorIndex = PandemoniumCloak.RandomStartIndex;
        private float _rainbowTim;
        private float _randomizeTim;

        private delegate void StatsUpdatedEvent(List<OutfitModStat> modStats);
        private event StatsUpdatedEvent OnStatsUpdated;
        private bool _outfitEquipped;

        public override void OnEquip(Player player, bool equipStatus)
        {
            _randomizeTim = -1;
            _rainbowTim = -1;
            _colorIndex = Random.Range(PandemoniumCloak.RandomStartIndex, PandemoniumCloak.RandomEndIndex);
            if (_paletteMod == null) { 
                _paletteMod = new NumVarStatMod(ID, _colorIndex, 10, VarStatModType.Override, false); 
            }
            _pandemoniumHUD = player.hud.GetComponentInChildren<PandemoniumHUD>();
            if(!_pandemoniumHUD)
            {
                InitializeHud();
            }

            OnSetOutfit(true);

            if (equipStatus == false)
            {
                player.equippedOutfit.modList = new List<OutfitModStat>
                {
                    _customModStat,
                };
                player.SetPlayerOutfitColor(_paletteMod, false);
                OnSetOutfit(false);
            }
        }

        public override void OnPlayerDestroyed()
        {
            OnSetOutfit(false);
        }


        private void OnSetOutfit(bool setStatus)
        {

            if (setStatus == _outfitEquipped)
                return;
            _outfitEquipped = setStatus;
            if (setStatus)
            {
                OnStatsUpdated += _pandemoniumHUD.DisplayStats;
            }
            else
            {
                OnStatsUpdated -= _pandemoniumHUD.DisplayStats;
            }
        }

        private void InitializeHud()
        {
            _pandemoniumHUD = UnityEngine.Object.Instantiate(Assets.pandemoniumHUDPrefab, player.hud.transform, false).GetComponent<PandemoniumHUD>();

            _pandemoniumHUD.Init(player.hud.transform.Find("PlayerDeathMenu/RespawnText").GetComponent<Text>().font);
        }

        public override void Update()
        {
            _randomizeTim -= Time.deltaTime;
            if (_randomizeTim > START_RAINBOW_INTERVAL)
                return;
            if (_randomizeTim < 0)
            {
                SwapStats(false);
                _randomizeTim = RANDOMIZE_INTERVAL;
            }
            RouletteColors();
        }
     
        #region stats
        private void SwapStats(bool allowUpdate)
        {
            _customModStat = player.equippedOutfit.modList.Find(stat => stat.modType == LegendAPI.Outfits.CustomModType);

            player.equippedOutfit.modList.Remove(_customModStat);
            BootlegSetMods(false, allowUpdate);
            //player.equippedOutfit.SetMods(false, allowUpdate);
            player.equippedOutfit.modList = new List<OutfitModStat>
            {
                _customModStat,
            };
            List<OutfitModStat> randomStats = GenerateRandomStats();
            player.equippedOutfit.modList.AddRange(randomStats);
            //player.equippedOutfit.SetMods(true, allowUpdate);
            BootlegSetMods(true, allowUpdate);

            OnStatsUpdated?.Invoke(randomStats);

            RefreshHUDInfo();
        }

        private void RefreshHUDInfo()
        {
            if (player.lowerHUD.outfitMenu.hasFocus)
            {
                player.lowerHUD.outfitMenu.SetFocus(true);
            }
        }

        private void BootlegSetMods(bool setStatus, bool allowUpdate)
        {
            for (int i = 0; i < player.equippedOutfit.modList.Count; i++)
            {
                OutfitModStat modStat = player.equippedOutfit.modList[i];
                if (modStat == _customModStat)
                    continue;
                modStat.SetModStatus(player, setStatus, allowUpdate);
            }
        }

        private List<OutfitModStat> GenerateRandomStats()
        {
            List<OutfitModStat> randomStats = new List<OutfitModStat>();

            List<float> scores = getRandomScores();

            _randomModTypes = new List<OutfitModType>(statModTypeList);
            for (int i = 0; i < scores.Count; i++)
            {
                randomStats.Add(GetRandomOutfitStat(scores[i]));
            }

            return randomStats;
        }

        private List<float> getRandomScores()
        {
            int statFieldsAmount = Random.Range(1, 4);

            List<float> scores = new List<float>();
            for (int i = 0; i < statFieldsAmount; i++)
            {
                scores.Add(Random.value-0.2f);
            }
            bool noPositive = true;
            for (int i = 0; i < scores.Count; i++)
            {
                if(scores[i] > 0)
                {
                    scores[i] = Mathf.Max(scores[i], 0.5f);
                    noPositive = false;
                }
                else
                {
                    scores[i] = Mathf.Min(scores[i], -0.1f);
                }
            } 
            if (noPositive)
            {
                scores[0] = 0.5f;
            }

            float sum = scores.Sum();
            for (int i = 0; i < scores.Count; i++)
            {
                scores[i] /= sum;
            }

            return scores;
        }

        private OutfitModStat GetRandomOutfitStat(float score)
        {
            OutfitModType randomModType = _randomModTypes[Random.Range(0, _randomModTypes.Count)];
            _randomModTypes.Remove(randomModType);

            float value = score * statModScoreMultipliers[randomModType];
            value = Mathf.RoundToInt(value * 100) / 100.00f;

            if (TailorNpc.playerBuffed[player.playerID])
            {
                TryUpgradeStat(randomModType, ref value);
            }

            switch (randomModType)
            {
                default:
                case OutfitModType.Health:
                case OutfitModType.Cooldown:
                case OutfitModType.HealAmount:
                case OutfitModType.Gold:
                case OutfitModType.Speed:
                case OutfitModType.ODRate:
                case OutfitModType.ODDamage:
                case OutfitModType.Damage:
                    return new OutfitModStat(randomModType, 0, value, 0, false);
                case OutfitModType.Armor:
                case OutfitModType.Evade:
                case OutfitModType.CritChance:
                case OutfitModType.CritDamage:
                    return new OutfitModStat(randomModType, value, 0, 0, false);
            }
        }

        private void TryUpgradeStat(OutfitModType randomModType, ref float value)
        {
            switch (randomModType)
            {
                default:
                    if(value > 0)
                    {
                        value *= TailorNpc.upgradeValue;
                    }
                    return;
                case OutfitModType.Cooldown:
                    if (value < 0)
                    {
                        value *= TailorNpc.upgradeValue;
                    }
                    break;
            }

            value *= TailorNpc.upgradeValue;
        }
        #endregion stats

        private void RouletteColors()
        {
            _rainbowTim -= Time.deltaTime;
            if (_rainbowTim > 0)
                return;
            _rainbowTim = RAINBOW_INTERVAL;

            _paletteMod.modValue++;
            while(_paletteMod.modValue == 25 || _paletteMod.modValue == 26)
            {
                _paletteMod.modValue++;
            }


            //if (player.outfitID != "Chaos")
            //{
                if (_paletteMod.modValue > PandemoniumCloak.RandomEndIndex || _paletteMod.modValue < PandemoniumCloak.RandomStartIndex)
                {
                    _paletteMod.modValue = PandemoniumCloak.RandomStartIndex;
                }
            //} 
            //else
            //{
            //    if (_paletteMod.modValue > PandemoniumCloak.ShadowEndIndex || _paletteMod.modValue < PandemoniumCloak.ShadowStartIndex)
            //    {
            //        _paletteMod.modValue = PandemoniumCloak.ShadowStartIndex;
            //    }
            //}

            player.SetPlayerOutfitColor(_paletteMod, true);
        }
    }
}