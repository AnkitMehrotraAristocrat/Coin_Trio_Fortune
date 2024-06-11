using Milan.FrontEnd.Bridge.Meta;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Betting;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class BetModelBetProvider : MonoBehaviour, ServiceLocator.IHandler, ICurrentBetProvider
	{
		[FieldRequiresModel] private IBetModel _betModel;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public long CurrentBetValue => _betModel.Amount.Value;
	}
}
