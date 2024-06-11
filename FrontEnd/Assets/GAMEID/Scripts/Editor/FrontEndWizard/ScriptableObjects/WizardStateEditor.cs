using System;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CustomEditor(typeof(WizardState))]
	class WizardStateEditor : Editor
	{
		WizardState _state;

		private void OnEnable()
		{
			_state = target as WizardState;
			FrontEndWizardHelper.GetApplicableMechanicConfigurations();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			ShowHasExecuted();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowExecutorStates();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowReelWindowGenerationState();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			ShowMechanicStates();
		}

		private void ShowHasExecuted()
		{
			EditorGUILayout.LabelField("Execution State", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("HasExecuted: " + _state.HasExecuted);
			EditorGUILayout.Space();
		}

		private void ShowExecutorStates()
		{
			EditorGUILayout.LabelField("Executor States", EditorStyles.boldLabel);

			for (int index = 0; index < _state.ExecutorStates.Count; ++index)
			{
				ExecutorState entry = _state.ExecutorStates[index];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(entry.Name + ": " + entry.HasExecuted);

				if (GUILayout.Button("Toggle State"))
				{
					entry.HasExecuted = !entry.HasExecuted;
					EditorUtility.SetDirty(_state);
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		private void ShowReelWindowGenerationState()
		{
			EditorGUILayout.LabelField("Reel Window Generation State", EditorStyles.boldLabel);

			for (int index = 0; index < _state.ReelWindowGenerationStates.Count; ++index)
			{
				ReelWindowGenerationState entry = _state.ReelWindowGenerationStates[index];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(entry.ReelWindowName + ": " + entry.HasGenerated);

				if (GUILayout.Button("Toggle State"))
				{
					entry.HasGenerated = !entry.HasGenerated;
					EditorUtility.SetDirty(_state);
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		private void ShowMechanicStates()
		{
			EditorGUILayout.LabelField("Mechanic States", EditorStyles.boldLabel);

			EditorGUI.BeginChangeCheck();
			for (int index = 0; index < _state.MechanicStates.Count; ++index)
			{
				MechanicState entry = _state.MechanicStates[index];
				EditorGUILayout.LabelField(entry.Name + ":");

				++EditorGUI.indentLevel;
				entry.SubGraphStatus = (MechanicStatus)EditorGUILayout.EnumPopup("SubGraphs: ", entry.SubGraphStatus);
				entry.TriggerStatus = (MechanicStatus)EditorGUILayout.EnumPopup("Triggers: ", entry.TriggerStatus);
				entry.SceneElementStatus = (MechanicStatus)EditorGUILayout.EnumPopup("Scene Elements: ", entry.SceneElementStatus);
				--EditorGUI.indentLevel;

				EditorGUILayout.Space();
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_state);
			}
		}
	}
}
