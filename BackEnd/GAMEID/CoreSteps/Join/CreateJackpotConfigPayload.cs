using GameBackend.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System;
using System.Threading.Tasks;

namespace GameBackend.Steps.Join
{
	public class CreateJackpotConfigPayload : BaseStep<GameContext>
	{
		public override async Task ExecuteAsync(GameContext context)
		{
			var jackpotInits = await JackpotHelper.GetJackpotConfigAsync(context, GameConstants.JackpotID);
			context.ConfigPayload.AddPayload(GameConstants.JackpotInitValuePayloadName, jackpotInits);
		}

		public override bool Validate(GameContext context)
		{
			if (context == null)
				throw new ArgumentNullException($"Context can't be null");

			return true;
		}
	}
}
