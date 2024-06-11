using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
    [Serializable]
    [CreateAssetMenu(fileName = "Reel Window Configuration", menuName = "NMG/Wizard/Reel Window Configuration")]
    public class ReelWindowConfiguration : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

        [SerializeField] private GameObject _reelViewPrefab;
        public GameObject ReelViewPrefab => _reelViewPrefab;

        [SerializeField] private GameObject _symbolsPrefab;
        public GameObject SymbolsPrefab => _symbolsPrefab;

        [SerializeField] private GameObject _symbolPrefab;
        public GameObject SymbolPrefab => _symbolPrefab;

        [SerializeField] private GameObject _winLinesPrefab;
        public GameObject WinLinesPrefab => _winLinesPrefab;

        [SerializeField] private GameObject _winLineSymbolPrefab;
        public GameObject WinLineSymbolPrefab => _winLineSymbolPrefab;

        [SerializeField] private Material _reelMaskMaterial;
        public Material ReelMaskMaterial => _reelMaskMaterial;
    }
}
