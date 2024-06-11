using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
    /// <summary>
    /// Driver that supports DynamicSwapsPerReel payload message handling.
    /// </summary>
    public class DynamicSymbolModelDriver : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresGlobal] private ServiceLocator _serviceLocator = default;
        [FieldRequiresParent] private MainDriver _mainDriver = default;

        [SerializeField] string _slotConfigTag = "standard";

        private DynamicSymbolClientModelPresenter _clientModelPresenter = default;
        private SymbolMapConfig _slotConfig;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();

            _mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("DynamicSwapsPerReel", SetDynamicSymbols));

            _slotConfig = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>().Get<SymbolMapConfig>(_slotConfigTag);
            _clientModelPresenter = transform.root.GetComponentInChildren<DynamicSymbolClientModelPresenter>();
        }

        private async Task SetDynamicSymbols(string json, MainDriver.IPayloadWriter payloadWriter)
        {
            DynamicSymbolData dynamicSymbolSwaps = await JsonUtils.DeserializeObjectAsync<DynamicSymbolData>(json);
            payloadWriter.Set(dynamicSymbolSwaps);

            // This deserialization is an exception to the "don't update the model in a deserialization
            // method" rule. The reason is that reels outcome modification requires this data but it doesn't
            // make sense to perform all of the lookups twice (once to update the model and again to modify
            // the outcome). As this update doesn't have any ancillary effects (like ending the spin early)
            // this should be left as is.

            if (dynamicSymbolSwaps == null)
            {
                dynamicSymbolSwaps = new DynamicSymbolData();
            }

            DynamicSymbolServerModel serverModel = _serviceLocator.GetOrCreate<DynamicSymbolServerModel>(dynamicSymbolSwaps.Id);
            _clientModelPresenter.AddServerModelSubscription(dynamicSymbolSwaps.Id, serverModel);
            ReplacementsDict dynamicSymbolReplacements = GetDynamicSymbolReplacements(dynamicSymbolSwaps.DynamicSymbolPairs);
            serverModel.SetDynamicSymbolReplacements(dynamicSymbolReplacements);
        }

        public ReplacementsDict GetDynamicSymbolReplacements(DynamicSymbolPair[] dynamicSymbolSwaps)
        {
            ReplacementsDict dynamicSymbolReplacements = new ReplacementsDict();
            if (dynamicSymbolSwaps == null)
            {
                return dynamicSymbolReplacements;
            }

            foreach (DynamicSymbolPair dynamicSymbolSwapEntry in dynamicSymbolSwaps)
            {
                if (dynamicSymbolReplacements.ContainsKey(dynamicSymbolSwapEntry.ReelIndex))
                {
                    AddToExistingReelEntry(ref dynamicSymbolReplacements, dynamicSymbolSwapEntry);
                }
                else
                {
                    CreateReelEntry(ref dynamicSymbolReplacements, dynamicSymbolSwapEntry);
                }
            }

            return dynamicSymbolReplacements;
        }

        private void AddToExistingReelEntry(ref ReplacementsDict dynamicSymbolReplacements, DynamicSymbolPair dynamicSymbolSwapEntry)
        {
            (SymbolId fromIdent, SymbolId toIdent) = GetFromToIdents(dynamicSymbolSwapEntry);

            if (dynamicSymbolReplacements[dynamicSymbolSwapEntry.ReelIndex].ContainsKey(fromIdent))
            {
                GameIdLogger.Logger.Error("MilanDynamicSymbolModelDriver :: duplicate swap pair key provided for reel index" + dynamicSymbolSwapEntry.ReelIndex);
            }
            dynamicSymbolReplacements[dynamicSymbolSwapEntry.ReelIndex].Add(fromIdent, toIdent);
        }

        private void CreateReelEntry(ref ReplacementsDict dynamicSymbolReplacements, DynamicSymbolPair dynamicSymbolSwapEntry)
        {
            Dictionary<SymbolId, SymbolId> symbolPair = new Dictionary<SymbolId, SymbolId>();
            (SymbolId fromIdent, SymbolId toIdent) = GetFromToIdents(dynamicSymbolSwapEntry);

            symbolPair.Add(fromIdent, toIdent);
            dynamicSymbolReplacements.Add(dynamicSymbolSwapEntry.ReelIndex, symbolPair);
        }

        private (SymbolId, SymbolId) GetFromToIdents(DynamicSymbolPair dynamicSymbolSwapEntry)
        {
            SymbolId fromIdent = _slotConfig.Find(dynamicSymbolSwapEntry.SwapSymbolFrom).id;
            SymbolId toIdent = _slotConfig.Find(dynamicSymbolSwapEntry.SwapSymbolTo).id;
            return (fromIdent, toIdent);
        }
    }

    public class DynamicSymbolPair
    {
        public int ReelIndex;
        public string SwapSymbolFrom;
        public string SwapSymbolTo;
    }

    public class DynamicSymbolData
    {
        public string Id;
        public DynamicSymbolPair[] DynamicSymbolPairs;

        public DynamicSymbolData()
        {
            Id = string.Empty;
            DynamicSymbolPairs = new DynamicSymbolPair[] { };
        }
    }
}
