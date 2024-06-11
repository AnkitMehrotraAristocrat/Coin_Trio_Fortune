using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Spinoff of the MilanSpinModelDriver to support reel sets per bet index / dynamic symbols.
	/// </summary>
	public class CustomSpinDriver : MonoBehaviour, ISpinDriver, ServiceLocator.IHandler, ServiceLocator.IService
	{
		const string CHIPS = "credit";

		[FieldRequiresModel] private GameStateModel _gameStateModel = default;

		[FieldRequiresGlobal] private MainDriver _mainDriver = default;
		[FieldRequiresGlobal] private ServiceLocator _serviceLocator = default;

		[FieldRequiresChild(optional = true)] private ReelsOutcomeModifierSequencer _modifierSequencer = default;
		[FieldRequiresChild] private ISpinTypeEvaluator _spinTypeEvaluator = default;

		[SerializeField] private string _serviceId = "Spin";

		private WhenSpinComplete _spinYield;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("spinResponse", NoOpDeserialize)
				.Add("rewards", DeserializeTotalWinAmount, optional: true)
				.Then(OnResponse)
			);
		}

		public WhenSpinComplete Spin(SpinRequest spinRequest)
		{
			var request = new Request
			{
				serviceId = _serviceId,
				payload = new Dictionary<string, object>
				{
					{ "betIndex", spinRequest.betIndex }
				}
			};

			_spinYield = new WhenSpinComplete();
			_mainDriver.HandleRequest(request);
			return _spinYield;
		}

        private async Task NoOpDeserialize(string json, MainDriver.IPayloadWriter payloadWriter)
        {
            // does nothing
        }

		private async Task DeserializeTotalWinAmount(string json, MainDriver.IPayloadWriter payloadWriter)
		{
			var rewardJsons = await JsonUtils.DeserializeObjectAsync<RewardJson[]>(json);
			var totalWon = rewardJsons
				.Where(reward => reward.CurrencyType == CHIPS)
				.Select(reward => reward.TotalWon);

			payloadWriter.Set(new TotalWinAmount { value = totalWon.Any() ? totalWon.First() : 0L });
		}

		private void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			if (_spinYield != null && !_spinYield.Completed)
			{
				EvaluateOutcomeModifiers();
				SetSpinType(payloadReader);
				var totalWinAmount = payloadReader.Get<TotalWinAmount>() ?? new TotalWinAmount { value = 0L };
				_spinYield.WinAmount = totalWinAmount.value;
			}
		}

		private void EvaluateOutcomeModifiers()
		{
			var outcomeModel = _serviceLocator.Get<ReelsOutcomeServerModel>(_gameStateModel.GameState.ToString());
			if (!outcomeModel.ModifiersEvaluated)
			{
				_modifierSequencer?.EvaluateModifiers(_gameStateModel.GameState.ToString(), outcomeModel);
				outcomeModel.SetModifiersEvaluated(true);
			}
		}

		private void SetSpinType(MainDriver.IPayloadReader payloadReader)
		{
			if (_spinTypeEvaluator == null)
			{
				return;
			}
			_spinTypeEvaluator.EvaluateSpinType(payloadReader);
		}

		private class RewardJson
		{
			public string CurrencyType;
			public long TotalWon;
		}

		public class TotalWinAmount
		{
			public long value;
		}
	}
}
