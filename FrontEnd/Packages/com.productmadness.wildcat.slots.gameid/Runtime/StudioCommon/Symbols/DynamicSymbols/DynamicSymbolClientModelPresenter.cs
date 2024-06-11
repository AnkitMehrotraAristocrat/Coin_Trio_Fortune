using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
	/// <summary>
	/// Presenter used to set the active dynamic symbols on the client model.
	/// </summary>
	public class DynamicSymbolClientModelPresenter : MonoBehaviour, IPresenter, ServiceLocator.IHandler, ServiceLocator.IService
	{
		[FieldRequiresModel] private DynamicSymbolClientModel _clientModel = null;

		private Dictionary<string, IDisposable> _serverSubscriptions = new Dictionary<string, IDisposable>();

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public void AddServerModelSubscription(string tag, DynamicSymbolServerModel serverModel)
		{
			if (_serverSubscriptions.ContainsKey(tag))
			{
				return;
			}
			IDisposable subscription = serverModel.DynamicSymbolReplacements.Subscribe(symbols => _clientModel.SetDynamicSymbolReplacements(symbols)).AddTo(this);
			_serverSubscriptions.Add(tag, subscription);
		}

		public void RemoveServerModelSubscription(string tag)
		{
			_serverSubscriptions[tag].Dispose();
			_serverSubscriptions.Remove(tag);
		}
	}
}
