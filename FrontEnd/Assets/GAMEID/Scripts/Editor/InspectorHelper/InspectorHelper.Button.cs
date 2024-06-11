using UnityEngine;

/// <summary>
/// Editor creation utility methods for creating button elements.
/// </summary>
public static partial class InspectorHelper
{
    #region Without Tabbing

    /// <summary>
    /// Create a nested button that has a label.
    /// </summary>
    /// <param name="label">Label to put on the button.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layouting properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Flag indicating if the button was pressed.</returns>
    /// <example>
    /// if(InspectorHelper.Button("Button Label"))
    /// {
    ///     // Do button pressed actions here.
    /// }
    /// </example>
    public static bool Button(string label, params GUILayoutOption[] options)
    {
        return Button(label, null, 0, options);
    }

    /// <summary>
    /// Create a button that has a label and tool tip.
    /// </summary>
    /// <param name="label">Label to put on the button.</param>
    /// <param name="toolTip">Tool tip information for users.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layouting properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Flag indicating if the button was pressed.</returns>
    /// <example>
    /// if(InspectorHelper.Button("Button Label", "Important Info For the User."))
    /// {
    ///     // Do button pressed actions here.
    /// }
    /// </example>
    public static bool Button(string label, string toolTip, params GUILayoutOption[] options)
    {
        return Button(label, toolTip, 0, options);
    }

    #endregion

    #region With Tabbing

    /// <summary>
    /// Create a nested button that has a label and is tabbed in N times.
    /// </summary>
    /// <param name="label">Label to put on the button.</param>
    /// <param name="nestLevel">Number of levels to tab in this GUI element.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layouting properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Flag indicating if the button was pressed.</returns>
    /// <example>
    /// if(InspectorHelper.Button("Button Label", 1))
    /// {
    ///     // Do button pressed actions here.
    /// }
    /// </example>
    public static bool Button(string label, int nestLevel, params GUILayoutOption[] options)
    {
        return Button(label, null, nestLevel, options);
    }

    /// <summary>
    /// Create a nested button that has a label and tool tip.
    /// </summary>
    /// <param name="label">Label to put on the button.</param>
    /// <param name="toolTip">Tool tip information for users.</param>
    /// <param name="nestLevel">Number of levels to tab in this GUI element.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layouting properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Flag indicating if the button was pressed.</returns>
    /// <example>
    /// if(InspectorHelper.Button("Button Label", "Important Info For the User.", 1))
    /// {
    ///     // Do button pressed actions here.
    /// }
    /// </example>
    public static bool Button(string label, string toolTip, int nestLevel, params GUILayoutOption[] options)
    {
        BeginHorizontal(nestLevel);

        var labelContent = string.IsNullOrEmpty(toolTip)
                                ? new GUIContent(label)
                                : new GUIContent(label, toolTip);
        var buttonResult = GUILayout.Button(labelContent, options);

        EndHorizontal();

        return buttonResult;
    }

    #endregion
}