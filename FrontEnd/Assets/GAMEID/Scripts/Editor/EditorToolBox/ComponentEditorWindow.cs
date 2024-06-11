using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ComponentEditorWindow : EditorWindow
{
	private static Editor _editor;
	private static Component _component;
	private static int _index = 0;
	private Vector2 _scrollPosition;

	public void SetEditor(Editor editor, Component component, int componentIndex)
	{
		_editor = editor;
		_component = component;
		_index = componentIndex;
	}

	private void OnGUI()
	{
		if (_editor != null)
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			_editor.OnInspectorGUI();
			EditorGUILayout.Space();
			EditorGUILayout.SelectableLabel(EditorToolBox.GetGameObjectPath(_component.gameObject) + "\\" + _component.GetType().Name);
			EditorGUILayout.LabelField("Current Component Index: " + _index);
			EditorGUILayout.EndScrollView();
		}
		else
		{
			_index = 0;
		}
	}
}
