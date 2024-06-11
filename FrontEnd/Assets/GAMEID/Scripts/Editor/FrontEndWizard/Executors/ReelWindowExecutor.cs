using PixelUnited.NMG.Slots.Milan.GAMEID.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public enum ReelConfigurationType
	{
		Standard,
		HoldAndSpin
	}

	public class ReelWindowExecutor : BaseWizardExecutor
	{
		private const string _scenePathSeparator = "/";
		private const string _reelsParent = "ReelWindow";
		private const string _reelsGameObjectPrefix = "Reels-";
		private const string _linePayMechanicKey = "Lines";

		private WizardConfiguration _configuration;
		private WizardState _state;

		public ReelWindowExecutor()
		{
			_canRerun = true;
		}

		/// <summary>
		/// IWizardExecutor implementation to generate reel stubs and populate with ReelGridBuilder components
		/// </summary>
		/// <param name="data"></param>
		public override void Execute(WizardInputData data)
		{
			Debug.Log("Generating reel window stubs..");
			Initialize();
			bool generatePaylineObjects = data.Features.Contains(_linePayMechanicKey);
			GenerateReelStubs(data.ReelWindows, generatePaylineObjects);
			Save();
			Debug.Log("Reel window stubs generation complete!");
		}

		/// <summary>
		/// Initializes the configuration and state fields
		/// </summary>
		private void Initialize()
		{
			FrontEndWizardHelper.GetWizardConfig(out _configuration);
			_state = _configuration.WizardState;
		}

		/// <summary>
		/// Loops over the supplied reelWindows list attempting to stub a game object in the slot scene
		/// </summary>
		/// <param name="reelWindows"></param>
		private void GenerateReelStubs(List<ReelWindowDefinition> reelWindows, bool generatePaylineObjects)
		{
			foreach (ReelWindowDefinition entry in reelWindows)
			{
				GenerateReelStub(entry, generatePaylineObjects);
			}
		}

		/// <summary>
		/// Handles reel window stub generation
		/// </summary>
		/// <param name="entry">The reel window to be potentially stubbed</param>
		private void GenerateReelStub(ReelWindowDefinition entry, bool generatePaylineObjects)
		{
			// short circuit if we should not generate a stub
			if (!entry.StubData.Generate)
			{
				return;
			}

			// generate the target object name and scene path
			string targetObjectName = string.Concat(_reelsGameObjectPrefix, entry.Name);
			string path = string.Join(_scenePathSeparator, _configuration.GameViewRoot, entry.StubData.RootGameObjectName, _reelsParent);

			// add the Reels prefab
			SceneManipulationHelper.AddPrefab(entry.StubData.ReelWindowConfiguration.Prefab, path, targetObjectName, true, true);
			path = string.Join(_scenePathSeparator, path, targetObjectName);

			// update the state scriptable object
			UpdateState(entry.Name, path);

			// add the ReelGridBuilder to the target object
			ReelGridBuilder reelGridBuilder = (ReelGridBuilder)SceneManipulationHelper.AddComponent(typeof(ReelGridBuilder).AssemblyQualifiedName, "", path, false);

			// configure the ReelGridBuilder
			ConfigureReelGridBuilder(entry, reelGridBuilder, generatePaylineObjects);
		}

		/// <summary>
		/// Updates the state scriptable object adding an entry for the supplied id and entering it's scene path info
		/// </summary>
		/// <param name="id">The id associated with this reel window</param>
		/// <param name="scenePath">The scene path the reel window will be populated from</param>
		private void UpdateState(string id, string scenePath)
		{
			_state.AddReelWindowState(id);
			_state.SetReelWindowRoot(id, scenePath);
		}

		// TODO: This method could change depending on ReelGridBuilder updates
		/// <summary>
		/// Configures the supplied reel grid builder with basic settings
		/// </summary>
		/// <param name="entry">The reel window data used to configure the reel grid builder</param>
		/// <param name="reelGridBuilder">The ReelGridBuilder instance to configure</param>
		private void ConfigureReelGridBuilder(ReelWindowDefinition entry, ReelGridBuilder reelGridBuilder, bool generatePaylineObjects)
		{
			// determine type
			ReelConfigurationType reelConfigType = GetReelConfigType(entry);

			// update the generate paylines flag if we aren't making standard reels
			generatePaylineObjects &= reelConfigType.Equals(ReelConfigurationType.Standard);

			// set the columns/rows data
			SetReelHeights(entry, reelGridBuilder);

			// set the paylines flag
			SetPaylineOptions(reelGridBuilder, generatePaylineObjects);

			// set the reel configuration and layout
			SetReelConfiguration(entry, reelGridBuilder, reelConfigType);

			// assign prefabs
			SetPrefabs(entry, reelGridBuilder, generatePaylineObjects);
		}

		private ReelConfigurationType GetReelConfigType(ReelWindowDefinition entry)
		{
			if (AreReelHeightsEquivalent(entry.ReelHeights, 1))
			{
				// for now we assume if all reels are 1 symbol tall its a hold and spin layout
				return ReelConfigurationType.HoldAndSpin;
			}
			return ReelConfigurationType.Standard;
		}

		/// <summary>
		/// Assigns row / column configurations
		/// </summary>
		/// <param name="entry">The reel window under processing</param>
		/// <param name="reelGridBuilder">The ReelGridBuilder to configure</param>
		private void SetReelHeights(ReelWindowDefinition entry, ReelGridBuilder reelGridBuilder)
		{

			if (AreReelHeightsEquivalent(entry.ReelHeights, entry.ReelHeights[0]))
			{
				reelGridBuilder.columns = entry.ColumnCount;
				reelGridBuilder.rows = entry.RowCount;
			}
			else
			{
				reelGridBuilder.supportAsymmetricalReels = true;
				reelGridBuilder.columnHeights = entry.ReelHeights.ToArray();
			}
		}

		private void SetPaylineOptions(ReelGridBuilder reelGridBuilder, bool generatePaylineObjects)
		{
			reelGridBuilder.supportPaylines = generatePaylineObjects;
		}

		/// <summary>
		/// Sets the reel configuration and layout
		/// NOTE: We do not support ghost reels at this time, not even sure why this concept continues to exist
		/// TODO: At this time the ReelGridBuilder doesn't serialize the Reel Configuration Type (reelConfiguration in editor class)
		///  therefore we can't assign it or the Hold And Spin Reel Render Order (ReelGridBuilder.leftToRight)
		/// </summary>
		/// <param name="entry">The reel window under processing</param>
		/// <param name="reelGridBuilder">The ReelGridBuilder to configure</param>
		private void SetReelConfiguration(ReelWindowDefinition entry, ReelGridBuilder reelGridBuilder, ReelConfigurationType reelConfigType)
		{
			if (reelConfigType.Equals(ReelConfigurationType.HoldAndSpin))
			{
				// for hold and spin reels:
				// - set the Reel Configuration Type to "HoldAndSpin"
				// - set the Hold And Spin Reel Render Order to entry.Layout
				reelGridBuilder.reelGridConfiguration = ReelGridConfiguration.HoldAndSpin;
				reelGridBuilder.holdAndSpinReelRenderOrder = GetHoldAndSpinReelRenderOrder(entry.Layout);
			}
			else
			{
				// for standard reels:
				// - set the Reel Configuration Type to "Standard"
				reelGridBuilder.reelGridConfiguration = ReelGridConfiguration.Standard;
			}
		}

		private HoldAndSpinReelRenderOrder GetHoldAndSpinReelRenderOrder(string layout)
		{
			switch (layout)
			{
				case "TopToBottom":
					return HoldAndSpinReelRenderOrder.TopToBottom;
				case "LeftToRight":
				default:
					return HoldAndSpinReelRenderOrder.LeftToRight;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="reelGridBuilder"></param>
		/// <exception cref="System.NotImplementedException"></exception>
		private void SetPrefabs(ReelWindowDefinition entry, ReelGridBuilder reelGridBuilder, bool generatePaylineObjects)
		{
			reelGridBuilder.reelViewObject = entry.StubData.ReelWindowConfiguration.ReelViewPrefab;
			reelGridBuilder.symbolsObject = entry.StubData.ReelWindowConfiguration.SymbolsPrefab;
			reelGridBuilder.symbolObject = entry.StubData.ReelWindowConfiguration.SymbolPrefab;
			reelGridBuilder.reelMaskMaterial = entry.StubData.ReelWindowConfiguration.ReelMaskMaterial;

			if (generatePaylineObjects)
			{
				reelGridBuilder.winLinesObject = entry.StubData.ReelWindowConfiguration.WinLinesPrefab;
				reelGridBuilder.winLineSymbolObject = entry.StubData.ReelWindowConfiguration.WinLineSymbolPrefab;
			}
		}

		/// <summary>
		/// Saves the state scriptable object and scene
		/// </summary>
		private void Save()
		{
			_state.Save();
			SceneManipulationHelper.SaveScene();
		}

		/// <summary>
		/// Returns true if the supplied reel heights all equal the supplied compareValue
		/// </summary>
		/// <param name="reelheights">Reel heights requiring evaluation</param>
		/// <param name="compareValue">Value to compare reel heights against</param>
		/// <returns></returns>
		private bool AreReelHeightsEquivalent(List<int> reelheights, int compareValue)
		{
			return reelheights.All(height => height.Equals(compareValue));
		}
	}
}
