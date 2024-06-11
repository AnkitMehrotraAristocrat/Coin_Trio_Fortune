using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.MetaEvents;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class WinStartEventBroadcaster : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresGlobal] private ServiceLocator _serviceLocator;

        private MetaEventManager _metaEvents;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            _serviceLocator.TryGet(out _metaEvents);
        }

        public void Broadcast(WinStartData data)
        {
            _metaEvents?.Broadcast(new WinStart
            {
                Index = data.Index,
                Multiplier = data.Multiplier,
                WinAmount = data.WinAmount,
                Duration = data.Duration,
                Title = data.Title
            });
        }
    }
}
