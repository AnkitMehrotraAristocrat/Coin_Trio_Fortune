using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinSetNextPrizePresenter : BaseStatePresenter, IStatePresenter, ServiceLocator.IHandler
    {
        [FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel = default;
        [FieldRequiresModel] private GameStateModel _gameStateModel = default;

        protected override void Execute()
        {
            _symbolOutcomeModel.SetNextPrize(_gameStateModel.GameState);
        }
    }
}
