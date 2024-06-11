using Milan.FrontEnd.Core.v5_1_1;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public abstract class BaseLockingReelsProvider : MonoBehaviour, ServiceLocator.IHandler
	{
		[SerializeField] protected string _clientModelTag;
		protected LockingReelsClientModel _clientModel;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

			ServiceLocator serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();
			_clientModel = serviceLocator.GetOrCreate<LockingReelsClientModel>(_clientModelTag);
		}

		public virtual bool ShouldReelSpin(int reelIndex)
		{
			// checks the clientModel to see if the reelIndex is locked
			// if so, return false
			return !_clientModel.LockedReels.Contains(reelIndex);
		}
	}
}
