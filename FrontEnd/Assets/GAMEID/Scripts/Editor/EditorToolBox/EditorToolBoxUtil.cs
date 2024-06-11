using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Linq;
using FoundReferenceSet = System.Collections.Generic.HashSet<(string msg, UnityEngine.GameObject obj)>;
using System.Text.RegularExpressions;
using Spine.Unity.AttachmentTools;
//using UnityEngine.Rendering.Universal;
using AttributeType = NMG.EditorToolBoxUtil.AttributeList.AttributeType;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;

//------------------------------------------------------------------------
// Debugging Utils
//------------------------------------------------------------------------
namespace NMG.EditorToolBoxUtil
{

	public class DbgHelp
	{
		public static Dictionary<string, int> histogram = new Dictionary<string, int>();

		public static void LogComponentMsg(Component component, string message)
		{
			string tag = TaggerExtensions.GetTag(component);
			string msg = component.GetType().Name + "->{" + tag + "}: " + message;
			Debug.Log(msg, component);
		}

		public static void RecordString(string text)
		{
			if(text.Length == 0)
			{
				return;
			}
			int count = 0;
			if(histogram.ContainsKey(text))
			{
				count = histogram[text];
			}
			histogram[text] = count + 1;

		}
		public static void LogHistogram()
		{
			List<KeyValuePair<string, int>> output = new List<KeyValuePair<string, int>>();
			foreach(var k in histogram)
			{
				output.Add(k);
			}
			List<KeyValuePair<string, int>> sorted = output.OrderBy(o => o.Value).ToList();
			foreach(var item in sorted)
			{
				Debug.Log("\"" + item.Key + "\" has " + item.Value + " instances.");
			}

		}

		public static void BreakHanger()
		{
		}

		public static bool WildCardCompare(string pattern, string input, bool ignoreCase)
		{
			return MatchWildcardString(pattern, input, ignoreCase);

		}
		private static bool MatchWildcardString(string pattern, string input, bool ignoreCase)
		{
			string regexPattern = "^" + (ignoreCase ? "(?i)" : "(?-i)");
			
			foreach(Char c in pattern)
			{
				switch(c)
				{
					case '*':
						regexPattern += ".*";
						break;
					case '?':
						regexPattern += ".";
						break;
					default:
						regexPattern += "[" + c + "]";
						break;
				}
			}

			return new Regex(regexPattern + "$").IsMatch(input);
		}

	}

	
	//------------------------------------------------------------------------
	// Helper class that logs to a text file.
	//------------------------------------------------------------------------
	public class TextLog
	{
		private readonly static string fname = "Log.txt";
		private readonly static StreamWriter _writer = File.CreateText(fname);
		private readonly static string filepath = Path.GetFullPath(fname);
		private static bool isAttached = false;

		public static bool breakOnAssert = false;
		public static bool breakOnError = false;
		public static bool breakOnException = false;
		public static bool breakOnWarning = false;
		public static bool breakOnLog = false;

		public static bool logInfo = true;
		public static bool logWarning = false;
		public static bool logAssert = true;
		public static bool logError = true;
		public static bool logException = true;

		public static string logTextToMatch = "";




		public static void Log(string msg)
		{
			_writer.WriteLine(msg);
			_writer.Flush();
		}
		public static void HandleLog(string logString, string stackTrace, LogType type)
		{
			bool shouldLog = false;
			string logMsg = "";
			switch(type)
			{
				case LogType.Assert:
					logMsg = "(Assert): ";
					shouldLog = logAssert;
					break;
				case LogType.Error:
					logMsg = "(Error): ";
					shouldLog = logError;
					break;
				case LogType.Exception:
					logMsg = "(Exception): ";
					shouldLog = logException;
					break;
				case LogType.Log:
					logMsg = "(Info): ";
					shouldLog = logInfo;
					break;
				case LogType.Warning:
					logMsg = "(Warning): ";
					shouldLog = logWarning;
					break;
			}
			if(shouldLog)
			{
				logMsg += logString;
				_writer.WriteLine(logMsg);
				_writer.Flush();
			}
			if(System.Diagnostics.Debugger.IsAttached)
			{
				HandleBreakPointOnLogMsg(type, logString);
			}
		}
		public static void AttachToUnity()
		{
			if(!isAttached)
			{
				Debug.Log("LogFile -> " + filepath);
				Application.logMessageReceived += TextLog.HandleLog;
				isAttached = true;

			}
		}
		public static void DetachFromUnity()
		{
			if(isAttached)
			{
				Application.logMessageReceived -= TextLog.HandleLog;
				isAttached = false;
			}
		}

		private static void HandleBreakPointOnLogMsg(LogType type, string logString)
		{
			bool debugBreakTriggered = false;
			switch(type)
			{
				case LogType.Assert:
					debugBreakTriggered = breakOnAssert;
					break;
				case LogType.Error:
					debugBreakTriggered = breakOnError;
					break;
				case LogType.Exception:
					debugBreakTriggered = breakOnException;
					break;
				case LogType.Log:
					debugBreakTriggered = breakOnLog;
					break;
				case LogType.Warning:
					debugBreakTriggered = breakOnWarning;
					break;
			}
			if(debugBreakTriggered)
			{
				if(string.IsNullOrEmpty(logTextToMatch))
				{
					System.Diagnostics.Debugger.Break();
				}
				else if(DbgHelp.WildCardCompare(logTextToMatch, logString, true))
				{
					System.Diagnostics.Debugger.Break();
				}
			}
		}
	}

	//------------------------------------------------------------------------
	// Helper class to allow search to ignore specific data types.
	//------------------------------------------------------------------------
	class IgnoreFilter
	{
		private readonly static List<Type> ignoredTypes = new List<Type>()
	{
        //typeof(Animator),
        typeof(Canvas),
		typeof(Color),
		typeof(EventSystem),
		typeof(Material),
		typeof(MeshFilter),
		typeof(MeshRenderer),
		typeof(Renderer),
		typeof(SymbolId),
		typeof(WinningSymbolLocator),
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(Spine.Attachment),
		typeof(Spine.AttachmentTimeline),
		typeof(Spine.Bone),
		typeof(Spine.ColorTimeline),
		typeof(Spine.RotateTimeline),
		typeof(Spine.ScaleTimeline),
		typeof(Spine.BoneData),
		typeof(Spine.TranslateTimeline),
		typeof(Spine.Skeleton),
		typeof(Spine.Unity.SkeletonUtilityBone.Mode),
		typeof(System.Boolean),
		typeof(System.Char),
		typeof(System.Int32),
		typeof(System.Int32),
		typeof(System.Single),
		typeof(Unity.Mathematics.float4),
		typeof(Unity.Mathematics.float2),
		typeof(TextMeshPro),
		typeof(TMPro.TMP_Character),
		typeof(TMPro.TMP_FontAsset),
		typeof(TMPro.TMP_SubMesh),
		typeof(Transform),
		typeof(UnityEngine.Collider),
		typeof(UnityEngine.InputSystem.Editor.InputControlPicker.Mode),
		typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule),
		typeof(UnityEngine.Mesh),
		typeof(UnityEngine.TextCore.Glyph),
		typeof(UniversalAdditionalCameraData),
		typeof(UniRx.Triggers.ObservableAnimatorTrigger),
		typeof(UniRx.Triggers.ObservableBeginDragTrigger),
		typeof(UniRx.Triggers.ObservableCancelTrigger),
		typeof(UniRx.Triggers.ObservableCanvasGroupChangedTrigger),
		typeof(UniRx.Triggers.ObservableCollision2DTrigger),
		typeof(UniRx.Triggers.ObservableCollisionTrigger),
		typeof(UniRx.Triggers.ObservableDeselectTrigger),
		typeof(UniRx.Triggers.ObservableDestroyTrigger),
		typeof(UniRx.Triggers.ObservableDragTrigger),
		typeof(UniRx.Triggers.ObservableDropTrigger),
		typeof(UniRx.Triggers.ObservableEnableTrigger),
		typeof(UniRx.Triggers.ObservableEndDragTrigger),
		typeof(UniRx.Triggers.ObservableEventTrigger),
		typeof(UniRx.Triggers.ObservableFixedUpdateTrigger),
		typeof(UniRx.Triggers.ObservableInitializePotentialDragTrigger),
		typeof(UniRx.Triggers.ObservableJointTrigger),
		typeof(UniRx.Triggers.ObservableLateUpdateTrigger),
		//typeof(UniRx.Triggers.ObservableMouseTrigger),
		typeof(UniRx.Triggers.ObservableMoveTrigger),
		typeof(UniRx.Triggers.ObservableParticleTrigger),
		typeof(UniRx.Triggers.ObservablePointerClickTrigger),
		typeof(UniRx.Triggers.ObservablePointerDownTrigger),
		typeof(UniRx.Triggers.ObservablePointerEnterTrigger),
		typeof(UniRx.Triggers.ObservablePointerExitTrigger),
		typeof(UniRx.Triggers.ObservablePointerUpTrigger),
		typeof(UniRx.Triggers.ObservableRectTransformTrigger),
		typeof(UniRx.Triggers.ObservableScrollTrigger),
		typeof(UniRx.Triggers.ObservableSelectTrigger),
		typeof(UniRx.Triggers.ObservableStateMachineTrigger),
		typeof(UniRx.Triggers.ObservableSubmitTrigger),
		typeof(UniRx.Triggers.ObservableTransformChangedTrigger),
		typeof(UniRx.Triggers.ObservableTrigger2DTrigger),
		typeof(UniRx.Triggers.ObservableTriggerBase),
		typeof(UniRx.Triggers.ObservableTriggerExtensions),
		typeof(UniRx.Triggers.ObservableTriggerExtensions),
		typeof(UniRx.Triggers.ObservableTriggerTrigger),
		typeof(UniRx.Triggers.ObservableUpdateSelectedTrigger),
		typeof(UniRx.Triggers.ObservableUpdateTrigger),
		typeof(UniRx.Triggers.ObservableVisibleTrigger),
		typeof(Vector2),
		typeof(Vector3),
		typeof(Canvas),

		typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule)
	};


		public static bool IsIgnoredType(Type type)
		{
			if(ignoredTypes.Contains(type))
			{
				return true;
			}
			if(type.Name == "MonoProperty")
			{
				return true;
			}
			return false;
		}
		public static bool IsIgnored(object obj)
		{
			return IsIgnoredType(obj.GetType());
		}
		public static bool IsIgnored(IDataAttribute attrb)
		{
			return IsIgnoredType(attrb.GetAttributeType());
		}
		public static bool IsIgnored(FieldInfo field)
		{
			return IsIgnoredType(field.FieldType);
		}
		public static bool IsIgnored(PropertyInfo prop)
		{
			return IsIgnoredType(prop.GetType());
		}
	}

	public interface IDataAttribute
	{

		bool GetValue(object obj, ref object value);
		string GetName();
		Type GetAttributeType();
	}

	class FieldAtribute : IDataAttribute
	{
		private readonly FieldInfo _field;
		public FieldAtribute(FieldInfo field)
		{
			_field = field;
		}
		public bool GetValue(object obj, ref object value)
		{
			bool validValue;
//         try
//         {
			value = _field.GetValue(obj);
			validValue = value != null && !value.Equals(null);
			if(validValue && value.GetType() == typeof(string))
			{
				DbgHelp.RecordString(value.ToString());
			}
//         }
//         catch
//         {
//             Debug.Log("EXCEPTION ON: " + GetAttributeType().FullName);
//         }
			return validValue;
		}
		public string GetName() { return _field.Name; }
		public Type GetAttributeType() { return _field.FieldType; }
	}

	class PropertyAtribute : IDataAttribute
	{
		private readonly PropertyInfo _property;
		public PropertyAtribute(PropertyInfo property)
		{
			_property = property;
		}
		public bool GetValue(object obj, ref object value)
		{
			bool validValue = false;
			try
			{
				value = _property.GetValue(obj);
				validValue = value != null && !value.Equals(null);
				if(validValue && value.GetType() == typeof(string))
				{
					DbgHelp.RecordString(value.ToString());
				}
			}
			catch
			{
				Debug.Log("EXCEPTION ON: " + GetAttributeType().FullName);
			}
			return validValue;
		}
		public string GetName() { return _property.Name; }
		public Type GetAttributeType() { return _property.GetType(); }
	}

	class ComponentAtribute : IDataAttribute
	{
		private readonly string _name;
		private readonly Type _type;
		public ComponentAtribute(string name, Type type)
		{
			_name = name;
			_type = type;
		}
		public bool GetValue(object obj, ref object value)  { return false; }
		public string GetName() { return _name; }
		public Type GetAttributeType() { return _type; }
	}
	//------------------------------------------------------------------------
	// Helper class to abstract Fields and Properties during text search
	//------------------------------------------------------------------------
	class AttributeList : List<IDataAttribute>
	{
		[Flags]
		public enum AttributeType
		{
			AttrubuteUndefined = 0,
			AttrubuteProperty = 1,
			AttrubuteField = 2,
		};
		private
			static readonly BindingFlags bindingFlags =
		BindingFlags.Instance |
		BindingFlags.NonPublic |
		BindingFlags.Public |
		BindingFlags.GetField |
		BindingFlags.GetProperty;

		//------------------------------------------------------------------------
		public AttributeList(object obj, AttributeType types)
		{
			Type type = obj.GetType();
			if((types & AttributeType.AttrubuteField) == AttributeType.AttrubuteField)
			{
				Append(type.GetFields(bindingFlags));
			}
			if((types & AttributeType.AttrubuteProperty) == AttributeType.AttrubuteProperty)
			{
				Append(type.GetProperties());
			}

		}
		//------------------------------------------------------------------------
		void Append(FieldInfo[] fields)
		{
			foreach(var field in fields.Where(x => !IgnoreFilter.IsIgnored(x)))
			{
				FieldAtribute attrb = new FieldAtribute(field);
				this.Add(attrb);

			}
		}
		//------------------------------------------------------------------------
		void Append(PropertyInfo[] properties)
		{
			foreach(var property in properties.Where(x => !IgnoreFilter.IsIgnored(x)))
			{
				if(property.IsDefined(typeof(ObsoleteAttribute), true))
				{
					continue;
				}

				PropertyAtribute prop = new PropertyAtribute(property);
				this.Add(prop);

			}
		}
	}

}
