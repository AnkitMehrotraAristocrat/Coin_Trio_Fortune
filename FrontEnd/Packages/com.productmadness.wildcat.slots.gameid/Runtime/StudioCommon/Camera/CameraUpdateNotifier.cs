using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A component that handles notifying responder classes of aspect ratio inducing camera updates.
    /// </summary>
    public class CameraUpdateNotifier : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresChild(includeInactive = true)] private ICameraUpdateResponder[] _responders;

        private YieldUntilComplete _initialized = new YieldUntilComplete();

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            _initialized.Complete();
        }

        public void NotifyCameraUpdated()
        {
            Coroutine.Start(NotifyResponders());
        }

        private IEnumerator<Yield> NotifyResponders()
        {
            yield return _initialized;

            foreach (ICameraUpdateResponder responder in _responders)
            {
                responder.CameraUpdated();
            }
        }
    }
}
