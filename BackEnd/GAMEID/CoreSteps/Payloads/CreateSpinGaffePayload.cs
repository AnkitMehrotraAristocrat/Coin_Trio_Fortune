using GameBackend.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using Milan.XSlotEngine.Interfaces.Helpers;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace GameBackend.Steps.Payloads 
{
    public class CreateSpinGaffePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            SpinGaffePayload payload = CreatePayload(context);
            context.Payloads.AddPayload(GameConstants.SpinGaffePayloadName, payload);     
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return context.SpinGaffeInfo.Count > 0;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public static SpinGaffePayload CreatePayload(GameContext context)
        {
            SpinGaffePayload payload = new() {
                GameState = context.PersistentData.PreviousState,
                SpinGuid = context.SpinGuid,
                RandomValues = new List<ulong>(),
                TimeStamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")
            };

            foreach (IGaffeInfo info in context.SpinGaffeInfo) {
                ulong value = info.RandomNumber == null ? 0 : (ulong)info.RandomNumber;
                payload.RandomValues.Add(value);
            }
            return payload;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public class SpinGaffePayload
        {
            public string GameState { get; set; }
            public string TimeStamp { get; set; }
            public Guid SpinGuid { get; set; }
            public List<ulong> RandomValues { get; set; }
        }
    }
}
