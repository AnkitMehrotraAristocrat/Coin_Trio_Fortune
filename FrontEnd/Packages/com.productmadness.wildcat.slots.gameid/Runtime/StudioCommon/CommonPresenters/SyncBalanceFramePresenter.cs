using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SyncBalanceFramePresenter : BaseStatePresenter
	{
		[FieldRequiresModel] private BalanceServerModel _balanceServerModel;
		[FieldRequiresModel] private BalanceClientModel _balanceClientModel;

		[FieldRequiresChild] private RoundWinAmountProvider _roundWinAmountProvider;

		[SerializeField] private string _currencyType = "chips";

		protected override void Execute()
		{
			_balanceServerModel.Amounts[_currencyType] += _roundWinAmountProvider.WinAmount;
			_balanceClientModel.Amounts[_currencyType] = _balanceServerModel.Amounts[_currencyType];
		}
	}
}
