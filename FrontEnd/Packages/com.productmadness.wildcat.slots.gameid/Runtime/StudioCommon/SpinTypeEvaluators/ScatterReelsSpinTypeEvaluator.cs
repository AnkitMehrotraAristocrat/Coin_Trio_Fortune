using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Spin type evaluator used by the CustomSpinDriver.
	/// Evaluates which spin type is to be applied if the only variants are due to scatter(s) appearing
	/// on a the reels. Assumes a left to right reel stop order.
	/// </summary>
	public class ScatterReelsSpinTypeEvaluator : MonoBehaviour, ISpinTypeEvaluator, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private ModalReelsModel _modalReelsModel = default;
		[FieldRequiresModel] private GameStateModel _gameStateModel = default;

        [SerializeField] private List<GameStateEnum> _allowedGameStates;
		[SerializeField] private SymbolId _scatterId;
		[SerializeField] private int _scatterCountThreshold = 2;
		[SerializeField] private string _defaultSpinType = "normal";
		[SerializeField] private string _anticipationSpinTypePrefix = "anticipate-";

        [FieldRequiresGlobal] private ServiceLocator _serviceLocator;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		/// <summary>
		/// Loops through the visible reels and counts if a single scatter is present on that reel.
		/// If it is, it increments the scatter count and updates the anticipation type string.
		/// If we meet the minimum count for anticipation, the spin type is set for this spin.
		/// </summary>
		/// <param name="payloads"></param>
		public void EvaluateSpinType(MainDriver.IPayloadReader payloadReader)
		{
			// Only run this on the game states permitted.
            if (!_allowedGameStates.Contains(_gameStateModel.GameState))
            {
                return;
            }

			if (_scatterId == null)
			{
				return;
			}

			var reelsOutcomeServerModel = _serviceLocator.Get<ReelsOutcomeServerModel>(_gameStateModel.GameState.ToString());
			var visibleReels = reelsOutcomeServerModel.Symbols;
            ReelWindowDataServerModel windowModel = _serviceLocator.Get<ReelWindowDataServerModel>(reelsOutcomeServerModel.ReelWindowId.Value);
			int reelCount = windowModel.Width.Value;
			int reelHeight = windowModel.Height.Value;

			int scatterCount = 0;

			string spinType = _defaultSpinType;
			string anticipationType = _anticipationSpinTypePrefix;
			bool isAnticipationSpin = false;
			for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
			{
				if ((scatterCount >= _scatterCountThreshold))
				{
					anticipationType += reelIndex.ToString();
					isAnticipationSpin = true;
				}
				for (int symbolIndex = 0; symbolIndex < reelHeight; ++symbolIndex)
				{
					if (visibleReels[reelIndex][symbolIndex] == _scatterId)
					{
						scatterCount++;
						break;
					}
				}
			}
			if (isAnticipationSpin)
			{
				spinType = anticipationType;
			}
			_modalReelsModel.SpinType.Value = spinType;
		}
	}
}
