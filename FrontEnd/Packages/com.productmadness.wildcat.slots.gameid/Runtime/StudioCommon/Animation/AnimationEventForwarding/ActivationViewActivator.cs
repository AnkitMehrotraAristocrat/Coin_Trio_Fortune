using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Utility;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID {
	/// <summary>
	/// A spinoff of the ActivationPresenter that removes the Coroutine implementation but 
	/// instead simply activates/deactivates ActivationViews instantly.
	/// </summary>
	public class ActivationViewActivator : MonoBehaviour, ServiceLocator.IHandler {
		[FieldRequiresChild(includeInactive = true)]
		private ActivationView[] _activationViews;

		public enum ActivationAction {
			Nothing,
			Activate,
			Deactivate
		}

		[SerializeField] private ActivationAction _activationAction;

		public void OnServicesLoaded() {
			this.InitializeDependencies();
		}

		public void SetViewActiveState() {
			if (_activationAction == ActivationAction.Nothing) {
				GameIdLogger.Logger.Error(GetType() + " (" + Tag + ") :: " + "Activation Action is nothing!", this);
				return;
			}

			foreach (var view in _activationViews) {
				if (_activationAction == ActivationAction.Activate) {
					view.Activate();
				} else {
					view.Deactivate();
				}
			}
		}

		public string Tag => this.GetTag();

	}
}
