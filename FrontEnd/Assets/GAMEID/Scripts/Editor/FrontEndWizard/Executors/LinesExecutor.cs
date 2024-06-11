using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.WinLine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	/// <summary>
	/// Must run before GameIdReplacementExecutor!!!
	/// </summary>
	public class LinesExecutor : BaseWizardExecutor
    {
		public LinesExecutor()
		{
			_canRerun = true;
		}

		public override void Execute(WizardInputData data)
        {
            if(data.Paylines != null)
            {
                PopulateLineConfig(data.Paylines);
            }
            else
            {
                RemoveLinesConfig();
            }
        }

        private void PopulateLineConfig(List<PaylinesDefinition> paylinesDef)
        {
			BaseLinesConfig config = new BaseLinesConfig();

			string[] guids = AssetDatabase.FindAssets("t:BaseLinesConfig");
			if (guids.Length == 0)
			{
				// add handling to generate a line config
				AssetDatabase.CreateAsset(config, "Assets/GAMEID/Configs/LinesConfig.asset");
			}
			else
            {
				string lineConfigPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				config = AssetDatabase.LoadAssetAtPath<BaseLinesConfig>(lineConfigPath);
				config.modes.Clear();
			}
			
			foreach (PaylinesDefinition def in paylinesDef)
			{
				Lines paylines = new Lines();
				foreach (PaylineDefinition linePattern in def.Lines)
				{
					Line payline = new Line();
					List<Location> locations = new List<Location>();
					foreach (PaylinePosition pattern in linePattern)
					{
						locations.Add(new Location()
						{
							colIndex = pattern.ColIndex,
							rowIndex = pattern.RowIndex,
						});
					}
					payline.locations = locations.ToArray();
					paylines.Add(payline);
				}

				config.modes.Add(new ModeLineSet()
				{
					modeName = def.Name,
					lines = paylines
				});
			}

			EditorUtility.SetDirty(config);
			AssetDatabase.SaveAssetIfDirty(config);
		}

        private void RemoveLinesConfig()
        {
			var linesConfigs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BaseLinesConfig)));
			foreach (var lineConfig in linesConfigs)
            {
				var path = AssetDatabase.GUIDToAssetPath(lineConfig);
				AssetDatabase.DeleteAsset(path);
			}
        }
    }
}
