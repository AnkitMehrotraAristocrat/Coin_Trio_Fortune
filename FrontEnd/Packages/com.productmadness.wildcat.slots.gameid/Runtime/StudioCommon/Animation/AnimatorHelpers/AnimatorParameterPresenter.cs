#region Using

using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Presenter to handle setting animator parameters in a more dynamic way.
    /// Allows for any type of parameter to be set (or toggled).
    /// Allows for flexibility of when to execute, either on Enter or on Exit.
    /// </summary>
    public class AnimatorParameterPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        #region Enter/Exit Enum

        public enum WaitSchema
        {
            None,
            WaitForTag,
            WaitForAnimationComplete,
            Delay,
        }

        #endregion

        #region Inspector

        /// <summary>
        /// Animator controller to modify parameters on.
        /// </summary>
        public Animator Animator;

        /// <summary>
        /// List of animation parameter data.
        /// These parameters and their data will be sent to the animator in question at the right timing.
        /// </summary>
        [HideInInspector]
        public List<AnimatorParameterData> AnimatorParameters;

        /// <summary>
        /// The timing of when to update the parameters on the animator.
        /// </summary>
        public StateExecutionTime ExecuteOn;

        /// <summary>
        /// What type of waiting are we doing?
        /// </summary>
        [Tooltip("Wait to advance the state? If so, wait for tag, or wait for animation to finish? If delay is chosen, will only advance the state's Enter or Exit after the delay amount.")]
        public WaitSchema WaitType;

        /// <summary>
        /// We will sit and wait for a specific tag event fired from the TaggedObservableStateMachineTrigger on an animator state.
        /// </summary>
        [HideInInspector]
        public string WaitForTag = string.Empty;

        /// <summary>
        /// Waits for the specified delay amount if the wait schema is delay based.
        /// </summary>
        [HideInInspector]
        public float WaitForDelay = 0.0f;

        /// <summary>
        /// Will reset all triggers before applying any new parameters when called.
        /// </summary>
        [Tooltip("If true, will reset all triggers before applying any new parameters when called.")]
        public bool ResetTriggers = false;

        /// <summary>
        /// How long to wait before setting the parameters on the animator.
        /// </summary>
        [Tooltip("How long to wait before setting the parameters on the animator.")]
        public float DelayBeforeTriggerTime = 0.0f;

        #endregion

        #region Interface Implementations

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        #endregion

        #region State Handling

        public IEnumerator<Yield> Enter()
        {
            // Only execute if Enter or Both was selected.
            if (ExecuteOn == StateExecutionTime.Enter || ExecuteOn == StateExecutionTime.Both)
            {
                if (ResetTriggers)
                {
                    ResetAllTriggers();
                }

                yield return new YieldForSeconds(DelayBeforeTriggerTime);

                // Set all animator parameters.
                SetAnimatorParameters();

                yield return Coroutine.Start(HandleWaitSchema());

                if (WaitForDelay > 0.0f)
                {
                    yield return new YieldForSeconds(WaitForDelay);
                }
            }

            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            // Only execute if Enter or Both was selected.
            if (ExecuteOn == StateExecutionTime.Exit || ExecuteOn == StateExecutionTime.Both)
            {
                if (ResetTriggers)
                {
                    ResetAllTriggers();
                }

                yield return new YieldForSeconds(DelayBeforeTriggerTime);

                // Set all animator parameters.
                SetAnimatorParameters();

                yield return Coroutine.Start(HandleWaitSchema());

                if (WaitForDelay > 0.0f)
                {
                    yield return new YieldForSeconds(WaitForDelay);
                }
            }

            yield break;
        }

        #endregion

        #region Animator Parameter Setting Methods

        /// <summary>
        /// Handles the current wait schema.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Yield> HandleWaitSchema()
        {
            switch (WaitType)
            {
                case WaitSchema.None:
                    yield break;
                case WaitSchema.WaitForTag:
                    // Leave if we don't have a tag to wait for.
                    if (string.IsNullOrEmpty(WaitForTag))
                        yield break;
                    // Otherwise wait until the animator has hit the tag in question.
                    yield return Animator.WhenStateEnter(WaitForTag);
                    break;
                case WaitSchema.WaitForAnimationComplete:
                    // Wait a frame so the animator can update.
                    yield return new YieldForSeconds(0.1f);
                    // Get the length of the currently playing animation clip and wait for that long.
                    // Multiplying by 2 as it seems to only return half the length of the clip for some reason...
                    var waitTime = Animator.GetCurrentAnimatorClipInfo(0).First().clip.length;
                    yield return new YieldForSeconds(waitTime);
                    break;
            }
        }

        /// <summary>
        /// Iterates through all parameters in the AnimatorParameters data list and sets the parameters on the animator.
        /// </summary>
        public virtual void SetAnimatorParameters()
        {
            foreach (var parameter in AnimatorParameters)
            {
                switch (parameter.ParameterType)
                {
                    case AnimatorControllerParameterType.Trigger:
                        Animator.ResetTrigger(parameter.ParameterName);
                        Animator.SetTrigger(parameter.ParameterName);
                        break;
                    case AnimatorControllerParameterType.Float:
                        Animator.SetFloat(parameter.ParameterName, parameter.FloatValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        Animator.SetBool(parameter.ParameterName, parameter.BoolValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        Animator.SetInteger(parameter.ParameterName, parameter.IntValue);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Resets all triggers on the animator.
        /// </summary>
        protected virtual void ResetAllTriggers()
        {
            Animator.ResetTriggers();
        }

        #endregion

    }
}
