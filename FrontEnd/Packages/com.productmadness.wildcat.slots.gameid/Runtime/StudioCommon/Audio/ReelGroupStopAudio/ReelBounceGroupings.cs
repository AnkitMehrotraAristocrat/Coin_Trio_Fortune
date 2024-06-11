using Malee;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[Preserve]
	[Serializable]
	public class ReelIndexBounceGroup
	{
		public string ReelStopAudioEventName;
		public int[] ReelIndices;

		private Dictionary<int, bool> _reelHasBounced = new Dictionary<int, bool>();
		public bool AllReelsBounced => _reelHasBounced.All(entry => entry.Value);

		public void InitializeGroup()
		{
			_reelHasBounced.Clear();
			foreach (int reelIndex in ReelIndices)
			{
				_reelHasBounced.Add(reelIndex, true);
			}
		}

		public void SetReelHasBounced(int reelIndex, bool hasBounced)
		{
			_reelHasBounced[reelIndex] = hasBounced;
		}
	}

	[Preserve]
	[Serializable]
	public class ReelIndexBounceGroups : ReorderableArray<ReelIndexBounceGroup> { }

	[CreateAssetMenu(fileName = "ReelBounceGroupings", menuName = "NMG/Reel Group Stop Audio/Reel Bounce Groupings")]
	public class ReelBounceGroupings : ScriptableObject
	{
		public string QuickStopAudioEventName;
		[Reorderable] public ReelIndexBounceGroups Groups;
	}
}
