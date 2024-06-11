using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CustomEditor(typeof(MechanicConfiguration))]
	public class MechanicConfigurationEditor : Editor
	{
		private float _lineHeight => EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

		private ReorderableList _subGraphElements;
		private ReorderableList _triggerElements;
		private ReorderableList _mechanicElements;

		private Dictionary<string, ReorderableList> _subGraphElementTransitionsList = new Dictionary<string, ReorderableList>();
		private Dictionary<string, ReorderableList> _triggerElementTransitionsList = new Dictionary<string, ReorderableList>();
		private Dictionary<string, ReorderableList> _mechanicElementNodeList = new Dictionary<string, ReorderableList>();

		private void OnEnable()
		{
			InitializeSubGraphList();
			InitializeTriggersList();
			InitializeMechanicElementsList();
		}

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			ShowId();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			ShowSubGraphElements();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			ShowTriggerElements();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			ShowMechanicElements();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}

			if (serializedObject.ApplyModifiedProperties())
			{
				EditorUtility.SetDirty(target);
			}
		}

		#region Id Support
		private void ShowId()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_id"));
		}
		#endregion

		#region SubGraphElement List Support
		private void InitializeSubGraphList()
		{
			var subGraphElements = serializedObject.FindProperty("_subGraphElements");
			_subGraphElements = new ReorderableList(serializedObject, subGraphElements, true, true, true, true);
			_subGraphElements.drawElementCallback = DrawSubGraphElementsListItems;
			_subGraphElements.drawHeaderCallback = DrawSubGraphElementsExectorHeader;
			_subGraphElements.elementHeightCallback = SetSubGraphElementsElementHeight;
			_subGraphElements.onAddCallback = SubGraphElementsOnAddCallback;
		}

		private void ShowSubGraphElements()
		{
			_subGraphElements.DoLayoutList();
		}

		private void DrawSubGraphElementsListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _subGraphElements.serializedProperty.GetArrayElementAtIndex(index);
			SerializedProperty transitions = element.FindPropertyRelative("ExitTransitions");

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(GetRect(rect, 0), element.FindPropertyRelative("FeatureSubgraph"));
			Subgraph subGraph = element.FindPropertyRelative("FeatureSubgraph").objectReferenceValue as Subgraph;
			string listKey = element.propertyPath;
			if (EditorGUI.EndChangeCheck())
			{
				_subGraphElementTransitionsList.Remove(listKey);
				transitions.ClearArray();
			}

			if (subGraph != null)
			{
				ReorderableList subGraphTransitionList;
				if (_subGraphElementTransitionsList.ContainsKey(listKey))
				{
					subGraphTransitionList = _subGraphElementTransitionsList[listKey];
				}
				else
				{
					subGraphTransitionList = new ReorderableList(element.serializedObject, transitions, true, true, true, true)
					{
						drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
						{
							SerializedProperty transitionsElement = transitions.GetArrayElementAtIndex(index);

							transitionsElement.FindPropertyRelative("ExitStatePort").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
								GetRect(rect, 0),
								"Exit State Port",
								transitionsElement.FindPropertyRelative("ExitStatePort").stringValue,
								GUI.skin.textField,
								transitionsElement,
								_ => GetSubgraphConnectedExitStates(subGraph),
								"");

							string priorNodeName = transitionsElement.FindPropertyRelative("DestinationNode").stringValue;
							transitionsElement.FindPropertyRelative("DestinationNode").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
								GetRect(rect, 1),
								"Destination Node",
								transitionsElement.FindPropertyRelative("DestinationNode").stringValue,
								GUI.skin.textField,
								transitionsElement,
								_ => GetNodeAndSubGraphNodeNames(),
								"");

							if (!priorNodeName.Equals(transitionsElement.FindPropertyRelative("DestinationNode").stringValue))
							{
								var selectedNode = StateMachineManipulationHelper.GetNodesAndSubGraphs().FirstOrDefault(node => node.name.Equals(transitionsElement.FindPropertyRelative("DestinationNode").stringValue));
								if (selectedNode != null)
								{
									switch (selectedNode.GetType().Name)
									{
										case "SubGraphNode":
											transitionsElement.FindPropertyRelative("DestinationType").enumValueIndex = (int)StateNodeType.SubGraphNode;
											break;
										case "StateModel":
										default:
											transitionsElement.FindPropertyRelative("DestinationType").enumValueIndex = (int)StateNodeType.StateModel;
											break;
									}
								}
							}
						},

						drawHeaderCallback = (Rect rect) =>
						{
							EditorGUI.LabelField(rect, "Transitions List");
						},

						elementHeightCallback = (int index) =>
						{
							return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						},

						onAddCallback = (ReorderableList list) =>
						{
							++list.serializedProperty.arraySize;
							int lastIndex = list.serializedProperty.arraySize - 1;
							SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);

							newEntry.FindPropertyRelative("ExitStatePort").stringValue = "";
							newEntry.FindPropertyRelative("DestinationNode").stringValue = "";
							newEntry.FindPropertyRelative("DestinationType").enumValueIndex = 0;
						}
					};
					_subGraphElementTransitionsList[listKey] = subGraphTransitionList;
				}

				subGraphTransitionList.DoList(GetRect(rect, 1));
			}

			if (EditorGUI.EndChangeCheck())
			{
				element.serializedObject.ApplyModifiedProperties();
			}
		}

		private void DrawSubGraphElementsExectorHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "SubGraph Elements List");
		}

		private float SetSubGraphElementsElementHeight(int index)
		{
			int rowCount = 1;
			SerializedProperty element = _subGraphElements.serializedProperty.GetArrayElementAtIndex(index);
			var subGraph = element.FindPropertyRelative("FeatureSubgraph").objectReferenceValue;

			if (subGraph != null)
			{
				rowCount += 2; // add two rows for empty table
				int arraySize = element.FindPropertyRelative("ExitTransitions").arraySize;
				rowCount += arraySize > 0 ? arraySize * 2 : 1; // add 2 rows per entry or only 1 row for empty table
			}

			return (rowCount * _lineHeight) + (EditorGUIUtility.standardVerticalSpacing * 2) + ReorderableList.Defaults.padding;
		}

		private void SubGraphElementsOnAddCallback(ReorderableList list)
		{
			++list.serializedProperty.arraySize;
			int lastIndex = list.serializedProperty.arraySize - 1;
			SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);

			newEntry.FindPropertyRelative("FeatureSubgraph").objectReferenceValue = null;
			newEntry.FindPropertyRelative("ExitTransitions").ClearArray();
		}
		#endregion

		#region TriggerElement List Support
		private void InitializeTriggersList()
		{
			var triggerElements = serializedObject.FindProperty("_triggerElements");
			_triggerElements = new ReorderableList(serializedObject, triggerElements, true, true, true, true);
			_triggerElements.drawElementCallback = DrawTriggerElementsListItems;
			_triggerElements.drawHeaderCallback = DrawTriggerElementsExectorHeader;
			_triggerElements.elementHeightCallback = SetTriggerElementsElementHeight;
			_triggerElements.onAddCallback = TriggerElementsOnAddCallback;
		}

		private void ShowTriggerElements()
		{
			_triggerElements.DoLayoutList();
		}

		private void DrawTriggerElementsListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _triggerElements.serializedProperty.GetArrayElementAtIndex(index);
			SerializedTrigger trigger = element.FindPropertyRelative("Trigger").objectReferenceValue as SerializedTrigger;
			SerializedProperty transitions = element.FindPropertyRelative("StateNodes");

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(GetRect(rect, 0), element.FindPropertyRelative("Trigger"));

			if (trigger != null)
			{
				string listKey = element.propertyPath;
				ReorderableList triggerTransitionList;
				if (_triggerElementTransitionsList.ContainsKey(listKey))
				{
					triggerTransitionList = _triggerElementTransitionsList[listKey];
				}
				else
				{
					triggerTransitionList = new ReorderableList(element.serializedObject, transitions, true, true, true, true)
					{
						drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
						{
							SerializedProperty transitionsElement = transitions.GetArrayElementAtIndex(index);

							transitionsElement.FindPropertyRelative("TargetNode").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
								GetRect(rect, 0),
								"Target Node",
								transitionsElement.FindPropertyRelative("TargetNode").stringValue,
								GUI.skin.textField,
								transitionsElement,
								_ => GetNodeAndSubGraphNodeNames(),
								transitionsElement.FindPropertyRelative("TargetNode").stringValue,
								true);

							string priorNodeName = transitionsElement.FindPropertyRelative("DestinationNode").stringValue;
							transitionsElement.FindPropertyRelative("DestinationNode").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
								GetRect(rect, 1),
								"Destination Node",
								transitionsElement.FindPropertyRelative("DestinationNode").stringValue,
								GUI.skin.textField,
								transitionsElement,
								_ => GetNodeAndSubGraphNodeNames(),
								transitionsElement.FindPropertyRelative("DestinationNode").stringValue,
								true);

							if (!priorNodeName.Equals(transitionsElement.FindPropertyRelative("DestinationNode").stringValue))
							{
								var selectedNode = StateMachineManipulationHelper.GetNodesAndSubGraphs().FirstOrDefault(node => node.name.Equals(transitionsElement.FindPropertyRelative("DestinationNode").stringValue));
								if (selectedNode != null)
								{
									switch (selectedNode.GetType().Name)
									{
										case "StateModel":
											transitionsElement.FindPropertyRelative("DestinationType").enumValueIndex = (int)StateNodeType.StateModel;
											break;
										case "SubGraphNode":
											transitionsElement.FindPropertyRelative("DestinationType").enumValueIndex = (int)StateNodeType.SubGraphNode;
											break;
										default:
											break;
									}
								}
							}
						},

						drawHeaderCallback = (Rect rect) =>
						{
							EditorGUI.LabelField(rect, "Transitions List");
						},

						elementHeightCallback = (int index) =>
						{
							return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
						},

						onAddCallback = (ReorderableList list) =>
						{
							++list.serializedProperty.arraySize;
							int lastIndex = list.serializedProperty.arraySize - 1;
							SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);

							newEntry.FindPropertyRelative("TargetNode").stringValue = "";
							newEntry.FindPropertyRelative("DestinationNode").stringValue = "";
							newEntry.FindPropertyRelative("DestinationType").enumValueIndex = 0;
						}
					};
					_triggerElementTransitionsList[listKey] = triggerTransitionList;
				}

				triggerTransitionList.DoList(GetRect(rect, 1));
			}

			if (EditorGUI.EndChangeCheck())
			{
				element.serializedObject.ApplyModifiedProperties();
			}
		}

		private void DrawTriggerElementsExectorHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Trigger Elements List");
		}

		private float SetTriggerElementsElementHeight(int index)
		{
			int rowCount = 1;
			SerializedProperty element = _triggerElements.serializedProperty.GetArrayElementAtIndex(index);
			var subGraph = element.FindPropertyRelative("Trigger").objectReferenceValue;

			if (subGraph != null)
			{
				rowCount += 2; // add two rows for empty table
				int arraySize = element.FindPropertyRelative("StateNodes").arraySize;
				rowCount += arraySize > 0 ? arraySize * 2 : 1; // add 2 rows per entry or only 1 row for empty table
			}

			return (rowCount * _lineHeight) + (EditorGUIUtility.standardVerticalSpacing * 2) + ReorderableList.Defaults.padding;
		}

		private void TriggerElementsOnAddCallback(ReorderableList list)
		{
			++list.serializedProperty.arraySize;
			int lastIndex = list.serializedProperty.arraySize - 1;
			SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);

			newEntry.FindPropertyRelative("Trigger").objectReferenceValue = null;
			newEntry.FindPropertyRelative("StateNodes").ClearArray();
		}
		#endregion

		#region MechanicElement List Support
		private void InitializeMechanicElementsList()
		{
			var mechanicElements = serializedObject.FindProperty("_mechanicElements");
			_mechanicElements = new ReorderableList(serializedObject, mechanicElements, true, true, true, true);
			_mechanicElements.drawElementCallback = DrawMechanicElementsListItems;
			_mechanicElements.drawHeaderCallback = DrawMechanicElementsExectorHeader;
			_mechanicElements.elementHeightCallback = SetMechanicElementsElementHeight;
			_mechanicElements.onAddCallback = MechanicElementsOnAddCallback;
		}

		private void ShowMechanicElements()
		{
			_mechanicElements.DoLayoutList();
		}

		private void DrawMechanicElementsListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _mechanicElements.serializedProperty.GetArrayElementAtIndex(index);
			SerializedProperty stateMachineNodes = element.FindPropertyRelative("StateNodes");
			int elementIndex = 0;

			EditorGUI.BeginChangeCheck();

			SerializedProperty type = element.FindPropertyRelative("Type");
			EditorGUI.PropertyField(GetRect(rect, elementIndex), element.FindPropertyRelative("Type"));

			if (EditorGUI.EndChangeCheck())
			{
				ResetMechanicElement(element);
			}

			EditorGUI.BeginChangeCheck();

			bool isComponent = element.FindPropertyRelative("Type").enumValueIndex == (int)SceneElementType.Component;
			if (isComponent)
			{
				//Component
				element.FindPropertyRelative("Component").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
					GetRect(rect, ++elementIndex),
					"Component",
					element.FindPropertyRelative("Component").stringValue,
					GUI.skin.textField,
					element,
					_ => GetAllComponents(),
					element.FindPropertyRelative("Component").stringValue,
					false,
					false);
			}

			if (isComponent && !string.IsNullOrEmpty(element.FindPropertyRelative("Component").stringValue))
			{
				//Tag
				EditorGUI.PropertyField(GetRect(rect, ++elementIndex), element.FindPropertyRelative("Tag")/*, new GUIContent("Tag") <-- adds label*/);
			}

			if (!isComponent)
			{
				//Prefab
				element.FindPropertyRelative("Prefab").objectReferenceValue = EditorGUI.ObjectField(GetRect(rect, ++elementIndex), "Prefab", element.FindPropertyRelative("Prefab").objectReferenceValue, typeof(GameObject), false);
			}

			if (!string.IsNullOrEmpty(element.FindPropertyRelative("Component").stringValue) || element.FindPropertyRelative("Prefab").objectReferenceValue != null)
			{
				//ScenePath
				element.FindPropertyRelative("ScenePath").stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
					GetRect(rect, ++elementIndex),
					"Scene Path",
					element.FindPropertyRelative("ScenePath").stringValue,
					GUI.skin.textField,
					element,
					_ => GetAllGameObjectPaths(),
					element.FindPropertyRelative("ScenePath").stringValue,
					true,
					true);
			}

			if (!string.IsNullOrEmpty(element.FindPropertyRelative("Component").stringValue) || element.FindPropertyRelative("Prefab").objectReferenceValue != null)
			{
                //Replace Existing
                EditorGUI.PropertyField(GetRect(rect, ++elementIndex), element.FindPropertyRelative("ReplaceExisting")/*, new GUIContent("Tag") <-- adds label*/);
            }

            if (isComponent && !string.IsNullOrEmpty(element.FindPropertyRelative("Component").stringValue))
			{
				//StateNodes
				string listKey = element.propertyPath;
				ReorderableList stateMachineNodeList;
				if (_mechanicElementNodeList.ContainsKey(listKey))
				{
					stateMachineNodeList = _mechanicElementNodeList[listKey];
				}
				else
				{
					stateMachineNodeList = new ReorderableList(element.serializedObject, stateMachineNodes, true, true, true, true)
					{
						drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
						{
							SerializedProperty stateMachineNodeElement = stateMachineNodes.GetArrayElementAtIndex(index);

							stateMachineNodeElement.stringValue = WizardAutoCompleteTextField.EditorGUI.NMGAutoCompleteTextField(
								GetRect(rect, 0),
								"",
								stateMachineNodeElement.stringValue,
								GUI.skin.textField,
								stateMachineNodeElement,
								_ => GetNodeNames(),
								stateMachineNodeElement.stringValue);
						},

						drawHeaderCallback = (Rect rect) =>
						{
							EditorGUI.LabelField(rect, "State Machine Node List");
						},

						elementHeightCallback = (int index) =>
						{
							return EditorGUIUtility.singleLineHeight;
						},

						onAddCallback = (ReorderableList list) =>
						{
							++list.serializedProperty.arraySize;
							int lastIndex = list.serializedProperty.arraySize - 1;
							SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);
							newEntry.stringValue = "";
						}
					};
					_mechanicElementNodeList[listKey] = stateMachineNodeList;
				}

				stateMachineNodeList.DoList(GetRect(rect, ++elementIndex));
			}

			if (EditorGUI.EndChangeCheck())
			{
				element.serializedObject.ApplyModifiedProperties();
			}
		}

		private string[] GetAllComponents()
		{
			Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			List<string> components = new List<string>() { "" }; // initializing with an empty entry so we can clear it out if desired

			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<string> assemblyQualifiedNames = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(MonoBehaviour))).Select(target => target.AssemblyQualifiedName);
				components.AddRange(assemblyQualifiedNames);
			}
			components.Sort();
			return components.ToArray();
		}

		private string[] GetAllGameObjectPaths()
		{
			List<string> paths = new List<string>() { "" }; // initializing with an empty entry so we can clear it out if desired
			Scene slotScene = FrontEndWizardHelper.GetActiveSlotScene();

			foreach (var root in slotScene.GetRootGameObjects())
			{
				GetObjectPaths(root, ref paths);
			}

			AddSelectedPaths(ref paths);

			paths.Sort();
			return paths.ToArray();
		}

		private void GetObjectPaths(GameObject current, ref List<string> paths)
		{
			string path = GetGameObjectPath(current, "/");
			//paths.Add(path);
			paths.Add(path + "/");

			for (int childIndex = 0; childIndex < current.transform.childCount; ++childIndex)
			{
				GetObjectPaths(current.transform.GetChild(childIndex).gameObject, ref paths);
			}
		}

		private string GetGameObjectPath(GameObject gameObject, string seperator = "/", bool includeName = true)
		{
			string path = includeName ? gameObject.name : "";
			Transform transform = gameObject.transform;
			while (transform.parent != null)
			{
				path = transform.parent.name + seperator + path;
				transform = transform.parent;
			}
			return path;
		}

		private void AddSelectedPaths(ref List<string> paths)
		{
			int elementsCount = _mechanicElements.serializedProperty.arraySize;
			for (int elementIndex = 0; elementIndex < elementsCount; ++elementIndex)
			{
				SerializedProperty element = _mechanicElements.serializedProperty.GetArrayElementAtIndex(elementIndex);

				string path = element.FindPropertyRelative("ScenePath").stringValue;

				string[] pathArray = path.Split('/');

				string partialPath = "";
				for (int pathIndex = 0; pathIndex < pathArray.Length; ++pathIndex)
				{
					partialPath += pathArray[pathIndex];

					if (paths.FirstOrDefault(pathsEntry => pathsEntry.Contains(partialPath)) == null)
					{
						//paths.Add(partialPath);
						paths.Add(partialPath + "/");
					}

					partialPath += "/";
				}
			}
		}

		private void DrawMechanicElementsExectorHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Mechanic Elements List");
		}

		private float SetMechanicElementsElementHeight(int index)
		{
			SerializedProperty element = _mechanicElements.serializedProperty.GetArrayElementAtIndex(index);
			bool isComponent = element.FindPropertyRelative("Type").enumValueIndex == (int)SceneElementType.Component;
			int rowCount = 2;

			if (isComponent && !string.IsNullOrEmpty(element.FindPropertyRelative("Component").stringValue))
			{
				rowCount = 7;
				int arraySize = element.FindPropertyRelative("StateNodes").arraySize;
				rowCount += arraySize > 0 ? arraySize : 1;
			}
			else if (element.FindPropertyRelative("Prefab").objectReferenceValue != null)
			{
				rowCount = 4;
			}

			return (rowCount * _lineHeight) + (EditorGUIUtility.standardVerticalSpacing * 2) + ReorderableList.Defaults.padding;
		}

		private void MechanicElementsOnAddCallback(ReorderableList list)
		{
			++list.serializedProperty.arraySize;
			int lastIndex = list.serializedProperty.arraySize - 1;
			SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);
			ResetMechanicElement(newEntry, true);
		}

		private void ResetMechanicElement(SerializedProperty entry, bool includeType = false)
		{
			if (includeType)
			{
				entry.FindPropertyRelative("Type").enumValueIndex = 0;
			}
			entry.FindPropertyRelative("Component").stringValue = "";
			entry.FindPropertyRelative("Tag").stringValue = "";
			entry.FindPropertyRelative("Prefab").objectReferenceValue = null;
			entry.FindPropertyRelative("ScenePath").stringValue = "";
			entry.FindPropertyRelative("StateNodes").ClearArray();
		}
		#endregion

		#region General Helpers
		private string[] GetSubgraphConnectedExitStates(Subgraph subGraph)
		{
			List<string> names = new List<string>() { "" };
			names.AddRange(subGraph.ExitNode.GetConnectedStates());
			return names.OrderBy(name => name).ToArray();
		}

		private string[] GetNodeAndSubGraphNodeNames()
		{
			List<string> names = new List<string>() { "" };
			names.AddRange(StateMachineManipulationHelper.GetNodesAndSubGraphs().Select(node => node.name));
			return names.OrderBy(name => name).ToArray();
		}

		private string[] GetNodeNames()
		{
			List<string> names = new List<string>() { "" };
			names.AddRange(StateMachineManipulationHelper.GetStateNodes().Select(node => node.name));
			return names.OrderBy(name => name).ToArray();
		}

		private Rect GetRect(Rect rect, int elementIndex)
		{
			return new Rect(rect.x, rect.y + _lineHeight * elementIndex, rect.width, EditorGUIUtility.singleLineHeight);
		}
		#endregion
	}
}
