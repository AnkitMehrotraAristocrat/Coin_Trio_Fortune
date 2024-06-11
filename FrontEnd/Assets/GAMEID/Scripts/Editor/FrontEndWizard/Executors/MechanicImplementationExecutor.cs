using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class MechanicImplementationExecutor : BaseWizardExecutor
	{
		public MechanicImplementationExecutor()
		{
			_canRerun = true; // do we want to be able to re-execute this?
		}

		public override void Execute(WizardInputData data)
		{
			Debug.Log("Starting MechanicImplementationExecutor..");

			FrontEndWizardHelper.GetWizardState(out WizardState wizardState);

			// find all existing configuration files
			List<MechanicConfiguration> configs = FrontEndWizardHelper.GetApplicableMechanicConfigurations(data, wizardState);

			AddSubGraphs(configs, wizardState);
			AddTriggers(configs, wizardState);
			AddSceneElements(configs, wizardState);

			SceneManipulationHelper.SaveScene();

			Debug.Log("MechanicImplementationExecutor Completed!");
		}

		private void AddSubGraphs(List<MechanicConfiguration> configs, WizardState wizardState)
		{
			// loop over each for subgraphs
			foreach (MechanicConfiguration config in configs)
			{
				if (!ShouldExecuteStage(config.Id, MechanicStage.SubGraphs, wizardState.GetMechanicState(config.Id).SubGraphStatus))
				{
					continue;
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.SubGraphs, MechanicStatus.Attempted);

				List<SubGraphElement> subgraphs = config.SubGraphElements;
				foreach (SubGraphElement subgraph in subgraphs)
				{
					AddSubGraph(subgraph);
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.SubGraphs, MechanicStatus.Completed);
			}
		}

		private void AddTriggers(List<MechanicConfiguration> configs, WizardState wizardState)
		{
			// loop over each for triggers
			foreach (MechanicConfiguration config in configs)
			{
				if (!ShouldExecuteStage(config.Id, MechanicStage.Triggers, wizardState.GetMechanicState(config.Id).TriggerStatus))
				{
					continue;
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.Triggers, MechanicStatus.Attempted);

				List<TriggerElement> triggers = config.TriggerElements;
				foreach (TriggerElement trigger in triggers)
				{
					AddTrigger(trigger);
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.Triggers, MechanicStatus.Completed);
			}
		}

		private void AddSceneElements(List<MechanicConfiguration> configs, WizardState wizardState)
		{
			// loop over each for mechanic elements
			foreach (MechanicConfiguration config in configs)
			{
				if (!ShouldExecuteStage(config.Id, MechanicStage.SceneElements, wizardState.GetMechanicState(config.Id).SceneElementStatus))
				{
					continue;
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.SceneElements, MechanicStatus.Attempted);

				List<MechanicElement> mechanicElements = config.MechanicElements;
				foreach (MechanicElement mechanicElement in mechanicElements)
				{
					AddMechanicElement(mechanicElement);
				}

				wizardState.SetMechanicState(config.Id, MechanicStage.SceneElements, MechanicStatus.Completed);
			}
		}

		private void AddSubGraph(SubGraphElement subGraphElement)
		{
			StateMachineManipulationHelper.AddSubGraph(subGraphElement);
		}

		private void AddTrigger(TriggerElement trigger)
		{
			StateMachineManipulationHelper.AddTrigger(trigger);
		}

		private void AddMechanicElement(MechanicElement element)
		{
			switch (element.Type)
			{
				case SceneElementType.Component:
					SceneManipulationHelper.AddComponent(element.Component, element.Tag, element.ScenePath, element.ReplaceExisting); // replace existing??
					if (element.StateNodes.Count > 0)
					{
						StateMachineManipulationHelper.AddStatePresenter(element.Component, element.StateNodes, element.Tag);
					}
					break;
				case SceneElementType.Prefab:
					SceneManipulationHelper.AddPrefab(element.Prefab, element.ScenePath, element.Prefab.name, true, element.ReplaceExisting);
					break;
				default:
					break;
			}
		}

		private bool ShouldExecuteStage(string id, MechanicStage stage, MechanicStatus status)
		{
			if (status.Equals(MechanicStatus.Pending))
			{
				return true;
			}

			if (status.Equals(MechanicStatus.Attempted))
			{
				Debug.LogWarning("Skipped adding " + stage.ToString() + " for " + id + " as the current status was " + MechanicStatus.Attempted.ToString() + " (you will need to add these elements manually or go super user and remove these elements, update the state and attempt again)");
				return false;
			}

			if (status.Equals(MechanicStatus.Completed))
			{
				Debug.Log("Skipped adding " + stage.ToString() + " for " + id + " as the current status was " + MechanicStatus.Completed.ToString());
				return false;
			}
			return false;
		}
	}
}
