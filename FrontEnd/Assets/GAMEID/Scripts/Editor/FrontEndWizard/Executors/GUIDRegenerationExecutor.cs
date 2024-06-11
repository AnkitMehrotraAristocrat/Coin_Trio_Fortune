using UnityEditor.SceneManagement;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class GUIDRegenerationExecutor : BaseWizardExecutor
	{
		public GUIDRegenerationExecutor()
		{
			_canRerun = false;
		}

		public override void Execute(WizardInputData data)
		{
			// ensure slot and test harness scenes are not open (otherwise guid regeneration fails for the scene)
			FrontEndWizardHelper.CloseAllScenes();

			Debug.Log("Regenerating guids..");

			Jads.Tools.AssetGUIDRegeneratorMenu.RegenerateGUIDs();

			Debug.Log("GUID Regeneration complete!");

			FrontEndWizardHelper.LoadSlotScene(OpenSceneMode.Single);
			FrontEndWizardHelper.LoadScene("LocalTestHarness", OpenSceneMode.Additive);
		}
	}
}
