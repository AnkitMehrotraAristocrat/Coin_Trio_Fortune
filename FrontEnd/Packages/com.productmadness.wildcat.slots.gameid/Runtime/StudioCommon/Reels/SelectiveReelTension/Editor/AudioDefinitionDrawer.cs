#if UNITY_EDITOR
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Sag;
using Sag.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[CustomPropertyDrawer(typeof(AudioDefinition))]
	public class AudioDefinitionDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var eventNameProp = property.FindPropertyRelative("AudioEventName");
			var optionList = Object.FindObjectOfType<AudioEventBindings>().EventBindingData.audioEvents.Select(audioEvent => audioEvent.eventName).ToList();
			optionList.Insert(0, "None");
			var options = optionList.ToArray();
			var index = options.IndexOf(item => item == eventNameProp.stringValue);
			if (index == -1)
				index = 0;

			index = EditorGUI.Popup(position.Single(), "Audio Event Name", index, options);
			eventNameProp.stringValue = options[index];
		}
	}
}
#endif
