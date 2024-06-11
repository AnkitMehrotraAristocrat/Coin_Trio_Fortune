using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class PendingReelWindowPrompt : EditorWindow
	{
		private static Vector2 _scrollbarPosition = new Vector2();
		private WizardState _state = default;

		private Dictionary<string, GameObject> _gameObjectReferences = new Dictionary<string, GameObject>();

		public void OnEnable()
		{
			Initialize();
		}

		[MenuItem("Tools/NMG Vegas/Pending Reel Windows")]
		public static void ShowWindow()
		{
			var window = GetWindow<PendingReelWindowPrompt>();
			FrontEndWizardHelper.CenterOnMainWin(window, 3.0f, 2.0f);
			window.Show();
		}

		public void OnGUI()
		{
			GUIStyle style = FrontEndWizardHelper.GetDefaultStyle();
			_scrollbarPosition = GUILayout.BeginScrollView(_scrollbarPosition, style);

			ShowHeader();
			ShowReelWindowGenerationState(_state);
			ShowButtons();

			GUILayout.EndScrollView();
		}

		private void Initialize()
		{
			FrontEndWizardHelper.GetWizardState(out WizardState state);
			_state = state;
			FrontEndWizardHelper.RefreshReelWindowGeneratedStates(_state, out bool _);
		}

		private void ShowHeader()
		{
			// configure the label style
			int height = 30;
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = height;
			style.fixedHeight = height + 10;
			style.fontStyle = FontStyle.Bold;

			// draw the label
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("Reel Windows Pending Generation", style, GUILayout.ExpandWidth(true), GUILayout.MinHeight(height + 10));
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}

		private void ShowReelWindowGenerationState(WizardState state)
		{
			// configure the label style
			GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.fontStyle = FontStyle.Bold;

			// loop over each pending reel window
			foreach (ReelWindowGenerationState reelWindowEntry in state.FetchPendingReelWindowGenerationStates())
			{
				GameObject gameObject;
				if (!_gameObjectReferences.ContainsKey(reelWindowEntry.RootPath))
				{
					// get the root reel window object
					_gameObjectReferences.Add(reelWindowEntry.RootPath, SceneManipulationHelper.GetTargetObject(reelWindowEntry.RootPath));
				}
				gameObject = _gameObjectReferences[reelWindowEntry.RootPath];

				// draw the reel window name
				EditorGUILayout.LabelField(reelWindowEntry.ReelWindowName, labelStyle);

				// draw the root object field and a select button
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUILayout.ObjectField(gameObject, typeof(UnityEngine.Object), false);

				if (GUILayout.Button("Select"))
				{
					Selection.activeGameObject = gameObject;
				}

				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				// space entries out
				EditorGUILayout.Space();
			}
		}

		private void ShowButtons()
		{
			// refreshes reel window generation states
			GUILayout.Space(5f);
			if (GUILayout.Button("Refresh"))
			{
				FrontEndWizardHelper.RefreshReelWindowGeneratedStates(_state, out bool _);
			}

			// closes the window
			GUILayout.Space(5f);
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
}
