using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System;
using System.Collections.Generic;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[CreateAssetMenu(fileName = "SymbolCountTensionType", menuName = "NMG/Selective Reel Tension/Tension Type/Symbol Count")]
	public class SymbolCountTensionType : TensionType
	{
		protected ReelsOutcomeClientModel _reelsOutcomeClientModel;
		//protected ReelWindowDataServerModel _reelWindowServerModel;

		protected List<bool> _reelIndexEligibility = new List<bool>();

		private IDisposable _visibleReelsSubscription;
		private IDisposable _reelWindowModelSubscription;

		[SerializeField] protected int _symbolCountThreshold;

		public override void Initialize(ServiceLocator serviceLocator, Transform prefabParent, SymbolLocator symbolLocator)
		{
			_reelsOutcomeClientModel = serviceLocator.GetOrCreate<ReelsOutcomeClientModel>();
			base.Initialize(serviceLocator, prefabParent, symbolLocator);
		}

		public override bool IsEligible(int reelIndex)
		{
			if (_reelIndexEligibility.Count <= reelIndex)
			{
				return false;
			}

			return _reelIndexEligibility[reelIndex];
		}

		public override void SetSpinSubscriptions()
		{
			//SetReelWindowModelSubscription();
			SetVisibleReelsSubscriptions();
			base.SetSpinSubscriptions();
		}

		public override void SpinCompleted()
		{
			//DisposeSubscriptions();
			Unsubscribe();
			base.SpinCompleted();
		}

		private void SetVisibleReelsSubscriptions()
		{
			_visibleReelsSubscription = _reelsOutcomeClientModel.VisibleReels
				.SkipWhile(visibleReels => visibleReels == null)
				.Skip(1) // We skip the first eligible attempt as the intent is to update when the client model is updated as a result of as spin response
				.Subscribe(result => VisibleReelsUpdated(result));
			Subscriptions.Add(_visibleReelsSubscription);
		}

		// ROB: Commenting this out as we have Milan updates that don't force notify causing null ref errors down the line
		//	due to the _reelWindowServerModel not getting assigned
		//private void SetReelWindowModelSubscription()
		//{
		//	_reelWindowModelSubscription = _reelsOutcomeClientModel.ReelWindowId
		//		.SkipWhile(reelWindow => string.IsNullOrEmpty(reelWindow))
		//		.Skip(1) // We skip the first eligible attempt as the intent is to update when the client model is updated as a result of as spin response
		//		.Subscribe(result => _reelWindowServerModel = _serviceLocator.Get<ReelWindowDataServerModel>(result));
		//	Subscriptions.Add(_reelWindowModelSubscription);
		//}

		//private void DisposeSubscriptions()
		//{
		//	_visibleReelsSubscription?.Dispose();
		//	_reelWindowModelSubscription?.Dispose();
		//}

		protected virtual void VisibleReelsUpdated(SymbolId[][] result)
		{
			InitializeEligibility(result);
		}

		private void InitializeEligibility(SymbolId[][] result)
		{
			//int reelCount = _reelWindowServerModel.Width.Value;
			//int reelHeight = _reelWindowServerModel.Height.Value;

			_reelIndexEligibility.Clear();

			string reelWindowId = _reelsOutcomeClientModel.ReelWindowId.Value;
			if (string.IsNullOrEmpty(reelWindowId))
			{
				return;
			}

			ReelWindowDataServerModel reelWindowDataServerModel = _serviceLocator.Get<ReelWindowDataServerModel>(reelWindowId);
			int reelCount = reelWindowDataServerModel.Width.Value;
			int reelHeight = reelWindowDataServerModel.Height.Value;

			int count = 0;

			for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
			{
				if ((count >= _symbolCountThreshold))
				{
					_reelIndexEligibility.Add(true);
				}
				else
				{
					_reelIndexEligibility.Add(false);
				}

				for (int symbolIndex = 0; symbolIndex < reelHeight; ++symbolIndex)
				{
					if (result[reelIndex][symbolIndex] == _symbolId)
					{
						count++;
					}
				}
			}
		}
	}
}
