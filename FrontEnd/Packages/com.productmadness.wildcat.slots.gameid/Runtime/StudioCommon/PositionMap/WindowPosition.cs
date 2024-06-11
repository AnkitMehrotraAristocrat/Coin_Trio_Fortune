using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps
{
    public class WindowPosition
    {
        public int ReelIndex;
        public int SymbolIndex;

        public WindowPosition(int reelIndex, int symbolIndex)
        {
            ReelIndex = reelIndex;
            SymbolIndex = symbolIndex;
        }

        public class EqualityComparer : IEqualityComparer<WindowPosition>
        {
            bool IEqualityComparer<WindowPosition>.Equals(WindowPosition mapA, WindowPosition mapB)
            {
                return mapA.ReelIndex == mapB.ReelIndex && mapA.SymbolIndex == mapB.SymbolIndex;
            }

            int IEqualityComparer<WindowPosition>.GetHashCode(WindowPosition obj)
            {
                return obj.ReelIndex ^ obj.SymbolIndex;
            }
        }
    }
}
