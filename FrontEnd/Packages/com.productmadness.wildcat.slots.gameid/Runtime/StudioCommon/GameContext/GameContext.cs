namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Which context is the game currently in?
    /// </summary>
    public enum GameContext
    {
        Recovery, // The game is in a recovery context and not in a normal gameplay flow.
        Normal // The game if flowing normally.
    }
}
