using Malee;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CreateAssetMenu(fileName = "WizardConfiguration", menuName = "NMG/Wizard/Configuration")]
	public class WizardConfiguration : ScriptableObject
	{
		/// <summary>
		/// The input json file that will be used by the front end wizard.
		/// Produced by the Project Wizard.
		/// </summary>
		[SerializeField] private string _inputJson = "Assets/WizardConfigs/WizardInput.json";
		public string InputJson => _inputJson;

		/// <summary>
		/// The state tracking scriptable object maintaing execution state
		/// and reel window generation states
		/// </summary>
		[SerializeField] private WizardState _wizardState;
		public WizardState WizardState => _wizardState;

		/// <summary>
		/// The input json file that will be used by the front end wizard.
		/// Produced by the Project Wizard.
		/// </summary>
		[SerializeField] private string _gameViewRoot = "SlotMachine/GameView";
		public string GameViewRoot => _gameViewRoot;

		/// <summary>
		/// Reel window root object options
		/// Example: BaseGame, FreeSpins, HoldAndSpin, etc
		/// </summary>
		[SerializeField] private List<string> _reelWindowRootOptions = new List<string>();
		public List<string> ReelWindowRootOptions => _reelWindowRootOptions;

		/// <summary>
		/// Reel window configurations
		/// Example: Standard, HoldAndSpin, etc
		/// </summary>
		[SerializeField] private List<ReelWindowConfiguration> _reelWindowConfigurations = new List<ReelWindowConfiguration>();
		public List<ReelWindowConfiguration> ReelWindowConfigurations => _reelWindowConfigurations;

		/// <summary>
		/// A serialized list of executors that should run
		/// </summary>
		[SerializeReference] public List<BaseWizardExecutor> Executors;
	}

	[Serializable]
	public class WizardExecutors : ReorderableArray<BaseWizardExecutor> { }
}
