using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class LockingReelsSpinTypeProvider : BaseSpinTypeProvider
	{
		[SerializeField] private string _clientModelTag;
		[SerializeField] private LockingReelsSpinTypePatternsSO _lockingReelsSpinTypePatterns;

		private LockingReelsClientModel _clientModel;

		public override void OnServicesLoaded()
		{
			base.OnServicesLoaded();

			ServiceLocator serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();
			_clientModel = serviceLocator.GetOrCreate<LockingReelsClientModel>(_clientModelTag);
		}

		public override string GetSpinType(MainDriver.IPayloadReader payloadReader = null)
		{
			// Only run this on the game states permitted.
			if (!_allowedGameStates.Contains(_gameStateModel.GameState))
			{
				return null;
			}

			if (_clientModel.LockedReels.Length == 0)
			{
				return null;
			}

			if (_lockingReelsSpinTypePatterns.GetSpinType(_clientModel.LockedReels, out string targetSpinType))
			{
				return targetSpinType;
			}
			else
			{
				GameIdLogger.Logger.Warning(GetType() + ":: No spin type pattern present for the supplied locked reels array!", this);
				return null;
			}
		}
	}
}
