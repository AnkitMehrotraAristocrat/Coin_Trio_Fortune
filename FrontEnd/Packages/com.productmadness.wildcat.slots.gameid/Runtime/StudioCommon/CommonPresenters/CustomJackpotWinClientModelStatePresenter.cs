using Malee;
using Milan.FrontEnd.Slots.v5_1_1.Jackpots;
using System;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class CustomJackpotWinClientModelStatePresenter : BaseClientModelStateOptionalPresenter<JackpotWinsServerModel, JackpotWinsClientModel>
	{
		[SerializeField] private string _balanceKey = "chips";

		[Serializable]
		public class JackpotTriggerSequenceList : ReorderableArray<JackpotConfigurationData> { }
		[SerializeField, Reorderable]
		private JackpotTriggerSequenceList _sequenceList;

        protected override void SetResult()
        {
			// Propogate each win from the server to the client model one at a time. We do this by looking for
			//  matching wins in the order provided by the sequence list. This allows for customizing the order of
			//   jackpot wins presented.

			bool foundWin = false;
			foreach (var jackpot in _sequenceList)
			{
				var jackpotValueData = jackpot.GetJackpotData();
				var win = _serverModel.Wins.Value.FirstOrDefault(w => w.JackpotValue == jackpotValueData);
				if (win != default && win.Amount > 0)
				{
					_clientModel.WinningJackpotValue.Value = jackpotValueData;
                    _clientModel.WinAmount.Value = win.Amount;
                    _clientModel.HasWon.Value = true;

					// Prevent this win from triggering again.
					win.Amount = 0;
					foundWin = true;
					break;
				}
			}

			if (!foundWin)
			{
                _clientModel.WinningJackpotValue.Value = null;
                _clientModel.WinAmount.Value = 0;
                _clientModel.HasWon.Value = false;
			}
        }
	}
}
