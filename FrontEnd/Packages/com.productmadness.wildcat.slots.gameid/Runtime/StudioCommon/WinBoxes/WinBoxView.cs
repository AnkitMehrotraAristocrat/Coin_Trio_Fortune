using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class WinBoxView : MonoBehaviour, IWinningSymbolView, ILosingSymbolView, ServiceLocator.IHandler
	{
		[FieldRequiresChild] private Renderer[] _renderers;
		[FieldRequiresChild(optional = true)] private ParticleSystem[] _particleSystems;
		[FieldRequiresParent] private ScreenSymbolView _winLocation;

		[Reorderable]
		[SerializeField] private SymbolIdList _ineligibleSymbols;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			SetRenderersEnabled(false);
		}

		public void OnWin(Location winLocation, SymbolHandle symbol)
		{
            // Don't break if you don't have a win location. We weren't planning on using win boxes anyway...
            if (_winLocation == null)
            {
                return;
            }

			if (_winLocation.Location != winLocation || _ineligibleSymbols.Contains(symbol.id))
			{
				return;
			}

			SetRenderersEnabled(true);

			foreach (var ps in _particleSystems)
			{
				ps.Play();
			}
		}

		public void OnIdle(Location winLocation, SymbolHandle symbol)
		{
			if (_winLocation.Location != winLocation || _ineligibleSymbols.Contains(symbol.id)) 
			{
				return;
			}
			SetRenderersEnabled(false);
		}

		public void OnLoss(Location location, SymbolHandle symbol)
		{
			OnIdle(location, symbol);
		}

		private void SetRenderersEnabled(bool isEnabled)
		{
			foreach (var renderer in _renderers)
			{
				renderer.enabled = isEnabled;
			}
		}

        public void OnCycleIdle(Location winLocation, SymbolHandle symbol, string triggerName = "")
        {
			OnIdle(winLocation, symbol);
        }
    }
}
