using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class CustomReelsOutcomeClientModelStatePresenter : BaseStatePresenter
	{
		[SerializeField] private string _serverModelTag;
		[SerializeField] private string _clientModelTag;

		private ReelsOutcomeServerModel _serverModel;
		private ReelsOutcomeClientModel _clientModel;

		public override void OnServicesLoaded()
		{
			base.OnServicesLoaded();

			ServiceLocator serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();

			_serverModel = serviceLocator.GetOrCreate<ReelsOutcomeServerModel>(_serverModelTag);
			_clientModel = serviceLocator.GetOrCreate<ReelsOutcomeClientModel>(_clientModelTag);
		}

		public void UpdateClientModel()
		{
			Execute();
		}

		protected override void Execute()
		{
			_clientModel.UpdateModel(
				_serverModel.Id.Value,
				_serverModel.Offsets.Value,
				_serverModel.ReelWindowId.Value,
				_serverModel.ReelStripIds.Value,
				_serverModel.Symbols
				);
		}
	}
}
