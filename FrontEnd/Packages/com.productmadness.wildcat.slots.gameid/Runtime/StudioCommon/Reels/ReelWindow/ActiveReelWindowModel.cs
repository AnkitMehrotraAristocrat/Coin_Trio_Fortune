using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ActiveReelWindow
{
    /// <summary>
    /// Model that maintains the active reel window on a per game state basis.
    /// </summary>
    public class ActiveReelWindowModel : IModel
    {
        private Dictionary<string, string> _reelWindowPerGameState = new Dictionary<string, string>();

        public ActiveReelWindowModel(ServiceLocator serviceLocator)
        {

        }

        public void SetReelWindow(string gameState, string reelWindowName)
		{
            _reelWindowPerGameState[gameState] = reelWindowName;
		}

        public bool GetReelWindow(string gameState, out string reelWindow)
		{
            if (!_reelWindowPerGameState.ContainsKey(gameState))
			{
                reelWindow = null;
                return false;
			}
            reelWindow = _reelWindowPerGameState[gameState];
            return true;
		}
    }
}

