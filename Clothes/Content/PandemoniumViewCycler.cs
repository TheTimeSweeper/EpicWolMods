using UnityEngine;
using UnityEngine.UI;

namespace Clothes
{
    public class PandemoniumViewCycler: MonoBehaviour
    {
        private Material _spriteMaterial;
        private float _rainbowTim;
        private int _palette;
        private bool _rainbow;

        void Awake()
        {
            _spriteMaterial = GetComponent<Image>().material;
        }

        void Update()
        {
            RouletteColors();
        }

        private void RouletteColors()
        {
            if (!_rainbow)
                return;

            _rainbowTim -= Time.deltaTime;
            if (_rainbowTim > 0)
                return;
            _rainbowTim = PandemoniumMod.RAINBOW_INTERVAL;

            _palette++;
            while (_palette == 25 || _palette == 26)
            {
                _palette++;
            }

            if (_palette > PandemoniumCloak.RandomEndIndex || _palette < PandemoniumCloak.RandomStartIndex)
            {
                _palette = PandemoniumCloak.RandomStartIndex;
            }
            _spriteMaterial.SetFloat("_Palette", _palette);
        }

        public void SetRainbow(bool setStatus)
        {
            _rainbow = setStatus;
        }
    }
}