using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Sag;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using Slotsburg.Slots.SharedFeatures;
using System.Linq;
using UnityEngine;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System.Collections.Generic;
using System;
using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Bridge.Meta;
using TMPro;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
	public class SymbolDataAssigner : BaseSymbolSpawnHandler
	{
		[FieldRequiresModel] private GameStateModel _gameStateModel;
		[FieldRequiresModel] private IBetModel _betModel;
		[FieldRequiresModel] protected SymbolOutcomeModel _symbolOutcomeModel;
		[FieldRequiresModel] protected SymbolSpinningModel _symbolSpinningModel;

		[FieldRequiresParent] protected SymbolLocatorDigital _symbolLocator;

		[SerializeField] private string _spinningModelTag;
		[SerializeField][Reorderable] private SymbolSkinConfigs _skinConfigs;
		public SymbolSkinConfigs SkinConfigs => _skinConfigs;

		public override void OnServicesLoaded()
		{
			base.OnServicesLoaded();
			_symbolSpinningModel = _serviceLocator.GetOrCreate<SymbolSpinningModel>(_spinningModelTag);
		}

		protected override void OnSymbolSpawn(SpawnedSymbolData spawnedSymbolData)
		{
			int symbolId = spawnedSymbolData.SymbolId.Value;

			if (!GetSkinConfig(symbolId, out SymbolSkinConfig skinConfig))
			{
				return;
			}

			SymbolOutcomeData symbolOutcomeData = GetSymbolSkinData(symbolId, spawnedSymbolData, skinConfig);
			SymbolSkinMap skinMap = skinConfig.SkinMaps.FirstOrDefault(map => map.Type.Equals(symbolOutcomeData.SymbolData.Skin));
			Debug.Assert(skinMap != null, "SymbolSkinAssigner could not find a skin map for:\n- Type: " + symbolOutcomeData.SymbolData.Skin, this);

			GameObject symbolObject = spawnedSymbolData.SymbolHandle.gameObject;

			UpdateText(symbolObject, symbolOutcomeData);

			SpineAnimationController[] spineAnimationControllers = symbolObject.GetComponentsInChildren<SpineAnimationController>();

			foreach (SpineAnimationController controller in spineAnimationControllers)
			{
				controller._skinIndex = skinMap.SkinIndex;
				controller.SetShouldRefreshMesh(true);
				controller.UpdateFrame(true);
			}
		}

		private bool GetSkinConfig(int symbolId, out SymbolSkinConfig skinConfig)
		{
			skinConfig = _skinConfigs.FirstOrDefault(config => config.SymbolId.Equals(symbolId));
			if (skinConfig == null)
			{
				return false;
			}
			return true;
		}

		private SymbolOutcomeData GetSymbolSkinData(int symbolId, SpawnedSymbolData spawnedSymbolData, SymbolSkinConfig skinConfig)
		{
			Debug.Assert(_symbolOutcomeModel != null, "SymbolSkinAssigner could not find a SymbolSkinModel for:\n- SymbolId: " + symbolId, this);

			DefaultSymbolData defaultSymbolData = skinConfig.DefaultSymbolData.FirstOrDefault(defaultData => defaultData.GameState.Equals(_gameStateModel.GameState.ToString()));
			Debug.Assert(defaultSymbolData != null, GetType() + " (" + this.GetTag() + ") :: " + name + " has is missing defaultSymbolData for the game state: " + _gameStateModel.GameState, gameObject);

			SymbolOutcomeData defaultData = new SymbolOutcomeData();
			defaultData.SymbolData = new SymbolData();
			defaultData.SymbolData.TextValue = defaultSymbolData.DefaultData.TextValue;
			defaultData.SymbolData.ShouldMultiply = defaultSymbolData.DefaultData.ShouldMultiply;
			defaultData.SymbolData.Skin = defaultSymbolData.DefaultData.Skin;

			if (_reelLanding[spawnedSymbolData.Location.colIndex] || _reelQuickStopped[spawnedSymbolData.Location.colIndex])
			{
				return _symbolOutcomeModel.GetSymbolData(_gameStateModel.GameState, spawnedSymbolData.Location, out SymbolOutcomeData symbolData) ? symbolData : defaultData;
			}

			return defaultSymbolData.UseDefaultDuringSpin ? defaultData : _symbolSpinningModel.GetDummySymbol(symbolId, _gameStateModel.GameState);
		}

		private void UpdateText(GameObject symbolObject, SymbolOutcomeData symbolOutcomeData)
		{
			TextMeshPro textView = symbolObject.GetComponentInChildren<TextMeshPro>(true);
			bool hasTextValue = !string.IsNullOrEmpty(symbolOutcomeData.SymbolData.TextValue) && Convert.ToInt32(symbolOutcomeData.SymbolData.TextValue) > 0;

			if (hasTextValue && textView == null)
			{
				GameIdLogger.Logger.Error("SymbolSkinAssigner :: SymbolData has a TextValue but this symbol does not have a TextMeshView!", this);
			}

			if (textView == null)
			{
				return;
			}

			if (hasTextValue)
			{
				textView.gameObject.SetActive(true);
				SetText(textView, symbolOutcomeData.SymbolData.TextValue, symbolOutcomeData.SymbolData.ShouldMultiply);
				return;
			}
			textView.gameObject.SetActive(false);
		}

		private void SetText(TextMeshPro textView, string textValue, bool shouldMultiply)
		{
			if (float.TryParse(textValue, out float value))
			{
				if (shouldMultiply)
				{
					value *= _betModel.Amount.Value;
				}

				textView.SetText(NumberFormat.SmartAbbreviate((double)value, 4));
				return;
			}
			textView.SetText(textValue);
		}

		protected override void OnSymbolDespawn(SpawnedSymbolData symbolData)
		{
			// Does nothing
			// TODO: Perhaps this could reset the symbol to default state?
		}

        public void ActivateAnimationSnap(bool hnsToBaseValues = false)
		{
			var skinsData = _symbolOutcomeModel.SymbolData[_gameStateModel.GameState];

			foreach (KeyValuePair<Location,SymbolOutcomeData> pair in skinsData)
			{
				var location = pair.Key;

				if (!hnsToBaseValues)
				{
					// TODO: Use WindowPositionMap to transfer positions from HoldAndSpin to BaseSpin
					//location = ;
				}

				var symbolHandle = _symbolLocator.ScreenSymbols[location].CurrentSymbol.Instance;

				// Play the activate snap trigger on this COR to get it to properly skin.
				if (pair.Value != null)
				{
					var activateParam = "ActivateSnap";
					var animator = symbolHandle.gameObject.GetComponent<Animator>();
					animator.SetTrigger(activateParam);
				}
			}
		}
	}
}
