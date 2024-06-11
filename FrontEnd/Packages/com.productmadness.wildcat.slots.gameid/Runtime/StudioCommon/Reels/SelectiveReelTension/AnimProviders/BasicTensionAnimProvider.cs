using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	public class BasicTensionAnimProvider : BaseTensionAnimProvider
	{
		public BasicTensionAnimProvider(TensionType tensionType, ServiceLocator serviceLocator, string idleAnimTrigger, AnimationDefinition[] animDefinitions)
			: base(tensionType, serviceLocator, idleAnimTrigger, animDefinitions) { }

		public override bool GetAnimProperties(out string animStartTrigger, out string animStopTrigger, out string animCompleteState)
		{
			animStartTrigger = _animationDefinitions[0].AnimationStartTrigger;
			animStopTrigger = _animationDefinitions[0].AnimationStopTrigger;
			animCompleteState = _animationDefinitions[0].AnimCompleteState;
			return true;
		}

		public override void Validate()
		{
			if (_animationDefinitions == null || _animationDefinitions.Length == 0)
			{
				GameIdLogger.Logger.Error(GetType() + " :: Has no animation definitions!", _tensionType.AnimProviderSO);
			}

			if (string.IsNullOrEmpty(_idleAnimTrigger))
			{
				GameIdLogger.Logger.Error(GetType() + " :: Idle anim trigger is null or empty!", _tensionType.AnimProviderSO);
			}
		}

		public override void ViewDisabled() { }

		public override void ViewEnabled() { }

		public override void SpinCompleted() { }

		public override void SpinStarted() { }

		public override void SetSpinSubscriptions()
		{
			// does nothing
		}

		public override void OnReelSpin(int reelIndex) { }

		public override void OnReelStop(int reelIndex) { }
	}
}
