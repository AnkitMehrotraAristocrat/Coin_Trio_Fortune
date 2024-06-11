using Milan.FrontEnd.Bridge.Meta;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using UniRx;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Stops win presentation via the WinStopSequencer on bet change events.
	/// </summary>
	public class BetUpdateWinStopPresenter : MonoBehaviour, IPresenter, ServiceLocator.IHandler
	{
		public string Tag => this.GetTag();

		[FieldRequiresModel] private IBetModel _betModel = null;
		[FieldRequiresGlobal] private WinStopSequencer _winStopSequencer = null;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			_betModel.Amount.Subscribe(index =>
			{
				Coroutine.Start(_winStopSequencer.Enter());
			}).AddTo(this);
		}
	}
}
