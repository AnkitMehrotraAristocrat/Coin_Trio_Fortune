using Malee;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[Preserve]
	[Serializable]
	public class ReelStopOutcome
	{
		public List<SymbolAudioEvent> SymbolAudioEvents = new List<SymbolAudioEvent>();
		public bool HasStopped = false;
		public bool HasQuickStopped = false;
	}

	[Preserve]
	[Serializable]
	public class ReelIndexStopGroup
	{
		public int[] ReelIndices;
		private Dictionary<int, ReelStopOutcome> _reelStopOutcomes = new Dictionary<int, ReelStopOutcome>();
		public bool AllReelsStopped => _reelStopOutcomes.All(entry => entry.Value.HasStopped);
		public bool ReelsQuickStopped => _reelStopOutcomes.Any(entry => entry.Value.HasQuickStopped);

		public void InitializeGroup()
		{
			_reelStopOutcomes.Clear();
			foreach (int reelIndex in ReelIndices)
			{
				_reelStopOutcomes.Add(reelIndex, new ReelStopOutcome() { HasStopped = true, HasQuickStopped = false });
			}
		}

		public void SetReelHasStopped(int reelIndex, bool isStopped)
		{
			_reelStopOutcomes[reelIndex].HasStopped = isStopped;
		}

		public void SetReelHasQuickStopped(int reelIndex, bool isQuickStopped)
		{
			_reelStopOutcomes[reelIndex].HasQuickStopped = isQuickStopped;
		}

		public void AddSymbolAudioEvent(int reelIndex, SymbolAudioEvent condition)
		{
			_reelStopOutcomes[reelIndex].SymbolAudioEvents.Add(condition);
		}

		public List<SymbolAudioEvent> GetHighestAudioPriorities()
		{
			List<SymbolAudioEvent> audioPriorities = new List<SymbolAudioEvent>();

			int? priority = _reelStopOutcomes
				.SelectMany(outcomes => outcomes.Value.SymbolAudioEvents)
				.Min(audioEvent => audioEvent?.Priority);

			if (priority.HasValue)
			{
				var conditions = _reelStopOutcomes
					.SelectMany(outcomes => outcomes.Value.SymbolAudioEvents)
					.Where(audioEvent => audioEvent.Priority.Equals(priority))
					.Distinct();

				audioPriorities = conditions.ToList();
			}

			return audioPriorities;
		}
	}

	[Preserve]
	[Serializable]
	public class ReelIndexStopGroups : ReorderableArray<ReelIndexStopGroup> { }

	[CreateAssetMenu(fileName = "ReelStopGroupings", menuName = "NMG/Conditional Landing Symbols/Reel Stop Groupings")]
	public class ReelStopGroupings : ScriptableObject
	{
		[Reorderable] public ReelIndexStopGroups Groups;
	}
}
