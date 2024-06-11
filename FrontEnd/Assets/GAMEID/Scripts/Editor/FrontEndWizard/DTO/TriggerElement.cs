using System;
using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public class TriggerElement
	{
		public SerializedTrigger Trigger;
		public List<StateTransition> StateNodes;
	}
}
