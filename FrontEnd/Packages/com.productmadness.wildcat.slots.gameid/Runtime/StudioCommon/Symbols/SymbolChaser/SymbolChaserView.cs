using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolChaser
{
	/// <summary>
	/// Supports executing symbol chaser animations.
	/// Typically resides on a game object adjacent to the Spin Sequencer (i.e. Reels game object).
	/// </summary>
	public class SymbolChaserView : MonoBehaviour, ServiceLocator.IHandler, IReelEventResponder, ISymbolSpawnResponder
	{
		[FieldRequiresChild] private RootReelView[] _reelViews;

		[SerializeField] [Reorderable] SymbolChaserSOs _symbolChasers;

		private List<bool> _playAnimOnReelIndex = new List<bool>(); // Indicates if the chaser should be played on a given reel index

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			// Initialize each chaser's prefab pool
			foreach (SymbolChaserSO symbolChaser in _symbolChasers)
			{
				symbolChaser.Initialize(transform.root);
			}

			// Populate the _playAnimOnReelIndex list (with false) for each reel view
			_playAnimOnReelIndex.AddRange(Enumerable.Repeat(false, _reelViews.Length));
		}

		/// <summary>
		/// Resets the _playAnimOnReelIndex flag to true when a given reel spin starts.
		/// This ensures only spinning reels will have the potential for the symbol chaser to play on it.
		/// </summary>
		/// <param name="reelIndex"></param>
		public void OnReelSpin(int reelIndex)
		{
			_playAnimOnReelIndex[reelIndex] = true;
		}

		/// <summary>
		/// Sets the _playAnimOnReelIndex flag to false. This method is invoked when the reels begin to land
		/// as we do not want to play the chase anim when the reels are coming to a stop.
		/// </summary>
		/// <param name="reelIndex"></param>
		public void OnReelLanding(int reelIndex)
		{
			_playAnimOnReelIndex[reelIndex] = false;
		}

		public void OnReelStop(int reelIndex)
		{
			// Does nothing, required by IReelEventResponder
		}

		public void OnReelQuickStop(int reelIndex)
		{
			// Does nothing, required by IReelEventResponder
		}

		/// <summary>
		/// Starts the private PlayChaseAnim() coroutine if the spawned symbol has a symbol chaser
		/// scriptable object present (and the given reel is spinning).
		/// </summary>
		/// <param name="symbolData"></param>
		public void SymbolSpawned(SpawnedSymbolData symbolData)
		{
			SymbolId spawnedSymbol = symbolData.SymbolId;
			SymbolChaserSO matchedSymbolChaser = _symbolChasers.FirstOrDefault(symbolChaser => symbolChaser.TargetSymbol.Equals(spawnedSymbol));
			if (matchedSymbolChaser != null && _playAnimOnReelIndex[symbolData.Location.colIndex])
			{
				Coroutine.Start(PlayChaseAnim(matchedSymbolChaser, symbolData.Location.colIndex));
			}
		}

		public void SymbolDespawned(SpawnedSymbolData symbolData)
		{
			// Does nothing, required by ISymbolSpawnResponder
		}

		/// <summary>
		/// Handles positioning and playing the symbol chaser animation.
		/// </summary>
		/// <param name="symbolChaser"></param>
		/// <param name="reelIndex"></param>
		/// <returns></returns>
		private IEnumerator<Yield> PlayChaseAnim(SymbolChaserSO symbolChaser, int reelIndex)
		{
			// Fetch the reelView that has a matching reel index, if one does not exist, log an error
			// message and short circuit
			RootReelView targetReelView = _reelViews.FirstOrDefault(reelView => reelView.ReelIndex.Equals(reelIndex));
			if (targetReelView == null)
			{
				GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Could not find matching reel view!", this);
			}

			// Spawn an instance from the chaser prefab pool and set it's parent to the previously
			// fetched reel view (worldPositionStays should be false) <-- this is what locates the prefab
			GameObject instance = symbolChaser.SpawnPrefab();
			instance.transform.SetParent(targetReelView.transform, false);

			// Get the animation's trigger name via GetTrigger()
			// Fetch this chaser prefab instance's animator and set it's trigger
			string trigger = symbolChaser.GetAnimTrigger();
			Animator animator = instance.GetComponent<Animator>();
			animator.SetTrigger(trigger);

			// Yield until we reach the idle state
			yield return animator.WhenStateEnter(symbolChaser.IdleStateTag);

			// Despawn the chaser prefab instance (returning it to the pool)
			symbolChaser.DespawnPrefab(instance);
		}
	}
}
