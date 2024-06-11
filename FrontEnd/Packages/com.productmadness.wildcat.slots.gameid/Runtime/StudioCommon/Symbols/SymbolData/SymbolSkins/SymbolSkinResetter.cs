using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Slotsburg.Slots.SharedFeatures;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    /// <summary>
    /// Meant to be attached to the Reels and make sure we're resetting symbol skins back to their defaults when they despawn.
    /// </summary>
    public class SymbolSkinResetter : BaseSymbolSpawnHandler
    {
        protected override void OnSymbolSpawn(SpawnedSymbolData spawnedSymbolData)
        {
            // Nothing to do here, this is handled in other scripts...
        }

        protected override void OnSymbolDespawn(SpawnedSymbolData symbolData)
        {
            var symbolHandle = symbolData.SymbolHandle;
            var spineAnimationControllers = symbolHandle.GetComponentsInChildren<SpineAnimationController>();
            
            // Sets the skin index back to 0.
            //spineAnimationController._skinIndex = 0;
            foreach (var controller in spineAnimationControllers)
            {
                controller._skinIndex = 0;
            }
        }
    }
}
