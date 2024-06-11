using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Implemented by concrete classes that need to know when
	/// a symbol spawns or despawns.
	/// </summary>
	public interface ISymbolSpawnResponder
	{
		void SymbolSpawned(SpawnedSymbolData symbolData);
		void SymbolDespawned(SpawnedSymbolData symbolData);
	}

	/// <summary>
	/// A component that notifies all ISymbolSpawnResponder implementations of spawn/despawn events.
	/// This typically resides on game objects that implement the PooledSymbolView.
	/// </summary>
	public class SymbolSpawnNotifier : MonoBehaviour, IPooledSymbolObserver, ServiceLocator.IHandler
	{
		[FieldRequiresParent] ISymbolSpawnResponder[] _symbolSpawnResponders;
		[FieldRequiresParent] ISymbolView _symbolView;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		/// <summary>
		/// Handles symbol spawn notifications.
		/// </summary>
		/// <param name="handle"></param>
		public void OnAttach(SymbolHandle handle)
		{
			SpawnedSymbolData spawnedSymbolData = new SpawnedSymbolData() { SymbolId = _symbolView.Instance.id, Location = _symbolView.Location, SymbolHandle = handle};
			ForEachResponder(responder => responder.SymbolSpawned(spawnedSymbolData));
		}

		/// <summary>
		/// Handles symbol despawn notifications.
		/// </summary>
		/// <param name="handle"></param>
		public void OnDetach(SymbolHandle handle)
		{
			SpawnedSymbolData despawnedSymbolData = new SpawnedSymbolData() { SymbolId = _symbolView.Instance.id, Location = _symbolView.Location, SymbolHandle = handle};
			ForEachResponder(responder => responder.SymbolDespawned(despawnedSymbolData));
		}

		/// <summary>
		/// Support method that iterates over each ISymbolSpawnResponder and executes the provided action.
		/// </summary>
		/// <param name="action"></param>
		private void ForEachResponder(Action<ISymbolSpawnResponder> action)
		{
			foreach (ISymbolSpawnResponder responder in _symbolSpawnResponders)
			{
				action(responder);
			}
		}
	}
}
