using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	public class ReelTensionView : MonoBehaviour, ServiceLocator.IHandler, ISpinResponder, IReelEventResponder, IRecoveryHandler
	{
		#region Helper Classes
		[Preserve]
		[Serializable]
		public class TensionTypes : ReorderableArray<TensionType> { };

		public enum TensionPositionReference
		{
			AtStartOfSpin,
			Current
		}
		#endregion

		[FieldRequiresGlobal] private ServiceLocator _serviceLocator;
		[FieldRequiresParent] private AudioEventBindings _audioEventBindings;
		[FieldRequiresChild] private RootReelView[] _reelViews;
		[FieldRequiresSelf] private SymbolLocator _symbolLocator;

		[SerializeField] private TensionPositionReference _positionUsingReelLocation = TensionPositionReference.Current;
		[SerializeField][Reorderable] private TensionTypes _tensionTypes;

		private bool _hasQuickStopped = false;
		private HashSet<string> _quickStopAudioEventNames = new HashSet<string>();
		private bool _initialized = false;
		private Dictionary<int, YieldUntilComplete> _reelStopYields = new Dictionary<int, YieldUntilComplete>();

		private List<Vector3> _startingReelPositions = new List<Vector3>();

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		private void InitializeTensionTypes()
		{
			ForEachTensionType(tensionType => tensionType.Initialize(_serviceLocator, transform, _symbolLocator));
		}

		public void OnInitialStateReady(ServiceLocator locator)
		{
			InitializeTensionTypes();
			_initialized = true;
		}

		public void SetSpinSubscriptions()
		{
			ForEachTensionType(tensionType => tensionType.SetSpinSubscriptions());
		}

		public void OnEnable()
		{
			if (!_initialized)
			{
				return;
			}
			ForEachTensionType(tensionType => tensionType.ViewEnabled());
		}

		public void OnDisable()
		{
			ForEachTensionType(tensionType => tensionType.ViewDisabled());
		}

		public IEnumerator<Yield> SpinComplete()
		{
			ForEachTensionType(tensionType => tensionType.SpinCompleted());
			yield break;
		}

		public IEnumerator<Yield> SpinStarted()
		{
			_hasQuickStopped = false;
			QuickStopAudioEvents();
			ForEachTensionType(tensionType => tensionType.SpinStarted());

			if (_positionUsingReelLocation.Equals(TensionPositionReference.AtStartOfSpin))
			{
				CacheStartingReelPositions();
			}

			yield break;
		}

		public void OnReelSpin(int reelIndex)
		{
			if (_reelStopYields.ContainsKey(reelIndex))
			{
				_reelStopYields[reelIndex].Complete();
			}
			_reelStopYields[reelIndex] = new YieldUntilComplete();
			ForEachTensionType(tensionType => tensionType.OnReelSpin(reelIndex));
		}

		public void OnReelStop(int reelIndex)
		{
			_reelStopYields[reelIndex].Complete();

			if (_hasQuickStopped)
			{
				return;
			}

			ForEachTensionType(tensionType => tensionType.OnReelStop(reelIndex));

			// short circuit if next reel does not exist
			int nextReelIndex = reelIndex + 1;
			if (nextReelIndex >= _reelViews.Length)
			{
				return;
			}

			// find the highest tension anim priority for the next reel
			TensionType[] eligibleTensionTypes = _tensionTypes
				.Where(tensionType => tensionType.IsEligible(nextReelIndex)).ToArray();

			if (eligibleTensionTypes.Length == 0)
			{
				return;
			}

			int highestPriority = eligibleTensionTypes.Select(tensionType => tensionType.Priority).Min();
			TensionType targetTensionType = eligibleTensionTypes.FirstOrDefault(tensionType => tensionType.Priority.Equals(highestPriority));

			// grab a prefab from it's pool
			GameObject prefab = targetTensionType.SpawnTensionPrefab();

			// position the spawned prefab on the next reel position
			Vector3? position;
			if (_positionUsingReelLocation.Equals(TensionPositionReference.AtStartOfSpin) && _startingReelPositions.Count > nextReelIndex)
			{
				position = _startingReelPositions[nextReelIndex];
			}
			else
			{
				position = _reelViews.FirstOrDefault(reelView => reelView.ReelIndex.Equals(nextReelIndex))?.transform.position;
			}
			prefab.transform.position = position ?? prefab.transform.position;

			// play the anim trigger
			Coroutine.Start(PlayTensionAnimation(targetTensionType, prefab, nextReelIndex));

			// check if the tension type chase changed
			if (targetTensionType.SoundProvider.GetAudioEvents(out string playAudioEventName, out string quickStopAudioEventName))
			{
				// if it has, play the audio associated with that tension anim
				/* TODO: Figure out if we want to just stop, ducking works ok
				if (!string.IsNullOrEmpty(_activeAudioEventName))
				{
					_audioEventBindings.Stop(_activeAudioEventName);
				}
				*/
				_quickStopAudioEventNames.Add(quickStopAudioEventName);
				_audioEventBindings.Play(playAudioEventName);
			}
		}

		private void CacheStartingReelPositions()
		{
			_startingReelPositions.Clear();
			_startingReelPositions.AddRange(_reelViews.Select(view => view.transform.position));
		}

		private IEnumerator<Yield> PlayTensionAnimation(TensionType tensionType, GameObject prefab, int reelIndex)
		{
			if (tensionType.AnimProvider.GetAnimProperties(out string animStartTrigger, out string animStopTrigger, out string animCompleteState))
			{
				Animator animator = prefab.GetComponentInChildren<Animator>();
				animator.SetTrigger(animStartTrigger);
				yield return _reelStopYields[reelIndex];

				animator.SetTrigger(animStopTrigger);
				yield return animator.WhenStateEnter(animCompleteState);

				tensionType.DespawnTensionPrefab(prefab);
			}
			yield break;
		}

		public void OnReelQuickStop(int reelIndex)
		{
			_hasQuickStopped = true;

			foreach (KeyValuePair<int, YieldUntilComplete> reelStopYield in _reelStopYields)
			{
				reelStopYield.Value.Complete();
			}

			QuickStopAudioEvents();
		}

		private void QuickStopAudioEvents()
		{
			foreach (string audioEventName in _quickStopAudioEventNames)
			{
				_audioEventBindings.Play(audioEventName);
			}
			_quickStopAudioEventNames.Clear();
		}

		private void ForEachTensionType(Action<TensionType> action)
		{
			foreach (TensionType tensionType in _tensionTypes)
			{
				action(tensionType);
			}
		}

		public void OnReelLanding(int reelIndex)
		{
			// does nothing
		}

		public void OnServerConfigsReady(ServiceLocator locator)
		{
			// does nothing
		}
	}
}
