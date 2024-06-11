#region Using

using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Finds everything that implements IRecoverable and runs the OnRecovery method.
    /// Meant to be attached to all recovery states.
    /// </summary>
    public class RecoveryPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        #region Fields

        /// <summary>
        /// List of scripts in the scene that implement IRecoverable.
        /// </summary>
        protected List<IRecoverable> Recoverables;

        /// <summary>
        /// Game state model, tells us the current state of the game.
        /// </summary>
        [FieldRequiresModel] private GameStateModel _gameStateModel;

        #endregion

        #region Interface Implementations

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }


        public void OnServicesLoaded()
        {
            this.InitializeDependencies();

            // Find all IRecoverables in the scene.
            Recoverables = transform.root.gameObject.GetComponentsInChildren<IRecoverable>().ToList();
        }

        #endregion

        #region State Handling

        public IEnumerator<Yield> Enter()
        {
            if (Recoverables?.Count > 0)
            {
                // Iterate through everything that implements IRecoverable...
                foreach (var recoverable in Recoverables)
                {
                    // If we're in an eligible game state, run the OnRecovery method.
                    if (recoverable.RecoveryEligibleGameStates.Contains(_gameStateModel.GameState))
                    {
                        recoverable.OnRecovery();
                    }
                }
            }

            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

        #endregion
    }
}
