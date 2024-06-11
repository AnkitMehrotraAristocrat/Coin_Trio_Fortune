using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SpinGaffeDriver : MonoBehaviour, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private SpinGaffeModel _spinGaffeModel;

		[FieldRequiresGlobal] private MainDriver _mainDriver;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
				.Add("SpinGaffe", Deserialize)
				.Then(OnResponse));
		}

		private async Task Deserialize(string json, MainDriver.IPayloadWriter payloads)
		{
			SpinGaffeData payload = await JsonUtils.DeserializeObjectAsync<SpinGaffeData>(json);
			payloads.Set(payload);
		}

		private void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			SpinGaffeData data = payloadReader.Get<SpinGaffeData>();
			_spinGaffeModel.Add(data);
		}
	}
}
