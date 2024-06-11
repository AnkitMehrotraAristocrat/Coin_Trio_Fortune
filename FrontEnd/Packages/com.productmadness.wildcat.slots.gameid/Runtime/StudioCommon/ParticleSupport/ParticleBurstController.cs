using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class ParticleBurstController : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresChild] private ParticleSystem _particleSystem;

        [SerializeField] private int _particleCount;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public void BurstParticles()
        {
            _particleSystem.Emit(_particleCount);
        }
    }
}
