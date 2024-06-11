#region Using

using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Presenter meant to change the current game context.
    /// </summary>
    public class GameContextPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        public GameContext ContextToChangeTo;

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public IEnumerator<Yield> Enter()
        {
            CurrentGameContext.Context = ContextToChangeTo;

            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

    }
}
