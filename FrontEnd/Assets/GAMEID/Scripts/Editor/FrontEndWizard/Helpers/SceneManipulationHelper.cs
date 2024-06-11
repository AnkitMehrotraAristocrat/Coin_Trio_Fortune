using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	public static class SceneManipulationHelper
	{
		/// <summary>
		/// Adds the supplied component to the scene (with tag if populated) at the supplied scene path.
		/// Will optionally replace the existing components (with matchin tags) if desired.
		/// </summary>
		/// <param name="componentType">Component type</param>
		/// <param name="tag">Optional tag</param>
		/// <param name="scenePath">Scene location</param>
		/// <param name="replaceExisting">Remove and replace any existing components with the same tag</param>
		/// <returns></returns>
		public static Component AddComponent(string componentType, string tag, string scenePath, bool replaceExisting)
		{
			tag ??= "";

			if (string.IsNullOrEmpty(componentType))
			{
				Debug.LogError("SceneManipulationHelper: componentType must not be null or empty!");
				return null;
			}

			Type type = Type.GetType(componentType);
			if (type == null)
			{
				Debug.LogError("SceneManipulationHelper: Could not find the desired type (" + componentType + ")");
				return null;
			}

			if (string.IsNullOrEmpty(scenePath))
			{
				Debug.LogError("SceneManipulationHelper: scenePath must not be null or empty!");
				return null;
			}

			GameObject targetObject = GetOrCreateTargetObject(scenePath);
			List<Component> components = targetObject.GetComponents(type).Where(entry => entry.GetTag().Equals(tag)).ToList();

			if (components.Any())
			{
				if (!replaceExisting)
				{
					return components[0];
				}

				RemoveComponents(targetObject, type, tag);
			}

			Component component = targetObject.AddComponent(type);

			if (!string.IsNullOrEmpty(tag))
			{
				Tagger tagger = targetObject.GetComponent<Tagger>();
				if (tagger == null)
				{
					tagger = targetObject.AddComponent<Tagger>();
				}
				tagger.Add(component, tag);
			}

			return component;
		}

        /// <summary>
        /// Removes the supplied component from the scene (with tag if populated) at the supplied scene path.
        /// </summary>
        /// <param name="componentType">Component type</param>
        /// <param name="tag">Optional tag</param>
        /// <param name="scenePath">Scene location</param>
        /// <param name="replaceExisting">Remove and replace any existing components with the same tag</param>
        /// <returns></returns>
        public static void RemoveComponent(string componentType, string tag, string scenePath)
        {
            tag ??= "";

            if (string.IsNullOrEmpty(componentType))
            {
                Debug.LogWarning("SceneManipulationHelper: componentType must not be null or empty!");
                return;
            }

            Type type = Type.GetType(componentType);
            if (type == null)
            {
                Debug.LogWarning("SceneManipulationHelper: Could not find the desired type (" + componentType + ")");
                return;
            }

            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogWarning("SceneManipulationHelper: scenePath must not be null or empty!");
                return;
            }

            GameObject targetObject = GetTargetObject(scenePath);
			if (targetObject == null)
			{
				Debug.LogWarning("Object at scenePath does not exist");
				return;
			}
            List<Component> components = targetObject.GetComponents(type).Where(entry => entry.GetTag().Equals(tag)).ToList();


            foreach (Component component in components)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    Tagger tagger = targetObject.GetComponent<Tagger>();
                    if (tagger != null)
                    {
                        tagger.Remove(component);
                    }
                }
            }

            if (components.Any())
            {
                RemoveComponents(targetObject, type, tag);
            }
        }

        /// <summary>
        /// Adds the supplied prefab to the scene
        /// </summary>
        /// <param name="prefab">Prefab to add</param>
        /// <param name="scenePath">Scene location</param>
        /// <param name="name">Scene implementation prefab name to use</param>
        /// <param name="replaceExisting">Optional flag to replace any existing prefabs with the same name content</param>
        /// <returns></returns>
        public static GameObject AddPrefab(GameObject prefab, string scenePath, string name, bool maintainPrefabReference, bool replaceExisting = false)
		{
			if (string.IsNullOrEmpty(scenePath))
			{
				Debug.LogError("SceneManipulationHelper.AddPrefab(): The scenePath argument cannot be null or empty!");
				return null;
			}

			GameObject targetObject = GetOrCreateTargetObject(scenePath);
			GameObject existing = targetObject.transform.Find(name)?.gameObject;
			if (existing != null && replaceExisting)
			{
				UnityEngine.Object.DestroyImmediate(existing);
			}

			GameObject gameObject;
			if (maintainPrefabReference)
			{
				gameObject = PrefabUtility.InstantiatePrefab(prefab, targetObject.transform) as GameObject;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(prefab, targetObject.transform);
			}
			
			gameObject.name = name;

			return gameObject;
		}

        /// <summary>
        /// Removes the supplied prefab from the scene
        /// </summary>
        /// <param name="prefab">Prefab to add</param>
        /// <param name="scenePath">Scene location</param>
        /// <param name="name">Scene implementation prefab name to use</param>
        /// <param name="replaceExisting">Optional flag to replace any existing prefabs with the same name content</param>
        /// <returns></returns>
        public static void RemovePrefab(GameObject prefab, string scenePath, string name)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogWarning("SceneManipulationHelper.AddPrefab(): The scenePath argument cannot be null or empty!");
                return;
            }

            GameObject targetObject = GetTargetObject(scenePath);
            if (targetObject == null)
            {
                Debug.LogWarning("Object at scenePath does not exist");
                return;
            }

            GameObject existing = targetObject.transform.Find(name)?.gameObject;
            if (existing == null)
            {
                Debug.LogWarning("Prefab does not exist");
				return;
            }

            UnityEngine.Object.DestroyImmediate(existing);
        }

        /// <summary>
        /// Removes the components with a matching type and tag from the supplied game object
        /// </summary>
        /// <param name="gameObject">Object to review</param>
        /// <param name="type">Type to remove</param>
        /// <param name="tag">Optional tag to utilize</param>
        /// <param name="saveScene">Optional flag to save the scene or not</param>
        public static void RemoveComponents(GameObject gameObject, Type type, string tag, bool saveScene = false)
		{
			tag ??= "";

			if (gameObject == null)
			{
				Debug.LogWarning("SceneManipulationHelper.RemoveComponents(): The gameObject argument cannot be null!");
				return;
			}

			if (type == null)
			{
				Debug.LogWarning("SceneManipulationHelper.RemoveComponents(): The type argument cannot be null!");
				return;
			}

			Component[] components = gameObject.GetComponents(type);
			if (!components.Any())
			{
				return;
			}

			Tagger tagger = gameObject.GetComponent<Tagger>();
			foreach (Component component in components)
			{
                if (component.GetTag().Equals(tag))
				{
                    if (tagger != null)
                    {
                        tagger.Remove(component);
                    }
                    UnityEngine.Object.DestroyImmediate(component);
                }
            }

			if (saveScene)
			{
				SaveScene();
			}
		}

		/// <summary>
		/// Marks the scene dirty and saves it
		/// </summary>
		public static void SaveScene()
		{
			Scene scene = FrontEndWizardHelper.GetActiveSlotScene();
			EditorSceneManager.MarkSceneDirty(scene);
			EditorSceneManager.SaveScene(scene);
		}

		/// <summary>
		/// Gets or creates and returns the target scene game object
		/// </summary>
		/// <param name="fullPath">Path to desired object</param>
		/// <returns></returns>
		public static GameObject GetOrCreateTargetObject(string fullPath)
		{
			Scene scene = FrontEndWizardHelper.GetActiveSlotScene();
			GameObject targetObject = scene.GetRootGameObjects()[0];

			string[] path = fullPath.Split('/');
			foreach (string name in path)
			{
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}

				if (!targetObject.name.Contains(name))
				{
					GameObject child = targetObject.transform.Find(name)?.gameObject;
					if (child != null)
					{
						targetObject = child;
					}
					else
					{
						var foundChild = false;
						foreach(Transform childTransform in targetObject.transform)
                        {
							if(childTransform.name.StartsWith(name))
                            {
								targetObject = childTransform.gameObject;
								foundChild = true;
								break;
                            }
                        }

						if (!foundChild)
                        {
							child = new GameObject(name);
							child.transform.parent = targetObject.transform;
							targetObject = child;
						}
					}
				}
			}

			return targetObject;
		}

		/// <summary>
		/// Fetches the desired scene game object (if it exists)
		/// </summary>
		/// <param name="fullPath">Full slot scene path to the target object</param>
		/// <returns></returns>
		public static GameObject GetTargetObject(string fullPath)
		{
			Scene scene = FrontEndWizardHelper.GetActiveSlotScene();
			GameObject targetObject = scene.GetRootGameObjects()[0];

			string[] path = fullPath.Split('/');
			foreach (string name in path)
			{
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}

				if (!targetObject.name.Equals(name))
				{
					GameObject child = targetObject.transform.Find(name)?.gameObject;
					if (child != null)
					{
						targetObject = child;
					}
					else
					{
						Debug.LogError("Target object does not exist!");
						targetObject = null;
						break;
					}
				}
			}

			return targetObject;
		}

		/// <summary>
		/// A small helper method to remove game objects from the scene
		/// </summary>
		/// <param name="gameObject"></param>
		public static void RemoveGameObject(GameObject gameObject, bool saveScene = false)
		{
			UnityEngine.Object.DestroyImmediate(gameObject);
			if (saveScene)
			{
				SaveScene();
			}
		}
	}
}
