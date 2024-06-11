using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using Milan.FrontEnd.Slots.v5_1_1.Betting;
using System;
using Milan.FrontEnd.Bridge.Logging;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Model that maintains the cached current bet index and the bet index a feature is to be played at.
	/// </summary>
	public class RestorableBetIndexModel : IModel
	{
		private LevelBasedBetServerModel _betModel = null;

		private int _featureBetIndex = 0;
		public int FeatureBetIndex => _featureBetIndex;

		private int _baseBetIndex = 0;
		public int BaseBetIndex => _baseBetIndex;

		private bool _initialized = false;

		public RestorableBetIndexModel(ServiceLocator serviceLocator) { }

		public void Initialize(ServiceLocator serviceLocator)
		{
			if (_initialized)
			{
				return;
			}
			_betModel = serviceLocator.GetOrCreate<LevelBasedBetServerModel>();
			_initialized = true;
		}

		public void SetFeatureBetIndex(int betIndex)
		{
			_featureBetIndex = /*GetSafeBetIndex(*/betIndex;//);
		}

		public void SetBaseBetIndex(int betIndex)
		{
			_baseBetIndex = /*GetSafeBetIndex(*/betIndex;//);
		}

		private int GetSafeBetIndex(int betIndex)
		{
			int targetBetIndex = Math.Max(Math.Min(betIndex, _betModel.GetAmounts().Length - 1), 0);
			if (targetBetIndex != betIndex)
			{
				GameIdLogger.Logger.Error("RestorableBetIndexModel :: Invalid bet index (" + betIndex + ") supplied, using (" + targetBetIndex + ") instead.");
			}
			return targetBetIndex;
		}
	}
}
