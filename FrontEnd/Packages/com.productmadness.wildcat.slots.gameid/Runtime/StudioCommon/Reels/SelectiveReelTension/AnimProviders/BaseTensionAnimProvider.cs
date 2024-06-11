using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using System;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	[Serializable]
	public class AnimationDefinition
	{
		public string AnimationStartTrigger;
		public string AnimationStopTrigger;
		public string AnimCompleteState;
	}

	[Preserve]
	[Serializable]
	public class AnimationDefinitions : ReorderableArray<AnimationDefinition> { }

	public abstract class BaseTensionAnimProvider
	{
		protected TensionType _tensionType;
		protected ServiceLocator _serviceLocator;
		protected AnimationDefinition[] _animationDefinitions;
		protected string _idleAnimTrigger;

		public BaseTensionAnimProvider(TensionType tensionType, ServiceLocator serviceLocator, string idleAnimTrigger, AnimationDefinition[] animDefinitions)
		{
			_tensionType = tensionType;
			_animationDefinitions = animDefinitions;
			_serviceLocator = serviceLocator;
			_idleAnimTrigger = idleAnimTrigger;
		}

		public abstract void Validate();

		public abstract bool GetAnimProperties(out string animStartTrigger, out string animStopTrigger, out string animCompleteState);

		public abstract void ViewEnabled();

		public abstract void ViewDisabled();

		public abstract void SpinStarted();

		public abstract void SpinCompleted();

		public abstract void SetSpinSubscriptions();

		public abstract void OnReelSpin(int reelIndex);

		public abstract void OnReelStop(int reelIndex);

		public string GetIdleAnimTrigger()
		{
			return _idleAnimTrigger;
		}
	}
}
