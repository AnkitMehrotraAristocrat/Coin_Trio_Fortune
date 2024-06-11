using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Handles disabling debug logs unless we're on a development build or in the unity editor.
    /// </summary>
    public class LoggingHandler : MonoBehaviour
    {
        protected void Start()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        protected void OnDestroy()
        {
            // Turn logging back on as we exit the slot.
            Debug.unityLogger.logEnabled = true;
        }
    }
}