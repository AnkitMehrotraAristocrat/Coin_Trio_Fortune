using Milan.FrontEnd.Core.v5_1_1;
using System.Linq;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips
{
    public class NextReelStripsModelDriver : GenericIdModelDriver<NextReelStripsServerModel, NextReelStripsPayloadData>
    {
        protected override void OnResponse(NextReelStripsServerModel model, NextReelStripsPayloadData data)
        {
            model.PerBetIndexEnabled = data.PerBetIndexEnabled;
            foreach(var dataPair in data.NextReelStripsData)
            {
                model.SetNextReelStripsForBetIndex(dataPair.Key, dataPair.Value);
            }
        }
    }
}
