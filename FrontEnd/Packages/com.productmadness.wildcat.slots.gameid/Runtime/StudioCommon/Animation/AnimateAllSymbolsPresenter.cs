using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A state presenter that instructs any spawned symbol found in the pairs scriptable object
    /// to reset all triggers and set the prescribed trigger.
    /// 
    /// Use cases: Idle symbols on spin start, dim symbols on transition, etc.
    /// </summary>
    public class AnimateAllSymbolsPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        [FieldRequiresChild] private ISymbolView[] _symbolViews;

        [SerializeField] SymbolAnimationPairSO _symbolAnimPairs;


        public string Tag => this.GetTag();

        public INotifier Notifier
        {
            get; set;
        }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public IEnumerator<Yield> Enter()
        {
            SetSymbolAnimations();
            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

        public void SetSymbolAnimations()
        {
            foreach (var symbol in _symbolViews)
            {
                SymbolAnimPair symbolAnimPair = _symbolAnimPairs.SymbolAnimationPairs.FirstOrDefault(pair => pair.symbolId.Equals(symbol.Instance.id.Value));
                if (symbolAnimPair == null)
                {
                    continue;
                }

                Animator animator = symbol.Instance.GetComponent<Animator>();
                animator.ResetTriggers();
                animator.SetTrigger(symbolAnimPair.animTrigger);
                animator.Update(0.1f); // TODO: If this works, change the magic number
            }
        }
    }
}
