using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutDriver : GenericIdModelDriver<BlackoutServerModel, BlackoutPayload>
    {
        protected override void OnResponse(BlackoutServerModel model, BlackoutPayload data)
        {
            // update with id once available
            model.Data = data;
        }
    }
}
