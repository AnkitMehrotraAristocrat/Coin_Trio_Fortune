using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using UnityEngine.Scripting;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditorInternal;
using Milan.FrontEnd.Core.v5_1_1.Editor;
using AutoCompleteEditor = PixelUnited.NMG.Slots.Milan.GAMEID.RotaryHeart.Lib.AutoComplete.AutoCompleteTextField.EditorGUI;
#endif

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    #region Editor Support
#if UNITY_EDITOR
    [CustomEditor(typeof(AnimationEventForwarder))]
    public class AnimationEventForwarderEditor : BaseEditor
    {
        private bool showAnimationEventList = true;
        private bool showSequencesList = true;

        SerializedProperty animationEvents;
        ReorderableList list;

        private void OnEnable()
        {
            animationEvents = serializedObject.FindProperty("_animationEvents");
            list = new ReorderableList(serializedObject, animationEvents, true, true, true, true);

            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
            list.elementHeightCallback = SetElementHeight;
            list.onAddCallback = AddDefaultItem;
        }

        public override void OnInspectorGUI()
        {
            DrawRevEngEditorUI();

            serializedObject.Update();

            showAnimationEventList = EditorGUILayout.Foldout(showAnimationEventList, "Animation Events");
            if (showAnimationEventList)
            {
                list.DoLayoutList();
            }

            showSequencesList = EditorGUILayout.Foldout(showSequencesList, "Animation Event Sequences");
            if (showSequencesList)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_eventSequences"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            GameObject gameObject = element.FindPropertyRelative("GameObject").objectReferenceValue as GameObject;

            string componentName = null;
            Component component = null;

            string methodName = null;

            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.standardVerticalSpacing, 20, EditorGUIUtility.singleLineHeight), "Id");
            EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + EditorGUIUtility.standardVerticalSpacing, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Id"), GUIContent.none);

            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2, 100, EditorGUIUtility.singleLineHeight), "GameObject");
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("GameObject"), GUIContent.none);

            if (EditorGUI.EndChangeCheck() || gameObject == null)
            {
                element.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue = null;
                element.FindPropertyRelative("Method").stringValue = null;
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3, 100, EditorGUIUtility.singleLineHeight), "Component");
                DrawComponentList(element, new Rect(rect.x + 100, rect.y + EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3, rect.width - 100, EditorGUIUtility.singleLineHeight));
                componentName = element.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue;
            }

            if (!string.IsNullOrEmpty(componentName))
            {
                string componentTag = element.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue;
                component = gameObject.GetComponents(typeof(Component)).FirstOrDefault(entry => entry.GetType().Name.Equals(componentName) && entry.GetTag().Equals(componentTag));

                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4, 100, EditorGUIUtility.singleLineHeight), "Method");
                DrawMethodList(element, new Rect(rect.x + 100, rect.y + EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 4, rect.width - 100, EditorGUIUtility.singleLineHeight));

                methodName = element.FindPropertyRelative("Method").stringValue;
            }
            else
            {
                element.FindPropertyRelative("Method").stringValue = null;
            }

            if (!string.IsNullOrEmpty(methodName))
            {
                Type componentType = component.GetType();
                MethodInfo method = componentType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

                if (method.ReturnType == typeof(IEnumerator<Yield>))
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 5, 100, EditorGUIUtility.singleLineHeight), "ShouldYield");
                    EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 5, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("ShouldYield"), GUIContent.none);
                }
                else
                {
                    element.FindPropertyRelative("ShouldYield").boolValue = false;
                }
            }

            element.serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Events List");
        }

        private float SetElementHeight(int index)
        {
            int rowCount = 2;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            GameObject gameObject = null;
            Component component = null;

            if (element.FindPropertyRelative("GameObject").objectReferenceValue != null)
            {
                ++rowCount;
                gameObject = element.FindPropertyRelative("GameObject").objectReferenceValue as GameObject;
            }

            if (!string.IsNullOrEmpty(element.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue))
            {
                component = gameObject.GetComponents(typeof(Component)).FirstOrDefault(entry => entry.GetType().Name.Equals(element.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue) && entry.GetTag().Equals(element.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue));
                ++rowCount;
            }

            if (!string.IsNullOrEmpty(element.FindPropertyRelative("Method").stringValue))
            {
                MethodInfo method = component.GetType().GetMethod(element.FindPropertyRelative("Method").stringValue, BindingFlags.Public | BindingFlags.Instance);
                if (method.ReturnType == typeof(IEnumerator<Yield>))
                {
                    ++rowCount;
                }
            }

            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * rowCount + ReorderableList.Defaults.padding;
        }

        private void AddDefaultItem(ReorderableList list)
        {
            ++list.serializedProperty.arraySize;
            int lastIndex = list.serializedProperty.arraySize - 1;
            SerializedProperty newEntry = list.serializedProperty.GetArrayElementAtIndex(lastIndex);

            newEntry.FindPropertyRelative("Id").stringValue = "";
            newEntry.FindPropertyRelative("GameObject").objectReferenceValue = null;
            newEntry.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue = "";
            newEntry.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue = "";
            newEntry.FindPropertyRelative("Method").stringValue = "";
            newEntry.FindPropertyRelative("ShouldYield").boolValue = false;
        }

        private void DrawComponentList(SerializedProperty property, Rect rect)
        {
            string priorValue = property.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue;
            string tag = property.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue;
            priorValue += string.IsNullOrEmpty(tag) ? "" : "." + tag;

            string setValue = AutoCompleteEditor.NMGAutoCompleteTextField(rect, "", priorValue, GUI.skin.textField, property, _ => GetComponentList(property), "Select component..");
            if (setValue != priorValue)
            {
                property.FindPropertyRelative("Method").stringValue = null;
            }

            if (setValue.Contains("."))
            {
                string[] identifiers = setValue.Split('.');
                property.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue = identifiers[0];
                property.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue = identifiers[1];
            }
            else
            {
                property.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue = setValue;
                property.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue = "";
            }
        }

        private string[] GetComponentList(SerializedProperty property)
        {
            //StaticLogForwarder.Logger.Log("hit GetComponentList func");

            GameObject gameObject = property.FindPropertyRelative("GameObject").objectReferenceValue as GameObject;
            Component[] components = gameObject.GetComponents<Component>();
            List<string> componentList = new List<string>() { };
            foreach (Component comp in components)
            {
                string name = comp.GetType().Name;
                string tag = comp.GetTag();
                name += string.IsNullOrEmpty(tag) ? "" : "." + comp.GetTag();
                componentList.Add(name);
            }

            return componentList.ToArray();
        }

        private void DrawMethodList(SerializedProperty property, Rect rect)
        {
            property.FindPropertyRelative("Method").stringValue = AutoCompleteEditor.NMGAutoCompleteTextField(rect, "", property.FindPropertyRelative("Method").stringValue, GUI.skin.textField, property, _ => GetMethodList(property), "Select method..");
        }

        private string[] GetMethodList(SerializedProperty property)
        {
            //StaticLogForwarder.Logger.Log("hit get methods list func");

            string targetMethod = property.FindPropertyRelative("Method").stringValue;
            string targetComponentName = property.FindPropertyRelative("Component").FindPropertyRelative("Name").stringValue;
            string targetComponentTag = property.FindPropertyRelative("Component").FindPropertyRelative("Tag").stringValue;

            GameObject gameObject = property.FindPropertyRelative("GameObject").objectReferenceValue as GameObject;
            Component component = gameObject.GetComponents(typeof(Component)).FirstOrDefault(entry => entry.GetType().Name.Equals(targetComponentName) && entry.GetTag().Equals(targetComponentTag));

            Type componentType = component.GetType();
            MethodInfo[] methods = componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            List<string> methodList = new List<string>() { };
            foreach (MethodInfo method in methods)
            {
                methodList.Add(method.Name);
            }

            return methodList.ToArray();
        }
    }

    [CustomPropertyDrawer(typeof(AnimationEventForwarder.SequenceEvent))]
    public class SequenceEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string stringValue = property.FindPropertyRelative("Value").stringValue;
            property.FindPropertyRelative("Value").stringValue = AutoCompleteEditor.NMGAutoCompleteTextField(position, "", stringValue, GUI.skin.textField, property, _ => GetEntries(property), "Select event..");
        }

        private string[] GetEntries(SerializedProperty property)
        {
            //StaticLogForwarder.Logger.Log("hit fetch animation event ids func effecient");
            var animationEvents = property.serializedObject.FindProperty("_animationEvents").GetValue<AnimationEventForwarder.AnimationEvent[]>();
            return animationEvents.Select(entry => entry.Id).ToArray();
        }
    }


    //[CustomEditor(typeof(AnimationEventForwarder))]
    //public class AnimationEventForwarderEditor : BaseEditor
    //{
    //private ReorderableArray<AnimationEventForwarder.AnimationEventSequence> _sequences;
    //private ReorderableArray<AnimationEventForwarder.AnimationEvent> _animEvents;

    //public override void OnInspectorGUI()
    //{
    //	base.OnInspectorGUI();

    //if (GUILayout.Button("Sync sequences to animation events list"))
    //{
    //	UpdateAnimationEventsUsingSequences();
    //}
    //}

    //private void UpdateAnimationEventsUsingSequences()
    //{
    //	Undo.RecordObject(serializedObject.targetObject, "AnimationEventForwarder Sequence Sync");

    //	_sequences = serializedObject.FindProperty("_eventSequences").GetValue<ReorderableArray<AnimationEventForwarder.AnimationEventSequence>>();
    //	_animEvents = serializedObject.FindProperty("_animationEvents").GetValue<ReorderableArray<AnimationEventForwarder.AnimationEvent>>();

    //	for (int sequenceIndex = 0; sequenceIndex < _sequences.Length; ++sequenceIndex)
    //	{
    //		for (int eventIndex = 0; eventIndex < _sequences[sequenceIndex].AnimationEvents.Length; ++eventIndex)
    //		{
    //			var entry = _sequences[sequenceIndex].AnimationEvents[eventIndex];
    //			if (!IsAnimEventSerialized(entry))
    //			{
    //				_animEvents.Add(entry);
    //			}
    //		}
    //	}

    //	serializedObject.FindProperty("_animationEvents").SetValue(_animEvents);
    //}

    //private bool IsAnimEventSerialized(AnimationEventForwarder.AnimationEvent animEvent)
    //{
    //	bool eventExists = false;
    //	foreach (AnimationEventForwarder.AnimationEvent serializedEvent in _animEvents)
    //	{
    //		bool idMatch = animEvent.Id.Equals(serializedEvent.Id);
    //		bool gameObjectMatch = animEvent.GameObject.Equals(serializedEvent.GameObject);
    //		bool componentMatch = animEvent.Component.Equals(serializedEvent.Component);
    //		bool componentTagMatch = animEvent.ComponentTag.Equals(serializedEvent.ComponentTag);
    //		bool methodMatch = animEvent.Method.Equals(serializedEvent.Method);
    //		bool shouldYieldMatch = animEvent.ShouldYield.Equals(serializedEvent.ShouldYield);

    //		eventExists = idMatch && gameObjectMatch && componentMatch && componentTagMatch && methodMatch && shouldYieldMatch;
    //		if (eventExists)
    //		{
    //			break;
    //		}
    //	}
    //	return eventExists;
    //}
    //}
#endif
    #endregion

    /// <summary>
    /// A component that allows an engineer to define animation events by name that invoke
    /// a public method on any game object / component (tagged or not). The animation event
    /// is then added to an animation via the animation event at a specific keyframe.
    /// </summary>
    public class AnimationEventForwarder : MonoBehaviour, ServiceLocator.IHandler
    {
        #region AnimationEvent Helper Classes
        [Preserve]
        [Serializable]
        public class ReflectedComponent
        {
            public string Name;
            public string Tag;
        }

        [Preserve]
        [Serializable]
        public class AnimationEvent
        {
            public string Id;
            public GameObject GameObject;
            public ReflectedComponent Component;
            public string Method;
            public bool ShouldYield;
        }

        [Preserve]
        [Serializable]
        public class SequenceEvent
        {
            public string Value;
        }

        [Preserve]
        [Serializable]
        public class SequenceEvents : ReorderableArray<SequenceEvent> { }

        [Preserve]
        [Serializable]
        public class AnimationEventSequence
        {
            public string Id;
            [Reorderable] public SequenceEvents AnimationEvents;
        }

        [Preserve]
        [Serializable]
        public class AnimationEventSequences : ReorderableArray<AnimationEventSequence> { }

        [Preserve]
        [Serializable]
        public class AnimationEventPair
        {
            public string Id;
            public Component Component;
            public MethodInfo Method;
            public Action Action;
            public Func<IEnumerator<Yield>> Coroutine;
            public bool IsYieldable;
            public bool ShouldYield;
        }
        #endregion

        [SerializeField] private AnimationEvent[] _animationEvents;
        [Reorderable][SerializeField] private AnimationEventSequences _eventSequences;

        private Dictionary<string, AnimationEventPair> _animationEventPairs = new Dictionary<string, AnimationEventPair>();

        public void OnServicesLoaded()
        {
            if (ValidateUniqueIds())
            {
                InitializeAnimationEventPairs();
            }
        }

        public void ExecuteSingleAnimationEvent(string eventId)
        {
            _animationEventPairs.TryGetValue(eventId, out AnimationEventPair animEventPair);
            if (animEventPair == null)
            {
                GameIdLogger.Logger.Error(GetType() + " :: Could not find an animation event with an ID of: " + eventId, this);
            }

            GameIdLogger.Logger.Debug("Executed animation event: " + eventId + " on frame " + Time.frameCount, this);
            if (!animEventPair.IsYieldable)
            {
                animEventPair.Action();
            }
            else
            {
                Coroutine.Start(animEventPair.Coroutine());
            }
        }

        public void ExecuteSequence(string sequenceId)
        {
            AnimationEventSequence sequence = _eventSequences.FirstOrDefault(entry => entry.Id.Equals(sequenceId));
            if (sequence == null)
            {
                GameIdLogger.Logger.Error(GetType() + " :: Could not find a sequence with an ID of: " + sequenceId, this);
                return;
            }
            GameIdLogger.Logger.Debug("Executing sequence: " + sequenceId + " at frame " + Time.frameCount);
            Coroutine.Start(ExecuteSequenceEvents(sequence));
        }

        private IEnumerator<Yield> ExecuteSequenceEvents(AnimationEventSequence sequence)
        {
            foreach (SequenceEvent eventId in sequence.AnimationEvents)
            {
                GameIdLogger.Logger.Debug("Executed animation event: " + eventId.Value + " on frame " + Time.frameCount, this);
                if (!_animationEventPairs[eventId.Value].IsYieldable)
                {
                    _animationEventPairs[eventId.Value].Action();
                }
                else if (_animationEventPairs[eventId.Value].ShouldYield)
                {
                    yield return Coroutine.Start(_animationEventPairs[eventId.Value].Coroutine());
                }
                else
                {
                    Coroutine.Start(_animationEventPairs[eventId.Value].Coroutine());
                }
            }
            yield break;
        }

        #region Initialization Support
        private void InitializeAnimationEventPairs()
        {
            foreach (AnimationEvent animationEvent in _animationEvents)
            {
                InitializeAnimationEventPair(animationEvent);
            }
        }

        private void InitializeAnimationEventPair(AnimationEvent animationEvent)
        {
            if (_animationEventPairs.ContainsKey(animationEvent.Id))
            {
                return;
            }

            GameObject targetGameObject = animationEvent.GameObject;

            if (targetGameObject == null)
            {
                GameIdLogger.Logger.Error("AnimationEventForwarder :: Missing gameObject " + gameObject, this);
                return;
            }

            Component targetComponent = targetGameObject.GetComponents(typeof(Component)).FirstOrDefault(entry => entry.GetType().Name.Equals(animationEvent.Component.Name) && entry.GetTag().Equals(animationEvent.Component.Tag));

            if (targetComponent == null)
            {
                GameIdLogger.Logger.Error("AnimationEventForwarder :: " + gameObject + " does not contain a " + animationEvent.Component.Name + " component with a " + "tag of '" + animationEvent.Component.Tag + "'.", this);
                return;
            }

            Type targetComponentType = targetComponent.GetType();
            MethodInfo targetMethod = targetComponentType.GetMethod(animationEvent.Method, BindingFlags.Public | BindingFlags.Instance);

            if (targetMethod == null)
            {
                GameIdLogger.Logger.Error("AnimationEventForwarder :: " + animationEvent.Component.Name + " does not contain a method named " + animationEvent.Method, this);
                return;
            }

            Action action = null;
            Func<IEnumerator<Yield>> coroutine = null;
            bool isCoroutine = targetMethod.ReturnType == typeof(IEnumerator<Yield>);

            if (isCoroutine)
            {
                coroutine = (Func<IEnumerator<Yield>>)targetMethod.CreateDelegate(typeof(Func<IEnumerator<Yield>>), targetComponent);
            }
            else
            {
                action = (Action)targetMethod.CreateDelegate(typeof(Action), targetComponent);
            }

            _animationEventPairs.Add(
                animationEvent.Id,
                new AnimationEventPair()
                {
                    Id = animationEvent.Id,
                    Component = targetComponent,
                    Method = targetMethod,
                    Action = action,
                    Coroutine = coroutine,
                    IsYieldable = isCoroutine,
                    ShouldYield = animationEvent.ShouldYield
                }
            );
        }
        #endregion

        #region Validation Support
        private bool ValidateUniqueIds()
        {
            if (_eventSequences.Length == 0 && _animationEvents.Length == 0)
            {
                return true;
            }

            bool areAllSequenceIdsUnique = _eventSequences.All(entryA => _eventSequences.Count(entryB => entryA.Id == entryB.Id) == 1);
            if (!areAllSequenceIdsUnique)
            {
                GameIdLogger.Logger.Error(GetType() + " :: Multiple sequence entries exist with the same ID on " + name + ". These must be unique.", this);
                return false;
            }

            bool areAllEventIdsUnique = _animationEvents.All(entryA => _animationEvents.Count(entryB => entryA.Id == entryB.Id) == 1);
            if (!areAllEventIdsUnique)
            {
                GameIdLogger.Logger.Error(GetType() + " :: Multiple event entries exist with the same ID on " + name + ". These must be unique.", this);
                return false;
            }

            return true;
        }
        #endregion
    }
}
