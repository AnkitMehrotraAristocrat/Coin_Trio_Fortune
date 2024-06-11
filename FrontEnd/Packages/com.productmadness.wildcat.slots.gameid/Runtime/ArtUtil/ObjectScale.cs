using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[System.Serializable]
	public class ObjectScale : MonoBehaviour
	{
		[SerializeField] private Vector3 _targetScale = new Vector3(1.1f, 1.1f, 1.1f);
		[SerializeField] private AnimationCurve _scaleAnimationCurve;
		[SerializeField] private float _scaleDuration = 0.25f;
		[SerializeField] private GameObject _gameObjectToScale;

		private Vector3 _startScale;

        public void Start()
		{
			if (_gameObjectToScale == null)
			{
				_gameObjectToScale = this.gameObject;
			}

			_startScale = _gameObjectToScale.transform.localScale;
		}

        public void PlayScaleAnimation()
        {
            Coroutine.Start(StartScale(_scaleDuration));
        }

        IEnumerator<Yield> StartScale(float time)
        {
            onPreScale();

            float timer = 0.0f;
            Vector3 startScale = _startScale;

            while (timer < time)
            {
                onPostScale();

                Vector3 scale = _targetScale * _scaleAnimationCurve.Evaluate(timer / time);
                scale += startScale;
                _gameObjectToScale.transform.localScale = scale;
                timer += Time.deltaTime;
                yield return new YieldForUpdate();
            }

            onPostScale();
        }

        void onPreScale()
		{
			_gameObjectToScale.transform.localScale = _startScale;
        }

		void onPostScale()
		{
			_gameObjectToScale.transform.localScale = _startScale;
        }
	}
}
