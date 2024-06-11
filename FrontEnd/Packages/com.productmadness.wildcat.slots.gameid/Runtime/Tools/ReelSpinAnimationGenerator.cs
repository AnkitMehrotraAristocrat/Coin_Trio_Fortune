#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Milan.FrontEnd.Core.v5_1_1;
using System.IO;
using Milan.FrontEnd.Bridge.Logging;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
	public class ReelSpinAnimationGenerator : EditorWindow
	{

		/// <summary>
		/// The Reel Spin Animation Generator Automatically generates all of the standard animations with the proper keyframes 
		/// and events needed for the reels including Start, Loop, Stop, Quickstop, and Stop Anticipation, 
		/// and can either populate an existing Reel Animator Controller with the new animations, 
		/// or create a new Controller with the proper triggers, transitions, and behaviors.
		/// </summary>

		public float symbolHeight = 256f;
		public float windupDistance = 1f;
		public float bounceDistance = 1f;
		public float reelSpeed = 1f;
		public int matrixColumns = 5;
		public int matrixRows = 3;
		public string animationName = "Standard";

		readonly int frameSamples = 60;

		float symbolSize;
		float reelSpinDistance;
		int totalSymbols;

		AnimationClip clip;
		AnimationCurve curve;
		Keyframe[] keys;
		AnimationEvent[] evts;
		float keyFrame = 0f;
		float keyTime = 0f;
		float keyValue = 0f;

		bool usingCustomSpinResultProvider = false;
		public bool useExistingController = false;
		public UnityEditor.Animations.AnimatorController reelController;
		UnityEditor.Animations.AnimatorStateMachine stateMachine;

		private string animationFolderPath = "Assets/GAMEID/Animations/Reels/ToolGenerated/";

		//add script to tools menu
		[MenuItem("Tools/NMG Vegas/Reel Spin Animation Generator")]
		static void Init()
		{
			//get existing open window or make a new one if there is none
			ReelSpinAnimationGenerator reelspinAnimationGenerator = (ReelSpinAnimationGenerator)EditorWindow.GetWindow(typeof(ReelSpinAnimationGenerator));
			reelspinAnimationGenerator.minSize = new Vector2(350f, 220f);
			reelspinAnimationGenerator.Show();
		}

		void OnGUI()
		{
			symbolHeight = EditorGUILayout.FloatField("Symbol Height", symbolHeight);
			windupDistance = EditorGUILayout.FloatField(new GUIContent("Windup Distance", "Windup Distance in number of symbols"), windupDistance);
			bounceDistance = EditorGUILayout.FloatField(new GUIContent("Bounce Distance", "Bounce Distance in number of symbols"), bounceDistance);
			reelSpeed = EditorGUILayout.FloatField(new GUIContent("Reel Speed", "Reel Speed in number of symbols moved off the reels every 2 frames"), reelSpeed);
			matrixColumns = EditorGUILayout.IntField("Reel Grid Columns", matrixColumns);
			matrixRows = EditorGUILayout.IntField("Reel Grid Rows", matrixRows);
			animationName = EditorGUILayout.TextField("Animation Name", animationName);
			usingCustomSpinResultProvider = EditorGUILayout.ToggleLeft("Using NMG custom Spin Result Provider", usingCustomSpinResultProvider);
			useExistingController = EditorGUILayout.BeginToggleGroup("Use existing Animator Controller", useExistingController);

			reelController = (UnityEditor.Animations.AnimatorController)EditorGUILayout.ObjectField("Reel Animator Controller", reelController, typeof(UnityEditor.Animations.AnimatorController), false);
			EditorGUILayout.EndToggleGroup();

			if (ValidateInputs() && GUILayout.Button("Generate Animation Clips"))
			{
				Main();
			}
		}

		public bool ValidateInputs()
		{
			if (symbolHeight == 0f)
				return false;
			if (windupDistance == 0f)
				return false;
			if (reelSpeed == 0f)
				return false;
			if (matrixColumns == 0)
				return false;
			if (matrixRows == 0)
				return false;
			if (animationName == "")
				return false;
			if (useExistingController && reelController == null)
				return false;
			return true;
		}

		public void CreateAnimatorController()
		{
			//delete the old controller asset before creating and saving the new one (will not error if the controller does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/ReelAnimator" + animationName + ".controller");
			reelController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(animationFolderPath + animationName + "/ReelAnimator" + animationName + ".controller");

			//add parameters
			reelController.AddParameter("spin", AnimatorControllerParameterType.Trigger);
			reelController.AddParameter("stop", AnimatorControllerParameterType.Trigger);
			reelController.AddParameter("quickStop", AnimatorControllerParameterType.Trigger);
			reelController.AddParameter("stop-anticipate", AnimatorControllerParameterType.Trigger);

			//add state machine
			stateMachine = reelController.layers[0].stateMachine;

			//add states
			var idleState = stateMachine.AddState("Idle"); //must be first to be the default state
			var startState = stateMachine.AddState("spin_start");
			var spinState = stateMachine.AddState("spin_loop");
			var stopState = stateMachine.AddState("spin_stop");
			var quickStopState = stateMachine.AddState("quick_stop");
			var anticipationStopState = stateMachine.AddState("spin_stop_anticipation");

			//add behaviors
			idleState.AddStateMachineBehaviour<TaggedObservableStateMachineTrigger>();
			spinState.AddStateMachineBehaviour<TaggedObservableStateMachineTrigger>();
			TaggedObservableStateMachineTrigger[] behaviors = reelController.GetBehaviours<TaggedObservableStateMachineTrigger>();
			behaviors[0].tag = "idle";
			behaviors[1].tag = "spinLoop";

			//add transitions
			//idle to spin start
			var startTransition = idleState.AddTransition(startState);
			startTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "spin");
			startTransition.exitTime = 0.75f;
			startTransition.duration = 0f;
			startTransition.hasFixedDuration = true;
			startTransition.hasExitTime = false;

			//spin start to spin loop
			var spinTransition = startState.AddTransition(spinState);
			spinTransition.exitTime = 1f;
			spinTransition.duration = 0f;
			spinTransition.hasFixedDuration = false;
			spinTransition.hasExitTime = true;

			//spin loop to spin stop
			var loopStopTransition = spinState.AddTransition(stopState);
			loopStopTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "stop");
			loopStopTransition.exitTime = 1f;
			loopStopTransition.duration = 0f;
			loopStopTransition.hasFixedDuration = true;
			loopStopTransition.hasExitTime = true;

			//spin loop to quick stop
			var loopQuickStopTransition = spinState.AddTransition(quickStopState);
			loopQuickStopTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "quickStop");
			loopQuickStopTransition.exitTime = 1f;
			loopQuickStopTransition.duration = 0f;
			loopQuickStopTransition.hasFixedDuration = false;
			loopQuickStopTransition.hasExitTime = false;

			//spin loop to anticipation stop
			var loopAnticipationStopTransition = spinState.AddTransition(anticipationStopState);
			loopAnticipationStopTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "stop-anticipate");
			loopAnticipationStopTransition.exitTime = 1f;
			loopAnticipationStopTransition.duration = 0f;
			loopAnticipationStopTransition.hasFixedDuration = true;
			loopAnticipationStopTransition.hasExitTime = false;

			//spin stop to quick stop
			var stopQuickStopTransition = stopState.AddTransition(quickStopState);
			stopQuickStopTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "quickStop");
			stopQuickStopTransition.exitTime = 0.75f;
			stopQuickStopTransition.duration = 0f;
			stopQuickStopTransition.hasFixedDuration = false;
			stopQuickStopTransition.hasExitTime = false;

			//spin stop to idle
			var stopIdleTransition = stopState.AddTransition(idleState);
			stopIdleTransition.exitTime = 1f;
			stopIdleTransition.duration = 0f;
			stopIdleTransition.hasFixedDuration = true;
			stopIdleTransition.hasExitTime = true;

			//anticipation stop to quick stop
			var anticipationStopQuickStopTransition = anticipationStopState.AddTransition(quickStopState);
			anticipationStopQuickStopTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "quickStop");
			anticipationStopQuickStopTransition.exitTime = 1f;
			anticipationStopQuickStopTransition.duration = 0f;
			anticipationStopQuickStopTransition.hasFixedDuration = false;
			anticipationStopQuickStopTransition.hasExitTime = false;

			//anticipation stop to idle
			var anticipationStopIdleTransition = anticipationStopState.AddTransition(idleState);
			anticipationStopIdleTransition.exitTime = 1f;
			anticipationStopIdleTransition.duration = 0f;
			anticipationStopIdleTransition.hasFixedDuration = true;
			anticipationStopIdleTransition.hasExitTime = true;

			//quick stop to idle
			var quickStopIdleTransition = quickStopState.AddTransition(idleState);
			quickStopIdleTransition.exitTime = 1f;
			quickStopIdleTransition.duration = 0f;
			quickStopIdleTransition.hasFixedDuration = false;
			quickStopIdleTransition.hasExitTime = true;
		}

		public void Main()
		{
			//create a new folder for the animations if one doesn't already exist
			if (!AssetDatabase.IsValidFolder(animationFolderPath + animationName))
			{
				GameIdLogger.Logger.Debug("Created new folder: " + animationName + ". Path: \"" + animationFolderPath + "\"");
				Directory.CreateDirectory(animationFolderPath + animationName);
			}


			if (!useExistingController)
				CreateAnimatorController();
			GenerateClips();
		}

		public void GenerateClips()
		{
			//set data based on the input
			symbolSize = symbolHeight / 100f;
			totalSymbols = matrixRows + 2;
			reelSpinDistance = totalSymbols * symbolSize;

			//generate each animation
			GenerateSpinStart();
			GenerateSpinStop();
			GenerateSpinLoop();
			GenerateSpinQuickstop();
			GenerateSpinStopAnticipation();
		}

		public void AddClipsToController(AnimationClip c, string stateName)
		{
			stateMachine = reelController.layers[0].stateMachine;
			for (int i = 0; i < stateMachine.states.Length; i++)
			{
				if (stateMachine.states[i].state.name == stateName)
				{
					stateMachine.states[i].state.motion = c;
				}
			}
		}

		void GenerateSpinStart()
		{
			//reset data
			clip = new AnimationClip();
			evts = new AnimationEvent[totalSymbols];
			keyFrame = 0f;
			keyTime = 0f;

			//go through each symbol and set their animation keyframes and events
			for (int i = totalSymbols; i > 0; i--)
			{
				keys = new Keyframe[2];
				keys[0] = new Keyframe(0f, 0f);

				//on the first loop, start with 15 frames, then add 2 divided by the reelspin (rounded up) for each subsequent loop
				if (i == totalSymbols)
					keyFrame += 15;
				else
					keyFrame += Mathf.Ceil(2 / reelSpeed);

				keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
				keys[1] = new Keyframe(keyTime, reelSpinDistance);

				//create the animation curve and set both tangents on the second key to be constant
				curve = new AnimationCurve(keys);
				AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);
				AnimationUtility.SetKeyRightTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);

				//add the animation curve to the current clip with the proper object name
				string objName = "Symbol" + (i - 1).ToString();
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);

				//create the animation event with the proper function and parameter
				int elmt = i - 1;
				evts[elmt] = new AnimationEvent();
				evts[elmt].intParameter = i - 1;
				evts[elmt].time = keyTime;
				evts[elmt].functionName = "SetTopSymbol";
			}

			//create the keyframes for the parent 'Symbols' object
			keys = new Keyframe[5];

			//set the last keyframe
			keys[0] = new Keyframe(keyTime, -reelSpinDistance); //use the latest key time set by the loop
			keys[0].inTangent = -70f;

			//set the first frame
			keys[1] = new Keyframe(0f, 0f);

			//set the windup
			keyFrame = 5;
			keyTime = keyFrame / frameSamples;
			keys[2] = new Keyframe(keyTime, (symbolSize * windupDistance));

			keyFrame = 10;
			keyTime = keyFrame / frameSamples;
			keys[3] = new Keyframe(keyTime, 0f);
			keys[3].inTangent = -5f;
			keys[3].outTangent = -20f;

			keyFrame = 15;
			keyTime = keyFrame / frameSamples;
			keys[4] = new Keyframe(keyTime, -symbolSize * 1.01f); //set the key slightly lower than the symbol size
			keys[4].inTangent = -45f;
			//keys[4].outTangent = -45f;

			//create and set the animation curve with the new keys
			curve = new AnimationCurve(keys);

			//set the tangent mode of the last two keys to be linear
			AnimationUtility.SetKeyBroken(curve, 3, true);
			AnimationUtility.SetKeyRightTangentMode(curve, 3, AnimationUtility.TangentMode.Linear);
			AnimationUtility.SetKeyLeftTangentMode(curve, 4, AnimationUtility.TangentMode.Linear);
			AnimationUtility.SetKeyRightTangentMode(curve, 4, AnimationUtility.TangentMode.Linear);


			clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
			//attach the animation events to the animation clip
			AnimationUtility.SetAnimationEvents(clip, evts);

			//delete the old animation asset before creating and saving the new one (will not error if the animation does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/SpinStart" + animationName + ".anim");
			AssetDatabase.CreateAsset(clip, animationFolderPath + animationName + "/SpinStart" + animationName + ".anim");

			//add the animation to the given animator
			AddClipsToController(clip, "spin_start");
		}

		void GenerateSpinStop()
		{
			//reset data
			clip = new AnimationClip();
			evts = new AnimationEvent[totalSymbols + 1];
			keyFrame = 0;
			keyTime = 0f;

			//go through each symbol and set their animation keyframes and events
			for (int i = totalSymbols; i > 0; i--)
			{
				keys = new Keyframe[2];
				keys[0] = new Keyframe(0f, 0f);


				if (reelSpeed > 1)
					keyFrame += 2;
				else
					keyFrame += Mathf.Ceil(2 / reelSpeed);
				/*
				//on the first and second loop, add 4 frames, otherwise add 3 for each subsequent loop
				if (i == totalSymbols || i == totalSymbols - 1)
					keyFrame += 4;
				else
					keyFrame += 3;
				*/

				keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
				keys[1] = new Keyframe(keyTime, reelSpinDistance);

				//create the animation curve and set both tangents on the second key to be constant
				curve = new AnimationCurve(keys);
				AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);
				AnimationUtility.SetKeyRightTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);

				//add the animation curve to the current clip with the proper object name
				string objName = "Symbol" + (i - 1).ToString();
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);

				//create the animation event with the proper function and parameter
				evts[i] = new AnimationEvent();
				evts[i].intParameter = i - 1;
				evts[i].time = keyTime;
				evts[i].functionName = "SetTopSymbol";
			}

			//create the keyframes for the parent 'Symbols' object
			keys = new Keyframe[3];

			//set the first keyframe
			keys[0] = new Keyframe(0.0f, 0.0f);

			keyFrame += 7; //add to the last keyframe set by the loop
			keyTime = keyFrame / frameSamples;
			keys[1] = new Keyframe(keyTime, -1f * (reelSpinDistance + (symbolSize * bounceDistance)));

			keyFrame += 14;
			keyTime = keyFrame / frameSamples;
			keys[2] = new Keyframe(keyTime, -1f * reelSpinDistance);

			//create the animation curve and set the first and second keyframe tangents to be linear
			curve = new AnimationCurve(keys);
			AnimationUtility.SetKeyLeftTangentMode(curve, 0, AnimationUtility.TangentMode.Linear);
			AnimationUtility.SetKeyRightTangentMode(curve, 0, AnimationUtility.TangentMode.Linear);
			AnimationUtility.SetKeyBroken(curve, 1, true);
			AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Linear);

			//set the animation curve with they new keys
			clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

			//populate the first array element with the proper function and parameter
			evts[0] = new AnimationEvent();
			keyFrame = 5;
			keyTime = keyFrame / frameSamples;

			//if the user is implementing the NMG custom spin result provider, set the Use Results event to be at frame 0
			if (usingCustomSpinResultProvider)
			{
				evts[0].time = 0f;
			}
			else
			{
				evts[0].time = keyTime;
			}
			evts[0].functionName = "UseResults";

			//attach the animation events to the animation clip
			AnimationUtility.SetAnimationEvents(clip, evts);

			//delete the old animation asset before creating and saving the new one (will not error if the animation does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/SpinStop" + animationName + ".anim");
			AssetDatabase.CreateAsset(clip, animationFolderPath + animationName + "/SpinStop" + animationName + ".anim");

			//add the animation to the given animator
			AddClipsToController(clip, "spin_stop");
		}

		void GenerateSpinLoop()
		{
			//reset data
			clip = new AnimationClip();
			evts = new AnimationEvent[totalSymbols];
			keyFrame = 0;
			keyTime = 0f;

			//go through each symbol and set their animation keyframes and events
			for (int i = totalSymbols; i > 0; i--)
			{
				keys = new Keyframe[2];
				keys[0] = new Keyframe(0f, 0f);

				keyFrame += Mathf.Ceil(2 / reelSpeed);
				keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
				keys[1] = new Keyframe(keyTime, reelSpinDistance);

				//create the animation curve and set both tangents on the second frame to be constant
				curve = new AnimationCurve(keys);
				AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);
				AnimationUtility.SetKeyRightTangentMode(curve, 1, AnimationUtility.TangentMode.Constant);

				//add the animation curve to the current clip with the proper object name
				string objName = "Symbol" + (i - 1).ToString();
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);

				//create the animation event with the proper function and parameter
				int elmt = i - 1;
				evts[elmt] = new AnimationEvent();
				evts[elmt].intParameter = i - 1;
				evts[elmt].time = keyTime;
				evts[elmt].functionName = "SetTopSymbol";
			}

			//create the keyframes for the parent 'Symbols' object
			keys = new Keyframe[2];

			//set the last keyframe
			keys[0] = new Keyframe(keyTime, -1f * reelSpinDistance); //using the keytime from the last symbol

			//set the first keyframe
			keys[1] = new Keyframe(0.0f, 0.0f);

			//create the animation curve with new keys and set the tangents to be Linear
			curve = new AnimationCurve(keys);
			AnimationUtility.SetKeyRightTangentMode(curve, 0, AnimationUtility.TangentMode.Linear);
			AnimationUtility.SetKeyLeftTangentMode(curve, 1, AnimationUtility.TangentMode.Linear);
			clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

			//make it a looping animation
			AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
			clipSettings.loopTime = true;
			AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

			//attach the animation events to the animation clip
			AnimationUtility.SetAnimationEvents(clip, evts);

			//delete the old animation asset before creating and saving the new one (will not error if the animation does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/SpinLoop" + animationName + "Single.anim");
			AssetDatabase.CreateAsset(clip, animationFolderPath + animationName + "/SpinLoop" + animationName + "Single.anim");

			//add the animation to the given animator
			AddClipsToController(clip, "spin_loop");
		}

		void GenerateSpinQuickstop()
		{
			//reset data
			clip = new AnimationClip();
			evts = new AnimationEvent[totalSymbols + 1];
			keyFrame = 0f;
			keyTime = 0f;

			//go through each symbol and set their animation keyframes and events
			for (int i = totalSymbols; i > 0; i--)
			{
				keys = new Keyframe[1];
				keys[0] = new Keyframe(0f, matrixRows * matrixColumns * symbolSize);
				curve = new AnimationCurve(keys);

				//add the animation curve to the current clip with the proper object name
				string objName = "Symbol" + (i - 1).ToString();
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);

				//on the last loop, set the events for the first and second elements in the array, otherwise, set the events as normal
				//the order of these events in the array is very important, since they are all placed on the same time (0)
				if (i == 1)
				{
					//if the user is implementing the NMG custom spin result provider, set the Use Results event to be first
					if (usingCustomSpinResultProvider)
					{
						evts[0] = new AnimationEvent();
						evts[0].intParameter = totalSymbols - i;
						evts[0].time = 0f;
						evts[0].functionName = "UseResults";

						evts[1] = new AnimationEvent();
						evts[1].intParameter = totalSymbols - i;
						evts[1].time = 0f;
						evts[1].functionName = "SetTopSymbol";
					}
					else
					{
						evts[1] = new AnimationEvent();
						evts[1].intParameter = totalSymbols - i;
						evts[1].time = 0f;
						evts[1].functionName = "UseResults";

						evts[0] = new AnimationEvent();
						evts[0].intParameter = totalSymbols - i;
						evts[0].time = 0f;
						evts[0].functionName = "SetTopSymbol";
					}
				}
				else
				{
					evts[i] = new AnimationEvent();
					evts[i].intParameter = totalSymbols - i;
					evts[i].time = 0f;
					evts[i].functionName = "SetTopSymbol";
				}
			}
			//create the keyframes for the parent 'Symbols' object
			keys = new Keyframe[2];

			//set the first and last keyframes
			keyFrame = 2f;
			keyTime = keyFrame / frameSamples;
			keys[0] = new Keyframe(keyTime, -1 * matrixRows * matrixColumns * symbolSize);
			keys[1] = new Keyframe(0.0f, -1 * matrixRows * matrixColumns * symbolSize);

			//create the animation curve with new keys
			curve = new AnimationCurve(keys);
			clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

			//attach the animation events to the animation clip
			AnimationUtility.SetAnimationEvents(clip, evts);

			//delete the old animation asset before creating and saving the new one (will not error if the animation does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/SpinQuickstop" + animationName + ".anim");
			AssetDatabase.CreateAsset(clip, animationFolderPath + animationName + "/SpinQuickstop" + animationName + ".anim");

			//add the animation to the given animator
			AddClipsToController(clip, "quick_stop");
		}

		void GenerateSpinStopAnticipation()
		{
			//reset data
			clip = new AnimationClip();
			evts = new AnimationEvent[(totalSymbols * 8) + 1];
			int elmt = 0;
			keyFrame = 0f;
			keyTime = 0f;

			for (int i = 0; i < totalSymbols; ++i)
			{
				keys = new Keyframe[9];
				//evts = new AnimationEvent[8];

				for (int j = 0; j < keys.Length; j++) // each keyframe
				{
					// keyframe spacing for anticipation
					if (j == 0)
					{
						keyFrame = 0;
					}
					else if (j == 1)
					{
						keyFrame += 3 + i;
					}
					else if (j == 2)
					{
						keyFrame += 2 + (totalSymbols - 1);
					}
					else if (j == 3)
					{
						keyFrame += 2 + (totalSymbols - 1);
					}
					else if (j == 4)
					{
						keyFrame += 2 + (totalSymbols - 1);
						keyFrame += i * 1; // ramp up frame frequency by 1x
					}
					else if (j == 5 || j == 6)
					{
						keyFrame += (totalSymbols - 1) * 2; // update starting frame, because of the frame frequency ramp up
						keyFrame += 3;
					}
					else if (j == 7)
					{
						keyFrame += (totalSymbols - 1) * 2; // update starting frame, because of the frame frequency ramp up
						keyFrame += 4;
						keyFrame += i * 1; // ramp up frame frequency by 1x
					}
					else if (j == 8)
					{
						keyFrame += (totalSymbols - 1) * 3; // update starting frame, because of the frame frequency ramp up
						keyFrame += 5;
						keyFrame += i * 1; // ramp up frame frequency by 1x
					}

					keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
					keyValue = reelSpinDistance * j;

					keys[j] = new Keyframe(keyTime, keyValue);
					//StaticLogForwarder.Logger.Log(keyFrame + " , " + keyValue);

					//on every loop except the first one
					if (j > 0)
					{
						//create the animation event with the proper function and parameter
						evts[elmt] = new AnimationEvent();
						evts[elmt].intParameter = (totalSymbols - 1) - i;
						evts[elmt].time = keyTime;
						evts[elmt].functionName = "SetTopSymbol";
						elmt++;
					}

					//on the last loop of the first symbol - create the UseResults event
					if (i == 0 && j == 8)
					{
						//if the user is implementing the NMG custom spin result provider, set the Use Results event to be before the Set Top Symbol
						if (usingCustomSpinResultProvider)
						{
							keyTime = (keyFrame - 1) / frameSamples;
						}
						else
						{
							keyTime = (keyFrame + 1) / frameSamples;
						}

						evts[elmt] = new AnimationEvent();
						evts[elmt].intParameter = (totalSymbols - 1) - i;
						evts[elmt].time = keyTime;
						evts[elmt].functionName = "UseResults";
						elmt++;
					}
				}
				//add the keys to the animation curve
				curve = new AnimationCurve(keys);

				// set all keyframe tangents to constant
				for (int k = 0; k < keys.Length; ++k)
				{
					AnimationUtility.SetKeyRightTangentMode(curve, k, AnimationUtility.TangentMode.Constant);
					AnimationUtility.SetKeyLeftTangentMode(curve, k, AnimationUtility.TangentMode.Constant);
				}

				//add the animation curve to the current clip with the proper object name
				string objName = "Symbol" + ((totalSymbols - 1) - i).ToString();
				clip.SetCurve(objName, typeof(Transform), "localPosition.y", curve);
			}

			//set the keyframes for the parent Symbol
			keyFrame += 30; //for the last keyframe
			keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
			curve.AddKey(new Keyframe(keyTime, keyValue)); //add the last keyframe

			keyFrame -= 23; //for the bounce before the last keyframe
			keyTime = keyFrame / frameSamples; //calculate the time the keyframe should be placed
			curve.AddKey(new Keyframe(keyTime, keyValue + (symbolSize / 2))); //add the bounce keyframe

			//make each key value in the curve negative for the parent Symbol
			keys = curve.keys;
			for (int k = 0; k < curve.keys.Length; k++)
			{
				float v = keys[k].value;
				v *= -1;
				keys[k].value = v;
			}
			curve.keys = keys;

			//set the keyframe tangents to Auto
			for (int k = 0; k < curve.keys.Length; k++)
			{
				AnimationUtility.SetKeyRightTangentMode(curve, k, AnimationUtility.TangentMode.Auto);
				AnimationUtility.SetKeyLeftTangentMode(curve, k, AnimationUtility.TangentMode.Auto);
			}

			//attach the animation events to the animation clip
			AnimationUtility.SetAnimationEvents(clip, evts);

			//add the parent Symbol animation curve to the clip
			clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

			//StaticLogForwarder.Logger.Log("end");
			//delete the old animation asset before creating and saving the new one (will not error if the animation does not already exist)
			AssetDatabase.DeleteAsset(animationFolderPath + animationName + "/SpinStopAnticipation" + animationName + ".anim");
			AssetDatabase.CreateAsset(clip, animationFolderPath + animationName + "/SpinStopAnticipation" + animationName + ".anim");

			//add the animation to the given animator
			AddClipsToController(clip, "spin_stop_anticipation");
		}
	}
}
#endif
