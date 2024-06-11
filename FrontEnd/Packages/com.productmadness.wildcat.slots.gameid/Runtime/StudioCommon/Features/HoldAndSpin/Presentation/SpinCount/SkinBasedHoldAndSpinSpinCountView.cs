using Slotsburg.Slots.SharedFeatures;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    /// <summary>
    /// Automatically sets the spin count on the spin count meter for hold and spin.
    /// This meter uses skins (for some reason) instead of an actual text box.
    /// </summary>
    public class SkinBasedHoldAndSpinSpinCountView : HoldAndSpinSpinCountView
    {
        [SerializeField] private SpineAnimationController _spineController;

        protected int currentSkinIndex = 0;

        protected override void UpdateText(int count)
        {
            // skin is offset by 1
            currentSkinIndex = count + 1;

            // Play a little animation if we reset the spin counter.
            if (count > _currentCount)
            {
                PlayResetAnim();
            }
            else
            {                
                UpdateCurrentSkin();
            }

            _currentCount = count;
        }

        public void UpdateCurrentSkin()
        {
            _spineController._skinIndex = currentSkinIndex;
        }
    }
}