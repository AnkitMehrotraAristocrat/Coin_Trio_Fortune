using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Linq;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using UnityEngine.Scripting;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    public enum HoldAndSpinPrizeType
	{
        None,
        Jackpot,
        Multiplier
	}

    [Preserve]
    [Serializable]
    public class HoldAndSpinSymbolAudioEvent : SymbolAudioEvent
    {
        public HoldAndSpinPrizeType PrizeType = HoldAndSpinPrizeType.None;
        public bool TriggerSound = false;
    }

    [Preserve]
    [Serializable]
    public class HoldAndSpinSymbolAudioEvents : ReorderableArray<HoldAndSpinSymbolAudioEvent> { }

    public class HoldAndSpinScatterSoundProvider : BaseConditionalSymbolSoundProvider
    {
        private SymbolOutcomeModel _symbolOutcomeModel;
        private GameStateModel _gameStateModel;
        private new HoldAndSpinSymbolAudioEvent[] _audioEvents;

        public HoldAndSpinScatterSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, HoldAndSpinSymbolAudioEvent[] audioEvents, bool considerTriggerAudio, BaseEligibilityModifier[] eligibilityModifiers)
            : base(condition, serviceLocator, symbolLocator, audioEvents, considerTriggerAudio, eligibilityModifiers)
        {
            _symbolOutcomeModel = serviceLocator.Get<SymbolOutcomeModel>();
            _gameStateModel = serviceLocator.Get<GameStateModel>();
            _audioEvents = audioEvents;
        }

        public override SymbolAudioEvent GetAudioEvent(Location location, int groupIndex)
        {
            if (ShouldPlayTriggerSound(location.colIndex))
			{
                return _audioEvents.FirstOrDefault(audioEvent => audioEvent.TriggerSound);
            }

            if(!_symbolOutcomeModel.SymbolData.ContainsKey(GameStateEnum.HoldAndSpin))
            {
                return null;
            }

            var skinData = _symbolOutcomeModel.SymbolData[_gameStateModel.GameState];
            var prizeType = skinData[location].SymbolData.Skin;

            switch (prizeType)
            {
                case CustomPrizeTypes.CreditCorType:
                    return _audioEvents.FirstOrDefault(audioEvent => audioEvent.PrizeType.Equals(HoldAndSpinPrizeType.Multiplier));
                case CustomPrizeTypes.GrandPrizeType:
                    return _audioEvents.FirstOrDefault(audioEvent => audioEvent.PrizeType.Equals(HoldAndSpinPrizeType.Jackpot));
                case CustomPrizeTypes.MajorPrizeType:
                    return _audioEvents.FirstOrDefault(audioEvent => audioEvent.PrizeType.Equals(HoldAndSpinPrizeType.Jackpot));
                case CustomPrizeTypes.MinorPrizeType:
                    return _audioEvents.FirstOrDefault(audioEvent => audioEvent.PrizeType.Equals(HoldAndSpinPrizeType.Jackpot));
                case CustomPrizeTypes.MiniPrizeType:
                    return _audioEvents.FirstOrDefault(audioEvent => audioEvent.PrizeType.Equals(HoldAndSpinPrizeType.Jackpot));
                default:
                    return null;
            }
        }

        public override void Reset() { }
    }
}
