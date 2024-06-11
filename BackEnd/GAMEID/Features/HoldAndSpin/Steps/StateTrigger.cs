using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Data;
using GameBackend.Features.HoldAndSpin.Configuration;
using Milan.XSlotEngine.Core.Helpers;
using Milan.XSlotEngine.Core.Models.WeightTables;
using System;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class StateTrigger : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            var rData = context.RoundData;

            // Red - Dragon
            // Green - Tiger
            // Blue - Koi

            rData.isDragonFeatureTriggered = false;
            rData.isTigerFeatureTriggered = false;
            rData.isKoiFeatureTriggered = false;

            var x = context.hasDragonLanded;
            var y = context.hasTigerLanded;
            var z = context.hasKoiLanded;

            WeightTableDefinition WeightTableDefinition = new WeightTableDefinition(context.MappedConfigurations.WeightTableDefinitions);

            object redValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Red_SCAT_Trigger_Table);
            object greenValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Green_SCAT_Trigger_Table);
            object blueValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Blue_SCAT_Trigger_Table);

            object redGreenValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Red_Green_SCAT_Trigger_Table);
            object redBlueValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Red_Blue_SCAT_Trigger_Table);
            object greenBlueValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Green_Blue_SCAT_Trigger_Table);
            object redGreenBlueValue = WeightTableDefinition.GetRandomEntry<object>(GameConstants.Red_Green_Blue_SCAT_Trigger_Table);

            if (x == true && y == false && z == false)   // ---------- 1  // Red
            {
                if(redValue.Equals("Red"))
                {
                    rData.isDragonFeatureTriggered = true;
                }
                else
                {
                    rData.isDragonFeatureTriggered = false;
                }
            }

            if(x == false && y == true && z == false)   // ---------- 2  // Green
            {
                if(greenValue.Equals("Green"))
                {
                    rData.isTigerFeatureTriggered = true;
                }
                else
                {
                    rData.isTigerFeatureTriggered= false;
                }
            }

            if(x == false && y == false && z == true)   // ---------- 3  // Blue
            {
                if(blueValue.Equals("Blue"))
                {
                    rData.isKoiFeatureTriggered = true;
                }
                else
                {
                    rData.isKoiFeatureTriggered= false;
                }
            }
            
            if(x == true && y == true && z == false)   // ------------- 4 // Red_Green
            {
                if(redGreenValue.Equals("Red"))
                {
                    rData.isDragonFeatureTriggered = true;
                }
                if(redGreenValue.Equals("Green"))
                {
                    rData.isTigerFeatureTriggered = true;
                }
                if (redGreenValue.Equals("Red_Green"))
                {
                    rData.isDragonFeatureTriggered = true;
                    rData.isTigerFeatureTriggered = true;
                }
                else
                {
                    rData.isDragonFeatureTriggered = false;
                    rData.isTigerFeatureTriggered = false;
                }
            }

            if (x == true && y == false && z == true)   // ------------- 5 // Red_Blue
            {
                if(redBlueValue.Equals("Red"))
                {
                    rData.isDragonFeatureTriggered= true;
                }
                if(redBlueValue.Equals("Blue"))
                {
                    rData.isKoiFeatureTriggered = true;
                }
                if(redBlueValue.Equals("Red_Blue"))
                {
                    rData.isDragonFeatureTriggered = true;
                    rData.isKoiFeatureTriggered= true;
                }
                else
                {
                    rData.isDragonFeatureTriggered = false;
                    rData.isKoiFeatureTriggered = false;
                }
            }

            if (x == false && y == true && z == true)   // ------------- 6 // Green_Blue
            {
                if (greenBlueValue.Equals("Green"))
                {
                    rData.isTigerFeatureTriggered = true;
                }
                if (greenBlueValue.Equals("Blue"))
                {
                    rData.isKoiFeatureTriggered = true;
                }
                if(greenBlueValue.Equals("Green_Blue"))
                {
                    rData.isTigerFeatureTriggered = true;
                    rData.isKoiFeatureTriggered = true;
                }
                else
                {
                    rData.isTigerFeatureTriggered = false;
                    rData.isKoiFeatureTriggered = false;
                }
            }

            if (x == true && y == true && z == true)   // ------------- 7 // Red_Green_Blue
            {
                if(redGreenBlueValue.Equals("Red"))
                {
                    rData.isDragonFeatureTriggered= true;
                }
                if (redGreenBlueValue.Equals("Green"))
                {
                    rData.isTigerFeatureTriggered = true;
                }
                if (redGreenBlueValue.Equals("Blue"))
                {
                    rData.isKoiFeatureTriggered = true;
                }
                if (redGreenBlueValue.Equals("Red_Green"))
                {
                    rData.isDragonFeatureTriggered = true;
                    rData.isTigerFeatureTriggered = true;
                }
                if (redGreenBlueValue.Equals("Red_Blue"))
                {
                    rData.isDragonFeatureTriggered = true;
                    rData.isKoiFeatureTriggered = true;
                }
                if (redGreenBlueValue.Equals("Green_Blue"))
                {
                    rData.isTigerFeatureTriggered = true;
                    rData.isKoiFeatureTriggered = true;
                }
                if (redGreenBlueValue.Equals("Red_Green_Blue"))
                {
                    rData.isDragonFeatureTriggered = true;
                    rData.isTigerFeatureTriggered = true;
                    rData.isKoiFeatureTriggered = true;
                }
                else
                {
                    rData.isDragonFeatureTriggered = false;
                    rData.isTigerFeatureTriggered = false;
                    rData.isKoiFeatureTriggered = false;
                }
            }

            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
    }
}
