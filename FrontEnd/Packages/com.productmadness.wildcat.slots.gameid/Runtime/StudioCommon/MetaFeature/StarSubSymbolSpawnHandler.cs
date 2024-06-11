using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
	/// Upon a symbol spawning, queries the StarSubSymbolVisibilityResponder to see if stars are allowed to be displayed.
	/// This is to resolve issues where transitioning to a feature could result in stars being shown on all the new symbols.
	/// </summary>
	public class StarSubSymbolSpawnHandler : BaseSymbolSpawnHandler
    {
		// reel index -> SymbolCyclingReelView
		protected Dictionary<int, SymbolCyclingReelView> _symbolCyclingReelViews = new Dictionary<int, SymbolCyclingReelView>();

		// reelIndex -> symbolIndex -> cyclingReelViewIndex
		protected Dictionary<int, Dictionary<int, int>> _cyclingReelViewIndices = new Dictionary<int, Dictionary<int, int>>();

		public override void OnServicesLoaded()
		{
			base.OnServicesLoaded();
			InitializeCyclingReelViewIndicesDictionary();
		}

		protected override void OnSymbolSpawn(SpawnedSymbolData symbolData)
		{
			StarSubSymbolVisibilityResponder subSymbolResponder = symbolData.SymbolHandle.GetComponent<StarSubSymbolVisibilityResponder>();
			if (subSymbolResponder != null)
			{
				Location location = symbolData.Location;
				subSymbolResponder.RootReelView = _reelViews[location.colIndex];
				subSymbolResponder.Location = location;
				
				SymbolCyclingReelView symbolCyclingReelView = _symbolCyclingReelViews[location.colIndex];
				subSymbolResponder.SymbolCyclingReelView = symbolCyclingReelView;
				subSymbolResponder.SymbolCyclingReelViewIndex = _cyclingReelViewIndices[symbolCyclingReelView.ReelIndex][location.rowIndex];
			}
		}

		protected override void OnSymbolDespawn(SpawnedSymbolData symbolData)
		{
			StarSubSymbolVisibilityResponder subSymbolRespondersubSymbolAllow = symbolData.SymbolHandle.GetComponent<StarSubSymbolVisibilityResponder>();
			if (subSymbolRespondersubSymbolAllow != null)
			{
				subSymbolRespondersubSymbolAllow.ClearReferences();
			}
		}

		protected virtual void InitializeCyclingReelViewIndicesDictionary()
		{
			SymbolCyclingReelView[] symbolCyclingReelViews = GetComponentsInChildren<SymbolCyclingReelView>();
			foreach (SymbolCyclingReelView cyclingReelView in symbolCyclingReelViews)
			{
				int reelIndex = cyclingReelView.GetComponentInParent<RootReelView>().ReelIndex;
				_symbolCyclingReelViews.Add(reelIndex, cyclingReelView);
				_cyclingReelViewIndices.Add(reelIndex, new Dictionary<int, int>());
				ISymbolView[] symbolViews = cyclingReelView.GetComponentsInChildren<ISymbolView>();
				for (int symbolViewIndex = 0; symbolViewIndex < symbolViews.Length; ++symbolViewIndex)
				{
					_cyclingReelViewIndices[reelIndex].Add(symbolViews[symbolViewIndex].Location.rowIndex, symbolViewIndex);
				}
			}
		}
	}
}
