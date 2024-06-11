
namespace GameBackend.Features.HoldAndSpin.Configuration
{
    public static class Constants
    {
        public const string PayloadNameFeature = "HoldAndSpin";

        public static readonly string[] BlankSymbols = { "BLANK", "PIC1", "PIC2", "PIC3", "PIC4" };

        public const int CountNeededToTrigger = 6;
        public const int CountNeededToRetrigger = 1;
        public const int FreeSpinsOnTrigger = 3;
        public const int FreeSpinsOnRetrigger = 3; //not added to total, sets value (should be >= FreeSpinsOnTrigger)

        public const int ReelStripNameIndexMax = 14;
        public const int ReelStripNameIndexMin = 6;
        public const string ReelStripNameFormat = "HoldAndSpinReelCell_{0}";
        public const string DefaultReelStripName = "HoldAndSpinReelCell_6";
    }
}
