using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Clothes
{
    public class PandemoniumHUD : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Transform _textGrid;
        [SerializeField]
        private List<Text> _statTexts;
    }
}
