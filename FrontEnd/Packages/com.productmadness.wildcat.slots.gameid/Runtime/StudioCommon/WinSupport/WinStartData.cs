using System;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    [Serializable]
    public class WinStartData
    {
        [Header("Required Properties")]
        public float Duration;

        [Header("Optional Properties")]
        public int Index;
        public float Multiplier;
        public long WinAmount;
        public string Title;
    }
}