using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// AnimateScatterView enables an animation trigger when a scatter symbol triggers a feature
	/// Targeted to be at the end of a spin
	/// </summary>
	public class AnimateScatterView : MonoBehaviour, ServiceLocator.IHandler 
    {
		[FieldRequiresGlobal] AudioEventBindings _audioBindings;

		[SerializeField] private string _animTrigger = "Trigger";
		[SerializeField] private string _triggerStateTag = "trigger";
		[SerializeField] private string _audioEventName = null;

		public void OnServicesLoaded() {
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> OnTrigger(SymbolHandle symbol, bool playSoundEffect) 
        {
			if (symbol == null) 
            {
				yield return new YieldImmediate();
			}

			var symbolAnimator = symbol.GetComponent<ISymbolVisibilityHandler>()?.Animator;
			if (symbolAnimator == null) 
            {
				yield return new YieldImmediate();
			}

			// Reset all triggers
			bool hasTrigger = false;
			foreach (var param in symbolAnimator.parameters) 
            {
				if (param.type == AnimatorControllerParameterType.Trigger) 
                {
					symbolAnimator.ResetTrigger(param.name);
					if (!hasTrigger && param.name == _animTrigger)
						hasTrigger = true;
				}
			}

			if (hasTrigger) 
            {
				symbolAnimator.SetTrigger(_animTrigger);
				if (_audioBindings != null && _audioEventName != null && playSoundEffect) 
                {
					_audioBindings.Play(_audioEventName);
				}
			}

			symbol.GetComponent<ISymbolClippingHandler>().ClippingEnabled = false;
			symbol.GetComponent<ISymbolSortingHandler>().WinDepth = 1;

			if (hasTrigger) 
            {
				yield return symbolAnimator.WhenStateComplete(_triggerStateTag);
			}
		}
	}
}
