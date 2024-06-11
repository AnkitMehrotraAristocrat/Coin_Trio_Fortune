using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateBetConfigPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var milanBetLevelsData = new MilanBetLevelsData {
                betLevels = BuildBetLevelsDataList(context)
            };
            context.ConfigPayload.AddPayload(GameConstants.MilanBetLevelsPayloadName, milanBetLevelsData);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.MappedConfigurations, nameof(context.MappedConfigurations));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static List<BetLevelData> BuildBetLevelsDataList(GameContext context)
        {
            // Get the bet indices for this game
            var betIndices = GetBetIndices(context);

            // Get the base cost for a spin and the bet multipliers for this game
            var currency = context.GetBetCurrencyType();
            var betCurrency = context.MappedConfigurations.BetItems.First(w => w.CurrencyType == currency);
            var betMultipliers = betCurrency.MultiplierIndexes.Multipliers;

            #if LINES_GAME
            var baseCost = betCurrency.BetLineIndexes.BetLines[context.RoundData.LineIndex].TotalLineCost;
            #else //WAYS_GAME
            var baseCost = betCurrency.ReelStripCostIndexes.GetTotalReelStripCost();
            #endif
            
            // For each bet index, create a bet level data object using the base cost and multiplier
            var betLevelsDataList = new List<BetLevelData>();
            foreach (var index in betIndices.Indices) {
                var multiplier = betMultipliers[(int)index];
                var betLevelData = new BetLevelData {
                    amount = baseCost * multiplier,
                    breakdown = new Dictionary<string, ulong> {
                        { "multiplier", multiplier },
                        { "baseBet", baseCost }
                    }
                };
                betLevelsDataList.Add(betLevelData);
            }
            return betLevelsDataList;
        }

        public static BetIndices GetBetIndices(GameContext context)
        {
            var currency = context.GetBetCurrencyType();
            var betCurrency = context.MappedConfigurations.BetItems.First(w => w.CurrencyType == currency);
            var betMultipliersU = betCurrency.MultiplierIndexes.Multipliers.ToArray();

            var betIndicesL = new long[betMultipliersU.Length];
            for (var i = 0; i < betMultipliersU.Length; ++i) {
                betIndicesL[i] = i;
            }
            return new BetIndices { Indices = betIndicesL };
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public class MilanBetLevelsData
        {
            public List<BetLevelData> betLevels { get; set; }
        }

        public class BetLevelData
        {
            public ulong amount { get; set; }
            public Dictionary<string, ulong> breakdown { get; set; }
        }

        public class BetIndices
        {
            public long[] Indices { get; set; }
        }
    }
}
