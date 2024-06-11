using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    [CreateAssetMenu(fileName = "BlackoutSerializedTrigger", menuName = "NMG/Triggers/BlackoutSerializedTrigger")]
    public class BlackoutSerializedTriggerData : SerializedTrigger<BlackoutSerializedTrigger>
    {
        public string modelTag = "";
    }
}
