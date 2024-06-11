using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public class FrontEndWizard : EditorWindow
	{
		private static Vector2 _scrollbarPosition = new Vector2();
		private bool _initialized = false;
		private WizardConfiguration _wizardConfiguration;
		private WizardInputData _inputData;
		private WizardState _state;

		private FrontEndWizardRemoval _wizardRemoval;

        /// <summary>
        /// Menu item to display the window.
        /// </summary>
        [MenuItem("Tools/NMG Vegas/Wizard")]
		public static void ShowWindow()
		{
			var window = GetWindow<FrontEndWizard>();
			FrontEndWizardHelper.CenterOnMainWin(window, 2.0f, 2.0f);
			window.Show();
		}

		public void OnEnable()
		{
			Initialize();
		}

		/// <summary>
		/// Main OnGUI loop.
		/// </summary>
		public void OnGUI()
		{
			GUIStyle style = FrontEndWizardHelper.GetDefaultStyle();
			_scrollbarPosition = GUILayout.BeginScrollView(_scrollbarPosition, style);

			ShowHeader();
			ShowState();
			ShowReelWindowOptions();

			if (!_state.HasExecuted)
			{
				ShowRecommendations();
				ShowExecuteButton();
			}
			else
			{
				ShowExecutorOptions();
			}

			ShowCloseButton();

			GUILayout.EndScrollView();
		}

		private void Initialize()
		{
			if (!FrontEndWizardHelper.GetWizardConfig(out _wizardConfiguration))
			{
				Debug.LogError("Wizard configuration file missing!");
				return;
			}

			if (!FrontEndWizardHelper.GetInputData(out _inputData, _wizardConfiguration))
			{
				Debug.LogError("Failed to fetch input data!");
				return;
			}

			if (!FrontEndWizardHelper.GetWizardState(out _state, _wizardConfiguration))
			{
				Debug.LogError("Failed to fetch WizardState!");
				return;
			}

			// add an executor for each present on the wizard configuration
			_wizardConfiguration.Executors.ForEach(executor => _state.AddExecutorState(executor.GetType().Name));

			_wizardRemoval = new FrontEndWizardRemoval();

            _initialized = true;
		}

		private void ShowHeader()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 40;
			style.fixedHeight = 40;
			style.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("FRONT END WIZARD", style, GUILayout.ExpandWidth(true), GUILayout.MinHeight(40));
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}

		private void ShowState()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.fixedHeight = 40;
			style.fontSize = 20;
			style.fontStyle = FontStyle.Bold;
			style.normal.textColor = Color.black;

			string statusText;
			if (_state.HasExecuted)
			{
				statusText = "Execution Completed";
				style.normal.background = MakeTex(1, 1, CustomColor.emeraldGreen); // Emerald Green
			}
			else
			{
				statusText = "Pending Execution";
				style.normal.background = MakeTex(1, 1, CustomColor.orangePeel); // Orange Peel
			}

			EditorGUILayout.LabelField(statusText, style, GUILayout.ExpandWidth(true), GUILayout.MinHeight(40));
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}

		private void ShowRecommendations()
		{
			string message = "Prior to execution, ensure the following:\n" +
				"- The Audio Mixer tab is closed (may impact GUID regeneration)";
			float height = message.Split('\n').Length * EditorGUIUtility.singleLineHeight;

			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.fixedHeight = height;

			EditorGUILayout.LabelField(message, style, GUILayout.MinHeight(height));
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}

		private void ShowReelWindowOptions()
		{
			float height = 20;
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.UpperCenter;
			style.fixedHeight = height;
			style.fontSize = 14;
			style.fontStyle = FontStyle.Bold;
			style.normal.textColor = Color.black;
			style.normal.background = MakeTex(1, 1, CustomColor.rajah);
			EditorGUILayout.LabelField("Reel Window Options", style, GUILayout.ExpandWidth(true), GUILayout.MinHeight(height));

			GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
			headerStyle.alignment = TextAnchor.UpperCenter;
			headerStyle.fixedHeight = height;
			headerStyle.fontStyle = FontStyle.Bold;

			GUIStyle windowNameStyle = new GUIStyle(GUI.skin.label);
			windowNameStyle.alignment = TextAnchor.UpperCenter;
			windowNameStyle.fixedHeight = height;

			GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
			toggleStyle.alignment = TextAnchor.MiddleCenter;
			toggleStyle.fixedHeight = height;

			int reelWindowNameColumnWidth = 140;
			int rootGameObjectColumnWidth = 150;
			int prefabColumnWidth = 150;
			int generateColumnWidth = 100;
			int columnGapWidth = 15;
			int toggleWidth = 15;

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Label("Reel Window Name", headerStyle, GUILayout.Width(reelWindowNameColumnWidth));

			EditorGUILayout.Space(columnGapWidth);

			GUILayout.Label("Root Game Object", headerStyle, GUILayout.Width(rootGameObjectColumnWidth));

			EditorGUILayout.Space(columnGapWidth);

			GUILayout.Label("Prefab", headerStyle, GUILayout.Width(prefabColumnWidth));

			EditorGUILayout.Space(columnGapWidth);

			GUILayout.Label("Generate", headerStyle, GUILayout.Width(generateColumnWidth));

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			foreach (var entry in _inputData.ReelWindows)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				// draw the reel window name
				GUILayout.Label(entry.Name, windowNameStyle, GUILayout.Width(reelWindowNameColumnWidth));

				EditorGUILayout.Space(columnGapWidth);

				// draw the select field to identify which root game object this should populate under
				entry.StubData.RootGameObjectIndex = EditorGUILayout.Popup(entry.StubData.RootGameObjectIndex, _wizardConfiguration.ReelWindowRootOptions.ToArray(), GUILayout.Width(rootGameObjectColumnWidth));
				if (_wizardConfiguration.ReelWindowRootOptions.Count > 0)
				{
					entry.StubData.RootGameObjectName = _wizardConfiguration.ReelWindowRootOptions[entry.StubData.RootGameObjectIndex];
				}

				EditorGUILayout.Space(columnGapWidth);

				// draw the select field to identify which Reels prefab to use
				entry.StubData.ReelsPrefabIndex = EditorGUILayout.Popup(entry.StubData.ReelsPrefabIndex, _wizardConfiguration.ReelWindowConfigurations.Select(config => config.Name).ToArray(), GUILayout.Width(prefabColumnWidth));
				if (_wizardConfiguration.ReelWindowConfigurations.Count > 0)
				{
					entry.StubData.ReelWindowConfiguration = _wizardConfiguration.ReelWindowConfigurations[entry.StubData.ReelsPrefabIndex];
				}

				EditorGUILayout.Space(columnGapWidth);

				// draw the generate toggle
				EditorGUILayout.BeginHorizontal(GUILayout.Width(generateColumnWidth));
				GUILayout.FlexibleSpace();
				entry.StubData.Generate = EditorGUILayout.Toggle("", entry.StubData.Generate, toggleStyle, GUILayout.Width(toggleWidth));
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();

				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		}

		private void ShowExecuteButton()
		{
			if (GUILayout.Button("Execute"))
			{
				Execute();
			}
		}

		private void ShowExecutorOptions()
		{
			foreach (BaseWizardExecutor executor in _wizardConfiguration.Executors)
			{
				if (!executor.CanRerun)
				{
					continue;
				}

				string name = executor.GetType().Name;

				if (GUILayout.Button("Rerun " + name))
				{
					if (EditorUtility.DisplayDialog("Wizard Execute Confirmation", "Ready to re-execute " + name + "?", "Yep!", "Nah.."))
					{
						executor.Execute(_inputData);
					}
				}
			}
		}

		private void ShowCloseButton()
		{
			GUILayout.Space(5f);
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}

		private void Execute()
		{
			if (ConfigValidationPassed() && EditorUtility.DisplayDialog("Wizard Execute Confirmation", "Ready to execute?", "Yep!", "Nah.."))
			{
                if (!_state.HasExecuted)
                {
                    _wizardRemoval.RemoveAll();
                }

                // executes all IWizardExecutor implementations found in the wizard executor config
                _wizardConfiguration.Executors.ForEach(executor =>
				{
					executor.Execute(_inputData);
					_state.SetExecutorState(executor.GetType().Name, true);
				});

				_state.SetHasExecuted();
				_state.Save();

				FrontEndWizardHelper.RefreshReelWindowGeneratedStates(_state, out bool hasPending);
				if (hasPending)
				{
					PendingReelWindowPrompt.ShowWindow();
				}

				Close();
			}
		}

		private Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
				pix[i] = col;

			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();

			return result;
		}

		private bool ConfigValidationPassed()
		{
			if (!_initialized)
			{
				return false;
			}

			List<string> executorClassNames = _wizardConfiguration.Executors.Select(executor => executor.GetType().FullName).ToList();

			if (executorClassNames.Distinct().Count() != executorClassNames.Count())
			{
				Debug.LogError(GetType() + " has failed Executors validation, please ensure no duplicates are present!", _wizardConfiguration);
				return false;
			}

			return true;
		}
	}
}