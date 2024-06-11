using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Meta;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public abstract class BaseStatePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        public enum Action
        {
            OnEnter,
            OnExit,
        }

        [FieldRequiresModel] protected IBetModel _betModel;

        [SerializeField] protected Action _action = Action.OnEnter;

        public string Tag => this.GetTag();

        public INotifier Notifier { get; set; }

        public virtual void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public virtual IEnumerator<Yield> Enter()
        {
            if (_action.Equals(Action.OnEnter))
			{
                Execute();
			}
            yield break;
        }

        public virtual IEnumerator<Yield> Exit()
        {
            if (_action.Equals(Action.OnExit))
            {
                Execute();
            }
            yield break;
        }

        protected virtual void Execute()
		{
            // does nothing by default
		}
    }
}
