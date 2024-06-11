using RotaryHeart.Lib.AutoComplete;
using System;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.RotaryHeart.Lib.AutoComplete
{
    public static class AutoCompleteTextField
    {
#if UNITY_EDITOR

        /// <summary>
        /// Uses UnityEditor.EditorGUILayout to draw the text field
        /// </summary>
        public static class EditorGUILayout
        {
            #region Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string text, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField("", text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string text, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField("", text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string text, GUIStyle style, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField("", text, style, entries, "", allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField("", text, style, entries, hint, allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string label, string text, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string label, string text, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string label, string text, GUIStyle style, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, style, entries, "", allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(string label, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(new GUIContent(label), text, style, entries, hint, allowCustom, options);
            }

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(GUIContent label, string text, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(GUIContent label, string text, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, options);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(GUIContent label, string text, GUIStyle style, string[] entries, bool allowCustom = false, params GUILayoutOption[] options)
            {
                return AutoCompleteTextField(label, text, style, entries, "", allowCustom, options);
            }

            #endregion Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="options">
            /// An optional list of layout options that specify extra layouting properties.<para>&#160;</para>
            /// Any values passed in here will override settings defined by the style.<para>&#160;</para>
            /// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.
            /// </param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(GUIContent label, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false, params GUILayoutOption[] options)
            {
                //Get the rect to draw the text field
                Rect lastRect = UnityEditor.EditorGUILayout.GetControlRect(!string.IsNullOrEmpty(label.text), UnityEditor.EditorGUIUtility.singleLineHeight, style, options);

                //Draw it without using layout
                return EditorGUI.AutoCompleteTextField(lastRect, label, text, style, entries, hint, allowCustom);
            }
        }

        /// <summary>
        /// Uses UnityEditor.EditorGUI to draw the text field
        /// </summary>
        public static class EditorGUI
        {
            #region NMG Mods
            public static string NMGAutoCompleteTextField(Rect position, string label, string text, GUIStyle style, SerializedProperty property, Func<SerializedProperty, string[]> entriesCallback, string hint, bool allowCustom = false)
            {
                return NMGAutoCompleteTextField(position, new GUIContent(label), text, style, property, entriesCallback, hint, allowCustom);
            }

            public static string NMGAutoCompleteTextField(Rect position, GUIContent label, string text, GUIStyle style, SerializedProperty property, Func<SerializedProperty, string[]> entriesCallback, string hint, bool allowCustom = false)
            {
                //Check for focus, the system is only shown if the text field is focused
                UnityEngine.GUI.SetNextControlName("CheckFocus");
                string value = UnityEditor.EditorGUI.TextField(position, label, text, style);

                //Hack to avoid hint being drawn on the wrong location for attribute drawers
                M_MyStyle.normal.textColor = Color.clear;
                UnityEngine.GUI.enabled = false;
                UnityEditor.EditorGUI.TextField(position, " ", " ", M_MyStyle);
                UnityEngine.GUI.enabled = true;

                //Display the hint
                if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(hint))
                {
                    //Nothing is typed, show a hint to the user
                    UnityEngine.GUI.enabled = false;

                    UnityEditor.EditorGUI.TextField(position, label, " " + hint, UnityEngine.GUI.skin.label);
                    UnityEngine.GUI.enabled = true;
                }

                return _NMGAutoCompleteLogic(position, label, value, property, entriesCallback, allowCustom, true, null);
            }

            static string _NMGAutoCompleteLogic(Rect position, GUIContent label, string text, SerializedProperty property, Func<SerializedProperty, string[]> entriesCallback, bool allowCustom, bool fromEditor, GUI.IStyle windowStyle)
            {
                //Used to draw the window
                Rect lastRect = position;
                Vector2 myScreenPos = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));

                //The system returned a value, need to check if this UI element is the one that called
                if (M_returnedValue && M_returnedScreenPos == myScreenPos)
                {
                    M_returnedValue = false;
                    string val = M_ReturnedContent.text;
                    M_ReturnedContent.text = "";
                    M_returnedScreenPos = Vector2.zero;
                    return val;
                }

                //Only display the system if the text field is focused
                if (UnityEngine.GUI.GetNameOfFocusedControl().Equals("CheckFocus"))
                {
                    //New position for this window
                    Rect newRect = lastRect;

                    //Remove focus
                    UnityEngine.GUI.FocusControl(null);

                    if (fromEditor)
                    {
#if UNITY_EDITOR
                        if (!string.IsNullOrEmpty(label.text))
                        {
                            float offset = UnityEditor.EditorGUIUtility.labelWidth - ((UnityEditor.EditorGUI.indentLevel) * 15);
                            newRect.x += offset;
                            newRect.width -= offset;
                        }

                        newRect.x += UnityEditor.EditorGUI.indentLevel * 15;
                        newRect.width -= UnityEditor.EditorGUI.indentLevel * 15;

                        EditorAddItemWindow.Show(newRect, entriesCallback(property), null, s =>
                        {
                            M_ReturnedContent.text = s;
                            M_returnedValue = true;
                            M_returnedScreenPos = myScreenPos;
                        }, "/", returnFullPath: false, allowCustom: allowCustom);
#endif
                    }
                    else
                    {
                        M_addItemWindow = new AddItemWindow();
                        M_addItemWindow.Show(newRect, entriesCallback(property), null, s =>
                        {
                            M_ReturnedContent.text = s;
                            M_returnedValue = true;
                            M_returnedScreenPos = myScreenPos;
                        }, "/", returnFullPath: false, allowCustom: allowCustom, style: windowStyle);
                    }
                }

                if (M_addItemWindow != null && new Rect(lastRect.position, new Vector2(lastRect.width, 320)) == M_addItemWindow.Position)
                {
                    if (M_addItemWindow.Closed)
                    {
                        M_addItemWindow = null;
                    }
                    else
                    {
                        M_addItemWindow.OnGUI();
                    }
                }

                return text;
            }
            #endregion


            #region Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, "", text, UnityEngine.GUI.skin.textField, entries, "", allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, string[] entries, string hint, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, "", text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, GUIStyle style, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, "", text, style, entries, "", allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, "", text, style, entries, hint, allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, string[] entries, string hint, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, GUIStyle style, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, style, entries, "", allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, new GUIContent(label), text, style, entries, hint, allowCustom);
            }

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, string[] entries, string hint, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, GUIStyle style, string[] entries, bool allowCustom = false)
            {
                return AutoCompleteTextField(position, label, text, style, entries, "", allowCustom);
            }

            #endregion Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false)
            {
                //Check for focus, the system is only shown if the text field is focused
                UnityEngine.GUI.SetNextControlName("CheckFocus");
                string value = UnityEditor.EditorGUI.TextField(position, label, text, style);

                //Hack to avoid hint being drawn on the wrong location for attribute drawers
                M_MyStyle.normal.textColor = Color.clear;
                UnityEngine.GUI.enabled = false;
                UnityEditor.EditorGUI.TextField(position, " ", " ", M_MyStyle);
                UnityEngine.GUI.enabled = true;

                //Display the hint
                if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(hint))
                {
                    //Nothing is typed, show a hint to the user
                    UnityEngine.GUI.enabled = false;

                    UnityEditor.EditorGUI.TextField(position, label, " " + hint, UnityEngine.GUI.skin.label);
                    UnityEngine.GUI.enabled = true;
                }

                return _AutoCompleteLogic(position, label, value, entries, allowCustom, true, null);
            }
        }

#endif

        public static class GUI
        {
            public interface IStyle
            {
                GUIStyle Header { get; }
                GUIStyle HeaderText { get; }
                GUIStyle ComponentButton { get; }
                Color BackgroundColor { get; }
                Color SelectionColor { get; }
                Color OddElementColor { get; }
                Color EvenElementColor { get; }
                Texture2D Background { get; }
                Texture2D WhiteTexture { get; }
                GUIStyle SearchBarStyle { get; }
                GUIStyle ClearSearchButtonStyle { get; }
            }

            #region Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, "", text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, "", text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, GUIStyle style, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, "", text, style, entries, "", allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="style">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string text, GUIStyle style, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, "", text, style, entries, hint, allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="inputStyle">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, GUIStyle inputStyle, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, inputStyle, entries, "", allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="inputStyle">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, string label, string text, GUIStyle inputStyle, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, new GUIContent(label), text, inputStyle, entries, hint, allowCustom, windowStyle);
            }

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, "", allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, UnityEngine.GUI.skin.textField, entries, hint, allowCustom, windowStyle);
            }
            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="inputStyle">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, GUIStyle inputStyle, string[] entries, bool allowCustom = false, IStyle windowStyle = null)
            {
                return AutoCompleteTextField(position, label, text, inputStyle, entries, "", allowCustom, windowStyle);
            }

            #endregion Polymorphism

            /// <summary>
            /// Make a TextField that has an <paramref name="AutoCompleteWindow"/> logic for selecting options.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the text field</param>
            /// <param name="label">Optional label to display in front of the text field</param>
            /// <param name="text">The text to edit</param>
            /// <param name="inputStyle">Optional GUIStyle</param>
            /// <param name="entries">Entries to display</param>
            /// <param name="hint">Hint information to show, if any</param>
            /// <param name="allowCustom">Should the system allow custom entries</param>
            /// <param name="windowStyle">Contains the style to use for the window (colors, textures, etc)</param>
            /// <returns>Selected value from autocomplete window</returns>
            public static string AutoCompleteTextField(Rect position, GUIContent label, string text, GUIStyle inputStyle, string[] entries, string hint, bool allowCustom = false, IStyle windowStyle = null)
            {
                //Check for focus, the system is only shown if the text field is focused
                UnityEngine.GUI.SetNextControlName("CheckFocus");
                Rect labelPos = new Rect(position.position, inputStyle.CalcSize(label));
                UnityEngine.GUI.Label(labelPos, label, inputStyle);

                position.x = labelPos.xMax;
                position.width -= labelPos.width;

                string value = UnityEngine.GUI.TextField(position, text, inputStyle);

                M_MyStyle.normal.textColor = Color.clear;
                //Display the hint
                if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(hint))
                {
                    //Nothing is typed, show a hint to the user
                    UnityEngine.GUI.enabled = false;

                    UnityEngine.GUI.Label(position, " " + hint, UnityEngine.GUI.skin.label);
                    UnityEngine.GUI.enabled = true;
                }

                position.y = position.yMax;
                return _AutoCompleteLogic(position, label, value, entries, allowCustom, false, windowStyle);
            }
        }

        //This is a hack to prevent saving the return value to the wrong UI element
        static Vector2 M_returnedScreenPos;
        //Saves the returned value (if any was selected)
        static readonly GUIContent M_ReturnedContent = new GUIContent();
        static readonly GUIStyle M_MyStyle = new GUIStyle(GUIStyle.none);

        static bool M_returnedValue;

        static AddItemWindow M_addItemWindow;

        /// <summary>
        /// Logic for the auto complete draw on text field focus
        /// </summary>
        /// <param name="position">TextField position</param>
        /// <param name="label">Label from the TextField</param>
        /// <param name="text">Value from the TextField</param>
        /// <param name="entries">Entries to display</param>
        /// <param name="allowCustom">Should the system allow custom entries</param>
        /// <param name="fromEditor">Is this being called from editor inspector</param>
        /// <returns>Selected value from autocomplete window</returns>
        static string _AutoCompleteLogic(Rect position, GUIContent label, string text, string[] entries, bool allowCustom, bool fromEditor, GUI.IStyle windowStyle)
        {
            //Used to draw the window
            Rect lastRect = position;
            Vector2 myScreenPos = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));

            //The system returned a value, need to check if this UI element is the one that called
            if (M_returnedValue && M_returnedScreenPos == myScreenPos)
            {
                M_returnedValue = false;
                string val = M_ReturnedContent.text;
                M_ReturnedContent.text = "";
                M_returnedScreenPos = Vector2.zero;
                return val;
            }

            //Only display the system if the text field is focused
            if (UnityEngine.GUI.GetNameOfFocusedControl().Equals("CheckFocus"))
            {
                //New position for this window
                Rect newRect = lastRect;

                //Remove focus
                UnityEngine.GUI.FocusControl(null);

                if (fromEditor)
                {
#if UNITY_EDITOR
                    if (!string.IsNullOrEmpty(label.text))
                    {
                        float offset = UnityEditor.EditorGUIUtility.labelWidth - ((UnityEditor.EditorGUI.indentLevel) * 15);
                        newRect.x += offset;
                        newRect.width -= offset;
                    }

                    newRect.x += UnityEditor.EditorGUI.indentLevel * 15;
                    newRect.width -= UnityEditor.EditorGUI.indentLevel * 15;

                    EditorAddItemWindow.Show(newRect, entries, null, s =>
                    {
                        M_ReturnedContent.text = s;
                        M_returnedValue = true;
                        M_returnedScreenPos = myScreenPos;
                    }, "/", returnFullPath: false, allowCustom: allowCustom);
#endif
                }
                else
                {
                    M_addItemWindow = new AddItemWindow();
                    M_addItemWindow.Show(newRect, entries, null, s =>
                    {
                        M_ReturnedContent.text = s;
                        M_returnedValue = true;
                        M_returnedScreenPos = myScreenPos;
                    }, "/", returnFullPath: false, allowCustom: allowCustom, style: windowStyle);
                }
            }

            if (M_addItemWindow != null && new Rect(lastRect.position, new Vector2(lastRect.width, 320)) == M_addItemWindow.Position)
            {
                if (M_addItemWindow.Closed)
                {
                    M_addItemWindow = null;
                }
                else
                {
                    M_addItemWindow.OnGUI();
                }
            }

            return text;
        }
    }
}
