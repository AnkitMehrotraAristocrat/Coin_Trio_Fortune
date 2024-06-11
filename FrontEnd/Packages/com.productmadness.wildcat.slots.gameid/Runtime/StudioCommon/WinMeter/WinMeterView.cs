using Animator = UnityEngine.Animator;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Sag;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class WinMeterView : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresModel] private WinMeterModel _winMeterModel;
        [SerializeField] private TextMeshPro _winText;
        [FieldRequiresChild(optional = true)] private Animator _animator;

        [SerializeField][Tooltip("Optional")] private string _hitAnimTrigger;
        [SerializeField][Tooltip("Optional")] private string _showAnimTrigger;
        [SerializeField][Tooltip("Optional")] private string _hideAnimTrigger;
        [SerializeField][Tooltip("Optional")] private string _hiddenIdleStateTag;
        [SerializeField][Tooltip("Optional")] private string _visibleIdleStateTag;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            _winMeterModel.WinAmount.Subscribe(amount => UpdateText(amount)).AddTo(this);
        }

        private void UpdateText(long winAmount)
        {          
            if (winAmount > 0)
			{
                PlayHitAnim();
                _winText.text = NumberFormat.CommaDelimited(winAmount);
            }
			else
			{
                _winText.text = "";
            } 
        }

        private void PlayHitAnim()
        {
            if (_animator == null || string.IsNullOrEmpty(_hitAnimTrigger))
			{
                return;
			}

            _animator.SetTrigger(_hitAnimTrigger);
        }

        public IEnumerator<Yield> PlayShowAnim()
        {
            if (_animator == null || string.IsNullOrEmpty(_showAnimTrigger))
            {
                yield break;
            }
            _animator.SetTrigger(_showAnimTrigger);
            yield return _animator.WhenStateEnter(_visibleIdleStateTag);
        }

        public IEnumerator<Yield> PlayHideAnim()
        {
            if (_animator == null || string.IsNullOrEmpty(_hideAnimTrigger))
            {
                yield break;
            }
            _animator.SetTrigger(_hideAnimTrigger);
            yield return _animator.WhenStateEnter(_hiddenIdleStateTag);
        }
    }
}
