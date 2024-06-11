using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public class LockingReelsUpdateLockedStateStatePresenter : BaseStatePresenter
	{
		[SerializeField] private GameObject _lockingReelsRoot;

		private LockingDigitalReelView[] _reelViews;

        public override void OnServicesLoaded()
        {
            base.OnServicesLoaded();
			_reelViews = _lockingReelsRoot.GetComponentsInChildren<LockingDigitalReelView>(true);
        }

        protected override void Execute()
		{
			UpdateLockedState();
		}

		public void UpdateLockedState()
		{
			foreach (LockingDigitalReelView reelView in _reelViews)
			{
				reelView.SetShouldReelSpin();
			}
		}
	}
}