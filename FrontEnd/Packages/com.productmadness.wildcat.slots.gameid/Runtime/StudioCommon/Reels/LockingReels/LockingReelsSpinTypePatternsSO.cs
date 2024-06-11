using System;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	[CreateAssetMenu(fileName = "LockingReelsSpinTypePatterns", menuName = "NMG/Locking Reels/Spin Type Patterns")]
	public class LockingReelsSpinTypePatternsSO : ScriptableObject
	{
		[SerializeField] private LockingReelsSpinTypePattern[] _spinTypePatterns;

		public bool GetSpinType(int[] lockedReels, out string spinType)
		{
			LockingReelsSpinTypePattern patternMap = _spinTypePatterns.FirstOrDefault(pattern => pattern.LockedReels.SequenceEqual(lockedReels));
			
			if (patternMap == null)
			{
				spinType = null;
				return false;
			}

			spinType = patternMap.SpinType;
			return true;
		}
	}

	[Serializable]
	public class LockingReelsSpinTypePattern
	{
		[Tooltip("The spin type associated with the prescribed locked reels.")]
		public string SpinType;

		[Tooltip("Expects the order of locked reels to be matching.")]
		public int[] LockedReels;
	}
}
