#region Using

using System;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Data set that encompasses parameters and their values for AnimatorControllers.
    /// </summary>
    [Serializable]
    public class AnimatorParameterData
    {
        #region Data

        /// <summary>
        /// The type of animator controller parameter.
        /// </summary>
        [HideInInspector]
        public AnimatorControllerParameterType ParameterType;

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        [HideInInspector]
        public string ParameterName;

        /// <summary>
        /// Float value to set the parameter to, if applicable.
        /// </summary>
        [HideInInspector]
        public float FloatValue;

        /// <summary>
        /// Int value to set the parameter to, if applicable.
        /// </summary>
        [HideInInspector]
        public int IntValue;

        /// <summary>
        /// Bool value to set the parameter to, if applicable.
        /// </summary>
        [HideInInspector]
        public bool BoolValue;

        #endregion

        #region Constructors

        public AnimatorParameterData(string parameterName, AnimatorControllerParameterType type)
        {
            ParameterName = parameterName;
            ParameterType = type;
        }

        public AnimatorParameterData(string parameterName, AnimatorControllerParameterType type, bool boolVal)
        {
            ParameterName = parameterName;
            ParameterType = type;
            BoolValue = boolVal;
        }

        public AnimatorParameterData(string parameterName, AnimatorControllerParameterType type, int intVal)
        {
            ParameterName = parameterName;
            ParameterType = type;
            IntValue = intVal;
        }

        public AnimatorParameterData(string parameterName, AnimatorControllerParameterType type, float floatVal)
        {
            ParameterName = parameterName;
            ParameterType = type;
            FloatValue = floatVal;
        }

        #endregion
    }
}
