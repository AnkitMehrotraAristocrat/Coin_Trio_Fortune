using Milan.Common.SlotEngine.Models;
using System;
using System.Collections.Generic;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using System.Threading.Tasks;
using GameBackend.Helpers;
using Milan.Common.SlotEngine.Models.BetManager;

namespace GameBackend.Steps.Payloads
{
    public class CreateLinesModelPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            context.Payloads.RemovePayload(GameConstants.WinningCombinationsPayloadName);
            context.PersistentData.LinesModelPayload.Id = context.GetCurrentState();
            if (context.SpinData.Results.WinnerCombinations == null) {
                context.PersistentData.LinesModelPayload.Lines = Array.Empty<LinesModelWinningComboData>();
                return Task.CompletedTask;
            }

            var lines = new List<LinesModelWinningComboData>();
            foreach (var combo in context.SpinData.Results.WinnerCombinations) {
                if (combo.PatternId == -1) {
                    continue;
                }
                lines.Add(new LinesModelWinningComboData {
                    Name = combo.CombinationName,
                    Pattern = new LinesModelPatternData {
                        Stops = GetStops(combo.Pattern.Stops),
                        Type = (int)combo.Pattern.Type,
                        PayLineNumber = combo.PatternId
                    },
                    Reward = new LinesModelRewardData {
                        RewardItems = GetRewards(combo.Reward.RewardItems)
                    },
                    Stops = GetStops(combo.Stops)
                });
            }

            context.PersistentData.LinesModelPayload.Lines = lines.ToArray();
            context.Payloads.AddPayload(GameConstants.LinesModelPayloadName, context.PersistentData.LinesModelPayload);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            #if LINES_GAME
            return true;
            #else
            return false;
            #endif
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        
        private static LinesModelStopData[] GetStops(List<StopData> stops)
        {
            List<LinesModelStopData> items = new();
            foreach (var stop in stops) {
                items.Add(new LinesModelStopData {
                    Coordinate = new LinesModelCoordinateData {
                        Position = stop.Coordinate.Position,
                        X = stop.Coordinate.X,
                        Y = stop.Coordinate.Y
                    },
                    Symbol = stop.Symbol
                });
            }
            return items.ToArray();
        }

        private static RewardItemData[] GetRewards(IList<RewardItemData> rewardItems)
        {
            List<RewardItemData> items = new();
            foreach (var reward in rewardItems) {
                items.Add(reward);
            }
            return items.ToArray();
        }
    }
}
