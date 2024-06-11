using System;
using System.Linq;
using Milan.Shared.DTO.Jackpot;
using GameBackend.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Milan.XSlotEngine.Core.Extensions;
using Wildcat.Milan.Game.Core.JackpotEngine.Contracts;
using Wildcat.Milan.Game.Core.JackpotEngine.Facade;
using Wildcat.Milan.Game.Core.Utilities;
using System.Threading;
using Wildcat.Milan.Game.Core.JackpotEngine.Helpers;
using Wildcat.Milan.Game.Core.JackpotEngine.Endpoints;
using System.Runtime.CompilerServices;
using NewRelic.Api.Agent;

namespace GameBackend.Helpers
{
    public static class JackpotHelper
    {
        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<JackpotValueData[]> ContributeToJackpotAsync(GameContext context)
        {
#if !JACKPOTS_OFF
            return await ContributeToJackpotNetworkAsync(context);
#else
            return await ContributeToJackpotLocal(context);
#endif
        }

        private static async Task<JackpotValueData[]> ContributeToJackpotNetworkAsync(GameContext context)
        {
            var totalBet = BetHelper.GetTotalBet(context);
            var spinRequestRefit = new ProcessSpinRequestData()
            {
                Wager = totalBet,
                ApplicationId = context.JackpotOperations.ApplicationId,
                Templates = context.JackpotOperations.Templates.Select(x => new TemplateData()
                {
                    TemplateId = x.TemplateId,
                    QualifyToWin = x.QualifyToWin
                }).ToList(),
                UserId = context.UserId
            };

            var jackpotEngine = ServiceManagerExtension.GetService<IJackpotEngineFacade>();
            var jackpotSpinPayload = await jackpotEngine.ContributeToJackpotAsync(spinRequestRefit, CancellationToken.None);

            return JackpotContractsHelpers.GenerateJackpotValueData(jackpotSpinPayload).ToArray();
        }

        private static Task<JackpotValueData[]> ContributeToJackpotLocal(GameContext context)
        {
            return GetInitialJackpotValuesLocal(context);
        }

        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<JackpotValueData[]> GetInitialJackpotValuesAsync(GameContext context, string jackpotID)
        {
#if !JACKPOTS_OFF
            return await GetInitialJackpotValuesNetworkAsync(context, jackpotID);
#else
            return await GetInitialJackpotValuesLocal(context);
#endif
        }

        private static async Task<JackpotValueData[]> GetInitialJackpotValuesNetworkAsync(GameContext context, string jackpotID)
        {
            var initRequest = new JackpotRequest
            {
                ApplicationId = context.JackpotOperations.ApplicationId,
                UserId = context.UserId,
                TemplateId = context.JackpotOperations.TemplateId
            };

            var jackpotEngine = ServiceManagerExtension.GetService<IJackpotEngineFacade>();
            var jackpotResult = await jackpotEngine.GetJackpotAsync(initRequest, CancellationToken.None);

            // Change to Generate State Models, might need new PM Engine Response paylod
            var milanJackpotInitModels = new List<JackpotValueData>();
            foreach (var jackpot in jackpotResult.JackpotData)
            {
                milanJackpotInitModels.Add(new JackpotValueData
                {
                    // The named jackpot concept is obsolete; everything is defaulted to "main"
                    JackpotId = jackpotID,
                    // TODO Milan: Add TierId to JackpotValueData
                    // TierId = jackpot.TierId,
                    TemplateId = context.JackpotOperations.TemplateId,
                    TierPosition = jackpot.TierPosition,
                    Bracket = jackpot?.Bracket?.BracketId,
                    Value = jackpot.Value
                });
            }
            return milanJackpotInitModels.ToArray();
        }

        private static Task<JackpotValueData[]> GetInitialJackpotValuesLocal(GameContext context)
        {
            JackpotSummary jackpotResult = new()
            {
                ApplicationId = 1,
                TemplateId = 1,
                JackpotData = new List<JackpotInitData>()
            };

            for (int tier = 0; tier < GameConstants.JackpotBaseValues.Length; ++tier)
            {
                var value = GameConstants.JackpotBaseValues[tier] * context.GetBetAmounts().Min();
                jackpotResult.JackpotData.Add(new JackpotInitData()
                {
                    TierPosition = tier,
                    JackpotId = "main",
                    TierId = tier + 1,
                    Bracket = new JackpotBracketData()
                    {
                        BracketId = 0,
                        WagerFrom = 0,
                        WagerTo = null
                    },
                    Value = value
                });
            }

            // Change to Generate State Models, might need new PM Engine Response paylod
            var milanJackpotInitModels = new List<JackpotValueData>();
            foreach (var jackpot in jackpotResult.JackpotData)
            {
                milanJackpotInitModels.Add(new JackpotValueData
                {
                    // The named jackpot concept is obsolete; everything is defaulted to "main"
                    JackpotId = "main",
                    // TODO Milan: Add TierId to JackpotValueData
                    // TierId = jackpot.TierId,
                    TierPosition = jackpot.TierPosition,
                    Bracket = jackpot?.Bracket?.BracketId,
                    Value = jackpot.Value
                });
            }
            return Task.FromResult(milanJackpotInitModels.ToArray());
        }

        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<JackpotInitResponseData> GetJackpotConfigAsync(GameContext context, string jackpotID)
        {
#if !JACKPOTS_OFF
            return await GetJackpotConfigNetworkAsync(context, jackpotID);
#else
			return await GetJackpotConfigLocal(context);
#endif
        }

        private static async Task<JackpotInitResponseData> GetJackpotConfigNetworkAsync(GameContext context, string jackpotID)
        {
            var requestModel = new JackpotInitRequestData
            {
                ApplicationId = context.JackpotOperations.ApplicationId,
                UserId = context.JackpotOperations.UserId,
                TemplateIds = new List<int> { context.JackpotOperations.TemplateId }//,
                //Wager = BetHelper.GetTotalBet(context)
            };

            var jackpotEngine = ServiceManagerExtension.GetService<IJackpotEngine>();
            var result = await jackpotEngine.GetJackpots(requestModel, CancellationToken.None);

            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error fetching jackpots from template {context.JackpotOperations.TemplateId}");
            }

            return result.Content;
        }

        private static Task<JackpotInitResponseData> GetJackpotConfigLocal(GameContext context)
        {
            JackpotInitResponseData response = new JackpotInitResponseData()
            {
                User = new UserData()
                {
                    ApplicationId = context.JackpotOperations.ApplicationId,
                    UserId = context.JackpotOperations.UserId
                },
                Jackpots = new List<JackpotInitDefinitionData>
                {
                    new JackpotInitDefinitionData()
                    {
                        ApplicationId = context.JackpotOperations.ApplicationId,
                        TemplateId = context.JackpotOperations.TemplateId,
                        JackpotWinTrigger = "game",
                        Instance = null,
                        Instances = null,
                        Brackets = new List<BracketInitData>
                        {
                            new BracketInitData()
                            {
                                Position = 0,
                                WagerFrom = 0,
                                WagerTo = 4,
                                Instances = new List<MultiTierInstanceData>
                                {
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 0,
                                        Status = null,
                                        TierId = 0,
                                        Value = 1000000,
                                        TierPosition = 0,
                                        StartingFund = 1000000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 1,
                                        Status = null,
                                        TierId = 1,
                                        Value = 100000,
                                        TierPosition = 1,
                                        StartingFund = 100000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 2,
                                        Status = null,
                                        TierId = 2,
                                        Value = 50000,
                                        TierPosition = 2,
                                        StartingFund = 50000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 3,
                                        Status = null,
                                        TierId = 3,
                                        Value = 25000,
                                        TierPosition = 3,
                                        StartingFund = 25000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    }
                                }
                            },
                            new BracketInitData()
                            {
                                Position = 1,
                                WagerFrom = 5,
                                WagerTo = 10,
                                Instances = new List<MultiTierInstanceData>
                                {
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 4,
                                        Status = null,
                                        TierId = 4,
                                        Value = 1000000,
                                        TierPosition = 0,
                                        StartingFund = 1000000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 5,
                                        Status = null,
                                        TierId = 5,
                                        Value = 100000,
                                        TierPosition = 1,
                                        StartingFund = 100000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 6,
                                        Status = null,
                                        TierId = 6,
                                        Value = 50000,
                                        TierPosition = 2,
                                        StartingFund = 50000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    },
                                    new MultiTierInstanceData()
                                    {
                                        InstanceId = 7,
                                        Status = null,
                                        TierId = 7,
                                        Value = 25000,
                                        TierPosition = 3,
                                        StartingFund = 25000,
                                        MultitieredType = null,
                                        OverflowAmount = 0,
                                        Qualified = false,
                                        Scalable = false
                                    }
                                }
                            }
                        },
                        JackpotTemplateType = null,
                        JackpotType = null,
                        Type = null,
                        StartingFund = 0,
                        ScalableJackpot = false,
                        PossibleFixedWins = null
                    }
                },
                JackpotDataTimestamp = (ulong)DateTime.Now.Ticks,
                Code = 200,
                Message = null
            };

            return Task.FromResult(response);
        }

        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////
        //////////////////////////////////////////////////////

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static async Task<ulong> AwardJackpotAsync(GameContext context, int tier, string jackpotID, int multiplier)
        {
#if !JACKPOTS_OFF
            return await JackpotHelper.AwardJackpotNetworkAsync(context, tier, jackpotID, multiplier);
#else
            return await JackpotHelper.AwardJackpotLocal(context, tier, jackpotID, multiplier);
#endif
        }

        private static async Task<ulong> AwardJackpotNetworkAsync(GameContext context, int tier, string jackpotID, int multiplier)
        {
            JackpotWinSummary jackpotWinSummary = await JackpotHitAsync(context, tier, jackpotID, multiplier);
            var winModel = jackpotWinSummary.ToJackpotWinData();

            var resetVal = new JackpotInitData()
            {
                JackpotId = winModel.JackpotId,
                TierId = winModel.TierId,
                TierPosition = winModel.TierPosition,
                Bracket = new JackpotBracketData() { BracketId = jackpotWinSummary.BracketId ?? -1 },
                Value = jackpotWinSummary.Instance.StartingFund,
                StartingValue = jackpotWinSummary.Instance.StartingFund
            };

            context.Payloads.AddPayload(GameConstants.JackpotWinsPayloadName, winModel);
            context.Payloads.AddPayload(GameConstants.JackpotResetsPayloadName, resetVal);
            return winModel.Value;
        }

        private static Task<ulong> AwardJackpotLocal(GameContext context, int tier, string jackpotID, int multiplier)
        {
            JackpotWinSummary jackpotWinSummary = JackpotHitLocal(context, tier, jackpotID, multiplier);
            var winModel = new JackpotWinData()
            {
                TemplateId = context.JackpotOperations.TemplateId,
                BracketId = jackpotWinSummary.BracketId,
                InstanceId = jackpotWinSummary.WinInfo.InstanceId,
                JackpotId = jackpotID,
                // TODO Milan: the Multiplier types do not match (ulong vs double)
                //Multiplier = jackpotWinSummary.WinInfo.Multiplier,
                Multiplier = 1,
                TierPosition = jackpotWinSummary.WinInfo.TierPosition,
                Value = jackpotWinSummary.WinInfo.Value,
                WinnerId = jackpotWinSummary.WinInfo.WinnerId,
                WonAt = jackpotWinSummary.WinInfo.WonAt,
            };

            var resetVal = new JackpotInitData()
            {
                JackpotId = jackpotID,
                TierId = jackpotWinSummary.WinInfo.TierPosition,
                Bracket = new JackpotBracketData() { BracketId = jackpotWinSummary.BracketId ?? -1 },
                Value = jackpotWinSummary.Instance.StartingFund,
                StartingValue = jackpotWinSummary.Instance.StartingFund
            };

            context.Payloads.AddPayload(GameConstants.JackpotWinsPayloadName, winModel);
            context.Payloads.AddPayload(GameConstants.JackpotResetsPayloadName, resetVal);
            return Task.FromResult(winModel.Value);
        }

        private static async Task<JackpotWinSummary> JackpotHitAsync(GameContext context, int tier, string jackpotID, int multiplier)
        {
            var totalBet = BetHelper.GetTotalBet(context);
            var jackpotWinRequest = new JackpotWinRequestData()
            {
                Wager = totalBet,
                ApplicationId = context.JackpotOperations.ApplicationId,
                TemplateId = context.JackpotOperations.TemplateId,
                UserId = context.UserId,
                TierPosition = tier
            };
            var jackpotEngine = ServiceManagerExtension.GetService<IJackpotEngineFacade>();

            JackpotWinSummary jackpotWinSummary = await jackpotEngine.JackpotHitAsync(jackpotWinRequest, CancellationToken.None);
            //var winModel = JackpotContractsHelpers.JackpotWinToWinModel(
            //    jackpotTemplateId: context.JackpotOperations.TemplateId,
            //    winInfo: jackpotWinSummary.WinInfo);
            var winModel = jackpotWinSummary.ToJackpotWinData();

            var jackpotId = context.JackpotOperations.TemplateId.ToString();
            var jackpotWin = new JackpotWinData
            {
                JackpotId = jackpotId
            };
            var jackpotValue = new JackpotValueData
            {
                JackpotId = jackpotId
            };

            //This is to implement logic where Jackpot resets only are mulitplied by the multiplier. Incement is not multiplied.
            //If jackpot reset value = 1500 and incremented value at the time of awarding is 2000, then zone multiplier will get
            //applied to 1500, that is if multiplier is 2 , jackpot award value would be  2000 + 1500 * (2-1) = 3500
            if (multiplier > 1)
            {
                ulong startingFund = (ulong)(jackpotWinSummary.Instance.StartingFund * winModel.Multiplier);
                winModel.Value = (startingFund * (ulong)multiplier) + (winModel.Value - startingFund);
            }

            // Jackpot win 
            jackpotWin.InstanceId = winModel.InstanceId;
            jackpotWin.Multiplier = winModel.Multiplier;
            jackpotWin.TierPosition = winModel.TierPosition;
            jackpotWin.WinnerId = winModel.WinnerId;
            jackpotWin.Value = winModel.Value;
            jackpotWin.WonAt = winModel.WonAt;

            jackpotValue.TierPosition = winModel.TierPosition;
            jackpotValue.Value = winModel.Value;

            // For Bracket, Position from first response bracket will be set,
            // as it is expected that this returns only 1.
            // This should be reviewed in a future in case more are added.
            jackpotValue.Bracket = jackpotWinSummary.BracketId;
            context.JackpotOperations.JackpotWins.Add(jackpotWin);
            context.JackpotOperations.JackpotValues.Add(jackpotValue);

            jackpotWinSummary.WinInfo.Value = winModel.Value;
            return jackpotWinSummary;
        }

        private static JackpotWinSummary JackpotHitLocal(GameContext context, int tier, string jackpotID, int multiplier)
        {
            var totalBet = BetHelper.GetTotalBet(context);
            ulong win = (GameConstants.JackpotBaseValues[tier] * totalBet) * (ulong)multiplier;
            JackpotWinSummary jackpotWinSummary = new()
            {
                WinInfo = new WinInfoData()
                {
                    InstanceId = 0,
                    Value = win,
                    TierPosition = tier,
                    WonAt = (int)DateTime.Now.Ticks,
                    Multiplier = BetHelper.GetTotalBet(context) / context.GetBetAmounts().Min()
                },
                Instance = new MultiTierInstanceData()
                {
                    TierId = tier + 1,
                    InstanceId = 0,
                    Value = win,
                    TierPosition = tier,
                    StartingFund = win,
                    Scalable = true
                },
                BracketId = 0,
                TemplateId = context.JackpotOperations.TemplateId
            };

            //var winModel = JackpotContractsHelpers.JackpotWinToWinModel(
            //    jackpotTemplateId: context.JackpotOperations.TemplateId,
            //    winInfo: jackpotWinSummary.WinInfo);
            var winModel = jackpotWinSummary.ToJackpotWinData();

            var jackpotId = context.JackpotOperations.TemplateId.ToString();
            var jackpotWin = new JackpotWinData
            {
                JackpotId = jackpotId
            };
            var jackpotValue = new JackpotValueData
            {
                JackpotId = jackpotId
            };

            //This is to implement logic where Jackpot resets only are mulitplied by the multiplier. Incement is not multiplied.
            //If jackpot reset value = 1500 and incremented value at the time of awarding is 2000, then zone multiplier will get
            //applied to 1500, that is if multiplier is 2 , jackpot award value would be  2000 + 1500 * (2-1) = 3500
            if (multiplier > 1)
            {
                ulong startingFund = (ulong)(jackpotWinSummary.Instance.StartingFund * winModel.Multiplier);
                winModel.Value = (startingFund * (ulong)multiplier) + (winModel.Value - startingFund);
            }

            // Jackpot win 
            jackpotWin.InstanceId = winModel.InstanceId;
            jackpotWin.Multiplier = winModel.Multiplier;
            jackpotWin.TierPosition = winModel.TierPosition;
            jackpotWin.WinnerId = winModel.WinnerId;
            jackpotWin.Value = winModel.Value;
            jackpotWin.WonAt = winModel.WonAt;

            jackpotValue.TierPosition = winModel.TierPosition;
            jackpotValue.Value = winModel.Value;

            // For Bracket, Position from first response bracket will be set,
            // as it is expected that this returns only 1.
            // This should be reviewed in a future in case more are added.
            jackpotValue.Bracket = jackpotWinSummary.BracketId;
            context.JackpotOperations.JackpotWins.Add(jackpotWin);
            context.JackpotOperations.JackpotValues.Add(jackpotValue);

            jackpotWinSummary.WinInfo.Value = winModel.Value;
            return jackpotWinSummary;
        }
    }
}