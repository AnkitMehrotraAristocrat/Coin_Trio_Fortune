using System;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    [Preserve]
    [Serializable]
    public class SymbolData
    {
        public int SymbolId;
        public string Skin;
        public string TextValue;
        public bool ShouldMultiply;
    }

    [Preserve]
    [Serializable]
    public class DummySymbolData
    {
        public string Skin;
        public string TextValue;
        public bool ShouldMultiply;
    }
}
