using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public class SubGraphElement
	{
		public Subgraph FeatureSubgraph;
		public List<SubGraphExitTransition> ExitTransitions;
	}
}
