using System.Threading.Tasks;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateWinComboRecoveryStatePayloads : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var gameMode = context.Transition.FromState;
            
            #if LINES_GAME
            var linesComboRecovery = context.PersistentData.LinesComboRecovery;
            var linesEntry = linesComboRecovery.Find(entry => (entry.Id == gameMode));
            if (linesEntry == null) {
                linesComboRecovery.Add(linesEntry = new ComboLinesRecoveryData() { Id = gameMode });
            }
            context.Payloads.AddPayload(GameConstants.LinesModelPayloadName, linesEntry);
            #else //WAYS_GAME
            var waysComboRecovery = context.PersistentData.WaysComboRecovery;
            var waysEntry = waysComboRecovery.Find(entry => (entry.Id == gameMode));
            if (waysEntry == null) {
                var reelWindow = context.CurrentReelWindowDefinition;
                waysComboRecovery.Add(waysEntry = new ComboWaysRecoveryData() { Id = gameMode, HitResult = new ComboWaysData() });
            }
            context.Payloads.AddPayload(GameConstants.WaysModelPayloadName, waysEntry);
            #endif

            var scatterComboRecovery = context.PersistentData.ScatterComboRecovery;
            var scatterEntry = scatterComboRecovery.Find(entry => (entry.Id == gameMode));
            if (scatterEntry == null) {
                scatterComboRecovery.Add(scatterEntry = new ComboScatterRecoveryData() { Id = gameMode });
            }
            context.Payloads.AddPayload(GameConstants.ScatterModelPayloadName, scatterEntry);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}