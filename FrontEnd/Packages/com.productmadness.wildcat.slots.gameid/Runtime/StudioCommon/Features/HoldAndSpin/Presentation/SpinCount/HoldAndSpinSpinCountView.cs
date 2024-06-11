using Milan.FrontEnd.Core.v5_1_1;
using TMPro;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinSpinCountView : MonoBehaviour, ServiceLocator.IHandler
    {  
        [FieldRequiresChild(optional = true)] protected TextMeshPro _spinCountText;
        [FieldRequiresChild(optional = true)] protected Animator _animator;

        [SerializeField] protected string _spinCountClientModelTag;
        [SerializeField] protected int _startingCount = 3;
        [SerializeField] protected string _resetAnimTrigger;

        [SerializeField] protected string _visibleStateTag;
        [SerializeField] protected string _lastSpinAnimTrigger;
        [SerializeField] protected string _decrementAnimTrigger;

        protected HoldAndSpinSpinCountClientModel _spinCountClientModel;
        protected int _currentCount;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();

            ServiceLocator serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();

            _spinCountClientModel = serviceLocator.GetOrCreate<HoldAndSpinSpinCountClientModel>(_spinCountClientModelTag);

            _spinCountClientModel.Count.Subscribe(spinCount => UpdateText(spinCount)).AddTo(this);
            _currentCount = _startingCount;
        }

        protected virtual void UpdateText(int count)
        {
            if (_spinCountText == null)
            {
                return;
            }

            if (_currentCount < count)
            {
                PlayResetAnim();
            }
            else
            {
                PlayDecrementAnim();
            }

            _spinCountText.text = count.ToString();
            _currentCount = count;
            UpdateSpinsText(count);
        }

        protected void UpdateSpinsText(int count)
        {
            if(count == 1)
            {
                _animator.SetTrigger(_lastSpinAnimTrigger);
            }
        }      

        protected void PlayResetAnim()
        {
            if (_animator == null || string.IsNullOrEmpty(_resetAnimTrigger))
			{
                return;
			}

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag(_visibleStateTag))
            {
                return;
            }

            _animator.SetTrigger(_resetAnimTrigger);
        }

        protected void PlayDecrementAnim()
        {
            if (_animator == null || string.IsNullOrEmpty(_decrementAnimTrigger))
            {
                return;
            }

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag(_visibleStateTag))
            {
                return;
            }

            _animator.SetTrigger(_decrementAnimTrigger);
        }
    }
}
