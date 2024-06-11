using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	[Serializable]
	public class AudioDefinition
	{
		public string PlayAudioEventName;
		public string QuickStopAudioEventName;
	}

	[Preserve]
	[Serializable]
	public class AudioDefinitions : ReorderableArray<AudioDefinition> { }

	public abstract class BaseTensionSoundProvider
	{
		protected TensionType _tensionType;
		protected ServiceLocator _serviceLocator;
		protected SymbolLocator _symbolLocator;
		protected AudioDefinition[] _audioEventDefinitions;

		public BaseTensionSoundProvider(TensionType tensionType, ServiceLocator serviceLocator, SymbolLocator symbolLocator, AudioDefinition[] audioDefinitions)
		{
			_tensionType = tensionType;
			_serviceLocator = serviceLocator;
			_symbolLocator = symbolLocator;
			_audioEventDefinitions = audioDefinitions;
		}

		public abstract void Validate();

		public abstract bool GetAudioEvents(out string playAudioEventName, out string quickStopAudioEventName);

		public abstract void ViewEnabled();

		public abstract void ViewDisabled();

		public abstract void SpinStarted();

		public abstract void SpinCompleted();

		public abstract void SetSpinSubcriptions();

		public abstract void OnReelSpin(int reelIndex);

		public abstract void OnReelStop(int reelIndex);
	}
}
