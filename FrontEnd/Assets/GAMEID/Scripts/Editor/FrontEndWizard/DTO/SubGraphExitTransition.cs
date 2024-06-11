using System;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public class SubGraphExitTransition
	{
		public string ExitStatePort; // StateName (PortName)
		public string DestinationNode;
		public StateNodeType DestinationType;
	}
}
