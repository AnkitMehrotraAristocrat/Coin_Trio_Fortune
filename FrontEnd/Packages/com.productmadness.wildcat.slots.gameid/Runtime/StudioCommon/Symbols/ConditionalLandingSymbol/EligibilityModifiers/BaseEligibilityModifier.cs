using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    public abstract class BaseEligibilityModifier : ScriptableObject
    {
        public abstract void Initialize(ServiceLocator serviceLocator);
        public abstract bool IsEligible();
    }
}
