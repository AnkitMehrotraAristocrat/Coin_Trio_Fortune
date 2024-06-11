
namespace GameBackend.Features.Cors.Configuration
{
    public static class Constants
    {
        public const string PrizesWeightTable = "CorPrizes";
        public static readonly string[] CorSymbols = { "SCAT" };

        public const string ScatSelection = "ScatSelection";

        // Set exclusions for symbol skinning...
        // Examples:
        // new() { Window = "FreeSpin" } >> Exclude entire FreeSpin window
        // new() { Window = "HoldAndSpin", Skin = "Grand" } >> Exclude Grand in HoldAndSpin window
        // new() { Skin = "Grand" } >> Exclude Grand in all windows
        public static readonly SymbolSkinExclusions[] SkinExclusions = {
            new() { Window = "FreeSpin" }
        };

        // Set exclusions for symbol skinning supported states...
        // Examples:
        // new() { State = "FreeSpin" } >> Exclude FreeSpin state from all symbols
        // new() { Symbol = "Cor", State = "FreeSpin" } >> Exclude FreeSpin state from Cor symbol
        public static readonly SymbolStateExclusions[] StateExclusions = {
            new() { State = "FreeSpin" }
        };
    }

    public class SymbolSkinExclusions
    {
        public string? Window { get; set; }
        public string? Symbol { get; set; }
        public string? Skin { get; set; }
    }

    public class SymbolStateExclusions
    {
        public string? Symbol { get; set; }
        public string? State { get; set; }
    }
}
