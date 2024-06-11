using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public abstract class BaseLockingReelsModel : IModel
	{
		private int[] _lockedReels = new int[] {};
		public int[] LockedReels => _lockedReels;

		public BaseLockingReelsModel(ServiceLocator serviceLocator) { }

		public void SetLockedReels(int[] lockedReels)
		{
			// updates the _lockedReels with the supplied lockedReels
			_lockedReels = lockedReels;
		}
	}
}
