using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class ReelBounceEventForwarder : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresParent] RootReelView _rootReelView;
        [FieldRequiresParent(optional = true)] ReelBounceEventView _reelBounceEventView;

        public void OnServicesLoaded()
		{
            this.InitializeDependencies();
		}

        public void NotifyReelStopBounce()
		{
            _reelBounceEventView?.OnReelBounce(_rootReelView.ReelIndex);
		}
    }
}
