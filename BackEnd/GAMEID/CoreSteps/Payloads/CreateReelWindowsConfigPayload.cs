using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateReelWindowsConfigPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var reelWindows = BuildReelWindowsJson(context);
            foreach (var window in reelWindows) {
                context.ConfigPayload.AddPayload(GameConstants.ReelWindowModelPayloadName, window);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.MappedConfigurations, nameof(context.MappedConfigurations));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static ReelWindowsJson BuildReelWindowsJson(GameContext customContext)
        {
            var result = new ReelWindowsJson();
            foreach (var windowPair in customContext.MappedConfigurations.ReelWindowDefinitions) {
                result.Add(new ReelWindowJson() {
                    id = windowPair.Key,
                    Width = windowPair.Value.Width,
                    Height = windowPair.Value.Height
                });
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public class ReelWindowsJson : List<ReelWindowJson>
        {
        }

        public class ReelWindowJson
        {
            public string id { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
