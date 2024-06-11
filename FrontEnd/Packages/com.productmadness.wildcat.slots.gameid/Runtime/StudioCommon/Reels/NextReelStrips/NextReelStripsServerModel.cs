using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Linq;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips
{
    public class NextReelStripsServerModel : BaseUnsubscribable, IModel
    {
        public ReactiveDictionary<int, string[]> NextReelStripsData = new ReactiveDictionary<int, string[]>();

        public bool PerBetIndexEnabled = false;

        public NextReelStripsServerModel(ServiceLocator serviceLocator)
        {
        }

        public string[] GetNextReelStrips(int betIndex = 0)
        {
            return PerBetIndexEnabled ? NextReelStripsData[betIndex] : NextReelStripsData.FirstOrDefault().Value;
        }

        public void SetNextReelStripsForBetIndex(int betIndex, string[] reelStripNames)
        {
            NextReelStripsData[betIndex] = reelStripNames;
        }
    }
}
