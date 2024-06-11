using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using RotaryHeart.Lib.AutoComplete;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ArtUtil
{
    public delegate void FlyUpCompleteEvent();

    public enum DurationType
    {
        SpeedBased,
        Constant
    }
    [Serializable]
    public class BezierControlPoint
    {
        [Range(-20f, 20f)] public float XOffset;
        [Range(-20f, 20f)] public float YOffset;
    }

    public class AutomatedFlyUp : MonoBehaviour
    {
        [SerializeField] DurationType _durationType = DurationType.SpeedBased;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
        [Tooltip("Randomize the bezier handle offset from 0 to the set value")]
        [SerializeField] private bool _randomizeHandleOffset = false;
        [Tooltip("Percentage of the defined handle offset to consider as the floor")]
        [SerializeField][Range(0.0f, 1.0f)] private float _randomizedOffsetFloor = 0.5f;
        [SerializeField] private BezierControlPoint _startHandle;
        [SerializeField] private BezierControlPoint _endHandle;
        [SerializeField] private bool _testDoober = false;

        [SerializeField][AutoComplete(typeof(AudioEventsPropertyDrawerHelper), "AudioEventsNames")] private string _dooberStartAudioEvent;
        [SerializeField][AutoComplete(typeof(AudioEventsPropertyDrawerHelper), "AudioEventsNames")] private string _dooberEndAudioEvent;
        [SerializeField][AutoComplete(typeof(AudioEventsPropertyDrawerHelper), "AudioEventsNames")] private string _cancelAudioEvent;
        private AudioEventBindings _audioEventBindings;
        private Transform _targetTransform;
        private ParticleSystem _hitParticle;
        private List<ParticleSystem> _particleSystems = new List<ParticleSystem>();
        //private Vector3 _startPosition;
        private BezierControlPoint _targetStartHandle;
        private BezierControlPoint _targetEndHandle;

#if UNITY_EDITOR
        public void Update()
        {
            if (_testDoober)
            {
                _testDoober = false;
                PositionPrefab(transform.position);
                SetTargetTransform(transform.parent);
                Coroutine.Start(MoveOnAnimationCurve());
            }
        }
#endif
        public void SetTargetTransform(Transform target)
        {
            _targetTransform = target;
        }

        public void SetHitParticle(ParticleSystem hitParticle)
        {
            _hitParticle = hitParticle;
        }

        public void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
            _targetStartHandle = new BezierControlPoint()
            {
                XOffset = _startHandle.XOffset,
                YOffset = _startHandle.YOffset
            };
            _targetEndHandle = new BezierControlPoint()
            {
                XOffset = _endHandle.XOffset,
                YOffset = _endHandle.YOffset
            };
            _audioEventBindings = transform.GetComponentInParent<AudioEventBindings>();
        }

        public void PositionPrefab(Vector3 position)
        {
            _particleSystems.ForEach(system => system.Stop());
            transform.position = position;
        }

        public void StartFlyUp(FlyUpCompleteEvent callback = null)
        {
            Coroutine.Start(MoveOnAnimationCurve(callback));
        }

        public IEnumerator<Yield> MoveOnAnimationCurve(FlyUpCompleteEvent callback = null)
        {
            if (_randomizeHandleOffset)
            {
                RandomizeTargetHandles();
            }

            PlayParticles();
            PlayDooberAudio(_dooberStartAudioEvent);

            var startPosition = transform.position;
            var endPosition = _targetTransform.position;

            float totalTime;
            if (_durationType.Equals(DurationType.SpeedBased))
            {
                float totalDistance = Vector3.Distance(startPosition, endPosition); //get the distance information before entering the loop
                totalTime = totalDistance / _speed; //move the particles along the curve to the end point
            }
            else
            {
                totalTime = _duration;
            }

            float timeProg = 0f;
            bool flipOffsetX = startPosition.x > endPosition.x;
            bool flipOffsetY = startPosition.y > endPosition.y;

            while (timeProg < totalTime)
            {
                var dt = Time.deltaTime;
                timeProg += dt;
                var curveTime = _curve.Evaluate(Mathf.Clamp01(timeProg / totalTime));
                transform.position = GetNextPosition(startPosition, endPosition, curveTime, flipOffsetX, flipOffsetY);
                yield return new YieldForUpdate();
            }
            transform.position = _targetTransform.position;

            StopParticles();

            PlayDooberAudio(_dooberEndAudioEvent);
            if (_hitParticle != null)
            {
                _hitParticle.Play();
            }
        }

        protected void PlayParticles()
        {
            _particleSystems.ForEach(system => system.Play());
        }

        public void StopParticles()
        {
            _particleSystems.ForEach(system => system.Stop());
        }

        public void OnCancel()
        {
            PlayDooberAudio(_cancelAudioEvent);
            _particleSystems.ForEach(system =>
            {
                system.Stop();
                system.Clear();
            });
        }

        private void RandomizeTargetHandles()
        {
            _targetStartHandle.XOffset = UnityEngine.Random.Range(_randomizedOffsetFloor * _startHandle.XOffset, _startHandle.XOffset);
            _targetStartHandle.YOffset = UnityEngine.Random.Range(_randomizedOffsetFloor * _startHandle.YOffset, _startHandle.YOffset);
            _targetEndHandle.XOffset = UnityEngine.Random.Range(_randomizedOffsetFloor * _endHandle.XOffset, _endHandle.XOffset);
            _targetEndHandle.YOffset = UnityEngine.Random.Range(_randomizedOffsetFloor * _endHandle.YOffset, _endHandle.YOffset);
        }

        private float Bezier(float p0, float p1, float p2, float p3, float t)
        {
            float tInv = (1 - t);
            float result = p0 * tInv * tInv * tInv;
            result += 3 * p1 * tInv * tInv * t;
            result += 3 * p2 * t * t * tInv;
            result += p3 * t * t * t;
            return result;
        }

        private Vector3 GetNextPosition(Vector3 startPosition, Vector3 targetPosition, float curveTime, bool flipOffsetX, bool flipOffsetY)
        {
            float x;
            float y;

            if (flipOffsetX)
            {
                x = Bezier(startPosition.x, startPosition.x + _targetStartHandle.XOffset, targetPosition.x - _targetEndHandle.XOffset, targetPosition.x, curveTime);
            }
            else
            {
                x = Bezier(startPosition.x, startPosition.x - _targetStartHandle.XOffset, targetPosition.x + _targetEndHandle.XOffset, targetPosition.x, curveTime);
            }

            if (flipOffsetY)
            {
                y = Bezier(startPosition.y, startPosition.y - _targetStartHandle.YOffset, targetPosition.y + _targetEndHandle.YOffset, targetPosition.y, curveTime);
            }
            else
            {
                y = Bezier(startPosition.y, startPosition.y + _targetStartHandle.YOffset, targetPosition.y - _targetEndHandle.YOffset, targetPosition.y, curveTime);
            }

            float yDist = Mathf.Abs(targetPosition.y - startPosition.y);
            float progY = Mathf.Abs(transform.position.y - startPosition.y);

            float zTime = progY / yDist;

            float z = Mathf.Lerp(startPosition.z, targetPosition.z, zTime);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Handles sound effect execution, if present
        /// </summary>
        private void PlayDooberAudio(string audioEvent)
        {
            if (!string.IsNullOrEmpty(audioEvent))
            {
                _audioEventBindings.Play(audioEvent);
            }
        }
    }
}
