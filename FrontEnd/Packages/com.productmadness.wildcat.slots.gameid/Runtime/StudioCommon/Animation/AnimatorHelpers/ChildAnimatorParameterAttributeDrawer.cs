using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using RotaryHeart.Lib.AutoComplete;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ChildAnimatorParameterAttribute))]
    public class ChildAnimatorParameterAttributeDrawer : PropertyDrawer
    {
        private bool _isSet = true;
        private string[] _entries;
        private string _oldValue = "";
        private Color _prevColor;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _prevColor = GUI.color;

            if (string.IsNullOrEmpty(property.stringValue))
            {
                GUI.color = Color.yellow;
            }
            else
            {
                GUI.color = Color.green;
            }

            if (_isSet && GUI.Button(position, label + ": " + property.stringValue))
            {
                _isSet = false;
                _oldValue = property.stringValue;
                property.stringValue = "";
                GetEntries(property);
            }

            if (!_isSet)
            {
                property.stringValue = AutoCompleteTextField.EditorGUI.AutoCompleteTextField(position, label, property.stringValue, GUI.skin.textField, _entries, "Old value: " + _oldValue);
            }

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                _isSet = true;
            }

            GUI.color = _prevColor;
        }

        private void GetEntries(SerializedProperty property)
        {
            ChildAnimatorParameterAttribute attribute = System.Attribute.GetCustomAttribute(fieldInfo, typeof(ChildAnimatorParameterAttribute)) as ChildAnimatorParameterAttribute;
            AnimatorControllerParameterType parameterType = attribute.ParameterType;

            List<string> entries = new List<string>();
            Component component = property.serializedObject.targetObject as Component;
            Animator[] animators = component.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animators)
            {
                UpdateEntries(animator, parameterType, ref entries);
            }
            _entries = entries.ToArray();
        }

        private void UpdateEntries(Animator animator, AnimatorControllerParameterType parameterType, ref List<string> entries)
        {
            bool wasEnabled = animator.enabled;

            animator.enabled = false;
            animator.enabled = true;

            if (animator.parameterCount > 0)
            {
                entries.AddRange(animator.parameters
                    .Where(parameter => parameter.type.Equals(parameterType))
                    .Select(trigger => trigger.name));
            }

            animator.enabled = wasEnabled;
        }
    }
#endif
}