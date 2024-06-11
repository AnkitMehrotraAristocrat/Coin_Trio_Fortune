using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Threading.Tasks;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ActiveReelWindow
{
	/// <summary>
	/// Driver that supports updating the ActiveReelWindowModel
	/// </summary>
	public class ActiveReelWindowModelDriver : MonoBehaviour, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private ActiveReelWindowModel _activeReelWindowModel = default;
		[FieldRequiresGlobal] private MainDriver _mainDriver = default;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
				.Add("activeReelWindows", DeserializeActiveReelWindows)
				.Then(OnResponse));
		}

		private async Task DeserializeActiveReelWindows(string json, MainDriver.IPayloadWriter payloadWriter)
		{
			ActiveReelWindow[] recoveredReelWindows = await JsonUtils.DeserializeObjectAsync<ActiveReelWindow[]>(json);
			payloadWriter.Set(recoveredReelWindows);
		}

		private void OnResponse(MainDriver.IPayloadReader payloadReader)
		{
			ActiveReelWindow[] recoveredReelWindows = payloadReader.Get<ActiveReelWindow[]>();
			foreach (ActiveReelWindow reelWindow in recoveredReelWindows)
			{
				_activeReelWindowModel.SetReelWindow(reelWindow.GameState, reelWindow.ReelWindow);
			}
		}

		public class ActiveReelWindow
		{
			public string GameState;
			public string ReelWindow;
		}
	}
}
