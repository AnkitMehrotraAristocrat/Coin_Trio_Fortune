using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public abstract class BaseSymbolSpawnHandler : MonoBehaviour, ServiceLocator.IHandler, ISymbolSpawnResponder, IReelEventResponder
    {
        [FieldRequiresGlobal] protected ServiceLocator _serviceLocator;
        [FieldRequiresChild(includeInactive = true)] protected RootReelView[] _reelViews;

        protected List<bool> _reelLanding = new List<bool>();
        protected List<bool> _reelQuickStopped = new List<bool>();

        public virtual void OnServicesLoaded()
        {
            this.InitializeDependencies();
            _reelLanding.AddRange(Enumerable.Repeat(true, _reelViews.Length));
            _reelQuickStopped.AddRange(Enumerable.Repeat(true, _reelViews.Length));
        }

		public void SymbolSpawned(SpawnedSymbolData symbolData)
		{
            OnSymbolSpawn(symbolData);
		}

		public void SymbolDespawned(SpawnedSymbolData symbolData)
		{
            OnSymbolDespawn(symbolData);
        }

        public virtual void OnReelSpin(int reelIndex)
		{
            _reelLanding[reelIndex] = false;
            _reelQuickStopped[reelIndex] = false;
        }

        public virtual void OnReelLanding(int reelIndex)
        {
            _reelLanding[reelIndex] = true;
        }

        public virtual void OnReelStop(int reelIndex)
        {
            // Does nothing
        }

        public virtual void OnReelQuickStop(int reelIndex)
		{
            _reelQuickStopped[reelIndex] = true;
        }

        protected abstract void OnSymbolSpawn(SpawnedSymbolData symbolData);
        protected abstract void OnSymbolDespawn(SpawnedSymbolData symbolData);
    }
}
