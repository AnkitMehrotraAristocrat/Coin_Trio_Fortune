using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// State machine presenter that caches the current bet index and updates to the boost
	/// mode bet index or restores the current bet index (bet index at boost activation).
	/// This allows explicit control over when the bet index is updated when entering and
	/// also the restoration of the prior bet index upon boost exit.
	/// </summary>
	public class RestorableBetIndexPresenter : BaseStatePresenter
	{
		public enum TargetBetIndex
		{
			None,
			Feature,
			Base
		}

		[FieldRequiresModel] private RestorableBetIndexModel _restorableBetIndexModel;

		[SerializeField] private TargetBetIndex _targetBetIndex;

		public override IEnumerator<Yield> Enter()
		{
			if (_action.Equals(Action.OnEnter))
			{
				UpdateBetIndex();
			}
			yield break;
		}
		public override IEnumerator<Yield> Exit()
		{
			if (_action.Equals(Action.OnExit))
			{
				UpdateBetIndex();
			}
			yield break;
		}

		public void UpdateBetIndex()
		{
			switch (_targetBetIndex)
			{
				default:
				case TargetBetIndex.None:
					GameIdLogger.Logger.Error(GetType() + "." + Tag + " has a Target Bet Index of None. LevelBasedBetServerModel's bet index will remain unchanged.");
					break;
				case TargetBetIndex.Feature:
					_betModel.SetIndex(_restorableBetIndexModel.FeatureBetIndex);
					break;
				case TargetBetIndex.Base:
					_betModel.SetIndex(_restorableBetIndexModel.BaseBetIndex);
					break;
			}
		}
	}
}
