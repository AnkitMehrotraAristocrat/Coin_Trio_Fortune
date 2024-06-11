using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public abstract class BaseBlackoutPrizePresenter : BaseStatePresenter
    {
		[FieldRequiresModel] private BlackoutClientModel _blackoutModel = default;

		public override IEnumerator<Yield> Enter()
		{
			if (_action.Equals(Action.OnEnter) 
				&& _blackoutModel.CurrentPrize != null)
			{
				yield return Coroutine.Start(PresentCurrentPrize(_blackoutModel.CurrentPrize));
			}
			yield break;
		}

        public override IEnumerator<Yield> Exit()
		{
			if (_action.Equals(Action.OnExit) 
				&& _blackoutModel.CurrentPrize != null)
			{
				yield return Coroutine.Start(PresentCurrentPrize(_blackoutModel.CurrentPrize));
			}
			yield break;
		}

		protected abstract IEnumerator<Yield> PresentCurrentPrize(PrizeInfo currentPrize);
    }
}
