using System.Collections.Generic;

namespace GameBackend.Features.SymbolSkins.Data
{
    public class SpinningIdData
    {
        public int SymbolId { get; set; }
        public List<SpinningGameStateData> Data { get; set; }
    }

    public class SpinningGameStateData
    {
        public List<string> GameStates { get; set; }
        public List<SkinData> Data { get; set; }
    }

    public class SkinData
    {
        public int SymbolId { get; set; }
        public string Skin { get; set; }
        public string TextValue { get; set; }
        public bool ShouldMultiply { get; set; }
    }
}
