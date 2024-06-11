#region Using

using System;
using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.GameState
{
    /// <summary>
    /// A model to keep track of what macro state the game is currently in, such as BaseGame, HoldAndSpin and so forth.
    /// </summary>
    public class GameStateModel : IModel
    {
        #region Properties

        /// <summary>
        /// Current game state as an enum.
        /// </summary>
        public GameStateEnum GameState { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceLocator"></param>
        public GameStateModel(ServiceLocator serviceLocator)
        {
            GameState = GameStateEnum.BaseSpin;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the game's state using the GameStateEnum.
        /// </summary>
        /// <param name="gameState"></param>
        public void SetGameState(GameStateEnum gameState)
        {
            GameState = gameState;
        }

        /// <summary>
        /// Given a string, will convert it to the GameStateEnum and set the GameState to it.
        /// </summary>
        /// <param name="gameState"></param>
        public void SetGameState(string gameState)
        {
            var convertedState = ConvertStateStringToEnum(gameState);

            GameState = convertedState;
        }

        public bool InHoldAndSpinState()
        {
            return GameState is GameStateEnum.HoldAndSpin;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts the string name of the state into a GameStateEnum.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual GameStateEnum ConvertStateStringToEnum(string state)
        {
            if (Enum.TryParse(state, out GameStateEnum stateEnum))
            {
                return stateEnum;
            }
            else
            {
                GameIdLogger.Logger.Error("ConvertStateStringToEnum failed! The state " + state + " does not exist in the GameStateEnum!" +
                               " The string value must be an exact match!");

                return GameStateEnum.Invalid;
            }
        }

        #endregion
    }
}
