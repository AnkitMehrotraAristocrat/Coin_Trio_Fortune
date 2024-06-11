using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolChaser
{
	/// <summary>
	/// Base class for symbol chaser trigger modifier scriptable objects to implement.
	/// Ex. EquivalentGrowingReelsSymbolChaserTriggerModifier
	/// </summary>
	public abstract class BaseSymbolChaserTriggerModifierSO : ScriptableObject
	{
		public abstract void Initialize(ServiceLocator serviceLocator);
		public abstract void ModifyTrigger(ref string trigger);
	}
}
