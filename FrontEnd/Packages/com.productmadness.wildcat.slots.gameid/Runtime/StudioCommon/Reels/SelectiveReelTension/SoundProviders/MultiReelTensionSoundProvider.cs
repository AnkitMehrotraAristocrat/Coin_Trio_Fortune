using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	[Serializable]
	public class MultiReelAudioDefinition : AudioDefinition
	{
		public uint PendingReelCount;
	}

	[Preserve]
	[Serializable]
	public class MultiReelAudioDefinitions : ReorderableArray<MultiReelAudioDefinition> { }

	public class MultiReelTensionSoundProvider : BaseTensionSoundProvider
	{
		private bool[] _reelHasStopped;
		private bool _isSoundActive;
		private new MultiReelAudioDefinition[] _audioEventDefinitions;

		public MultiReelTensionSoundProvider(TensionType tensionType, ServiceLocator serviceLocator,
			SymbolLocator symbolLocator, MultiReelAudioDefinition[] audioDefinitions)
			: base(tensionType, serviceLocator, symbolLocator, audioDefinitions)
		{
			_audioEventDefinitions = audioDefinitions;
		}

		public override void Validate()
		{
			if (_audioEventDefinitions == null || _audioEventDefinitions.Length == 0)
			{
				GameIdLogger.Logger.Error(GetType() + " :: Has no audio definitions!", _tensionType.SoundProviderSO);
			}
		}

		public override bool GetAudioEvents(out string playAudioEventName, out string quickStopAudioEventName)
		{
			if (_isSoundActive)
			{
				playAudioEventName = null;
				quickStopAudioEventName = null;
				return false;
			}

			uint pendingReelsCount = (uint)(_reelHasStopped.Length - _reelHasStopped.Where(hasStopped => hasStopped).Count());
			MultiReelAudioDefinition audioDefinition = _audioEventDefinitions.FirstOrDefault(definition => definition.PendingReelCount.Equals(pendingReelsCount));
			playAudioEventName = audioDefinition?.PlayAudioEventName;
			quickStopAudioEventName = audioDefinition?.QuickStopAudioEventName;

			if (string.IsNullOrEmpty(playAudioEventName))
			{
				return false;
			}

			_isSoundActive = true;
			return true;
		}

		public override void ViewEnabled() { }

		public override void ViewDisabled() { }

		public override void SpinStarted()
		{
			_isSoundActive = false;
			_reelHasStopped = new bool[_symbolLocator.ScreenSymbols.Cols];
			for (int reelIndex = 0; reelIndex < _reelHasStopped.Length; ++reelIndex)
			{
				_reelHasStopped[reelIndex] = true;
			}
		}

		public override void SpinCompleted() { }

		public override void SetSpinSubcriptions()
		{
			// does nothing
		}

		public override void OnReelSpin(int reelIndex)
		{
			_reelHasStopped[reelIndex] = false;
		}

		public override void OnReelStop(int reelIndex)
		{
			_reelHasStopped[reelIndex] = true;
		}
	}
}
