using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using System.Linq;

namespace GameBackend.Steps.Payloads
{
    public class CreateStatePayloads : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            UpdateCurrentSpinResult(context);
            AddReelOutcomes(context);
            context.JoinPayload.AddPayload(GameConstants.BetStatePayloadName, new BetState {
                FeatureBetIndex = context.RoundData.BetIndex,
                BaseBetIndex = context.PersistentData.BaseBetIndex
            });

            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static void UpdateCurrentSpinResult(GameContext context)
        {
            if (context.PersistentData.TriggeredStates.Queue.Count() != 0) {
                return;
            }
            var baseState = GeneralHelper.GetGameStateString(GameStates.BaseSpin);
            context.PersistentData.TriggeredStates.Queue.Enqueue(
                context.CustomConfigurations.StatesExcecutionPriority.StatePriority[baseState],
                baseState
            );
        }

        private static void AddReelOutcomes(GameContext context)
        {
            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/

            string[] gameStates = GeneralHelper.GetGameStatesArray();
            foreach (string state in gameStates) {
                var reelOutcome = GetOutcomeData(context, state);

                var payload = new ReelOutcomePayload() {
                    Id = reelOutcome.Id,
                    ReelWindowId = reelOutcome.ReelWindowId,
                    ReelStrips = reelOutcome.IndexedReelStrips,
                    Offsets = reelOutcome.IndexedOffsets
                };

                if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(state)]) {
                    var currHeight = context.GetCurrentReelWindowCurrentHeight();
                    var currWinWidth = context.GetCurrentReelWindowCurrentWidth();
                    payload.ReelStrips = GeneralHelper.GetWorldIndexedListInClientFormation(payload.ReelStrips, currHeight, currWinWidth);
                    payload.Offsets = GeneralHelper.GetWorldIndexedListInClientFormation(payload.Offsets, currHeight, currWinWidth);
                }
                context.JoinPayload.AddPayload(GameConstants.ReelsOutcomeModelPayloadName, payload);
            }
        }

        private static ReelOutcome GetOutcomeData(GameContext context, string state)
        {
            var outcomeData = context.PersistentData.ReelOutcomeData;
            if (!outcomeData.ContainsKey(state)) {
                var stateEnum = GeneralHelper.GetGameStateEnum(state);
                var reelWindowId = GameConstants.StateReelWindows[stateEnum][0];
                var betLevel = ReelSetsFeatureAccess.GetCurrentBetLevel(context);
                var reelStrips = ReelSetsFeatureAccess.GetReelStripsData(context, state, reelWindowId, betLevel).ToList();
                var reelOffsets = new int[reelStrips.Count];
                for (int i = 0; i < reelStrips.Count; i++) {
                    reelOffsets[i] = 0;
                }

                outcomeData.Add(state, new ReelOutcome() {
                    Id = state,
                    ReelWindowId = reelWindowId,
                    IndexedReelStrips = reelStrips,
                    IndexedOffsets = reelOffsets.ToList()
                });
            }
            return outcomeData[state];
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public class BetState
        {
            public int FeatureBetIndex { get; set; }
            public int BaseBetIndex { get; set; }
        }
    }
}