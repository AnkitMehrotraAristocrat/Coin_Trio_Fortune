#region Using

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using FoundReferenceSet = System.Collections.Generic.HashSet<(string msg, UnityEngine.Object obj)>;
using System.Text.RegularExpressions;
using NMG.EditorToolBoxUtil;
using AttributeType = NMG.EditorToolBoxUtil.AttributeList.AttributeType;
using Milan.FrontEnd.Core.v5_1_1;
using StateMachineModel = Milan.FrontEnd.Core.v5_1_1.StateMachineModel;

#endregion

/// <summary>
/// Editor toolbox window.
/// </summary>
public class EditorToolBox : EditorWindow
{
	#region Fields

	static int _gameObjectCount = 0;
	static int _componentCount = 0;
	static int _problemCount = 0;
	static int _foundObjectCount = 0;
	static int _foundComponentCount = 0;
	static readonly FoundReferenceSet _foundReferences = new FoundReferenceSet();
	private static string textToFind = "";
	private static string guidToFind = "";
	private static bool ignoreCase = false;
	private static bool showStringHistogram = false;
	private static readonly Regex stateTypeNameReg = new Regex("^(\\w*\\.)+(\\w*),\\s*Assembly_");
	private static readonly Regex presenterNameReg = new Regex("[a-zA-Z_][a-zA-Z0-9_]*");
	private static readonly Regex guidReg = new Regex("^[a-fA-F0-9]{32}");

	private static string componentToFind = "";
	private static string lastSearchedComponent = "";
	private static Editor searchedEditor = null;
	private static GameObject editorGameObject = null;
	private static Component[] editorComponents = new Component[0];
	private static Component[] searchedEditorComponents = new Component[0];

	private static ComponentEditorWindow _componentEditorWindow = null;
	private static int currentEditorComponentIndex = -1;
	private static bool repaintComponentEditor = false;

	private static bool logComponentsRecursive = false;
	private static bool sortLoggedComponents = false;
	private static bool createFolders = false;

	private static string outputFolder = "c:\\Dell\\";

    private static Vector2 scrollbarPosition = new Vector2();

    private bool _gameIsRunning;

	#endregion

    #region GUI Methods

	/// <summary>
	/// Menu item to display the window.
	/// </summary>
	[MenuItem("Tools/EditorToolBox")]
	public static void ShowWindow()
	{
		var window = EditorWindow.GetWindow<EditorToolBox>();
		window.Show();
	}

	/// <summary>
	/// Main OnGUI loop.
	/// </summary>
	[MenuItem("Examples/GUILayout TextField")]
	public void OnGUI()
	{
		TextLog.AttachToUnity();
		GUIStyle style = new GUIStyle();
		style.wordWrap = true;
		style.richText = true;

        scrollbarPosition= GUILayout.BeginScrollView(scrollbarPosition, style);

		// Displays time scale controller, lets user control how fast the game plays.
		TimeScaleControllerGUI(style);

		// Allows user to find the exact component they're looking for on the selected game object.
		ComponentEditorGUI(style);

		// Allows the user to search for text related to the selected game object and its children.
		TextSearchGUI(style);

		// Logs components listed to a file.
        ComponentLoggingGUI(style);

        // Find null tags on an object.
		EditorGUILayout.LabelField("<color=white><b>Recursively search for null from the selected object.</b></color>", style);
		if(GUILayout.Button("Find Null TAGS"))
		{
			FindNullTags();
		}

		// Finds missing scripts on an object and its children.
		EditorGUILayout.LabelField("<color=white><b>Recursively search for missing scripts from the selected object.</b></color>", style);
		if(GUILayout.Button("Find Missing Scripts"))
		{
			FindMissingScripts();
		}

        // Asset Path ID GUI.
		AssetPathGUI(style);

        GUILayout.EndScrollView();

		//----------------------
		// Log File options GUI 
		//----------------------
		// 		EditorGUILayout.LabelField("\r\n<color=white><b>Log to file options</b></color>", style);
		// 		TextLog.breakOnError = System.Diagnostics.Debugger.IsAttached && TextLog.breakOnError;
		// 		TextLog.logInfo = EditorGUILayout.Toggle("\tInfo logs", TextLog.logInfo);
		// 		TextLog.logWarning = EditorGUILayout.Toggle("\tWarnings", TextLog.logWarning);
		// 		TextLog.logAssert = EditorGUILayout.Toggle("\tAsserts", TextLog.logAssert);
		// 		TextLog.logError = EditorGUILayout.Toggle("\tErrors", TextLog.logError);
		// 		TextLog.logException = EditorGUILayout.Toggle("\tExceptions", TextLog.logException);

		//------------------
		// Break on LOG GUI 
		//------------------
		// 		EditorGUILayout.LabelField("\r\n<color=white> <b>Break on Log options</b></color>", style);
		// 		TextLog.logTextToMatch = EditorGUILayout.TextField("Log Text Filter", TextLog.logTextToMatch);
		// 		EditorGUI.BeginDisabledGroup(!System.Diagnostics.Debugger.IsAttached);
		// 		{
		// 			TextLog.breakOnAssert = EditorGUILayout.Toggle("\tBreak on Assert", TextLog.breakOnAssert);
		// 			TextLog.breakOnError = EditorGUILayout.Toggle("\tBreak on Error", TextLog.breakOnError);
		// 			TextLog.breakOnException = EditorGUILayout.Toggle("\tBreak on Exception", TextLog.breakOnException);
		// 			TextLog.breakOnWarning = EditorGUILayout.Toggle("\tBreak on Warning", TextLog.breakOnWarning);
		// 			TextLog.breakOnLog = EditorGUILayout.Toggle("\tBreak on Info", TextLog.breakOnLog);
		// 		}
		// 		EditorGUI.EndDisabledGroup();
	}

	#endregion

	#region GUI Element Methods

	/// <summary>
	/// Renders the time scale GUI.
	/// Pressing one of the buttons will change the game's current timescale.
	/// </summary>
	private void TimeScaleControllerGUI(GUIStyle style)
	{
		EditorGUILayout.LabelField("\r\n<color=white><b>Time Scale Controller</b></color>", style);

		if (InspectorHelper.Button("300%") && Application.isPlaying)
		{
			Time.timeScale = 3.0f;
		}

		if (InspectorHelper.Button("100%") && Application.isPlaying)
		{
			Time.timeScale = 1.0f;
		}

		if (InspectorHelper.Button("50%") && Application.isPlaying)
		{
			Time.timeScale = 0.5f;
		}

		if (InspectorHelper.Button("10%") && Application.isPlaying)
		{
			Time.timeScale = 0.1f;
		}
	}

	/// <summary>
	/// Searches for components on a game object and displays in a separate window.
	/// </summary>
	/// <param name="style"></param>
	private void ComponentEditorGUI(GUIStyle style)
	{
		EditorGUILayout.LabelField("\r\n<color=white><b>Component Editor</b></color>", style);

		componentToFind = EditorGUILayout.TextField("Component", componentToFind);
		EditorGUI.BeginDisabledGroup(componentToFind == "");
		{
			if (Selection.activeGameObject != null)
			{
				if (Selection.activeGameObject != editorGameObject)
				{
					editorGameObject = Selection.activeGameObject;
					editorComponents = editorGameObject.transform.GetComponents<Component>();
					lastSearchedComponent = "";
					searchedEditorComponents = new Component[0];
					currentEditorComponentIndex = -1;
					searchedEditor = null;
					repaintComponentEditor = true;
				}

				if (lastSearchedComponent != componentToFind)
				{
					lastSearchedComponent = componentToFind;
					searchedEditorComponents = editorComponents.Where(entry => entry.GetType().Name.Equals(lastSearchedComponent)).ToArray();
					currentEditorComponentIndex = searchedEditorComponents.Length > 0 ? 0 : -1;
					searchedEditor = null;
					repaintComponentEditor = true;
				}

				if (currentEditorComponentIndex >= 0)
				{
					if (repaintComponentEditor)
					{
						searchedEditor = Editor.CreateEditor(searchedEditorComponents[currentEditorComponentIndex]);
						_componentEditorWindow = EditorWindow.GetWindow<ComponentEditorWindow>("ComponentEditor", false);
						_componentEditorWindow.SetEditor(searchedEditor, searchedEditorComponents[currentEditorComponentIndex], currentEditorComponentIndex);
						_componentEditorWindow.Repaint();
					}
				}
				else
				{
					searchedEditor = null;
					if (_componentEditorWindow != null)
					{
						_componentEditorWindow.SetEditor(searchedEditor, null, 0);
						_componentEditorWindow.Repaint();
						_componentEditorWindow = null;
					}
				}
			}
		}
		if (searchedEditorComponents.Length > 1)
		{
			if (GUILayout.Button("Previous"))
			{
				currentEditorComponentIndex = Math.Max(0, currentEditorComponentIndex - 1);
				repaintComponentEditor = true;
			}
			if (GUILayout.Button("Next"))
			{
				currentEditorComponentIndex = Math.Min(searchedEditorComponents.Length - 1, currentEditorComponentIndex + 1);
				repaintComponentEditor = true;
			}
		}

		EditorGUI.EndDisabledGroup();
	}

	/// <summary>
	/// Given the string the user enters in Find Text, will find all string references on the selected game object and its children.
	/// </summary>
	/// <param name="style"></param>
	private void TextSearchGUI(GUIStyle style)
	{
		EditorGUILayout.LabelField("\r\n<color=white><b>Text Search options</b></color>", style);
		textToFind = EditorGUILayout.TextField("\tFind Text", textToFind);
		ignoreCase = EditorGUILayout.Toggle("\tIgnore Case", ignoreCase);
		//		showStringHistogram = EditorGUILayout.Toggle("\tText Histogram", showStringHistogram);
		EditorGUI.BeginDisabledGroup(textToFind == "");
		{
			if (GUILayout.Button("Find String References"))
			{
				FindText(textToFind);
			}
		}
		EditorGUI.EndDisabledGroup();
	}

	/// <summary>
	/// Lets you log the GUI.
	/// </summary>
	/// <param name="style"></param>
    private void ComponentLoggingGUI(GUIStyle style)
    {
        bool outputValid = outputFolder != "" && Directory.Exists(outputFolder);
        EditorGUILayout.LabelField("\r\n<color=white><b>List components on selected object to a text file.</b></color>", style);


        EditorGUI.BeginDisabledGroup(!outputValid);
        {
            if (GUILayout.Button("Log Components"))
            {
                LogComponents();
            }
        }
        EditorGUI.EndDisabledGroup();
        sortLoggedComponents = EditorGUILayout.Toggle("Sort Components", sortLoggedComponents);
        logComponentsRecursive = EditorGUILayout.Toggle("Recursive", logComponentsRecursive);
        createFolders = EditorGUILayout.Toggle("Create Hierarchy Folders", createFolders);
        if (GUILayout.Button("Select Component Log Folder"))
        {
            SelectOutputFolder();
        }
        EditorGUILayout.LabelField("<color=grey>Component Log Folder:</color> <color=white><b>" + outputFolder + "</b></color>", style);

		// State logging GUI.
        EditorGUILayout.LabelField("\r\n<color=white><b>List info on selected States to a text file.</b></color>", style);
        bool statesSelected = true && Selection.objects.Length > 0;
        foreach (var sel in Selection.objects)
        {
            if (sel == null)
            {
                continue;
            }
            if (sel.GetType() != typeof(PresentationNode))
            {
                statesSelected = false;
            }
        }

        EditorGUI.BeginDisabledGroup(!outputValid || !statesSelected);
        {
            if (GUILayout.Button("Log States"))
            {
                LogStates();
            }
        }
        EditorGUI.EndDisabledGroup();
	}

    private void AssetPathGUI(GUIStyle style)
    {
        EditorGUILayout.LabelField("\r\n<color=white><b>Asset path from GUID</b></color>", style);
        guidToFind = EditorGUILayout.TextField("GUID", guidToFind);
        EditorGUI.BeginDisabledGroup(guidToFind == "");
        {
            if (GUILayout.Button("Log asset path"))
            {
                LogAssetPath(guidToFind);
            }
        }
        EditorGUI.EndDisabledGroup();
	}

    #endregion

	#region Helper Methods

	/// <summary>
	/// Resets the counters.
	/// </summary>
	private void ResetCounters()
    {
        _gameObjectCount = 0;
        _componentCount = 0;
        _problemCount = 0;
        _foundObjectCount = 0;
        _foundComponentCount = 0;
        _foundReferences.Clear();
    }

	/// <summary>
	/// Component Logging GUI.
	/// </summary>
	private void SelectOutputFolder()
	{
		outputFolder = EditorUtility.OpenFolderPanel("Select Target Folder", outputFolder, "");
	}

	private void WriteTextFile(string filename, List<string> textlist)
	{
		StreamWriter _writer = File.CreateText(filename);
		foreach(var text in textlist)
		{
			_writer.WriteLine(text);
		}
		_writer.Flush();
		_writer.Close();
		_writer.Dispose();
	}

	public static string GetGameObjectPath(GameObject g, string seperator = "\\", bool includeName = true)
	{
		string s = includeName ? g.name : "";
		Transform t = g.transform;
		while(t.parent != null)
		{
			s = t.parent.name + seperator + s;
			t = t.parent;
		}
		return s;
	}

	private void LogSingleComponent(GameObject root)
	{

		string scripts  = "Scripts for " + GetGameObjectPath(root) + ":\r\n";
		Component[] components = root.GetComponents<Component>();
		List<string> compNames = new List<string>();
		foreach(var component in components)
		{
			var compType = component.GetType();
			string tag = TaggerExtensions.GetTag(component);
			string compText = compType.Name;
			if(tag.Length > 0)
			{
				compText += "->" + tag;
			}
			compNames.Add(compText);
			scripts += compText + "\r\n";
		}
		if(sortLoggedComponents)
		{
			compNames.Sort();
		}
		Debug.Log(scripts, root);
		string name;
		string path = string.Copy(outputFolder);
		path.Replace("/", "\\");
		if(!path.EndsWith("\\"))
		{
			path += "\\";
		}
		string fname;
		if(!createFolders)
		{
			name = GetGameObjectPath(root, "_");
			fname = path + name + ".txt";
		}
		else
		{
			path += GetGameObjectPath(root, "\\", false);
			if(!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			fname = path + root.name + ".txt";
		}
		WriteTextFile(fname, compNames);
	}

	private void LogComponent(GameObject root)
	{
		LogSingleComponent(root);
		if(logComponentsRecursive)
		{
			foreach(Transform childT in root.transform)
			{
				//Debug.Log("Searching " + childT.name  + " " );
				LogComponent(childT.gameObject);
			}

		}
	}

	private void LogComponents()
	{
		GameObject root = Selection.activeGameObject;
		if(root)
		{
			LogComponent(root);
		}
	}


	private void LogStates()
	{
		List<string> stateLog = new List<string>();
		foreach(var obj in Selection.objects)
		{

			PresentationNode state = (PresentationNode)obj;
			stateLog.Add("STATE: " + state.name);
			stateLog.Add("\tPresenters:");
			foreach(var presenter in state.activePresenters)
			{
				string tag = presenter.tags.Length == 0 ? "<No Tags>" : presenter.tags[0];
				stateLog.Add("\t\t" + presenter.ToString() + "->{" + tag + "}");
			}
		}
		string path = string.Copy(outputFolder);
		path.Replace("/", "\\");
		if(!path.EndsWith("\\"))
		{
			path += "\\";
		}
		string fname;
		string name = "StateInfo";
		fname = path + name + ".txt";
		WriteTextFile(fname, stateLog);

	}

	//------------------------------------------------------------------------
	// GUID/Asset functions
	//------------------------------------------------------------------------

	private void LogAssetPath(string guid)
	{
		if(guidReg.Match(guid).Success)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			Debug.Log("Path ["+guid+"]: " + path);
		}
		else
		{
			Debug.Log("Invalid GUID: " + guid);

		}
	}

	//------------------------------------------------------------------------
	// Text Search Functions
	//------------------------------------------------------------------------
	private void FindText(string text)
	{
		Debug.Log("Searching for " + textToFind);
		GameObject[] go = Selection.gameObjects;

		ResetCounters();
		foreach(GameObject g in go)
		{
			FindTextInGameObject(g, text);
		}
		if(showStringHistogram)
		{
			DbgHelp.LogHistogram();
		}
		if(_foundReferences.Count > 0)
		{
			foreach(var (msg, obj) in _foundReferences)
			{
				Debug.Log(msg, obj);

			}
			Debug.Log(string.Format("Found {0} references to {1} in {2} GameObjects, {3} Components", _foundReferences.Count, text, _foundObjectCount, _foundComponentCount));
		}
		else
		{
			Debug.Log(string.Format("Text {0} not Found - Searched {1} GameObjects and {2} Components", text, _gameObjectCount, _componentCount));
		}
	}


	//------------------------------------------------------------------------
	private void FindTextInGameObject(GameObject g, string text)
	{
		_gameObjectCount++;
		Component[] components = g.GetComponents<Component>();
		int index = 0;
		//TextLog.Log("Parsing: " + g.name);
		bool objHasText = false;
		foreach(var component in components.Where(x => x != null))
		{
			int i = index++;
			var compType = component.GetType();
			if(IgnoreFilter.IsIgnored(component))
			{
				continue;
			}

			string tag = TaggerExtensions.GetTag(component);
			if(tag.Length > 0 && MatchString(tag, text))
			{
				ComponentAtribute attrib = new ComponentAtribute(tag, compType);
				string msg = GenerateFoundMsg(g, component, attrib, i, tag);
				_foundReferences.Add((msg, g));
			}
			else
			if(compType.Name == text)
			{
				string path = GetGameObjectPath(g);
				string msg = path + " has a " + compType.Name + " with tag: " + tag + " position: " + i.ToString();
				_foundReferences.Add((msg, g));
			}

			_componentCount++;
			AttributeList attrbs = new AttributeList(component, AttributeType.AttrubuteField);
			foreach(var attrib in attrbs)
			{
				try
				{
					if(SearchObjectFields(g, component, component, attrib, text, true, out string matchedText))
					{
						objHasText = true;
						++_foundComponentCount;
						string msg = GenerateFoundMsg(g, component, attrib, i, matchedText);
						_foundReferences.Add((msg, g));
						break;
					}
				}
				catch
				{
					Debug.Log("ERROR EXCEPTION");
					break;
				}
			}
		}
		if(objHasText)
		{
			++_foundObjectCount;
		}
		// Now recurse through each child GO (if there are any):
		foreach(Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			FindTextInGameObject(childT.gameObject, text);
		}
	}

	private void SearchStateMachine(GameObject g, IDataAttribute attribute, object attrbValue, string text)
	{
		if(attribute.GetAttributeType() == typeof(StateMachineModel))
		{
			StateMachineModel sm = (StateMachineModel)attrbValue;
			if(sm != null)
			{
				//				string wildCardText = "*" + text + "*";
				foreach(var state in sm.States)
				{
					var presenterState = state as PresentationNode;
					if (presenterState == null)
					{
						continue;
					}
					string stateName = state.StateName;
					foreach(var presenter in presenterState.activePresenters)
					{
						string presenterName = presenter.ToString();
						if(MatchString(presenterName, "*" + text))
						{
							if(presenter.tags.Length > 0)
							{
								foreach(string tag in presenter.tags)
								{
									string msg = GenerateStatePresenterTagFoundMsg(g, stateName, presenterName, tag);
									_foundReferences.Add((msg, state));
								}
							}
							else
							{
								string msg = GenerateStatePresenterTagFoundMsg(g, stateName, presenterName, "<No Tags>");
								_foundReferences.Add((msg, state));
							}
						}
						foreach(string tag in presenter.tags)
						{
							if(MatchString(tag, text))
							{
								string msg = GenerateStatePresenterTagFoundMsg(g, stateName, presenterName, tag);
								_foundReferences.Add((msg, state));
							}
						}
					}
					foreach(var trigger in presenterState.triggersData)
					{
						string name = trigger.ToString();
						if(MatchString(name, "*" + text))
						{
							string msg = GenerateStateTriggerFoundMsg(g, stateName, name);
							_foundReferences.Add((msg, state));
						}
					}
				}
			}
			DbgHelp.BreakHanger();
		}
	}

	//------------------------------------------------------------------------
	private bool SearchObjectFields(GameObject g, Component component, object obj, IDataAttribute attribute, string text, bool allowProperties, out string matchedText)
	{
		var ct = obj.GetType();
		matchedText = "";
		// Check if we have a valid attribute that also has a value to compare with.
		object attrbValue = null;
		if(IgnoreFilter.IsIgnored(attribute) || !attribute.GetValue(obj, ref attrbValue))
		{
			return false;
		}
		SearchStateMachine(g, attribute, attrbValue, text);

		// Debug.Log("Testing " + attribute.GetType().FullName);
		// First see if the attribute value we have is a string and that it if it is a match.
		if(attribute.GetAttributeType() == typeof(string))
		{
			if(MatchIfIsString(attrbValue, text))
			{
				matchedText = attrbValue.ToString();
				// We have a match.
				return true;
			}
			return false;
		}
		// Next see if our attribute value a an enumerable collection.
		else if(attrbValue is IEnumerable)
		{
			foreach(var item in attrbValue as IEnumerable)
			{
				if(item == null)
				{
					continue;
				}
				if(IgnoreFilter.IsIgnored(item))
				{
					continue;
				}
				if(MatchIfIsString(item, text))
				{
					matchedText = item.ToString();
					return true;
				}
				AttributeList attrbs = new AttributeList(item, AttributeType.AttrubuteField);
				foreach(var attrib in attrbs.Where(x => !IgnoreFilter.IsIgnored(x)))
				{
					if(SearchObjectFields(g, component, item, attrib, text, false, out matchedText))
					{
						return true;
					}
				}
			}
		}
		else if(allowProperties)
		{
			AttributeList attrbs = new AttributeList(attrbValue, AttributeType.AttrubuteProperty);
			foreach(var attrib in attrbs.Where(x => !IgnoreFilter.IsIgnored(x)))
			{
				//Debug.Log("Testing AttributeList " + attrib.GetType().FullName);
				if(SearchObjectFields(g, component, attrbValue, attrib, text, false, out matchedText))
				{
					return true;
				}
			}
			attrbs = new AttributeList(attrbValue, AttributeType.AttrubuteField);
			foreach(var attrib in attrbs.Where(x => !IgnoreFilter.IsIgnored(x)))
			{
				//Debug.Log("Testing AttributeList " + attrib.GetType().FullName);
				if(SearchObjectFields(g, component, attrbValue, attrib, text, false, out matchedText))
				{
					return true;
				}
			}
		}
		return false;
	}

	//------------------------------------------------------------------------
	private static string GenerateFoundMsg(GameObject g, Component comp, IDataAttribute attrb, int i, string text)
	{
		string s = g.name + "->" + comp.GetType().Name + "->" + attrb.GetName();
		Transform t = g.transform;
		while(t.parent != null)
		{
			s = t.parent.name + "/" + s;
			t = t.parent;
		}
		s += " contains a string \"" + text + "\"  at position: " + i.ToString();
		return s;
	}
	private static string GenerateStateFoundMsg(GameObject g, Component comp, IDataAttribute attrb, string stateName)
	{
		string s = g.name + "->" + comp.GetType().Name + "->" + attrb.GetName();
		Transform t = g.transform;
		while(t.parent != null)
		{
			s = t.parent.name + "/" + s;
			t = t.parent;
		}
		s += " contains a State named \"" + stateName;
		return s;
	}
	// 	private static string GenerateStatePresenterFoundMsg(GameObject g, string stateName, string presenterName)
	// 	{
	// 		return "State Machine." + stateName + "[" + presenterName + "]";
	// 	}
	private static string GenerateStatePresenterTagFoundMsg(GameObject g, string stateName, string presenterName, string tag)
	{
		return "STATE: " + stateName + "->" + presenterName + " " + tag;
	}
	private static string GenerateStateTriggerFoundMsg(GameObject g, string stateName, string triggerName)
	{
		return "STATE: " + stateName + "->Trigger->" + triggerName;
	}


	//------------------------------------------------------------------------
	// Find Missing Script Functions
	//------------------------------------------------------------------------
	private static void FindNullTags()
	{
		GameObject[] go = Selection.gameObjects;
		_gameObjectCount = 0;
		_componentCount = 0;
		_problemCount = 0;
		foreach(GameObject g in go)
		{
			FindNullTagsInGameObject(g);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} null tags", _gameObjectCount, _componentCount, _problemCount));
	}

	//------------------------------------------------------------------------
	private static void FindNullTagsInGameObject(GameObject g)
	{
		_gameObjectCount++;
		Component[] components = g.GetComponents<Component>();
		foreach(var comp in components)
		{
			_componentCount++;
			if(comp == null || comp.GetType() != typeof(Tagger))
			{
				// Ignore anything that is not a 'Tagger'
				continue;
			}
			// See if our Tagger has any 'null' keys
			if(((Tagger)comp).TryFind(null, out Tagger.Tag tag))
			{
				_problemCount++;
				string s = g.name;
				Transform t = g.transform;
				while(t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				Debug.Log(s + " has a null tag ", g);
			}

		}
		// Now recurse through each child GO (if there are any):
		foreach(Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			FindNullTagsInGameObject(childT.gameObject);
		}
	}


	//------------------------------------------------------------------------
	// Find Missing Script Functions
	//------------------------------------------------------------------------
	private static void FindMissingScripts()
	{
		GameObject[] go = Selection.gameObjects;
		_gameObjectCount = 0;
		_componentCount = 0;
		_problemCount = 0;
		foreach(GameObject g in go)
		{
			FindMissingScriptsInGameObject(g);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", _gameObjectCount, _componentCount, _problemCount));
	}

	//------------------------------------------------------------------------
	private static void FindMissingScriptsInGameObject(GameObject g)
	{
		_gameObjectCount++;
		Component[] components = g.GetComponents<Component>();
		for(int i = 0; i < components.Length; i++)
		{
			_componentCount++;
			if(components[i] == null)
			{
				_problemCount++;
				string s = g.name;
				Transform t = g.transform;
				while(t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				Debug.Log(s + " has an empty script attached in position: " + i, g);
			}
		}
		// Now recurse through each child GO (if there are any):
		foreach(Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			FindMissingScriptsInGameObject(childT.gameObject);
		}
	}

	//------------------------------------------------------------------------
	// Utility functions
	//------------------------------------------------------------------------

	private bool MatchString(string str, string text)
	{
		return DbgHelp.WildCardCompare(text, str, ignoreCase);
	}

	//------------------------------------------------------------------------
	private bool MatchIfIsString(object obj, string text)
	{
		if(obj.GetType() != typeof(string))
		{
			return false;
		}
		return DbgHelp.WildCardCompare(text, obj.ToString(), ignoreCase);
	}
	//------------------------------------------------------------------------

	#endregion
}
