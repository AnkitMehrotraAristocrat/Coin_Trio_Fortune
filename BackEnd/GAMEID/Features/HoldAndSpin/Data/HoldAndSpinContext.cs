using GameBackend.Data;
using GameBackend.Features.HoldAndSpin.Configuration;
using Milan.Common.SlotEngine.Models;
using Milan.XSlotEngine.Core.Configurations.XSlotConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.HoldAndSpin.Data
{
    public class HoldAndSpinContext
    {
        public bool Triggered { get; set; }
        private Dictionary<ReelStripConfiguration, int> BlankStopsCache { get; set; } = new();

        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////

        public static PayloadData GetDefaultStatePayload(string state)
        {
            return new PayloadData {
                id = state,
                HoldAndSpinData = new HoldAndSpinPayloadData {
                    FreeSpinsRemaining = 0,
                    TriggeringSpin = false,
                    TriggeringState = null
                }
            };
        }

        public static List<SymbolData> GetSymbolsMapped(GameContext context)
        {
            var defs = context.XSlotConfigurations.SymbolsConfiguration.SymbolsDefinitions;
            return defs.Select(symbolDefinition => new SymbolData {
                Id = symbolDefinition.Id,
                IsScatter = symbolDefinition.Scatter,
                Name = symbolDefinition.Name
            }).ToList();
        }

        public int GetBlankStopIndex(ReelStripConfiguration reel)
        {
            if (BlankStopsCache.ContainsKey(reel)) {
                return BlankStopsCache[reel];
            }
            int index = reel.Stops.IndexOf(reel.Stops.FirstOrDefault(x => Constants.BlankSymbols.Contains(x.Symbol)));
            BlankStopsCache.Add(reel, index);
            return index;
        }
    }
}
