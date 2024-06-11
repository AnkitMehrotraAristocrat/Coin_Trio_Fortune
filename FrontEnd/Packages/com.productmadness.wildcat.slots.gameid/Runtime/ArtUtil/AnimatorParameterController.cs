#region Using

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Slotsburg.Slots.SharedFeatures;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ArtUtil 
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AnimatorParameterController))]
    public class AnimatorParameterControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //get local copies of the main class's variables for ease of use
            var animatorParameterController = target as AnimatorParameterController;
            var controller = animatorParameterController.Controller;
            var parameters = animatorParameterController.Parameters;
            var parameterIndex = animatorParameterController.ParameterIndex;

            if (controller != null && parameterIndex > controller.parameters.Length)
            {
                return;
            }

            //show the proper information for the type of parameter chosen
            if (controller && parameterIndex > -1 && controller.parameters.Length > 0)
            {
                switch (parameters[parameterIndex].type)
                {
                    case AnimatorControllerParameterType.Trigger:
                        animatorParameterController.ParameterName = controller.parameters[parameterIndex].name;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animatorParameterController.SetBool = EditorGUILayout.Toggle("Set Boolean", animatorParameterController.SetBool);
                        animatorParameterController.ParameterName = controller.parameters[parameterIndex].name;
                        break;
                    case AnimatorControllerParameterType.Int:
                        animatorParameterController.SetInt = EditorGUILayout.IntField("Set Integer", animatorParameterController.SetInt);
                        animatorParameterController.ParameterName = controller.parameters[parameterIndex].name;
                        break;
                    case AnimatorControllerParameterType.Float:
                        animatorParameterController.SetFloat = EditorGUILayout.FloatField("Set Float", animatorParameterController.SetFloat);
                        animatorParameterController.ParameterName = controller.parameters[parameterIndex].name;
                        break;
                    default:
                        break;
                }
            }
            else { return; }
        }
    }
#endif 

    /// <summary>
    /// A tool that helps to set a specific parameter on a specific animator controller.
    /// </summary>
    public class AnimatorParameterController : MonoBehaviour
    {
        #region Inspector

        /// <summary>
        /// Tag used to uniquely identify this animator parameter controller.
        /// </summary>
        public string Tag;

        public Animator Controller;
        public string[] AnimationParameters;
        public int ParameterIndex = -1;
        [Label] public string ParameterName = "";

        public AnimatorControllerParameter[] Parameters; 

        // The values to set to, if applicable.
        [HideInInspector] public bool SetBool = false;
        [HideInInspector] public int SetInt = 0;
        [HideInInspector] public float SetFloat = 0f;

        #endregion

        #region Initialization

#if UNITY_EDITOR
        //make sure the information in the inspector is up to date
        private void OnValidate() 
        {
            GetParameters();
        }
#else
        private void Awake() 
        {
            GetParameters();
		}
#endif
        /// <summary>
        /// Get the parameters from the current animation controller.
        /// </summary>
        private void GetParameters()
        {
            //validate input
            if (!Controller)
            {
                AnimationParameters = new string[0];
                return;
            }

            // There is a bug in Unity introduced sometime in Unity 2020 where, after saving the scene, an animator
            // incorrectly returns its parameter list as 0 until it's been reenabled. This code toggles the enabled
            // status of the animator in question, resolving the issue.
            Controller.enabled = !Controller.enabled;
            Controller.enabled = !Controller.enabled;

            Parameters = Controller.parameters;
            PopulateParameterNames();
        }

        /// <summary>
        /// Populate the editor with the current animation controller's parameters.
        /// </summary>
        void PopulateParameterNames()
        {
            AnimationParameters = new string[Parameters.Length];

            for (int i = 0; i < AnimationParameters.Length; i++)
            {
                AnimationParameters[i] = Parameters[i].name;
            }
        }

        #endregion

        #region Animation Parameter Methods

        /// <summary>
        /// Will set the preconfigured parameter on the animator controller.
        /// </summary>
        public void SetParamaterData()
        {
            // Get the type of parameter that this APC is set for.
            switch (Parameters[ParameterIndex].type)
            {
                // If it's a trigger, just set it.
                case AnimatorControllerParameterType.Trigger:
                    Controller.ResetTrigger(Parameters[ParameterIndex].name);
                    Controller.SetTrigger(Parameters[ParameterIndex].name);
                    break;
                // For the rest of these, just verify that the parameter isn't already set to what we want it to be and if it isn't then set it.
                case AnimatorControllerParameterType.Bool:
                    if (Controller.GetBool(Parameters[ParameterIndex].name) != SetBool)
                    {
                        Controller.SetBool(Parameters[ParameterIndex].name, SetBool);
                    }
                    break;
                case AnimatorControllerParameterType.Int:
                    if (Controller.GetInteger(Parameters[ParameterIndex].name) != SetInt)
                    {
                        Controller.SetInteger(Parameters[ParameterIndex].name, SetInt);
                    }
                    break;
                case AnimatorControllerParameterType.Float:
                    if (Controller.GetFloat(Parameters[ParameterIndex].name) != SetFloat)
                    {
                        Controller.SetFloat(Parameters[ParameterIndex].name, SetFloat);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
