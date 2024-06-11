using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CreateAssetMenu(fileName = "WizardState", menuName = "NMG/Wizard/State")]
	public class WizardState : ScriptableObject
	{
		[SerializeField] private bool _hasExecuted;
		public bool HasExecuted => _hasExecuted;

		[SerializeField] private List<ExecutorState> _executorStates = new List<ExecutorState>();
		public IReadOnlyList<ExecutorState> ExecutorStates => _executorStates;

		[SerializeField] private List<ReelWindowGenerationState> _reelWindowGenerationStates = new List<ReelWindowGenerationState>();
		public IReadOnlyList<ReelWindowGenerationState> ReelWindowGenerationStates => _reelWindowGenerationStates;

		[SerializeField] private List<MechanicState> _mechanicStates = new List<MechanicState>();
		public IReadOnlyList<MechanicState> MechanicStates => _mechanicStates;

		public void Reset()
		{
			FrontEndWizardHelper.GetApplicableMechanicConfigurations();
		}

		public void Save()
		{
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssetIfDirty(this);
		}

		public void SetHasExecuted()
		{
			_hasExecuted = true;
		}

		public void AddExecutorState(string name)
		{
			ExecutorState existingEntry = _executorStates.FirstOrDefault(entry => entry.Name.Equals(name));
			if (existingEntry == null)
			{
				_executorStates.Add(new ExecutorState(name, false));
			}
		}

		public void SetExecutorState(string name, bool hasExecuted)
		{
			ExecutorState existingEntry = _executorStates.FirstOrDefault(entry => entry.Name.Equals(name));
			if (existingEntry == null)
			{
				return;
			}

			existingEntry.HasExecuted = hasExecuted;
		}

		public void AddReelWindowState(string reelWindowName)
		{
			ReelWindowGenerationState existingEntry = _reelWindowGenerationStates.FirstOrDefault(entry => entry.ReelWindowName.Equals(reelWindowName));
			if (existingEntry != null)
			{
				return;
			}

			_reelWindowGenerationStates.Add(new ReelWindowGenerationState(reelWindowName, false));
		}

		public List<string> FetchPendingReelWindowNames()
		{
			return _reelWindowGenerationStates.Where(entry => !entry.HasGenerated).Select(entry => entry.ReelWindowName).ToList();
		}

		public List<ReelWindowGenerationState> FetchPendingReelWindowGenerationStates()
		{
			return _reelWindowGenerationStates.Where(entry => !entry.HasGenerated).ToList();
		}

		public void SetReelWindowRoot(string name, string rootPath)
		{
			ReelWindowGenerationState reelWindowGenerationState = _reelWindowGenerationStates.FirstOrDefault(entry => entry.ReelWindowName.Equals(name));
			if (reelWindowGenerationState == null)
			{
				Debug.LogError(GetType() + ": ReelWindowGenerationState with a ReelWindowName of " + name + " does not exit!");
			}

			reelWindowGenerationState.RootPath = rootPath;
		}

		public void SetReelWindowGenerationState(string name, bool hasGenerated)
		{
			ReelWindowGenerationState reelWindowGenerationState = _reelWindowGenerationStates.FirstOrDefault(entry => entry.ReelWindowName.Equals(name));
			if (reelWindowGenerationState == null)
			{
				Debug.LogError(GetType() + ": ReelWindowGenerationState with a ReelWindowName of " + name + " does not exit!");
			}

			reelWindowGenerationState.HasGenerated = hasGenerated;
		}

		public void AddMechanicState(string name)
		{
			MechanicState existingEntry = _mechanicStates.FirstOrDefault(entry => entry.Name.Equals(name));
			if (existingEntry == null)
			{
				_mechanicStates.Add(new MechanicState(name));
			}
		}

		public void SetMechanicState(string name, MechanicStage stage, MechanicStatus status)
		{
			MechanicState existingEntry = _mechanicStates.FirstOrDefault(entry => entry.Name.Equals(name));
			if (existingEntry == null)
			{
				return;
			}

			switch (stage)
			{
				case MechanicStage.SubGraphs:
					existingEntry.SubGraphStatus = status;
					break;
				case MechanicStage.Triggers:
					existingEntry.TriggerStatus = status;
					break;
				case MechanicStage.SceneElements:
					existingEntry.SceneElementStatus = status;
					break;
				default:
					break;
			}
		}

		public MechanicState GetMechanicState(string name)
		{
			return _mechanicStates.FirstOrDefault(entry => entry.Name.Equals(name));
		}
	}

	[Serializable]
	public class ExecutorState
	{
		[SerializeField] public string Name;
		[SerializeField] public bool HasExecuted;

		public ExecutorState(string name, bool hasExecuted)
		{
			Name = name;
			HasExecuted = hasExecuted;
		}
	}

	[Serializable]
	public class ReelWindowGenerationState
	{
		[SerializeField] public string ReelWindowName;
		[SerializeField] public bool HasGenerated;
		[SerializeField] public string RootPath = default;

		public ReelWindowGenerationState(string reelWindowName, bool hasGenerated)
		{
			ReelWindowName = reelWindowName;
			HasGenerated = hasGenerated;
		}
	}

	[Serializable]
	public class MechanicState
	{
		[SerializeField] public string Name;
		[SerializeField] public MechanicStatus SubGraphStatus;
		[SerializeField] public MechanicStatus TriggerStatus;
		[SerializeField] public MechanicStatus SceneElementStatus;

		public MechanicState(string name)
		{
			Name = name;
			SubGraphStatus = MechanicStatus.Pending;
			TriggerStatus = MechanicStatus.Pending;
			SceneElementStatus = MechanicStatus.Pending;
		}
	}

	public enum MechanicStage
	{
		SubGraphs,
		Triggers,
		SceneElements
	}

	public enum MechanicStatus
	{
		Pending,
		Attempted,
		Completed
	}
}
