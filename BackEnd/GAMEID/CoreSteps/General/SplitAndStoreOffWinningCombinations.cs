using MilanCombinationData = Milan.Shared.DTO.Jackpot.CombinationData;
using System.Collections.Generic;
using System.Linq;
using Milan.Common.SlotEngine.Models.BetManager;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Data;
using GameBackend.Helpers;
using Milan.Common.SlotEngine.Models;

namespace GameBackend.Steps.General
{
    public class SplitAndStoreOffWinningCombinations : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var winningCombinations = ProcessWinningCombinations(context);
            context.Payloads.RemovePayload(GameConstants.WinningCombinationsPayloadName);

            var winningNormalCombinations = winningCombinations.Where(entry => { return entry.Pattern.Type != EnumData.PatternType.Scatter; }).ToArray();
            var winningScatterCombinations = winningCombinations.Where(entry => { return entry.Pattern.Type == EnumData.PatternType.Scatter; }).ToArray();

            List<ComboScatterData> winningCombinationsScatterProcessed = GenerateCustomComboList(winningScatterCombinations.ToList());
            var gameMode = context.Transition.FromState;

            #if LINES_GAME
            var linesCombos = GenerateCustomLinesComboList(winningNormalCombinations);
            var linesComboRecovery = context.PersistentData.LinesComboRecovery;
            var linesEntry = linesComboRecovery.Find(entry => entry.Id == gameMode);
            if (linesEntry == null) {
                linesComboRecovery.Add(linesEntry = new ComboLinesRecoveryData() { Id = gameMode });
            }
            linesEntry.Lines = linesCombos;
            #else //WAYS_GAME
            ComboWaysData waysCombos = GenerateCustomWaysComboList(winningNormalCombinations);
            var waysComboRecovery = context.PersistentData.WaysComboRecovery;
            var waysEntry = waysComboRecovery.Find(entry => entry.Id == gameMode);
            if (waysEntry == null) {
                waysComboRecovery.Add(waysEntry = new ComboWaysRecoveryData() { Id = gameMode });
            }
            waysEntry.HitResult = waysCombos;
            #endif

            var scatterComboRecovery = context.PersistentData.ScatterComboRecovery;
            var scatterEntry = scatterComboRecovery.Find(entry => entry.Id == gameMode);
            if (scatterEntry == null) {
                scatterComboRecovery.Add(scatterEntry = new ComboScatterRecoveryData() { Id = gameMode });
            }
            scatterEntry.Combinations = winningCombinationsScatterProcessed;
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }

        ////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////

        private static IList<MilanCombinationData> ProcessWinningCombinations(GameContext context)
        {
            if (context.SpinData.Results.WinnerCombinations == null) {
                return new List<MilanCombinationData>();
            }

            return context.SpinData.Results.WinnerCombinations.Select(c => new MilanCombinationData {
                Name = c.CombinationName,
                Reward = c.Reward,
                Stops = c.Stops,
                Pattern = c.Pattern
            }).ToList();
        }

        public static List<ComboLinesData> GenerateCustomLinesComboList(IList<MilanCombinationData> originList)
        {
            List<ComboLinesData> newList = new();
            foreach (var entry in originList) {
                var pattern = new ComboPattern() {
                    Id = entry.Pattern.Id,
                    Stops = entry.Pattern.Stops.Select(e => new ComboStop() {
                        Coordinate = new ComboCoordinate() { Position = e.Coordinate.Position },
                        Symbol = e.Symbol
                    }).ToList(),
                    Type = entry.Pattern.Type,
                    PayLineNumber = entry.Pattern.PayLineNumber
                };

                var reward = new ComboReward() {
                    RewardItems = entry.Reward.RewardItems.Select(e => new RewardItemData() {
                        CurrencyType = e.CurrencyType,
                        Quantity = e.Quantity,
                        MultiplyWin = e.MultiplyWin,
                        TotalWon = e.TotalWon
                    }).ToList()
                };

                newList.Add(new ComboLinesData(entry.Name, entry.Stops, entry.Pattern.PayLineNumber, pattern, reward));
            }
            return newList;
        }

        public static ComboWaysData GenerateCustomWaysComboList(IList<MilanCombinationData> originList)
        {
            Dictionary<string, ComboWaysWinningData> combos = new();
            foreach (var entry in originList) {
                if (!combos.ContainsKey(entry.Name)) {
                    combos.Add(entry.Name, new ComboWaysWinningData() {
                        Coordinates = new List<ComboCoordinate>(),
                        TotalReward = new ComboReward() {
                            RewardItems = new List<RewardItemData>()
                        }
                    });
                }

                foreach (var coord in entry.Stops) {
                    // See if we have a coords here already
                    var existingCoord = combos[entry.Name].Coordinates.FirstOrDefault(coordEntry => coordEntry.X.Equals(coord.Coordinate.Position.X) && coordEntry.Y.Equals(coord.Coordinate.Position.Y));
                    if (existingCoord == null) {
                        combos[entry.Name].Coordinates.Add(new ComboCoordinate() { Position = coord.Coordinate.Position });
                    }
                }

                foreach (var reward in entry.Reward.RewardItems) {
                    var existingReward = combos[entry.Name].TotalReward.RewardItems.FirstOrDefault(rewardEntry => rewardEntry.CurrencyType.Equals(reward.CurrencyType));
                    if (existingReward == null) {
                        combos[entry.Name].TotalReward.RewardItems.Add(new RewardItemData() {
                            CurrencyType = reward.CurrencyType,
                            Quantity = reward.Quantity,
                            MultiplyWin = reward.MultiplyWin,
                            TotalWon = reward.TotalWon
                        });
                    }
                    else {
                        existingReward.TotalWon += reward.TotalWon;
                    }
                }
            }

            List<ComboWaysWinningData> hits = combos.Values.ToList();
            return new ComboWaysData(hits);
        }

        public static List<ComboScatterData> GenerateCustomComboList(IList<MilanCombinationData> originList)
        {
            List<ComboScatterData> newList = new();
            foreach (var entry in originList) {
                newList.Add(new ComboScatterData(entry.Name, entry.Stops, entry.Pattern.PayLineNumber));
            }
            return newList;
        }
    }
}
