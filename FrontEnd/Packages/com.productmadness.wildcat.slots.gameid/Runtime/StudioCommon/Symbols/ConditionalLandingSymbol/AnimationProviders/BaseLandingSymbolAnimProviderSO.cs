using System.Collections.Generic;
using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    [Preserve]
    public abstract class BaseLandingSymbolAnimProviderSO : ScriptableObject
    {
        [SerializeField] protected string _landingAnimTrigger;

        public abstract BaseLandingSymbolAnimProvider GetAnimProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers);

        protected List<SymbolId> GetSymbols(List<int> symbolIds)
        {
            var symbols = new List<SymbolId>();

            foreach (var id in symbolIds)
            {
                symbols.Add(new SymbolId(id));
            }

            return symbols;
        }
    }
}
