using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using BuildTarget = Milan.FrontEnd.Core.v5_1_1.Manager.Editor.BuildTarget;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public static class FrontEndWizardHelper
	{
		/// <summary>
		/// Retrieves the WizardConfiguration asset
		/// </summary>
		/// <param name="config">The found configuration asset</param>
		/// <returns></returns>
		public static bool GetWizardConfig(out WizardConfiguration config)
		{
			config = GetAsset<WizardConfiguration>("");

			if (config == null)
			{
				Debug.LogError("Wizard configuration file missing!");
				return false;
			}
			return true;
		}

        /// <summary>
        /// Retrieves the FrontEndWizardRemovalData asset
        /// </summary>
        /// <param name="config">The found configuration asset</param>
        /// <returns></returns>
        public static bool GetWizardRemovalData(out FrontEndWizardRemovalData config)
        {
            config = GetAsset<FrontEndWizardRemovalData>("");

            if (config == null)
            {
                Debug.LogError("Wizard configuration file missing!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves the WizardState asset
        /// </summary>
        /// <param name="state">The found state asset</param>
        /// <returns></returns>
        public static bool GetWizardState(out WizardState state, WizardConfiguration config = null)
		{
			if (config == null)
			{
				if (!GetWizardConfig(out config))
				{
					state = null;
					return false;
				}
			}

			if (config.WizardState == null)
			{
				Debug.LogError("Wizard configuration's WizardState is not defined!", config);
				state = null;
				return false;
			}

			state = config.WizardState;
			return true;
		}

		public static bool GetInputData(out WizardInputData data, WizardConfiguration config = null)
		{
			if (config == null)
			{
				if (!GetWizardConfig(out config))
				{
					data = null;
					Debug.LogError("Wizard configuration file missing!");
					return false;
				}
			}

			if (config.InputJson == null)
			{
				data = null;
				Debug.LogError("Wizard configuration input json not defined!", config);
				return false;
			}

			TextAsset jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(config.InputJson);
			if (jsonFile == null)
			{
				data = null;
				Debug.LogError("Could not find wizard input data!", config);
				return false;
			}

			data = JsonConvert.DeserializeObject<WizardInputData>(jsonFile.text);
			return true;
		}

		/// <summary>
		/// Fetches the target asset using a supplied filter
		/// </summary>
		/// <typeparam name="T">Asset type</typeparam>
		/// <param name="assetFilter">Search filter</param>
		/// <returns></returns>
		public static T GetAsset<T>(string assetFilter) where T : UnityEngine.Object
		{
			string[] guids = AssetDatabase.FindAssets(assetFilter + " t:" + typeof(T));
			if (guids.Length == 0)
			{
				return default(T);
			}
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
			T asset = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
			return asset;
		}

		/// <summary>
		/// Fetches the target asset using a supplied filter
		/// </summary>
		/// <typeparam name="T">Asset type</typeparam>
		/// <param name="assetFilter">Search filter</param>
		/// <returns></returns>
		public static List<T> GetAssetsOfType<T>() where T : UnityEngine.Object
		{
			List<T> assets = new List<T>();

			string[] guids = AssetDatabase.FindAssets("t:" + typeof(T));
			if (guids.Length == 0)
			{
				return default;
			}

			foreach (string guid in guids)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				T asset = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
				assets.Add(asset);
			}
			
			return assets;
		}

		/// <summary>
		/// Gets the scene asset from the meta capabilities scriptable object.
		/// </summary>
		/// <returns></returns>
		public static SceneAsset GetSlotSceneAsset()
		{
			var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BuildTarget)));
			if (guids.Length > 0)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

				BuildTarget asset = AssetDatabase.LoadAssetAtPath<BuildTarget>(assetPath);

				return asset.SceneAsset;
			}

			return null;
		}

		/// <summary>
		/// Fetches the active slot scene
		/// </summary>
		/// <returns></returns>
		public static Scene GetActiveSlotScene()
		{
			SceneAsset asset = GetSlotSceneAsset();
			Scene scene = EditorSceneManager.GetSceneByName(asset.name);
			if (!scene.IsValid())
			{
				LoadSlotScene(OpenSceneMode.Additive, asset);
				scene = EditorSceneManager.GetSceneByName(asset.name);
			}

			return scene;
		}

		/// <summary>
		/// Loads the slot scene
		/// </summary>
		/// <param name="asset"></param>
		public static void LoadSlotScene(OpenSceneMode openSceneMode, SceneAsset asset = null)
		{
			asset ??= GetSlotSceneAsset();
			string path = AssetDatabase.GetAssetOrScenePath(asset);
			Scene scene = EditorSceneManager.OpenScene(path, openSceneMode);
			EditorSceneManager.SetActiveScene(scene);
		}

		/// <summary>
		/// Loads the designated scene
		/// </summary>
		/// <param name="sceneName">The name of the scene to load</param>
		public static void LoadScene(string sceneName, OpenSceneMode openSceneMode)
		{
			if (string.IsNullOrEmpty(sceneName))
			{
				return;
			}

			if (EditorSceneManager.GetSceneByName(sceneName).IsValid())
			{
				return;
			}

			var guids = AssetDatabase.FindAssets(sceneName + " t:SceneAsset");
			if (guids.Length > 0)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				EditorSceneManager.OpenScene(assetPath, openSceneMode);
			}
		}

		/// <summary>
		/// Removes the slot scene if it is open
		/// </summary>
		public static void CloseSlotScene()
		{
			SceneAsset asset = GetSlotSceneAsset();
			Scene scene = EditorSceneManager.GetSceneByName(asset.name);
			if (!scene.IsValid())
			{
				return;
			}
			EditorSceneManager.MarkSceneDirty(scene);
			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.CloseScene(scene, true);
		}

		/// <summary>
		/// Closes all open scenes
		/// </summary>
		public static void CloseAllScenes()
		{
			int sceneCount = SceneManager.sceneCount;

			// While we could do this is the NewSceneMode being Single, we want to first ensure
			// any open scenes are saved
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			
			for (int index = 0; index < sceneCount; ++index)
			{
				Scene scene = EditorSceneManager.GetSceneAt(0);
				EditorSceneManager.MarkSceneDirty(scene);
				EditorSceneManager.SaveScene(scene);
				EditorSceneManager.CloseScene(scene, true);
			}

			Debug.Log("Scenes closed!");
		}

		/// <summary>
		/// Returns a default GUIStyle with word wrapping and rich text
		/// </summary>
		/// <returns></returns>
		public static GUIStyle GetDefaultStyle()
		{
			GUIStyle style = new GUIStyle();
			style.wordWrap = true;
			style.richText = true;
			return style;
		}

		/// <summary>
		/// Refreshes the reel window generation states in the WizardState asset
		/// </summary>
		/// <param name="state">The state asset to refresh</param>
		/// <param name="hasPending">A flag indicating if any reel windows are pending generation</param>
		public static void RefreshReelWindowGeneratedStates(WizardState state, out bool hasPending)
		{
			hasPending = false;
			foreach (ReelWindowGenerationState reelWindowEntry in state.FetchPendingReelWindowGenerationStates())
			{
				if (reelWindowEntry.HasGenerated)
				{
					continue;
				}

				GameObject gameObject = SceneManipulationHelper.GetTargetObject(reelWindowEntry.RootPath);
				if (gameObject == null)
				{
					hasPending = true;
					continue;
				}

				if (HasRootReelView(gameObject))
				{
					reelWindowEntry.HasGenerated = true;
					EditorUtility.SetDirty(state);
					continue;
				}
				hasPending = true;
			}
		}

		public static List<MechanicConfiguration> GetApplicableMechanicConfigurations(WizardInputData data = null, WizardState wizardState = null)
		{
			if (data == null)
			{
				GetInputData(out data);
			}

			if (wizardState == null)
			{
				GetWizardState(out wizardState);
			}

			// find all existing configuration files
			List<MechanicConfiguration> configs = GetAssetsOfType<MechanicConfiguration>();

			// select the ones present in the input json
			List<string> mechanics = new List<string>();
			mechanics.AddRange(data.Features);

			List<MechanicConfiguration> orderedConfigs = new List<MechanicConfiguration>();

			foreach (var mechanic in mechanics)
			{
				var configToAdd = configs.First(config => config.Id.Equals(mechanic));
				if (configToAdd != null)
				{
					orderedConfigs.Add(configToAdd);
					wizardState.AddMechanicState(configToAdd.Id);
				}
			}

			return orderedConfigs.ToList();

			return configs
				.Where(config =>
				{
					if (mechanics.Contains(config.Id))
					{
						wizardState.AddMechanicState(config.Id);
						return true;
					}
					return false;
				})
				.ToList();
		}

		/// <summary>
		/// Centers an editor window with respect to the main Unity window
		/// </summary>
		/// <param name="window">The window the position and size</param>
		/// <param name="widthScalar">A scalar used with the main Unity window width to set the target window width</param>
		/// <param name="heightScalar">A scalar used with the main Unity window height to set the target window height</param>
		public static void CenterOnMainWin(EditorWindow window, float widthScalar = 1.0f, float heightScalar = 1.0f)
		{
			Rect main = EditorGUIUtility.GetMainWindowPosition();
			Rect pos = window.position;

			pos.width = main.width / widthScalar;
			pos.height = main.height / heightScalar;

			float centerWidth = (main.width - pos.width) * 0.5f;
			float centerHeight = (main.height - pos.height) * 0.5f;
			pos.x = main.x + centerWidth;
			pos.y = main.y + centerHeight;
			window.position = pos;
		}

		/// <summary>
		/// Determines if the supplied object has a RootReelView on itself or child game objects
		/// </summary>
		/// <param name="rootObject">The root game object</param>
		/// <returns></returns>
		private static bool HasRootReelView(GameObject rootObject)
		{
			return rootObject.GetComponentsInChildren<RootReelView>().Length > 0;
		}
	}
}
