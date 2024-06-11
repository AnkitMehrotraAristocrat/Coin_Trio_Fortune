using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.FreespinCore;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using System.Linq;
using Milan.Common.SlotEngine.Models;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// A state machine presenter that supports playing the scatter win animations after the reels stop
	/// if free spins have been awarded.
	/// </summary>
	public class AnimateScatterPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler 
    {
		[FieldRequiresModel] private ScatterClientModel _scatterClientModel = null;
		[FieldRequiresModel] private FreeSpinServerModel _freeSpinServerModel = null;
		[FieldRequiresParent] private SymbolLocator _symbolLocator = null;
		[FieldRequiresChild] private AnimateScatterView _animateScatterView;

		private List<StopData[]> _scattersGroups = new List<StopData[]>();

		public string Tag => this.GetTag();

		public INotifier Notifier 
        {
			private get; set;
		}

		public void OnServicesLoaded() 
        {
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> Enter() 
        {
			if (_freeSpinServerModel.SpinsWon.Value > 0) 
            {
				yield return new WhenAll(PlayScatterFeatureTriggerAnim());
			}
			yield break;
		}

		private IEnumerator<Yield> PlayScatterFeatureTriggerAnim() 
        {
			List<Yield> triggerAnimations = new List<Yield>();
			InitScatterData();
			foreach (StopData[] scatterGroup in _scattersGroups) 
            {
				bool playSoundEffect = true; // only play the sound effect one time for each scatter type
				foreach (StopData stopData in scatterGroup) 
                {
	                // todo: find replacement for this
					// SymbolHandle symbol = _symbolLocator.ScreenSymbols[stopData].CurrentSymbol.Instance;
					// triggerAnimations.Add(Coroutine.Start(_animateScatterView.OnTrigger(symbol, playSoundEffect)));
					playSoundEffect = false;
				}
			}
			yield return new WhenAll(triggerAnimations.ToArray());
		}

		private void InitScatterData() 
        {
			ScattersHit[] scattersHitsData = _scatterClientModel.ScatterHitResult.Value.Hits;
			_scattersGroups = new List<StopData[]>();
			foreach (ScattersHit hit in scattersHitsData) 
            {
				AddScatterEntry(hit);
			}
		}

		private void AddScatterEntry(ScattersHit hit) 
        {
			StopData[] scatterGroup;
			scatterGroup = hit.Stops.Select(x => x).ToArray();
			_scattersGroups.Add(scatterGroup);
		}

		public IEnumerator<Yield> Exit() 
        {
			yield break;
		}
	}
}
