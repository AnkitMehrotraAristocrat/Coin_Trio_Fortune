using Malee;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    [Preserve]
    [Serializable]
    public class DefaultSymbolData
    {
        public string GameState;
        public bool UseDefaultDuringSpin;
        public DummySymbolData DefaultData;
    }

    [Preserve]
    [Serializable]
    public class DefaultSymbolSettings : ReorderableArray<DefaultSymbolData> { }

    [Preserve]
    [Serializable]
    public class SymbolSkinMap
    {
        public string Type;
        public int SkinIndex;
    }

    [Preserve]
    [Serializable]
    public class SymbolSkinMaps : ReorderableArray<SymbolSkinMap> { }

    [Serializable]
    public class SymbolSkinConfigs : ReorderableArray<SymbolSkinConfig> { }

    [Preserve]
    [CreateAssetMenu(fileName = "SymbolSkinConfig", menuName = "NMG/Symbol Skin Config")]
    public class SymbolSkinConfig : ScriptableObject
    {
        [SerializeField] private int _symbolId;
        [Header("Default Settings")]
        [SerializeField] [Reorderable] private DefaultSymbolSettings _defaultSymbolData;
        //[SerializeField] private bool _useDefaultDuringSpin;
        //[SerializeField] private SymbolSkinData _defaultSymbolData;
        [Header("Skin Maps")]
        [SerializeField] [Reorderable] private SymbolSkinMaps _skinMaps;

        public int SymbolId => _symbolId;
        public SymbolSkinMaps SkinMaps => _skinMaps;
        public DefaultSymbolSettings DefaultSymbolData => _defaultSymbolData;
        //public SymbolSkinData DefaultSymbolData => _defaultSymbolData;
        //public bool UseDefaultDuringSpin => _useDefaultDuringSpin;
    }
}
