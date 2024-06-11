using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Pooling;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SymbolVisibilityHandler : MonoBehaviour, GameObjectPool.ISpawnable, ISymbolVisibilityHandler
	{
		[FieldRequiresChild]                         private Animator         _animator          = null;
		[FieldRequiresChild]                         private Renderer[]       _attachedRenderers = null;

		[FieldRequiresChild(includeInactive = true)] private ParticleSystem[] _particleSystems = null;
		[FieldRequiresChild(includeInactive = true)] private SpriteRenderer[] _spriteRenderers = null;

		public Animator Animator => _animator;

        [SerializeField] private string _despawnAnimatorState = "Idle";
        [Tooltip("Disable to allow animator to restore its default state on disable"),
         SerializeField]
        private bool _keepAnimatorControllerStateOnDisable;

        private List<Renderer> _renderers;
		public IReadOnlyList<Renderer> Renderers => _renderers;

		[SerializeField] private bool _playParticlesOnSpawn = true;
		[SerializeField] private bool _stopParticlesOnDespawn = true;
		[SerializeField] private bool _clearParticlesOnDespawn = false;

		private void Awake()
		{
			this.InitializeDependencies();
			_renderers = new List<Renderer>(_attachedRenderers);
		}

		public void OnSpawn()
		{
			_animator.enabled = true;
			foreach (var renderer in _renderers)
			{
				if (renderer is ParticleSystemRenderer)
				{
					continue;
				}
				renderer.enabled = true;
			}

			foreach (var spriteRenderer in _spriteRenderers)
			{
				spriteRenderer.enabled = true;
			}

			if (!_playParticlesOnSpawn)
			{
				return;
			}

			foreach (var particleSystem in _particleSystems)
			{
				particleSystem.Play();
			}
		}

		public void OnDespawn()
		{
            _animator.keepAnimatorStateOnDisable = _keepAnimatorControllerStateOnDisable;

			_animator.Play(_despawnAnimatorState);

			_animator.enabled = false;

			foreach (var renderer in _renderers)
			{
				if (renderer is ParticleSystemRenderer)
				{
					continue;
				}
				renderer.enabled = false;
			}

			foreach (var spriteRenderer in _spriteRenderers)
			{
				spriteRenderer.enabled = false;
			}

			foreach (var particleSystem in _particleSystems)
			{
				if (_stopParticlesOnDespawn)
				{
					particleSystem.Stop();
				}
				
				if (_clearParticlesOnDespawn)
				{
					particleSystem.Clear();
				}
			}
		}

		public void AddRenderer(Renderer renderer)
		{
			_renderers.Add(renderer);
			var existingRenderer = _renderers[0];
			renderer.enabled = existingRenderer.enabled;
		}
	}
}
