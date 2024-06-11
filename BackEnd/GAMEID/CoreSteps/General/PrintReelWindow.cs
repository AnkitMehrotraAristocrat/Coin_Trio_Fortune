using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using System.Threading.Tasks;
using GameBackend.Helpers;
using System.Text;

namespace GameBackend.Steps.General
{
    public class PrintReelWindow : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            var hiddenName = "--";
            var space = "\t\t";
            var window = context.GetCurrentReelWindow();

            // Write in batch so console output is not interrupted  
            var batch = new StringBuilder();
            batch.AppendLine();
            batch.AppendLine();
            for (int r = 0, index = 0; r < window.WindowSize.Height; r++) {
                for (int c = 0; c < window.WindowSize.Width; c++, index++) {
                    var name = context.HiddenWindowCells[index] ? hiddenName : window.StopsContent[index].StopSymbol.Name;
                    batch.Append($"{name}{space}");
                }
                batch.AppendLine();
            }
            batch.AppendLine();

            DebugHelper.LogText(batch.ToString());
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}