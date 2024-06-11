using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Linq;
using System.Collections.Generic;
using Milan.XSlotEngine.Interfaces.Core;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreatePayLinesConfigPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.ConfigPayload.AddPayload(
                GameConstants.PayLinesPayloadName,
                context
                    .MappedConfigurations
                    .PayLineDefinitions
                    .Select(payline => new SimplePayLineConfig(payline.Key, payline.Value.PayLines)));
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.MappedConfigurations, nameof(context.MappedConfigurations));
            #if LINES_GAME
            return true;
            #else
            return false;
            #endif
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public partial class SimplePayLineConfig
        {
            public int LineCount { get; set; }
            public IList<IPayLine> Lines { get; set; }

            public SimplePayLineConfig(int lineCount, IList<IPayLine> lineConfig)
            {
                LineCount = lineCount;
                Lines = lineConfig.ToList();
            }
        }
    }
}
