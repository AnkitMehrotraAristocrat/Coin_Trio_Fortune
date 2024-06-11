namespace PixelUnited.NMG.Slots.Milan.GAMEID 
{
    /// <summary>
    /// Data structure to hold Free Spins information, sent every free spin.
    /// </summary>
    public class FreeSpinPayloadData
    {
        public int FreeSpinsRemaining { get; set; }
        public int FreeSpinsWon { get; set; }
        public ulong BetAmount { get; set; }
        public bool IsActive { get; set; }

        public FreeSpinPayloadData()
        {
            FreeSpinsRemaining = 0;
            FreeSpinsWon = 0;
            BetAmount = 0;
            IsActive = false;
        }
	}
}
