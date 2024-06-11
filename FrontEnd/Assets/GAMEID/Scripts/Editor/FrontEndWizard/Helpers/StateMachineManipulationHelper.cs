using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public static class StateMachineManipulationHelper
	{
		/// <summary>
		/// Validates and then adds the supplied IStatePresenter to the targeted state machine nodes
		/// </summary>
		/// <param name="componentAssemblyQualifiedName"></param>
		/// <param name="stateNodes"></param>
		/// <param name="tag"></param>
		public static void AddStatePresenter(string componentAssemblyQualifiedName, List<string> stateNodes, string tag)
		{
			tag ??= "";

			Type componentType = Type.GetType(componentAssemblyQualifiedName);
			if (!componentType.GetInterfaces().Contains(typeof(IStatePresenter)))
			{
				Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Cannot add component (" + componentAssemblyQualifiedName + ") as it is not an IStatePresenter implementation!");
				return;
			}

			List<BaseNode> states = GetStateNodes();
			if (states == null)
			{
				return;
			}

			foreach (string node in stateNodes)
			{
				if (string.IsNullOrEmpty(node))
				{
					continue;
				}

				PresentationNode state = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(node)) as PresentationNode;
				if (state == null)
				{
					Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Missing state node (" + node + ") for " + componentAssemblyQualifiedName);
					continue;
				}

				MonoScript monoScript = SerializedType.GetScriptByTypeName(componentAssemblyQualifiedName);

				PresenterType newPresenter = new PresenterType()
				{
					script = monoScript,
					scriptName = "",
					value = componentAssemblyQualifiedName,
					tags = new string[] { tag }
				};

				state.activePresenters.Add(newPresenter);

				EditorUtility.SetDirty(state);
				AssetDatabase.SaveAssetIfDirty(state);
			}
		}

        /// <summary>
        /// Validates and then removes the supplied IStatePresenter at the targeted state machine nodes
        /// </summary>
        /// <param name="componentAssemblyQualifiedName"></param>
        /// <param name="stateNodes"></param>
        /// <param name="tag"></param>
        public static void RemoveStatePresenter(string componentAssemblyQualifiedName, List<string> stateNodes, string tag)
        {
            tag ??= "";

            Type componentType = Type.GetType(componentAssemblyQualifiedName);
            if (!componentType.GetInterfaces().Contains(typeof(IStatePresenter)))
            {
                Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Cannot add component (" + componentAssemblyQualifiedName + ") as it is not an IStatePresenter implementation!");
                return;
            }

            List<BaseNode> states = GetStateNodes();
            if (states == null)
            {
                return;
            }

            foreach (string node in stateNodes)
            {
                if (string.IsNullOrEmpty(node))
                {
                    continue;
                }

                PresentationNode state = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(node)) as PresentationNode;
                if (state == null)
                {
                    Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Missing state node (" + node + ") for " + componentAssemblyQualifiedName);
                    continue;
                }						

                MonoScript monoScript = SerializedType.GetScriptByTypeName(componentAssemblyQualifiedName);

				var presenters = state.activePresenters.FirstOrDefault(state => state.script.Equals(monoScript) && state.tags.Contains(tag));

				if (presenters != null)
				{
                    state.activePresenters.Remove(presenters);
                }

                EditorUtility.SetDirty(state);
                AssetDatabase.SaveAssetIfDirty(state);
            }
        }

        /// <summary>
        /// Adds the supplied trigger to the target state machine nodes and connects them
        /// </summary>
        /// <param name="element"></param>
        public static void AddTrigger(TriggerElement element)
		{
			List<SubGraphNode> subNodes = GetSubGraphs();
			List<BaseNode> states = GetStateNodes();
			if (states == null)
			{
				Debug.LogError(typeof(StateMachineManipulationHelper) + ": No state nodes found!");
				return;
			}

			foreach (StateTransition transition in element.StateNodes)
			{
				if (string.IsNullOrEmpty(transition.TargetNode))
				{
					Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot add trigger as the TargetNode is empty!");
					continue;
				}

				PresentationNode targetState = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(transition.TargetNode)) as PresentationNode;
				if (targetState == null)
				{
					Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot add trigger as the node (" + transition.TargetNode + ") does not exist!");
					continue;
				}

				targetState.triggersData.Add(element.Trigger);

				if (string.IsNullOrEmpty(transition.DestinationNode))
				{
					Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Cannot connect trigger as the DestinationNode is empty!");
					continue;
				}

				XNode.Node destinationNode = null;

				switch (transition.DestinationType)
				{
					case StateNodeType.StateModel:
						destinationNode = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(transition.DestinationNode));
						break;
					case StateNodeType.SubGraphNode:
						destinationNode = subNodes.FirstOrDefault(stateNode => stateNode.name.Equals(transition.DestinationNode));
						break;
					default:
						break;
				}

				if (destinationNode == null)
				{
					Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot connect SubGraphNode transition as the DestinationNode (" + transition.DestinationNode + ") does not exist!");
				}

				targetState.AddTransition(element.Trigger.Name, destinationNode);
				targetState.RefreshPorts();

				EditorUtility.SetDirty(targetState);
				AssetDatabase.SaveAssetIfDirty(targetState);
			}
		}

        /// <summary>
        /// Removes the supplied trigger at the target state machine node
        /// </summary>
        /// <param name="element"></param>
        public static void RemoveTrigger(TriggerElement element)
        {
            List<SubGraphNode> subNodes = GetSubGraphs();
            List<BaseNode> states = GetStateNodes();
            if (states == null)
            {
                Debug.LogError(typeof(StateMachineManipulationHelper) + ": No state nodes found!");
                return;
            }

            foreach (StateTransition transition in element.StateNodes)
            {
                if (string.IsNullOrEmpty(transition.TargetNode))
                {
                    Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot remove trigger as the TargetNode is empty!");
                    continue;
                }

                PresentationNode targetState = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(transition.TargetNode)) as PresentationNode;
                if (targetState == null)
                {
                    Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot remove trigger as the node (" + transition.TargetNode + ") does not exist!");
                    continue;
                }

                targetState.triggersData.Remove(element.Trigger);
                targetState.RefreshPorts();

                EditorUtility.SetDirty(targetState);
                AssetDatabase.SaveAssetIfDirty(targetState);
            }
        }

        /// <summary>
        /// Adds the supplied subgraph to the state machine and connects the exit transitions
        /// </summary>
        /// <param name="entry"></param>
        public static void AddSubGraph(SubGraphElement entry)
		{
			List<SubGraphNode> subGraphNodes = GetSubGraphs();
			if (subGraphNodes.Any(subNode => subNode.name.Equals(entry.FeatureSubgraph.name)))
			{
				Debug.LogWarning(typeof(StateMachineManipulationHelper) + ": Could not add subgraph " + entry.FeatureSubgraph.name + " as state machine already has one present!");
				return;
			}

			List<BaseNode> states = GetStateNodes();
			if (states == null)
			{
				return;
			}

			StateMachineModel stateMachine = GetStateMachineAsset();

			SubGraphNode subGraphNode = stateMachine.AddNode<SubGraphNode>();

			entry.FeatureSubgraph = FrontEndWizardHelper.GetAsset<Subgraph>(entry.FeatureSubgraph.name);

			subGraphNode.SubgraphAsset = entry.FeatureSubgraph;
			subGraphNode.SetSubgraphAssetParentGraph(stateMachine);
			entry.FeatureSubgraph.AssignedSubgraphNode = subGraphNode;
			subGraphNode.OnValidate();

			foreach (SubGraphExitTransition transition in entry.ExitTransitions)
			{
				if (string.IsNullOrEmpty(transition.DestinationNode))
				{
					continue;
				}

				XNode.Node destinationNode = null;

				switch (transition.DestinationType)
				{
					case StateNodeType.StateModel:
						destinationNode = states.FirstOrDefault(stateNode => stateNode.StateName.Equals(transition.DestinationNode));
						break;
					case StateNodeType.SubGraphNode:
						destinationNode = subGraphNodes.FirstOrDefault(stateNode => stateNode.name.Equals(transition.DestinationNode));
						break;
					default:
						break;
				}

				if (destinationNode == null)
				{
					Debug.LogError(typeof(StateMachineManipulationHelper) + ": Cannot connect SubGraphNode transition as the DestinationNode (" + transition.DestinationNode + ") does not exist!");
				}

				string exitKey = transition.ExitStatePort;
				subGraphNode.AddTransition(exitKey, destinationNode);
			}

			subGraphNode.RefreshPorts();

			EditorUtility.SetDirty(stateMachine);
			EditorUtility.SetDirty(subGraphNode);
			EditorUtility.SetDirty(entry.FeatureSubgraph);

			AssetDatabase.AddObjectToAsset(subGraphNode, stateMachine);

			AssetDatabase.SaveAssets();
		}

        /// <summary>
        /// Removes the supplied subgraph on the state machine and removes the exit transitions
        /// </summary>
        /// <param name="entry"></param>
        public static void RemoveSubGraph(SubGraphElement entry)
        {
            List<SubGraphNode> subGraphNodes = GetSubGraphs();
			var subGraphNodesToRemove = subGraphNodes.Where(subNode => subNode.name.Equals(entry.FeatureSubgraph.name));

            StateMachineModel stateMachine = GetStateMachineAsset();

            foreach (var nodeToRemove in subGraphNodesToRemove)
			{
                stateMachine.RemoveNode(nodeToRemove);
                AssetDatabase.RemoveObjectFromAsset(nodeToRemove);
            }

            EditorUtility.SetDirty(stateMachine);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Fetches the state machine asset
        /// </summary>
        /// <returns></returns>
        public static StateMachineModel GetStateMachineAsset()
		{
			return FrontEndWizardHelper.GetAsset<StateMachineModel>("");
		}

		/// <summary>
		/// Fetches all state machine nodes (StateModel)
		/// </summary>
		/// <returns></returns>
		public static List<BaseNode> GetStateNodes()
		{
			StateMachineModel stateMachine = GetStateMachineAsset();
			return stateMachine.States;
		}

		/// <summary>
		/// Fetches all subgraphs in the state machine (SubGraphNode)
		/// </summary>
		/// <returns></returns>
		public static List<SubGraphNode> GetSubGraphs()
		{
			StateMachineModel stateMachine = GetStateMachineAsset();
			List<SubGraphNode> subgraphNodes = stateMachine.nodes
				.Where((node) => node is SubGraphNode sm && sm != null)
				.Select(n => n as SubGraphNode)
				.ToList();
			return subgraphNodes;
		}

		public static List<BaseNode> GetNodesAndSubGraphs()
		{
			StateMachineModel stateMachine = GetStateMachineAsset();
			List<BaseNode> nodes = stateMachine.nodes
				.Where((node) => node is SubGraphNode sm && sm != null)
				.Select(n => n as BaseNode)
				.ToList();
			
			nodes.AddRange(stateMachine.States);

			return nodes;
		}
	}
}
