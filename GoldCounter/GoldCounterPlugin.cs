using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoldCounter {
    [BepInPlugin("TheTimeSweeper.GoldCounter", "GoldCounter", "1.0.0")]
    public class GoldCounterPlugin : BaseUnityPlugin {

        public static GoldCountupper staticGoldCountupper;
        public static PlatCountupper staticPlatCountupper;

        void Awake() {

            On.GameUI.Awake += GameUI_Awake;
            On.GameUI.Update += GameUI_Update;
        }

        private void GameUI_Awake(On.GameUI.orig_Awake orig, GameUI self)
        {
            orig(self);
            staticGoldCountupper = self.gameObject.AddComponent<GoldCountupper>();
            staticPlatCountupper = self.gameObject.AddComponent<PlatCountupper>();
        }
        
        private void GameUI_Update(On.GameUI.orig_Update orig, GameUI self)
        {
            orig(self);
            if (GameUI.moneyStatus && Player.platWallet != null && Player.goldWallet != null)
            {
                if (staticGoldCountupper)
                {
                    GameUI.goldText.text = staticGoldCountupper.GetAmountText();
                }
                if (staticPlatCountupper)
                {
                    GameUI.platinumText.text = staticPlatCountupper.GetAmountText();
                }
            }
        }
    }

    public class GoldCountupper : Countupper 
    {
        protected override Countupper staticCountupper { 
            get => GoldCounterPlugin.staticGoldCountupper; 
            set => GoldCounterPlugin.staticGoldCountupper = (GoldCountupper)value; 
        }

        protected override Wallet wallet { get => Player.goldWallet; }
    }

    public class PlatCountupper : Countupper
    {
        protected override Countupper staticCountupper
        {
            get => GoldCounterPlugin.staticPlatCountupper;
            set => GoldCounterPlugin.staticPlatCountupper = (PlatCountupper)value;
        }

        protected override Wallet wallet { get => Player.platWallet; }
    }

    public abstract class Countupper : MonoBehaviour
    {
        const float TIMER_DELAY = 1.2f;
        const float COUNTING_TIME = 0.4f;

        protected abstract Countupper staticCountupper { get; set; }
        protected abstract Wallet wallet { get; }

        private int _totalTempAmount;
        private int _currentTempAmount;
        private int _lastAmount;

        private float _tim = 10;

        public string GetAmountText()
        {
            string tempAmountString = _currentTempAmount > 0 ? $" <color=yellow>+{_currentTempAmount}</color>" : "";
            return $"{GetCurrentCurrency() - _currentTempAmount}{tempAmountString}";
        }

        protected virtual int GetCurrentCurrency()
        {
            return wallet != null ? wallet.balance : int.MaxValue;
        }

        void Update()
        {
            DetectGoldChange();

            _tim += Time.deltaTime;

            if (_tim > TIMER_DELAY + COUNTING_TIME + 1)
                return;

            if (_tim > TIMER_DELAY)
            {
                _currentTempAmount = (int)UnityEngine.Mathf.Lerp(_totalTempAmount, 0, Mathf.InverseLerp(TIMER_DELAY, TIMER_DELAY + COUNTING_TIME, _tim));
            }
        }

        private void DetectGoldChange()
        {
            int currentAmount = GetCurrentCurrency();
            if (currentAmount > _lastAmount)
            {
                if (_tim > TIMER_DELAY)
                {
                    _currentTempAmount = 0;
                }

                _currentTempAmount += currentAmount - _lastAmount;
                _totalTempAmount = _currentTempAmount;
                _tim = 0;
            }

            _lastAmount = currentAmount;
        }

        void OnDestroy()
        {
            if (staticCountupper == this)
            {
                staticCountupper = null;
            }
        }
    }
}
