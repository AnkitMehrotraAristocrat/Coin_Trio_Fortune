using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CustomEditor(typeof(WizardConfiguration))]
	class WizardConfigurationEditor : Editor
	{
		private SerializedProperty _inputJson;
		private SerializedProperty _wizardState;
		private SerializedProperty _gameViewRoot;

		private ReorderableList _executorsList;
		//private bool showExecutorsList = true;

		private ReorderableList _rootReelWindowOptions;
		private ReorderableList _reelsConfigOptions;

		private Type[] _implementations;
		private int _implementationTypeIndex;

		private void OnEnable()
		{
			// initialize the properties
			_inputJson = serializedObject.FindProperty("_inputJson");
			_wizardState = serializedObject.FindProperty("_wizardState");
			_gameViewRoot = serializedObject.FindProperty("_gameViewRoot");
			SerializedProperty executors = serializedObject.FindProperty("Executors");
			SerializedProperty rootReelWindowOptions = serializedObject.FindProperty("_reelWindowRootOptions");
			SerializedProperty reelsConfigOptions = serializedObject.FindProperty("_reelWindowConfigurations");

			// configure the reorderable executors list
			_executorsList = new ReorderableList(serializedObject, executors, true, true, false, true);
			_executorsList.drawElementCallback = DrawExecutorListItems;
			_executorsList.drawHeaderCallback = DrawExectorHeader;
			_executorsList.elementHeightCallback = SetExecutorElementHeight;
			_executorsList.onAddCallback = null;

			// configure the reorderable root reel window object list
			_rootReelWindowOptions = new ReorderableList(serializedObject, rootReelWindowOptions, true, true, true, true);
			_rootReelWindowOptions.drawElementCallback = DrawRootReelWindowOptionsListItems;
			_rootReelWindowOptions.drawHeaderCallback = DrawRootReelWindowOptionsHeader;
			_rootReelWindowOptions.elementHeightCallback = SetRootReelWindowOptionsElementHeight;
			_rootReelWindowOptions.onAddCallback = null;

			// configure the reorderable reels prefab object list
			_reelsConfigOptions = new ReorderableList(serializedObject, reelsConfigOptions, true, true, true, true);
			_reelsConfigOptions.drawElementCallback = DrawReelsConfigOptionsListItems;
			_reelsConfigOptions.drawHeaderCallback = DrawReelsConfigOptionsHeader;
			_reelsConfigOptions.elementHeightCallback = SetReelsConfigOptionsElementHeight;
			_reelsConfigOptions.onAddCallback = null;
		}

		public override void OnInspectorGUI()
		{
			WizardConfiguration config = target as WizardConfiguration;
			if (config == null)
			{
				return;
			}

			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			ShowInputOptions();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowStateOptions();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowGameViewOptions();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowExecutorOptions(config);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(config);
			}

			if (serializedObject.ApplyModifiedProperties())
			{
				EditorUtility.SetDirty(config);
			}
		}

		private void ShowInputOptions()
		{
			EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_inputJson);
			EditorGUILayout.Space();
		}

		private void ShowStateOptions()
		{
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_wizardState);
			EditorGUILayout.Space();
		}

		private void ShowGameViewOptions()
		{
			EditorGUILayout.LabelField("Game View", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_gameViewRoot);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Reel Window Root Object Options", EditorStyles.boldLabel);
			_rootReelWindowOptions.DoLayoutList();

			EditorGUILayout.LabelField("Reels Prefab Options", EditorStyles.boldLabel);
			_reelsConfigOptions.DoLayoutList();
		}

		private void ShowExecutorOptions(WizardConfiguration config)
		{
			// draw a label
			EditorGUILayout.LabelField("Executor", EditorStyles.boldLabel);

			// initialzie or refresh the IWizardExecutor options list
			if (_implementations == null || GUILayout.Button("Refresh IWizardExecutor Options"))
			{
				_implementations = GetImplementations<BaseWizardExecutor>()
					.Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object)))
					.ToArray();
			}

			// draw the IWizardExecutor options list
			EditorGUILayout.Space();
			_implementationTypeIndex = EditorGUILayout.Popup(
				new GUIContent("Executor Options"),
				_implementationTypeIndex,
				_implementations.Select(impl => impl.FullName).ToArray());

			// draw the add executor button
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Executor", GUILayout.Width(200)))
			{
				config.Executors.Add((BaseWizardExecutor)Activator.CreateInstance(_implementations[_implementationTypeIndex]));
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// NOTE: Uncomment this stuff if you want a folding list
			//showExecutorsList = EditorGUILayout.Foldout(showExecutorsList, "Executors");
			EditorGUILayout.Space(12.0f);
			//if (showExecutorsList)
			//{
			_executorsList.DoLayoutList();
			//}
		}

		#region Reorderable List Callbacks
		private static Type[] GetImplementations<T>()
		{
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

			var interfaceType = typeof(T);
			return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
		}

		// Executors
		private void DrawExectorHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Executors List");
		}

		private void DrawExecutorListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _executorsList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.LabelField(rect, element.managedReferenceValue.ToString());
		}

		private float SetExecutorElementHeight(int index)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		// RootReelWindowNames
		private void DrawRootReelWindowOptionsHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Reel Window Root Options List");
		}

		private void DrawRootReelWindowOptionsListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _rootReelWindowOptions.serializedProperty.GetArrayElementAtIndex(index);

			rect.y += EditorGUIUtility.standardVerticalSpacing;
			rect.height = EditorGUIUtility.singleLineHeight;

			element.stringValue = EditorGUI.TextField(rect, element.stringValue);
		}

		private float SetRootReelWindowOptionsElementHeight(int index)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		// ReelsConfig
		private void DrawReelsConfigOptionsHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Reels Prefab Options List");
		}

		private void DrawReelsConfigOptionsListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _reelsConfigOptions.serializedProperty.GetArrayElementAtIndex(index);

			rect.y += EditorGUIUtility.standardVerticalSpacing;
			rect.height = EditorGUIUtility.singleLineHeight;

			element.objectReferenceValue = EditorGUI.ObjectField(rect, element.objectReferenceValue, typeof(ReelWindowConfiguration), false);
		}

		private float SetReelsConfigOptionsElementHeight(int index)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
		#endregion
	}
}