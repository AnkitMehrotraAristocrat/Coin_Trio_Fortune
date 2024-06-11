using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class IncrementWinClientPresenter : BaseWinPresenter
    {
        [Serializable]
        private enum FireTiming
        {
            FireOnComplete = 0,
            FireOnStart = 1,
        }

        [FieldRequiresModel] private WinClientModel _winClientModel = null;

        [SerializeField] private string _winKey = "chips";

        [Tooltip("Set the timing for when the WinClientModel updates its Amounts")]
        [SerializeField] private FireTiming _winModelUpdateTiming = FireTiming.FireOnComplete;

        private void Awake()
        {
            this.InitializeDependencies();
        }

        public override IEnumerator<Yield> OnWinStart(int winLevel, float multiplier, long winAmount, float duration, string title)
        {
            if (_winModelUpdateTiming == FireTiming.FireOnStart)
            {
                UpdateWinClientModel(winAmount);
            }

            yield break;
        }

        public override void OnWinStartComplete(int winLevel, float multiplier, long winAmount, float duration, string title)
        {
            if (_winModelUpdateTiming == FireTiming.FireOnComplete)
            {
                UpdateWinClientModel(winAmount);
            }
        }
        private void UpdateWinClientModel(long winAmount)
        {
            _winClientModel.Amounts[_winKey] += winAmount;
        }
    }
}
