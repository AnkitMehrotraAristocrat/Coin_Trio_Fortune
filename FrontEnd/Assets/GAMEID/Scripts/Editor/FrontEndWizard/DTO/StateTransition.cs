using System;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public class StateTransition
	{
		public string TargetNode;
		public string DestinationNode;
		public StateNodeType DestinationType;
	}
}
