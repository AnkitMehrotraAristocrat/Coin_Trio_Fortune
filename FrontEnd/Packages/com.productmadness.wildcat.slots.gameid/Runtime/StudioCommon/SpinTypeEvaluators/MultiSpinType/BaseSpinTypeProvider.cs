using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public abstract class BaseSpinTypeProvider : MonoBehaviour, ServiceLocator.IHandler
	{
		[FieldRequiresModel] protected GameStateModel _gameStateModel = default;

		[FieldRequiresGlobal] protected ServiceLocator _serviceLocator;

		[SerializeField] protected List<GameStateEnum> _allowedGameStates;

		public abstract string GetSpinType(MainDriver.IPayloadReader payloadReader = null);

		public virtual void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}
	}
}
