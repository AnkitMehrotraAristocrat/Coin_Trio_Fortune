#region Using

using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;

using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Calls other presenter's OnEnter or OnExit methods depending on the spin response heard.
    /// Will only do this during the listed EligibleGameStates.
    /// </summary>
    public class SpinRespondingExecutionPresenter : MonoBehaviour, ServiceLocator.IHandler, ISpinResponder
    {
        #region Inspector

        [Tooltip("List of presenters that will execute their OnEnter methods when the SpinStarted event is heard.")]
        public List<MonoBehaviour> SpinStartedOnEnterPresenters;

        [Tooltip("List of presenters that will execute their OnExit methods when the SpinStarted event is heard.")]
        public List<MonoBehaviour> SpinStartedOnExitPresenters;

        [Tooltip("List of presenters that will execute their OnEnter methods when the SpinComplete event is heard.")]
        public List<MonoBehaviour> SpinCompleteOnEnterPresenters;

        [Tooltip("List of presenters that will execute their OnExit methods when the SpinComplete event is heard.")]
        public List<MonoBehaviour> SpinCompleteOnExitPresenters;

        /// <summary>
        /// Protected version of eligible game states that can be edited in the inspector.
        /// </summary>
        [SerializeField]
        protected List<GameStateEnum> EligibleGameStates;

        #endregion

        #region Models

        [FieldRequiresModel]
        protected GameStateModel GameStateModel;

        #endregion

        #region Interface Implementations

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        #endregion

        #region Monobehaviour Methods

        public virtual void Start()
        {
            // Make sure the linked MonoBehaviours are state presenters.
            VerifyStatePresenters();
        }

        /// <summary>
        /// Checks to make sure all the included components implement IStatePresenter.
        /// </summary>
        protected virtual void VerifyStatePresenters()
        {
            var presenters = new List<MonoBehaviour>();

            presenters = presenters.Concat(SpinStartedOnEnterPresenters).ToList();
            presenters = presenters.Concat(SpinStartedOnExitPresenters).ToList();
            presenters = presenters.Concat(SpinCompleteOnEnterPresenters).ToList();
            presenters = presenters.Concat(SpinCompleteOnExitPresenters).ToList();

            if (!presenters.All(presenter => presenter is IStatePresenter))
            {
                GameIdLogger.Logger.Error("A component has been attached to the presenter lists that is not of type IStatePresenter! " +
                               "Please make sure only IStatePresenter are used in the presenters lists!");
            }
        }

        #endregion

        #region ISpinResponder Implementation

        public virtual IEnumerator<Yield> SpinStarted()
        {
            // Only execute this for eligible game states.
            if (EligibleGameStates.Contains(GameStateModel.GameState))
            {
                foreach (IStatePresenter presenter in SpinStartedOnEnterPresenters)
                {
                    Coroutine.Start(presenter.Enter());
                }

                foreach (IStatePresenter presenter in SpinStartedOnExitPresenters)
                {
                    Coroutine.Start(presenter.Exit());
                }
            }

            yield return null;
        }

        public virtual IEnumerator<Yield> SpinComplete()
        {
            // Only execute this for eligible game states.
            if (EligibleGameStates.Contains(GameStateModel.GameState))
            {
                foreach (IStatePresenter presenter in SpinCompleteOnEnterPresenters)
                {
                    Coroutine.Start(presenter.Enter());
                }

                foreach (IStatePresenter presenter in SpinCompleteOnExitPresenters)
                {
                    Coroutine.Start(presenter.Exit());
                }
            }

            yield return null;
        }

        #endregion
    }
}
