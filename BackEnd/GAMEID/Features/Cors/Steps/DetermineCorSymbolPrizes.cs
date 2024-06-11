using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Cors.Data;
using GameBackend.Features.Cors.Configuration;
using System;

namespace GameBackend.Features.Cors.Steps
{
    public class DetermineCorSymbolPrizes : BaseStep<GameContext>
	{
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.PersistentData.GaffeQueues.ConsumeCategoryQueue(GaffeCategories.DetermineCorSymbolPrizes, context.PersistentData.RandomNumberQueue);

			if (context.FeatureContext<CorsContext>().LandedCorSymbols.Count == 0) {
				return Task.CompletedTask;
			}

			var totalBet = BetHelper.GetTotalBet(context);
            var collection = context.FeatureRoundData<CorsRoundData>().PrizesCollected.Prizes;
            var landedCors = context.FeatureContext<CorsContext>().LandedCorSymbols;

            // Set prizes for Cor symbols landed this spin, all others are locked for the round
			foreach (var corStop in landedCors) {
                var randomEntry = GeneralHelper.GetRandomEntryFromTable<string>(context, Constants.PrizesWeightTable);
                
                CorPrizeInfo prize = EntryDeserializer.Deserialize(randomEntry);
                prize.Stop = corStop;
                if (prize.Type == GameConstants.MultiplierPrizeType) {
                    prize.Value *= totalBet;
                    prize.Value = Math.Round(prize.Value);
                }
                collection.Add(prize);
            }
			return Task.CompletedTask;
		}

		public override bool Validate(GameContext context)
		{
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
	}
}