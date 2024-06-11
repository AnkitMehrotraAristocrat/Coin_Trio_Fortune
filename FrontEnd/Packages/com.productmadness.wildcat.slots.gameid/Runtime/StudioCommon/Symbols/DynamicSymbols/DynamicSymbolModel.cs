using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using System.Collections.Generic;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
	public class ReplacementsDict : Dictionary<int, Dictionary<SymbolId, SymbolId>> { };

	public abstract class DynamicSymbolModel : IModel
	{
		protected IReactiveProperty<ReplacementsDict> _dynamicSymbolReplacements = new ReactiveProperty<ReplacementsDict>();
		public IReadOnlyReactiveProperty<ReplacementsDict> DynamicSymbolReplacements => _dynamicSymbolReplacements;

		public DynamicSymbolModel(ServiceLocator _) { }

		public void SetDynamicSymbolReplacements(ReplacementsDict dynamicSymbolReplacements)
		{
			_dynamicSymbolReplacements.Value = dynamicSymbolReplacements;
		}
	}

	public class DynamicSymbolServerModel : DynamicSymbolModel
	{
		public DynamicSymbolServerModel(ServiceLocator _) : base(_) { }
	}

	public class DynamicSymbolClientModel : DynamicSymbolModel
	{
		public DynamicSymbolClientModel(ServiceLocator _) : base(_) { }

		public void ResetDynamicSymbolReplacements()
		{
			_dynamicSymbolReplacements.Value = new ReplacementsDict();
		}
	}
}
