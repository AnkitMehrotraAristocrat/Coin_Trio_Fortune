using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;
using UnityEditor;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	/// <summary>
	/// Must run before GameIdReplacementExecutor!!!
	/// </summary>
    public class SymbolsExecutor : BaseWizardExecutor
    {
		private static string prefabPath = "Assets/GAMEID/Prefabs/WizardSymbol.prefab";
		private static string variantAssetPath = "Assets/GAMEID/Prefabs/Symbols/";

		public SymbolsExecutor()
		{
			_canRerun = true;
		}

		public override void Execute(WizardInputData data)
        {
			DuplicatePrefabVariant(data);
			PopulateSlotConfig(data);
        }

		private void DuplicatePrefabVariant( WizardInputData data)
		{
			prefabPath = EditorGUILayout.TextField("\tPrefab Path", prefabPath);
			variantAssetPath = EditorGUILayout.TextField("\tVariant Asset Path", variantAssetPath);

			if (string.IsNullOrEmpty(prefabPath) || string.IsNullOrEmpty(variantAssetPath))
			{
				return;
			}

            var defaultSymbols = AssetDatabase.FindAssets("t: prefab", new string[] { variantAssetPath });
            foreach (var sym in defaultSymbols)
            {
                var path = AssetDatabase.GUIDToAssetPath(sym);
				AssetDatabase.DeleteAsset(path);
            }

            Object source = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
			GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(source);

			foreach (SymbolDefinition def in data.Symbols)
			{
				string localPath = AssetDatabase.GenerateUniqueAssetPath(variantAssetPath + def.Name + ".prefab");
				var prefab = PrefabUtility.SaveAsPrefabAsset(objSource, localPath);
				SymbolHandle symbolHandle = prefab.GetComponentInChildren<SymbolHandle>();

				if (symbolHandle != null)
				{
					symbolHandle.id = new SymbolId(def.Id);
					EditorUtility.SetDirty(prefab);
					AssetDatabase.SaveAssetIfDirty(prefab);
				}

				// TODO: Find text mesh view replacement
				// var textView = prefab.GetComponentInChildren<TextMeshView>();
				// if (textView != null)
				// {
				// 	textView.enabled = true;
				// 	textView.SetText(def.Name);
				//
				// 	var standinRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
				// 	var randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
				// 	standinRenderer.color = randomColor;
				// }
			}

			Object.DestroyImmediate(objSource.gameObject);
		}

		private void PopulateSlotConfig(WizardInputData data)
		{
			var config = ScriptableObject.CreateInstance<SymbolMapConfig>();

			string[] guids = AssetDatabase.FindAssets("t:SlotConfig");
			if (guids.Length == 0)
			{
				// add handling to generate a slot config
				AssetDatabase.CreateAsset(config, "Assets/GAMEID/Configs/SlotConfig.asset");
			}
			else
            {
				string slotConfigPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				config = AssetDatabase.LoadAssetAtPath<SymbolMapConfig>(slotConfigPath);
				config.Symbols.Clear();
			}
			
			foreach (SymbolDefinition def in data.Symbols)
			{
				GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(variantAssetPath + def.Name + ".prefab");

				config.Symbols.Add(new SymbolConfig() { id = new SymbolId(def.Id), name = def.Name, prefab = prefab });
			}
			EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssetIfDirty(config);
		}
	}
}
