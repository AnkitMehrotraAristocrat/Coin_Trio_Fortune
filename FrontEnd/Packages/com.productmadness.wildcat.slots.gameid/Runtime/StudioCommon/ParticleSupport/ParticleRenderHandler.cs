using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    [ExecuteInEditMode]
    public class ParticleRenderHandler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _activeParticles;
        private HashSet<ParticleSystem.Particle> _trackedParticles = new HashSet<ParticleSystem.Particle>();
        private Collider _particleBoundaryCollider;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _activeParticles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
            enabled = false;
        }

		private void LateUpdate()
		{
            var particleCount = _particleSystem.GetParticles(_activeParticles);
            var newTrackedParticles = new HashSet<ParticleSystem.Particle>();
            bool particlesUpdated = false;

            // TODO: Optimize this with the nested if conditionals
            for (int i = 0; i < particleCount; ++i)
			{
                newTrackedParticles.Add(_activeParticles[i]);
                if (_trackedParticles.Add(_activeParticles[i]))
				{
                    if (_particleBoundaryCollider != null && _particleBoundaryCollider.bounds.Contains(_activeParticles[i].position))
					{
                        var color = _activeParticles[i].GetCurrentColor(_particleSystem);
                        color.a = 255; // TODO: Change this magic number
                        _activeParticles[i].startColor = color;
                        particlesUpdated = true;
                    }
				}
            }

            if (particlesUpdated)
			{
                _particleSystem.SetParticles(_activeParticles, particleCount);
                _trackedParticles = new HashSet<ParticleSystem.Particle>(newTrackedParticles);
            }
        }

        public void SetCollider(Collider collider)
		{
            _particleBoundaryCollider = collider;
		}
	}
}
