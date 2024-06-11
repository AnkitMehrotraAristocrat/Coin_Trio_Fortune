using Milan.Common.SlotEngine.Models.Data;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class CustomReelsOutcomeModelDriver : MonoBehaviour, ServiceLocator.IHandler
	{
		public enum ReelStopLocation
		{
			Center,
			Top
		}

		public string Tag => this.GetTag();

		[FieldRequiresParent] protected MainDriver _mainDriver;
		[FieldRequiresGlobal] protected ServiceLocator _serviceLocator;
		[FieldRequiresChild(optional = true)] protected ReelsOutcomeModifierSequencer _modifierSequencer;

		[SerializeField] protected ReelStopLocation _reelStopLocation;

		protected CustomReelsOutcomeClientModelPresenter _clientModelPresenter;

		private const string _payloadName = "ReelsOutcomeModel";


		#region Deserialization

		public virtual void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
				.Add(_payloadName, Deserialize)
				.Then(OnResponse));

			_clientModelPresenter = transform.root.GetComponentInChildren<CustomReelsOutcomeClientModelPresenter>();
		}

		protected virtual async Task Deserialize(string json, MainDriver.IPayloadWriter payloadWriter)
		{
			var reelOffsetsData = await JsonUtils.DeserializeObjectAsync<ReelOutcomeData>(json);
			var model = _serviceLocator.GetOrCreate<ReelsOutcomeServerModel>(reelOffsetsData.Id);

			_clientModelPresenter.AddServerModelSubscription(reelOffsetsData.Id, model);
			model.SetModifiersEvaluated(false);
			payloadWriter.Set(reelOffsetsData);

			// This deserialization is an exception to the "don't update the model in a deserialization
			// method" rule. The reason is that response handling order is not guaranteed; which means the
			// spin model driver could try and modify old data if this were to reside in the response handler
			// below.

			// If we really want this out of this method, we'd need Milan to provide a post response handler
			// so the spin model driver can not worry about outcome modification and set the spin type properly.

			var reelWindowName = reelOffsetsData.ReelWindowId;
			var reelWindowModel = _serviceLocator.Get<ReelWindowDataServerModel>(reelWindowName);
			var reelSetId = reelOffsetsData.ReelSetId;
			var reelStripIds = reelOffsetsData.ReelStrips;
			var offsets = Translate(reelOffsetsData.Offsets, reelWindowModel.Height.Value);
			var symbols = GenerateVisibleReels(reelWindowModel, reelStripIds, offsets);

			model.UpdateModel(reelOffsetsData.Id, offsets, reelWindowName, reelSetId, reelStripIds, symbols);
		}

		protected virtual void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			var outcomeDatas = payloadReader.GetAll<ReelOutcomeData>();

			foreach (ReelOutcomeData outcomeData in outcomeDatas)
			{
				var id = outcomeData.Id;
				var model = _serviceLocator.Get<ReelsOutcomeServerModel>(id);

				if (!model.ModifiersEvaluated)
				{
					_modifierSequencer?.EvaluateModifiers(id, model);
					model.SetModifiersEvaluated(true);
				}
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Backend stores reel stops at reel window's midpoint while frontend is at its top.
		/// </summary>
		/// <param name="offsets"></param>
		/// <param name="windowHeight"></param>
		/// <returns></returns>
		public virtual int[] Translate(int[] offsets, int windowHeight)
		{
			if (_reelStopLocation.Equals(ReelStopLocation.Top))
			{
				return offsets;
			}

			for (var i = 0; i < offsets.Length; ++i)
			{
				offsets[i] = offsets[i] - windowHeight / 2;
			}

			return offsets;
		}

		protected virtual SymbolId[][] GenerateVisibleReels(ReelWindowDataServerModel reelWindowModel,
			string[] reelStripIds, int[] offsets)
		{
			// Using reel strip IDs length instead, this seems to fix the scenario where hold and spin has a larger
			// window model than reel strips sent down, and works in all other scenarios too!
			var symbols = new SymbolId[reelStripIds.Length][];

			for (int reelIndex = 0; reelIndex < symbols.Length; ++reelIndex)
			{
				var reelStripModel = _serviceLocator.Get<ReelStripDataServerModel>(reelStripIds[reelIndex]);
				var offset = offsets[reelIndex] + reelWindowModel.Height.Value - 1;
				var symbolProvider = new FixedReelProvider(reelStripModel.Strip.Value, offset);

				symbols[reelIndex] = new SymbolId[reelWindowModel.Height.Value];

				for (int symbolIndex = symbols[reelIndex].Length; --symbolIndex >= 0;)
				{
					symbols[reelIndex][symbolIndex] = symbolProvider.Consume();
				}
			}

			return symbols;
		}

		#endregion
	}
}
