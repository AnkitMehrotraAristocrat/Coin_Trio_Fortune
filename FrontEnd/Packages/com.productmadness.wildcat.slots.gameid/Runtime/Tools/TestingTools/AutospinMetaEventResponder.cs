using Milan.FrontEnd.Core.v5_1_1.Data;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.MetaEvents;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class AutospinMetaEventResponder : MonoBehaviour
    {
        [SerializeField] private bool _shouldRespond;

        private MetaEventManager _events;
        private Frame _spinFrame = null;

        public void OnMetaLoaded(MetaLocator metaLocator)
        {
            // does nothing
        }

        public void OnCoreLoaded(MetaLocator metaLocator)
        {
            _events = metaLocator.Get<MetaEventManager>();
            _spinFrame = metaLocator.Get<Frame>("SpinFrame");
            SetSubscriptions();
        }

        private void SetSubscriptions()
        {
            _events.Subscribe<EnableAutoSpins>(_ => EnableAutospin());
            _events.Subscribe<DisableAutoSpins>(_ => DisableAutospin());
        }

        private void EnableAutospin()
        {
            if (!_shouldRespond)
            {
                return;
            }
            _spinFrame.Set("autospin", true);
        }

        private void DisableAutospin()
        {
            if (!_shouldRespond)
            {
                return;
            }
            _spinFrame.Set("autospin", false);
        }
    }
}
