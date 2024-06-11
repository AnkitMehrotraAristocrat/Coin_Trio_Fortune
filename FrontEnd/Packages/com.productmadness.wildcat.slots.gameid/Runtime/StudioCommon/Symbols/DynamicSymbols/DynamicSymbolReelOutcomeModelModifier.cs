using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
	public class DynamicSymbolReelOutcomeModelModifier : BaseReelsOutcomeModelModifier, ServiceLocator.IHandler
	{
		[FieldRequiresGlobal] private ServiceLocator _serviceLocator;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public override void ModifySymbols(string id, ReelsOutcomeServerModel model)
		{
			DynamicSymbolServerModel dynamicSymbolsModel = _serviceLocator.GetOrCreate<DynamicSymbolServerModel>(id);

			// If we've got no replacements, move along.
            if (dynamicSymbolsModel.DynamicSymbolReplacements.Value == null)
            {
                return;
            }

            var symbols = new SymbolId[model.Symbols.Length][];

			for (int reelIndex = 0; reelIndex < model.Symbols.Length; ++reelIndex)
			{
				symbols[reelIndex] = new SymbolId[model.Symbols[reelIndex].Length];
				for (int symbolIndex = 0; symbolIndex < model.Symbols[reelIndex].Length; ++symbolIndex)
				{
					symbols[reelIndex][symbolIndex] = ReplaceDynamicSymbol(dynamicSymbolsModel, reelIndex, model.Symbols[reelIndex][symbolIndex]);
				}
			}
			model.SetSymbols(symbols);
		}

		private SymbolId ReplaceDynamicSymbol(DynamicSymbolServerModel serverModel, int reelIndex, SymbolId symbolId)
		{
			if (!serverModel.DynamicSymbolReplacements.Value.ContainsKey(reelIndex))
			{
				return symbolId;
			}
			if (!serverModel.DynamicSymbolReplacements.Value[reelIndex].ContainsKey(symbolId))
			{
				return symbolId;
			}
			return serverModel.DynamicSymbolReplacements.Value[reelIndex][symbolId];
		}
	}
}
