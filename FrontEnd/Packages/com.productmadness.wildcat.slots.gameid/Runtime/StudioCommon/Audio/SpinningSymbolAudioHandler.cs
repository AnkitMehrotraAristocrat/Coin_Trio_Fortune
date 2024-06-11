using Milan.FrontEnd.Feature.v5_1_1.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class SpinningSymbolAudioHandler : MonoBehaviour
    {
        [SerializeField] private string _playAudioEventName;
        [SerializeField] private string _stopSoundEventName;

        private bool _audioActive;
        private bool _eligibleForRestart;
        private Collider _boundaryCollider;
        private AudioEventBindings _audioEventBindings;
        private Vector3 _lastPosition;

        private void Awake()
        {
            enabled = false;
        }

        private void LateUpdate()
        {
            if (_boundaryCollider == null || _audioEventBindings == null)
			{
                return;
			}

            bool hasMoved = Mathf.Abs(Vector3.Distance(_lastPosition, transform.position)) > Mathf.Epsilon;
            _lastPosition = transform.position;

            if (!_audioActive && _boundaryCollider.bounds.Contains(transform.position))
            {
                // play the sound
                _audioActive = true;
                _audioEventBindings.Play(_playAudioEventName);
                return;
            }

            if (_audioActive && !_boundaryCollider.bounds.Contains(transform.position))
			{
                // stop the sound
                _audioActive = false;
                _audioEventBindings.Play(_stopSoundEventName);
                return;
            }

            if (_audioActive && !hasMoved)
			{
                _eligibleForRestart = true;
                return;
            }

            if (_audioActive && hasMoved && _eligibleForRestart)
			{
                _eligibleForRestart = false;
                _audioEventBindings.Play(_playAudioEventName);
                return;
            }
        }

        public void SetCollider(Collider collider)
        {
            _boundaryCollider = collider;
        }

        public void SetAudioEventBindings(AudioEventBindings audioEventBindings)
		{
            _audioEventBindings = audioEventBindings;
        }
    }
}
