using System;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public abstract class BaseWizardExecutor
	{
		protected bool _canRerun = false;
		public bool CanRerun => _canRerun;


		public virtual void Execute(WizardInputData data)
		{
			// does nothing by default
		}
	}
}
