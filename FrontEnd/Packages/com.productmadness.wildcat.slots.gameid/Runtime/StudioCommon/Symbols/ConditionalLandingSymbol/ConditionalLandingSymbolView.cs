using Malee;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System;
using System.Linq;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	/// <summary>
	/// Supports conditional landing symbol sound/animation execution.
	/// Current support:
	/// - Scatter symbol
	/// </summary>
	[AddComponentMenu("Conditional Landing Symbol View V2")]
	public class ConditionalLandingSymbolView : SymbolTriggerView, ServiceLocator.IHandler, ISpinResponder, IReelEventResponder, IRecoveryHandler
	{
		#region Helper Classes
		[Preserve]
		[Serializable]
		public class SymbolConditions : ReorderableArray<SymbolCondition> { };
		#endregion

		[FieldRequiresGlobal] private ServiceLocator _serviceLocator;
		[FieldRequiresParent] private AudioEventBindings _audioBindings;
		[FieldRequiresSelf] private SymbolLocator _symbolLocator;

		[SerializeField] [Reorderable] private SymbolConditions _conditions;
		[SerializeField] private ReelStopGroupings _reelIndexGroups;
		[SerializeField] private bool _checkForQuickstop = true;

		private bool _initialized = false;
		private bool _quickStopped = false;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			InitializeConditions();
			_initialized = true;
			ValidateReelIndexGroups();
		}

		private void InitializeConditions()
		{
			foreach (SymbolCondition condition in _conditions)
			{
				condition.Initialize(_serviceLocator, _symbolLocator);
			}
		}
		
		public void OnReelStop(int reelIndex)
		{
			var group = _reelIndexGroups.Groups.FirstOrDefault(currentGroup => currentGroup.ReelIndices.Contains(reelIndex));
			var symbols = _symbolLocator.ScreenSymbols.ByColumn(reelIndex);
			foreach (var symbol in symbols)
			{
				OnLanded(symbol.Location, symbol.CurrentSymbol.Instance, group);
			}

			if (group == null)
			{
				return;
			}

			group.SetReelHasStopped(reelIndex, true);

			// Quick stop handling (quick stop notifications will arrive before this method is invoked)
			if (_checkForQuickstop && _quickStopped)
			{
				HandleQuickStop();
				return;
			}
				
			// Normal stop handling
			if (group.AllReelsStopped)
			{
				// play appropriate sound(s)
				List<SymbolAudioEvent> audioEvents = group.GetHighestAudioPriorities();
				PlayAudioEvents(audioEvents);
			}
		}

		private void OnLanded(Location location, SymbolHandle symbol, ReelIndexStopGroup group)
		{
            var symbolConditions = _conditions.Where(condition => condition.SymbolList.Contains(symbol.id.Value)).ToList();

            foreach (var symbolCondition in symbolConditions)
            {
                if (symbolCondition == null)
                {
                    return;
                }

                if (!IsEligible(symbolCondition))
                {
                    return;
                }

                if (!symbolCondition.AnimProvider.ShouldAnimate(location.colIndex, location.rowIndex))
                {
                    return;
                }

                TriggerHandle(symbol, symbolCondition.AnimProvider.GetLandingAnimTrigger());

                if (group == null)
                {
                    return;
                }

                int groupIndex = _reelIndexGroups.Groups.IndexOf(group);
                SymbolAudioEvent audioEvent = symbolCondition.SoundProvider?.GetAudioEvent(location, groupIndex);
                if (audioEvent != null)
                {
                    group.AddSymbolAudioEvent(location.colIndex, audioEvent);
                }
            }
        }

		private bool IsEligible(SymbolCondition symbolCondition)
		{
			bool isEligible = true;
			
			foreach (BaseEligibilityModifier eligibilityModifier in symbolCondition.EligibilityModifiers)
			{
				isEligible &= eligibilityModifier.IsEligible();
			}

			return isEligible;
		}

        private void HandleQuickStop()
        {
            if (_reelIndexGroups.Groups.All(potentialGroup => potentialGroup.AllReelsStopped))
            {
                var allAudioEvents = _reelIndexGroups.Groups.Where(potentialGroup => potentialGroup.ReelsQuickStopped)?.SelectMany(potentialGroup => potentialGroup.GetHighestAudioPriorities());
                allAudioEvents = allAudioEvents.Distinct();

                if (allAudioEvents.Count() == 0)
                {
                    return;
                }

                var highestPriority = allAudioEvents.Select(audioEvent => audioEvent.Priority).Min();

                var audioEvents = allAudioEvents.Where(audioEvent => audioEvent.Priority.Equals(highestPriority)).ToList();
                PlayAudioEvents(audioEvents);
            }
        }

		private void PlayAudioEvents(List<SymbolAudioEvent> audioEvents)
		{
			foreach (SymbolAudioEvent audioEvent in audioEvents)
			{
				_audioBindings.Play(audioEvent.AudioEventName);
			}
		}
		
		public void OnReelSpin(int reelIndex)
		{
			_reelIndexGroups.Groups.FirstOrDefault(group => group.ReelIndices.Contains(reelIndex))?.SetReelHasStopped(reelIndex, false);
		}

		public IEnumerator<Yield> SpinStarted()
		{
			if (!gameObject.activeInHierarchy)
			{
				yield break;
			}

			_quickStopped = false;

			foreach (ReelIndexStopGroup group in _reelIndexGroups.Groups)
			{
				group.InitializeGroup();
			}

			foreach (var condition in _conditions)
			{
				condition.SoundProvider?.Reset();
			}
		}

		public IEnumerator<Yield> SpinComplete()
		{
			yield break;
		}

		private void OnEnable()
		{
			if (!_initialized)
			{
				return;
			}

			foreach (var condition in _conditions)
			{
				condition.AnimProvider.OnEnable();
				condition.SoundProvider?.OnEnable();
			}
		}

		private void OnDisable()
		{
			foreach (var condition in _conditions)
			{
				condition.AnimProvider?.OnDisable();
				condition.SoundProvider?.OnDisable();
			}
		}

		public void OnReelQuickStop(int reelIndex) {
			_quickStopped = true;
			_reelIndexGroups.Groups.FirstOrDefault(group => group.ReelIndices.Contains(reelIndex))?.SetReelHasQuickStopped(reelIndex, true);
		}

		public void OnReelLanding(int reelIndex) { }

		#region Validation
		private void ValidateReelIndexGroups()
		{
			ValidateReelIndicesAreUnique();
		}

		private void ValidateReelIndicesAreUnique()
		{
			HashSet<int> hashedReelIndices = new HashSet<int>();
			var reelIndices = _reelIndexGroups.Groups.SelectMany(group => group.ReelIndices).ToList();
			bool allUnique = reelIndices.All(hashedReelIndices.Add);
			if (!allUnique)
			{
				GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Duplicate reel index entries present!", this);
				throw new ArgumentException();
			}
		}

		public void OnServerConfigsReady(ServiceLocator locator)
		{
			// DOes nothing
		}

		public void OnInitialStateReady(ServiceLocator locator)
		{
			// initialize anim and sound providers
			foreach (var condition in _conditions)
			{
				condition.AnimProvider.Initialize();
				condition.SoundProvider?.Initialize();
			}
		}
		#endregion
	}
}
