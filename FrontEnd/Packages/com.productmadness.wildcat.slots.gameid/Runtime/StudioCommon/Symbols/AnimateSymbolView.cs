using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class AnimateSymbolView : SymbolTriggerView, ServiceLocator.IHandler
    {
		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> AnimateSymbol(SymbolHandle instance, string animTrigger, string yieldUntilEnterTag = null)
		{
			TriggerHandle(instance, animTrigger);
			if (yieldUntilEnterTag != null)
			{
				yield return instance.GetComponent<Animator>().WhenStateEnter(yieldUntilEnterTag);
			}
		}
	}
}
