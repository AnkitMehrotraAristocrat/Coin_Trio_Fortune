using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System;
using System.Linq;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[Preserve]
	[Serializable]
	public class ScatterReelSpinTypeParam
	{
		public SymbolId ScatterId;
		public int ScatterCountThreshold = 2;

		private int _currentCount = 0;

		public void ResetCurrentCount()
		{
			_currentCount = 0;
		}

		public void IncrementCurrentCount()
		{
			++_currentCount;
		}

		public bool ThresholdMet()
		{
			return _currentCount >= ScatterCountThreshold;
		}
	}

	[Preserve]
	[Serializable]
	public class ScatterReelSpinTypeParams : ReorderableArray<ScatterReelSpinTypeParam> { }

	/// <summary>
	/// Spin type evaluator used by the CustomSpinDriver.
	/// Evaluates which spin type is to be applied if the only variants are due to scatter(s) appearing
	/// on a the reels. Assumes a left to right reel stop order.
	/// </summary>
	public class MultiSymbolScatterReelsSpinTypeEvaluator : MonoBehaviour, ISpinTypeEvaluator, ServiceLocator.IHandler
	{
		[FieldRequiresModel] protected ModalReelsModel ModalReelsModel = default;
		[FieldRequiresModel] protected GameStateModel GameStateModel = default;
		[FieldRequiresGlobal] protected ServiceLocator ServiceLocator;

		[Reorderable] [SerializeField] protected ScatterReelSpinTypeParams ScatterParams;
		[SerializeField] protected string DefaultSpinType = "normal";
		[SerializeField] protected string AnticipationSpinTypePrefix = "anticipate-";

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		/// <summary>
		/// Loops through the visible reels and counts if a single scatter is present on that reel.
		/// If it is, it increments the scatter count and updates the anticipation type string.
		/// If we meet the minimum count for anticipation, the spin type is set for this spin.
		/// </summary>
		/// <param name="payloads"></param>
		public virtual void EvaluateSpinType(MainDriver.IPayloadReader payloadReader)
		{
			if (ScatterParams.Any(scatterParam => scatterParam.ScatterId == null))
			{
				return;
			}
			var reelsOutcomeServerModel = ServiceLocator.Get<ReelsOutcomeServerModel>(GameStateModel.GameState.ToString());
			var visibleReels = reelsOutcomeServerModel.Symbols;
            ReelWindowDataServerModel windowModel = ServiceLocator.Get<ReelWindowDataServerModel>(reelsOutcomeServerModel.ReelWindowId.Value);
			int reelCount = windowModel.Width.Value;
			int reelHeight = windowModel.Height.Value;

			foreach (ScatterReelSpinTypeParam scatterParam in ScatterParams)
			{
				scatterParam.ResetCurrentCount();
			}

			string spinType = DefaultSpinType;
			string anticipationType = AnticipationSpinTypePrefix;
			bool isAnticipationSpin = false;
			for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
			{
				if (ScatterParams.Any(scatterParam => scatterParam.ThresholdMet()))
				{
					anticipationType += reelIndex.ToString();
					isAnticipationSpin = true;
				}
				for (int symbolIndex = 0; symbolIndex < reelHeight; ++symbolIndex)
				{
					ScatterReelSpinTypeParam matchingScatterParam = ScatterParams.FirstOrDefault(scatterParam => scatterParam.ScatterId.Equals(visibleReels[reelIndex][symbolIndex]));
					matchingScatterParam?.IncrementCurrentCount();
				}
			}
			if (isAnticipationSpin)
			{
				spinType = anticipationType;
			}
			ModalReelsModel.SpinType.Value = spinType;
		}
	}
}
