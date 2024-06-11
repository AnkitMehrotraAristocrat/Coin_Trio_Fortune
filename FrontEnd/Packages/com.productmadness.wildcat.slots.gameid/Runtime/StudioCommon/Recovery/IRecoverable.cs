using System.Collections.Generic;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Interface for anything that needs to run a method on recovery.
    /// Meant to be paired with a RecoveryPresenter to run the OnRecovery method.
    /// </summary>
    public interface IRecoverable
    {
        /// <summary>
        /// Valid game states that allow recovery.
        /// </summary>
        List<GameStateEnum> RecoveryEligibleGameStates { get; set; }

        /// <summary>
        /// Method to be run when the game is recovering.
        /// </summary>
        void OnRecovery();
    }
}
