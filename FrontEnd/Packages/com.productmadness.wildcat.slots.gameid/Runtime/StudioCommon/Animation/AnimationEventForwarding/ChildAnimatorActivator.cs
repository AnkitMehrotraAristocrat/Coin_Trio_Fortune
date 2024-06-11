using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixelUnited.NMG.Slots.Milan.GAMEID {
	/// <summary>
	/// A spinoff of the ActivateChildAnimatorPresenter that removes the Coroutine implementation
	/// and simply sets the defined trigger.
	/// Intended to be used in conjunction with the AnimationEventForwarder.
	/// </summary>
	public class ChildAnimatorActivator : MonoBehaviour, ServiceLocator.IHandler {
		public string Tag => this.GetTag();

		[FormerlySerializedAs("_trigger")]
		[SerializeField]
		private string _animTrigger;

		[FieldRequiresChild] private Animator _animator;

		public void OnServicesLoaded() {
			this.InitializeDependencies();
		}

		public void SetTrigger() {
			_animator.SetTrigger(_animTrigger);
		}

	}
}
