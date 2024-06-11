using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A component that notifies all IReelEventResponder implementations
    /// of reel landing and reel quick stop events via an animaiton event.
    /// This component should be a sibling of a given reel's animator.
    /// </summary>
    public class ReelViewAnimationEventNotifier : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresParent] RootReelView _rootReelView;
        [FieldRequiresParent] IReelEventResponder[] _reelEventResponders;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        /// <summary>
        /// This method informes each IReelEventResponder of the reel landing event
        /// This method is intended to be invoked from an animation event
        /// from the reel's quick stop and stop method.
        /// The animation event is typically placed just after the first SetTopSymbol animation event.
        /// </summary>
        public void NotifyReelLanding()
        {
            foreach (IReelEventResponder responder in _reelEventResponders)
            {
                responder.OnReelLanding(_rootReelView.ReelIndex);
            }
        }

        /// <summary>
        /// This method informes each IReelEventResponder of the quick stop event
        /// This method is intended to be invoked from an animation event
        /// from the reel's quick stop method.
        /// The animation event is placed on the first frame of the animation.
        /// </summary>
        public void NotifyQuickStop()
        {
            foreach (IReelEventResponder responder in _reelEventResponders)
			{
                responder.OnReelQuickStop(_rootReelView.ReelIndex);
            }
        }
    }
}
