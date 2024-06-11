using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class MechanicRemovalController
	{
		public MechanicRemovalController()
		{
		}

        public void Remove(MechanicConfiguration data)
        {
			var singleList = new List<MechanicConfiguration>()
			{ data };

			Remove(singleList);
        }

        public void Remove(List<MechanicConfiguration> data)
		{
			Debug.Log("Starting MechanicRemovalExecutor..");

			RemoveSceneElements(data);
            RemoveTriggers(data);
            RemoveSubGraphs(data);

			SceneManipulationHelper.SaveScene();

			Debug.Log("MechanicRemovalExecutor Completed!");
		}

		private void RemoveSubGraphs(List<MechanicConfiguration> configs)
		{
			// loop over each for subgraphs
			foreach (MechanicConfiguration config in configs)
			{
				List<SubGraphElement> subgraphs = config.SubGraphElements;
				foreach (SubGraphElement subgraph in subgraphs)
				{
					RemoveSubGraph(subgraph);
				}
			}
		}

		private void RemoveTriggers(List<MechanicConfiguration> configs)
		{
			// loop over each for triggers
			foreach (MechanicConfiguration config in configs)
			{
				List<TriggerElement> triggers = config.TriggerElements;
				foreach (TriggerElement trigger in triggers)
				{
					RemoveTrigger(trigger);
				}
			}
		}

		private void RemoveSceneElements(List<MechanicConfiguration> configs)
		{
			// loop over each for mechanic elements
			foreach (MechanicConfiguration config in configs)
			{
				List<MechanicElement> mechanicElements = config.MechanicElements;
				foreach (MechanicElement mechanicElement in mechanicElements)
				{
					RemoveMechanicElement(mechanicElement);
				}
			}
		}

		private void RemoveSubGraph(SubGraphElement subGraphElement)
		{
			StateMachineManipulationHelper.RemoveSubGraph(subGraphElement);
		}

		private void RemoveTrigger(TriggerElement trigger)
		{
			StateMachineManipulationHelper.RemoveTrigger(trigger);
		}

		private void RemoveMechanicElement(MechanicElement element)
		{
			switch (element.Type)
			{
				case SceneElementType.Component:
					SceneManipulationHelper.RemoveComponent(element.Component, element.Tag, element.ScenePath);
					if (element.StateNodes.Count > 0)
					{
						StateMachineManipulationHelper.RemoveStatePresenter(element.Component, element.StateNodes, element.Tag);
					}
					break;
				case SceneElementType.Prefab:
					SceneManipulationHelper.RemovePrefab(element.Prefab, element.ScenePath, element.Prefab.name);
					break;
				default:
					break;
			}
		}
	}
}
