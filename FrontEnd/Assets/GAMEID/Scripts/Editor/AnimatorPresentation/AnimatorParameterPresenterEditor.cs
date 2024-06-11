#region Using

using System;
using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1.Editor;
using PixelUnited.NMG.Slots.Milan.GAMEID;
using UnityEditor;
using UnityEngine;

#endregion

[CustomEditor(typeof(AnimatorParameterPresenter))]
public class AnimatorParameterPresenterEditor : BaseEditor
{
    #region Helper Fields

    private bool showParametersFoldout = false;
    private bool creatingAnimatorParameter = false;
    private string newParamName = string.Empty;
    private AnimatorControllerParameterType newParamType = AnimatorControllerParameterType.Trigger;

    #endregion

    #region UI Methods

    /// <summary>
    /// Called every frame that the inspector is updated.
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle();
        style.wordWrap = true;
        style.richText = true;

        // Get local copies of the target.
        var animatorParameterPresenter = target as AnimatorParameterPresenter;
        var animatorParameters = animatorParameterPresenter.AnimatorParameters;

        // Only display the "WaitForTag" text box if that was the selected wait schema.
        if (animatorParameterPresenter.WaitType == AnimatorParameterPresenter.WaitSchema.WaitForTag)
        {
            animatorParameterPresenter.WaitForTag = EditorGUILayout.TextField("Wait-For-Tag Name:", animatorParameterPresenter.WaitForTag);
        }
        else if (animatorParameterPresenter.WaitType == AnimatorParameterPresenter.WaitSchema.Delay)
        {
            animatorParameterPresenter.WaitForDelay = EditorGUILayout.FloatField("Wait Delay", animatorParameterPresenter.WaitForDelay);
        }

        // "Create Animator Parameter" button.
        if (InspectorHelper.Button("Create Animator Parameter"))
        {
            creatingAnimatorParameter = true;
        }

        // Details on what kind of animator parameter to create if the create animator parameter button was pressed.
        if (creatingAnimatorParameter)
        {
            newParamName = EditorGUILayout.TextField("Parameter Name:", newParamName);
            var enumType = EditorGUILayout.EnumPopup("Parameter Type", newParamType);

            newParamType = (AnimatorControllerParameterType) enumType;

            // Display the add parameter button which will commit the selections made and add a new parameter to the list.
            if (InspectorHelper.Button("Add Parameter", 2))
            {
                AddParameter(animatorParameterPresenter, newParamType, true, newParamName);
                creatingAnimatorParameter = false;
                newParamName = string.Empty;
                newParamType = AnimatorControllerParameterType.Trigger;
            }

            // Cancels the creation of the new parameter.
            if (InspectorHelper.Button("Cancel", 2))
            {
                creatingAnimatorParameter = false;
                newParamName = string.Empty;
                newParamType = AnimatorControllerParameterType.Trigger;
            }
        }

        // Don't display the parameters foldout if there aren't any parameters yet.
        if (animatorParameters?.Count > 0)
        {
            showParametersFoldout = EditorGUILayout.Foldout(showParametersFoldout, "Parameters");
        }

        // Display the full list of animator parameters.
        if (showParametersFoldout && animatorParameterPresenter && animatorParameters?.Count > 0)
        {
            var paramIndex = 0;

            foreach (var parameter in animatorParameters)
            {
                var boolVal = parameter.BoolValue;
                var floatVal = parameter.FloatValue;
                var intVal = parameter.IntValue;
                ++paramIndex;

                EditorGUILayout.LabelField("<color=white><b>Parameter " + paramIndex + "</b></color>", style);
                InspectorHelper.FieldLabel("\tName: " + parameter.ParameterName, "Name of the parameter to send.");
                InspectorHelper.FieldLabel("\tType: " + parameter.ParameterType, "Type of parameter.");

                switch (parameter.ParameterType)
                {
                    case AnimatorControllerParameterType.Trigger:
                        break;
                    case AnimatorControllerParameterType.Bool:
                        parameter.BoolValue = EditorGUILayout.Toggle("\tBool Value:", boolVal);
                        break;
                    case AnimatorControllerParameterType.Int:
                        parameter.IntValue = EditorGUILayout.IntField("\tInt Value:", intVal);
                        break;
                    case AnimatorControllerParameterType.Float:
                        parameter.FloatValue = EditorGUILayout.FloatField("\tFloat Value:", floatVal);
                        break;
                    default:
                        break;
                }

                if (InspectorHelper.Button("Delete Parameter", 2))
                {
                    animatorParameterPresenter.AnimatorParameters.Remove(parameter);

                    // Force the editor to make it think the presenter is dirty so it knows to save it.
                    // Otherwise, it may not save changes to the parameters list - specifically if done in a prefab.
                    EditorUtility.SetDirty(animatorParameterPresenter);

                    return;
                }
            }
        }
        else
        {
            return;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Generic method to add a parameter to the animator parameters list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="presenter"></param>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <param name="paramName"></param>
    private void AddParameter<T>(AnimatorParameterPresenter presenter, AnimatorControllerParameterType type, T data, string paramName)
    {
        if (presenter.AnimatorParameters == null)
        {
            presenter.AnimatorParameters = new List<AnimatorParameterData>();
        }

        switch (type)
        {
            case AnimatorControllerParameterType.Trigger:
                presenter.AnimatorParameters.Add(new AnimatorParameterData(paramName, AnimatorControllerParameterType.Trigger));
                break;
            case AnimatorControllerParameterType.Bool:
                presenter.AnimatorParameters.Add(new AnimatorParameterData(paramName, AnimatorControllerParameterType.Bool,Convert.ToBoolean(data)));
                break;
            case AnimatorControllerParameterType.Int:
                presenter.AnimatorParameters.Add(new AnimatorParameterData(paramName, AnimatorControllerParameterType.Int, Convert.ToInt32(data)));
                break;
            case AnimatorControllerParameterType.Float:
                presenter.AnimatorParameters.Add(new AnimatorParameterData(paramName, AnimatorControllerParameterType.Float, Convert.ToSingle(data)));
                break;
            default:
                break;
        }

        // Force the editor to make it think the presenter is dirty so it knows to save it.
        // Otherwise, it may not save changes to the parameters list - specifically if done in a prefab.
        EditorUtility.SetDirty(presenter);
    }

    #endregion
}
