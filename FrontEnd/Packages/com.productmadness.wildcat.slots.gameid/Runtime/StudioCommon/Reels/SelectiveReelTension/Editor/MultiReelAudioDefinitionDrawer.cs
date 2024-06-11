#if UNITY_EDITOR
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Sag;
using Sag.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[CustomPropertyDrawer(typeof(MultiReelAudioDefinition))]
	public class MultiReelAudioDefinitionDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 3;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var playEventNameProp = property.FindPropertyRelative("PlayAudioEventName");
			var stopEventNameProp = property.FindPropertyRelative("QuickStopAudioEventName");
			var optionList = Object.FindObjectOfType<AudioEventBindings>().EventBindingData.audioEvents.Select(audioEvent => audioEvent.eventName).ToList();
			optionList.Insert(0, "None");
			var options = optionList.ToArray();

			var playIndex = options.IndexOf(item => item == playEventNameProp.stringValue);
			if (playIndex == -1)
			{
				playIndex = 0;
			}

			var stopIndex = options.IndexOf(item => item == stopEventNameProp.stringValue);
			if (stopIndex == -1)
			{
				stopIndex = 0;
			}

			EditorGUI.PropertyField(position.Single(), property.FindPropertyRelative("PendingReelCount"));
			playIndex = EditorGUI.Popup(position.Single(), "Play Audio Event Name", playIndex, options);
			playEventNameProp.stringValue = options[playIndex];
			stopIndex = EditorGUI.Popup(position.Single(), "Quick Stop Audio Event Name", stopIndex, options);
			stopEventNameProp.stringValue = options[stopIndex];
		}
	}
}
#endif
