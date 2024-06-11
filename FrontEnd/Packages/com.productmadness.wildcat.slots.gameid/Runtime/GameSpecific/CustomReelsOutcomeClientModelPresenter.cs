using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class CustomReelsOutcomeClientModelPresenter : MonoBehaviour, IPresenter, ServiceLocator.IHandler,
		ServiceLocator.IService
	{
		public string Tag => this.GetTag();

		private ReelsOutcomeClientModel _clientModel;

		private Dictionary<string, IDisposable> _serverSubscriptions = new Dictionary<string, IDisposable>();

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			var serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();
			_clientModel = serviceLocator.GetOrCreate<ReelsOutcomeClientModel>(Tag);
		}

		/// <summary>
		/// Register for server model changes
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="serverModel"></param>
		public void AddServerModelSubscription(string tag, ReelsOutcomeServerModel serverModel)
		{
			if (_serverSubscriptions.ContainsKey(tag))
			{
				return;
			}

			IDisposable subscription = serverModel.ModelUpdate.Subscribe(serverData =>
			{
				_clientModel.UpdateModel(
					serverData.Id.Value,
					serverData.Offsets.Value,
					serverData.ReelWindowId.Value,
					serverData.ReelStripIds.Value,
					serverData.Symbols);
			}).AddTo(this);

			_serverSubscriptions.Add(tag, subscription);
		}

		/// <summary>
		/// Remove specific subscription from model
		/// </summary>
		/// <param name="tag"></param>
		public void RemoveServerModelSubscription(string tag)
		{
			_serverSubscriptions[tag].Dispose();
			_serverSubscriptions.Remove(tag);
		}

		public void OnDestroy()
		{
			foreach (var kvp in _serverSubscriptions)
			{
				kvp.Value.Dispose();
			}
		}
	}
}
