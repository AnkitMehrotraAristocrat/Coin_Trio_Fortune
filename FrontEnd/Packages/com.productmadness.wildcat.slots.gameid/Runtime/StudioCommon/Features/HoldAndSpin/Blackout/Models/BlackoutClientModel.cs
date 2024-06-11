using Milan.FrontEnd.Core.v5_1_1;
using System;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutClientModel : BaseBlackoutModel
    {
		private IEnumerator<PrizeInfo> _allPrizes = default;

		public PrizeInfo CurrentPrize { get; private set; } = null;

        public BlackoutClientModel(ServiceLocator serviceLocator)
        {

        }

		private void SetCurrentPrize(PrizeInfo prize)
        {
            CurrentPrize = prize;
        }

		public void SetNextPrize()
		{
			if (_allPrizes == null)
			{
				_allPrizes = Data.Prizes.GetEnumerator();
			}

			if (_allPrizes.MoveNext())
			{
				SetCurrentPrize(_allPrizes.Current);
			}
			else
			{
				CurrentPrize = null;
				_allPrizes.Dispose();
				_allPrizes = null;
			}
		}

		public bool IsCurrentPrizeTypeJackpot()
		{
			switch (CurrentPrize?.Type)
			{
				default:
					return false;
				case CustomPrizeTypes.JackpotPrizeType:
					return true;
			}
		}

		public bool IsCurrentPrizeTypeMultiplier()
		{
			return CurrentPrize?.Type.Equals(CustomPrizeTypes.CreditCorType, StringComparison.OrdinalIgnoreCase) ?? false;
		}
	}
}
