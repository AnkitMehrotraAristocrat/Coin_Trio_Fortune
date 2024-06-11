using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[InitializeOnLoad]
	public static class FrontEndWizardLoader
	{
		static FrontEndWizardLoader()
		{
			EditorApplication.update += Startup;
		}

		public static void Startup()
		{
			EditorApplication.update -= Startup;

			// fetch the config
			WizardConfiguration config = FrontEndWizardHelper.GetAsset<WizardConfiguration>("");

			// short circuit if no config is present
			if (config == null)
			{
				Debug.LogError("Wizard configuration file missing!");
				return;
			}

			// short circuit if no WizardState is hooked up on the config
			if (config.WizardState == null)
			{
				Debug.LogError("Wizard configuration's WizardState is not defined!", config);
				return;
			}

			// show the wizard window if it has not yet been executed and short circuit
			if (!config.WizardState.HasExecuted)
			{
				//FrontEndWizard.ShowWindow();
				return;
			}

			// refresh the reel window generation states and show the pending reel window generation window if any are pending
			FrontEndWizardHelper.RefreshReelWindowGeneratedStates(config.WizardState, out bool hasPendingReelWindowGeneration);
			if (hasPendingReelWindowGeneration)
			{
				PendingReelWindowPrompt.ShowWindow();
				return;
			}
		}
	}
}
