using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Jackpots;
using Milan.Shared.DTO.Jackpot;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
	public class HoldAndSpinJackpotWinClientModelStatePresenter : BaseClientModelStateOptionalPresenter<JackpotWinsServerModel, JackpotWinsClientModel>
	{
		[FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel;

		[SerializeField] private string _jackpotLabel = "jackpot";
		[Tooltip("The label applied in the game backend, so far, we only have 1 of these.")]
		[SerializeField] private string _jackpotId = "main";

		private void ResetClientModel()
		{
			_clientModel.WinningJackpotValue.Value = null;
            _clientModel.WinAmount.Value = 0;
            _clientModel.HasWon.Value = false;
		}

        protected override void SetResult()
        {
            // Reset the jackpot wins client model via ResetClientModel()
            ResetClientModel();

            // Take a peek at the upcoming prize to celebrate.
            var currentPrize = _symbolOutcomeModel.CurrentPrize;

            // Leave immediately if this isn't a jackpot.
            if (currentPrize == null
                || currentPrize.SymbolData.Skin == CustomPrizeTypes.CreditCorType)
            {
                return;
            }

            // Create a new JackpotDefinition
            JackpotValueData jackpotDefinition = new JackpotValueData
            {
                JackpotId = _jackpotId,
                TierPosition = currentPrize.Tier,
                Bracket = null
            };

            // Fetch the win from the _jackpotWinsServerModel where the definition matches and the win amount is non-zero
            var win = _serverModel.Wins.Value.Where(w =>
            {
                bool definitionMatches = w.JackpotValue == jackpotDefinition;
                bool hasWinAmount = w.Amount > 0;
                return definitionMatches && hasWinAmount;
            }).Aggregate((w1, w2) => w1.Amount > w2.Amount ? w1 : w2);

            if (win == null)
            {
                GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Winning jackpot definition not found: " + currentPrize.SymbolData.Skin, this);
                return;
            }

            _clientModel.WinningJackpotValue.Value = jackpotDefinition;
            _clientModel.WinAmount.Value = win.Amount;
            _clientModel.HasWon.Value = true;

            // Set the server model's entry win amount to zero (so it doesn't hit again)
            win.Amount = 0;
        }
    }
}
