#region Using

using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Component meant to only have one instance in the scene.
    /// As long as the component exists in the scene, it will make sure that all IUnsubscribables unsubscribe their UniRx subscriptions.
    /// </summary>
    public class UnsubscribeOnDestroy : MonoBehaviour
    {
        #region Public Fields

        /// <summary>
        /// A list of everything that's implemented IUnsubscribable and has written to its Subscriptions list.
        /// Whenever a Subscription is added to the list, it automatically registers itself to this static list.
        /// </summary>
        public static List<IUnsubscribable> Unsubscribables = new List<IUnsubscribable>();

        #endregion

        #region MonoBehaviour Methods

        protected virtual void Start()
        {
            // Display an error in the editor if more than once instance of this component exists.
            if (Object.FindObjectsOfType<UnsubscribeOnDestroy>().Length > 1)
            {
                GameIdLogger.Logger.Error("You have more than on instance of the UnsubscribeOnDestroy component in the scene! Make sure there's only one instance!");
            }
        }

        protected virtual void OnDestroy()
        {
            while (Unsubscribables.Count > 0)
            {
                Unsubscribables[0].Unsubscribe();
            }
        }

        #endregion
    }
}
