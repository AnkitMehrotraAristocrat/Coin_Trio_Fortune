using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Initializes the RestorableBetIndexModel when the server supplied configs have been processed.
	/// </summary>
	public class RestorableBetIndexModelInitializer : MonoBehaviour, ServiceLocator.IHandler, IRecoveryHandler
	{
		[FieldRequiresModel] private RestorableBetIndexModel _restorableBetIndexModel;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public void OnServerConfigsReady(ServiceLocator locator)
		{
			_restorableBetIndexModel.Initialize(locator);
		}

		public void OnInitialStateReady(ServiceLocator locator)
		{
			// does nothing
		}
	}
}
