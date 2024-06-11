using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public class LockingReelsDriver : MonoBehaviour, IModelDriver, ServiceLocator.IHandler, ServiceLocator.IService
	{
		[FieldRequiresGlobal] private ServiceLocator _serviceLocator = default;
		[FieldRequiresParent] private MainDriver _mainDriver = default;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
				.Add("LockingReels", DeserializeLockingReels)
				.Then(OnResponse)
				);

			// TEMP START
			//var model = _serviceLocator.GetOrCreate<LockingReelsServerModel>("FreeSpin");
			//model.SetLockedReels(new int[] { 1, 3 });
			// TEMP END
		}

		private async Task DeserializeLockingReels(string json, MainDriver.IPayloadWriter payloadWriter)
		{
			LockingReelsData nextReelStrips = await JsonUtils.DeserializeObjectAsync<LockingReelsData>(json);
			payloadWriter.Set(nextReelStrips);
		}

		private void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			LockingReelsData[] datum = payloadReader.GetAll<LockingReelsData>();
			foreach (LockingReelsData data in datum)
			{
				LockingReelsServerModel model = _serviceLocator.GetOrCreate<LockingReelsServerModel>(data.Id);
				model.SetLockedReels(data.Reels);
			}
		}
	}

	public class LockingReelsData
	{
		public string Id; // can be used to identify which reel window, game state, metamorphic, etc this data is intended for
		public int[] Reels;
	}
}
