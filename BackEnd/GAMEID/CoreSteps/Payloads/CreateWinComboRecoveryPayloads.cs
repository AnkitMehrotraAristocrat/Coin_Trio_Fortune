using System.Threading.Tasks;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateWinComboRecoveryPayloads : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            var gameModes = GeneralHelper.GetGameStatesArray();
            var waysComboRecovery = context.PersistentData.WaysComboRecovery;
            var scatterComboRecovery = context.PersistentData.ScatterComboRecovery;

            foreach (var gameMode in gameModes) {
                var waysEntry = waysComboRecovery.Find(entry => (entry.Id == gameMode));
                if (waysEntry == null) {
                    var reelWindow = context.CurrentReelWindowDefinition;
                    waysComboRecovery.Add(waysEntry = new ComboWaysRecoveryData() { Id = gameMode, HitResult = new ComboWaysData() });
                }
                context.JoinPayload.AddPayload(GameConstants.WaysModelPayloadName, waysEntry);

                var scatterEntry = scatterComboRecovery.Find(entry => (entry.Id == gameMode));
                if (scatterEntry == null) scatterComboRecovery.Add(scatterEntry = new ComboScatterRecoveryData() { Id = gameMode });
                context.JoinPayload.AddPayload(GameConstants.ScatterModelPayloadName, scatterEntry);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));

            #if WAYS_GAME
            return true;
            #else
            return false;
            #endif
        }
    }
}