using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SymbolParticleBoundaryAttacher : MonoBehaviour, IPooledSymbolObserver
	{
		[SerializeField] private Collider _boundaryCollider;

		public void OnAttach(SymbolHandle handle)
		{
			ParticleRenderHandler particleHandler = handle.GetComponentInChildren<ParticleRenderHandler>(true);
			if (particleHandler != null)
			{
				particleHandler.enabled = true;
				particleHandler.SetCollider(_boundaryCollider);
			}

		}

		public void OnDetach(SymbolHandle handle)
		{
			ParticleRenderHandler particleHandler = handle.GetComponentInChildren<ParticleRenderHandler>(true);
			if (particleHandler != null)
			{
				particleHandler.enabled = false;
			}
		}
	}
}
