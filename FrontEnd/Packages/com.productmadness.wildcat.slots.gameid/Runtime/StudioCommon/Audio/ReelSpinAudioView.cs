using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class ReelSpinAudioView : MonoBehaviour, IReelEventResponder, ServiceLocator.IHandler
	{
		#region Helper Classes
		[Serializable]
		public class AudioEventByGameState
		{
			public GameStateEnum GameState;
			public string AudioEvent;
			public bool Loop;
		}

		[Serializable]
		public class AudioEventsByGameState : ReorderableArray<AudioEventByGameState> { }
		#endregion

		[FieldRequiresModel] private GameStateModel _gameStateModel;
		[FieldRequiresParent] private AudioEventBindings _audioEventBindings;

		[SerializeField][Reorderable] AudioEventsByGameState _audioEventsByGameState;

		private Dictionary<int, bool> _reelSpinStates = new Dictionary<int, bool>();
		private string _activeAudioEvent;
		private bool _isPlaying = false;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public void OnReelSpin(int reelIndex)
		{
			PlayAudio();

			if (_reelSpinStates.ContainsKey(reelIndex))
			{
				_reelSpinStates[reelIndex] = true;
				return;
			}

			_reelSpinStates.Add(reelIndex, true);
		}

		public void OnReelStop(int reelIndex)
		{
			OnReelStopEvent(reelIndex);
		}

		public void OnReelQuickStop(int reelIndex)
		{
			OnReelStopEvent(reelIndex);
		}

		private void PlayAudio()
		{
			if (_isPlaying)
			{
				return;
			}

			_isPlaying = true;

			AudioEventByGameState audioEventByGameState = _audioEventsByGameState.FirstOrDefault(audioEventsByGameStateEntry => audioEventsByGameStateEntry.GameState.Equals(_gameStateModel.GameState));

			if (audioEventByGameState == null)
			{
				GameIdLogger.Logger.Error(GetType() + " :: Could not find an AudioEventByGameState entry for " + _gameStateModel.GameState + ".", this);
				return;
			}

			_activeAudioEvent = audioEventByGameState.AudioEvent;
			_audioEventBindings.Play(audioEventByGameState.AudioEvent, audioEventByGameState.Loop);
		}

		private void StopAudio()
		{
			if (!_isPlaying)
			{
				return;
			}

			_isPlaying = false;
			_audioEventBindings.Stop(_activeAudioEvent);
		}

		private void OnReelStopEvent(int reelIndex)
		{
			if (!_reelSpinStates.ContainsKey(reelIndex))
			{
				GameIdLogger.Logger.Error(GetType() + " :: Reel stop event occurred but reel spin event never occurred.", this);
				return;
			}

			_reelSpinStates[reelIndex] = false;

			if (_reelSpinStates.Values.All(isSpinning => !isSpinning))
			{
				StopAudio();
			}

		}

		#region Unused Interface Methods
		public void OnReelLanding(int reelIndex)
		{
			// does nothing
		}
		#endregion
	}
}
