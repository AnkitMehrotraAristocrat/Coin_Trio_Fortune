using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[System.Serializable]
	public class ObjectShake : MonoBehaviour
	{
		public bool enabled = false;
		public Vector3 shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
		[Range(0, 0.1f)] public float shakesDelay = 0;
		public float shakeDuration = 0;
		public GameObject gameObjectToShake;

		[System.NonSerialized] public bool isShaking = false;
		Vector3 objectPreRenderPosition;
		Vector3 shakeVector;
		float delaysTimer;
		bool refreshPosition;

		public void Start()
		{
			if (gameObjectToShake == null)
			{
				gameObjectToShake = this.gameObject;
			}

			objectPreRenderPosition = gameObjectToShake.transform.localPosition;
		}

		public void Update()
		{
			if (enabled)
			{
				if (shakeDuration > 0)
				{
					shakeDuration -= Time.deltaTime;
					if (shakeDuration <= 0)
					{
						enabled = false;
						shakeDuration = 0;
					}
					else
					{
						AnimateShake();
					}
				}
				else
				{
					enabled = false;
					//AnimateShake();
				}
			}
			else
			{
				if (refreshPosition)
				{
					StopShake();
					GameIdLogger.Logger.Debug("Refresh");
				}
			}
		}

		void onPreShake()
		{
			objectPreRenderPosition = gameObjectToShake.transform.localPosition;
			refreshPosition = true;
			GameIdLogger.Logger.Debug("Reset Refresh");
		}

		void onPostShake()
		{
			gameObjectToShake.transform.localPosition = objectPreRenderPosition;
			refreshPosition = false;
		}

		public void StartShake()
		{
			if (isShaking)
			{
				StopShake();
			}

			isShaking = true;
			onPreShake();
		}

		public void StopShake()
		{
			isShaking = false;
			shakeVector = Vector3.zero;
			onPostShake();
		}

		public void AnimateShake()
		{
			if (!isShaking)
			{
				this.StartShake();
			}

			// delay between each shake
			if (shakesDelay > 0)
			{
				delaysTimer += Time.deltaTime;
				if (delaysTimer < shakesDelay)
				{
					return;
				}
				else
				{
					while (delaysTimer >= shakesDelay)
					{
						delaysTimer -= shakesDelay;
					}
				}
			}

			var randomVec = new Vector3(Random.value, Random.value, Random.value);
			shakeVector = Vector3.Scale(randomVec, shakeStrength) * (Random.value > 0.5f ? -1 : 1);

			gameObjectToShake.transform.localPosition = objectPreRenderPosition + shakeVector;
		}
	}
}