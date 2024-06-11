using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class WizardInputData
	{
		public string StudioAcronym { get; set; }
		public string GameId { get; set; }
		public List<JackpotDefinition> Jackpots { get; set; }
		public List<SymbolDefinition> Symbols { get; set; }
		public List<ReelWindowDefinition> ReelWindows { get; set; }
		public List<string> Features { get; set; }
		public List<PaylinesDefinition> Paylines { get; set; }

	}

	public class JackpotDefinition
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public int Tier { get; set; }
		public ulong ResetValue { get; set; }
		public int EnabledBetIndex { get; set; }
	}

	public class SymbolDefinition
	{
		public string Name { get; set; }
		public int Id { get; set; }
	}

	public class ReelWindowDefinition
	{
		public string Name { get; set; }
		public List<int> ReelHeights { get; set; }
		public int ColumnCount { get; set; }
		public int RowCount { get; set; }
		public string Layout { get; set; }
		public ReelWindowStubData StubData { get; set; } = new ReelWindowStubData();
	}

	public class ReelWindowStubData
	{
		public bool Generate { get; set; } = false;
		public int RootGameObjectIndex { get; set; }
		public string RootGameObjectName { get; set; }
		public int ReelsPrefabIndex { get; set; }
		public ReelWindowConfiguration ReelWindowConfiguration { get; set; }
	}

	public class PaylinesDefinition
	{
		public string Name { get; set; }
		public List<PaylineDefinition> Lines { get; set; }
	}

	public class PaylineDefinition : List<PaylinePosition> { }

	public class PaylinePosition
	{
		public int ColIndex { get; set; }
		public int RowIndex { get; set; }
	}
}
