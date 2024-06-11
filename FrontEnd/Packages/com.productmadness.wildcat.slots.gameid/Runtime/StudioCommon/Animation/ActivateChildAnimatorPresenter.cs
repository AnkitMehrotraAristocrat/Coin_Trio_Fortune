using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A state presenter that supports setting a trigger, optionally resetting all triggers and optionally
    /// yielding on the animator state machine.
    /// </summary>
    public class ActivateChildAnimatorPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        public enum EnterOrExit
        {
            Enter,
            Exit
        }

        [FieldRequiresChild] private Animator _animator;

        [SerializeField] private EnterOrExit _onStateMachineState;
        [FormerlySerializedAs("_trigger")]
        [SerializeField] private string _animTrigger;

        [SerializeField] private EnterOrExit _waitForAnimatorState;
        [SerializeField] private string _waitForTag;
        [SerializeField] private string _waitForStateName;
        [SerializeField] private int _stateNameLayerIndex = 0;

        [SerializeField] private bool _resetTriggers;

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public IEnumerator<Yield> Enter()
        {
            if (_onStateMachineState != EnterOrExit.Enter)
            {
                yield break;
            }

            yield return Coroutine.Start(InstructAnimator());
        }

        public IEnumerator<Yield> Exit()
        {
            if (_onStateMachineState != EnterOrExit.Exit)
            {
                yield break;
            }

            yield return Coroutine.Start(InstructAnimator());
        }

        private IEnumerator<Yield> InstructAnimator()
        {
            if (_resetTriggers)
            {
                _animator.ResetTriggers();
            }

            _animator.SetTrigger(_animTrigger);

            if (string.IsNullOrEmpty(_waitForTag))
            {
                yield break;
            }

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_waitForStateName))
            {
                yield break;
            }

            switch (_waitForAnimatorState)
            {
                case EnterOrExit.Enter:
                    yield return _animator.WhenStateEnter(_waitForTag);
                    break;
                case EnterOrExit.Exit:
                    yield return _animator.WhenStateExit(_waitForTag);
                    break;
                default:
                    yield break;
            }
        }
    }
}
