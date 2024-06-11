#region Using

using System;
using System.Collections.Generic;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Interface to help ensure any UniRx subscriptions get unsubscribed. 
    /// This is necessary because we cannot use the .AddTo(this) call at the end of a subscription in non-monobehavior classes.
    /// Instead, anything that implements this interface must be responsible for adding their subscriptions to the Subscriptions list.
    /// </summary>
    public interface IUnsubscribable
    {
        /// <summary>
        /// A list of every subscription the implementing class has made.
        /// </summary>
        List<IDisposable> Subscriptions { get; set; }

        /// <summary>
        /// Disposes of all subscriptions.
        /// </summary>
        void Unsubscribe();
    }
}