using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.FreespinCore;
using System;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID {
	/// <summary>
	/// A state machine node that determines if free spins still remain.
	/// If not, provides two exit states to direct state machine flow.
	/// Ex.: Show a 'Bonus Complete' banner or a banner that also has the win amount.
	/// </summary>
	[StateTransitions("FreespinsRemaining", "ShowComplete", "ShowResults")]
	public class FreeGamesRemainingState : BaseState {
		public static Type SupportedPresenterInterface = typeof(IStatePresenter);

		protected readonly FreeSpinServerModel _freeSpinServerModel;

		public FreeGamesRemainingState(ServiceLocator serviceLocator, StateMachine stateMachine, List<BaseNode> parentNodes, string name, StateMetaData presenters)
			: base(serviceLocator, stateMachine, parentNodes, name, presenters) {
			_freeSpinServerModel = serviceLocator.GetOrCreate<FreeSpinServerModel>();
		}

		protected override IEnumerator<Yield> OnEnter() {
			bool hasFreeSpins = _freeSpinServerModel.SpinsRemaining.Value > 0 || _freeSpinServerModel.SpinsWon.Value > 0;
			if (hasFreeSpins) {
				Notify("FreespinsRemaining");
			}

			yield return new WhenAll(Run());
		}
	}
}
