using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Meta;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public abstract class BaseClientModelStateOptionalPresenter<TServerModel, TClientModel> : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler, IRecoveryHandler
	{
		public enum Action
		{
			OnEnter,
			OnExit,
		}

		[FieldRequiresModel] protected IBetModel _betModel;

		[FieldRequiresGlobal] protected ServiceLocator _serviceLocator = null;

		[SerializeField] private string _serverTag = "";
		[SerializeField] private string _clientTag = "";
		[SerializeField] private Action _action = Action.OnEnter;
		
		protected TServerModel _serverModel;
		protected TClientModel _clientModel;

		public string Tag => this.GetTag();

		public INotifier Notifier { private get; set; }

		public virtual void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public virtual void OnServerConfigsReady(ServiceLocator locator)
		{
			// does nothing
		}

		public virtual void OnInitialStateReady(ServiceLocator locator)
		{
			_serverModel = _serviceLocator.GetOrCreate<TServerModel>(_serverTag);
			_clientModel = _serviceLocator.GetOrCreate<TClientModel>(_clientTag);
		}

		public IEnumerator<Yield> Enter()
		{
			if (_action == Action.OnEnter)
			{
				UpdateClientModel();
			}
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			if (_action == Action.OnExit)
			{
				UpdateClientModel();
			}
			yield break;
		}

		public void UpdateClientModel() // intended to support animation event invocation via the AnimationEventForwarder
		{
			SetResult();
		}

		protected abstract void SetResult();
	}
}
