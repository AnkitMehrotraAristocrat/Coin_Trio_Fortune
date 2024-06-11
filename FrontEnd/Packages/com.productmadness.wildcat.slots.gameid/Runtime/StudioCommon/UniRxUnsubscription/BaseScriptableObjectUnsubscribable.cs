#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class BaseScriptableObjectUnsubscribable : ScriptableObject, IUnsubscribable
    {
        private List<IDisposable> _subscriptions = new List<IDisposable>();

        /// <summary>
        /// A list of every subscription the implementing class has made.
        /// </summary>
        public List<IDisposable> Subscriptions
        {
            get
            {
                // Make sure that we subscribe ourselves to the UnsubscribeOnDestroy static Unsubscribables list the first time get the subscriptions list.
                if (UnsubscribeOnDestroy.Unsubscribables.FirstOrDefault(unsub => unsub == this) == null)
                {
                    UnsubscribeOnDestroy.Unsubscribables.Add(this);
                }

                return _subscriptions;
            }
            set
            {
                _subscriptions = value;
            }
        }

        /// <summary>
        /// Disposes of all subscriptions.
        /// </summary>
        public void Unsubscribe()
        {
            foreach (var subscription in Subscriptions)
            {
                subscription.Dispose();
            }
            UnsubscribeOnDestroy.Unsubscribables.Remove(this);
        }
    }
}
