using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Threading.Tasks;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Driver to support the base bet index recovery
	/// </summary>
	public class RestorableBetIndexDriver : MonoBehaviour, ServiceLocator.IHandler
	{
		[FieldRequiresParent] private MainDriver _mainDriver;
		[FieldRequiresModel] private RestorableBetIndexModel _restorableBetIndexModel;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
				.Add("betState", Deserialize)
				.Then(OnResponse));
		}

		public async Task Deserialize(string json, MainDriver.IPayloadWriter payloadWriter)
		{
			BetStateJson betStatePayload = await JsonUtils.DeserializeObjectAsync<BetStateJson>(json);

			if (betStatePayload == null)
			{
				GameIdLogger.Logger.Error("Could not deserialize betState payload");
				return;
			}

			payloadWriter.Set(betStatePayload);
		}

		public void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			BetStateJson betStateData = payloadReader.Get<BetStateJson>();

			_restorableBetIndexModel.SetFeatureBetIndex(betStateData.FeatureBetIndex);
			_restorableBetIndexModel.SetBaseBetIndex(betStateData.BaseBetIndex);
		}

		public class BetStateJson
		{
			public int FeatureBetIndex;
			public int BaseBetIndex;
		}
	}
}
