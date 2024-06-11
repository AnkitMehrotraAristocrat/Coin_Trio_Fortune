using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SpinningSymbolAudioAttacher : MonoBehaviour, IPooledSymbolObserver, ServiceLocator.IHandler
	{
		[FieldRequiresGlobal] private AudioEventBindings _audioEventBindings;

		[SerializeField] private Collider _boundaryCollider;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public void OnAttach(SymbolHandle handle)
		{
			SpinningSymbolAudioHandler audioHandler = handle.GetComponentInChildren<SpinningSymbolAudioHandler>(true);
			if (audioHandler != null)
			{
				audioHandler.enabled = true;
				audioHandler.SetCollider(_boundaryCollider);
				audioHandler.SetAudioEventBindings(_audioEventBindings);
			}
		}

		public void OnDetach(SymbolHandle handle)
		{
			SpinningSymbolAudioHandler audioHandler = handle.GetComponentInChildren<SpinningSymbolAudioHandler>(true);
			if (audioHandler != null)
			{
				audioHandler.enabled = false;
			}
		}
	}
}
