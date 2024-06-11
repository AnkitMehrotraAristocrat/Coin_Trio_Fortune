using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips
{
	public class NextReelStripsClientModel : IModel
	{
		public IReactiveProperty<string[]> ActiveReelStrips { get; } = new ReactiveProperty<string[]>();

		public NextReelStripsClientModel(ServiceLocator serviceLocator)
		{
		}
	}
}
