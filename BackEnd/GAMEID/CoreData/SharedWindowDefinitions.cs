using Milan.Common.SlotEngine.Models;
using System.Collections.Generic;

namespace GameBackend.Data
{
    public class CustomReelWindow
    {
        public List<FlexibleStop> StopsContent { get; set; }
        public Size WindowSize { get; set; }

        public CustomReelWindow()
        {
            StopsContent = new List<FlexibleStop>();
            WindowSize = new Size();
        }
    }

    public class FlexibleStop
    {
        public int WorldIndex { get; set; }
        public SymbolData StopSymbol { get; set; }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size() {}
        public Size(int w, int h) { Width = w; Height = h; }
    }

    public class CellSwaps
    {
        public List<CellSwap> Swaps { get; set; } = new();

        public void AddSwap(int fromWorldIndex, int toWorldIndex)
        {
            var duplicate = Swaps.Find((item) => {
                return item.FromWorldIndex == fromWorldIndex
                || item.ToWorldIndex == fromWorldIndex
                || item.FromWorldIndex == toWorldIndex
                || item.ToWorldIndex == toWorldIndex;
            });
            if (duplicate == null) {
                Swaps.Add(new CellSwap {
                    FromWorldIndex = fromWorldIndex,
                    ToWorldIndex = toWorldIndex
                });
            }
        }
    }

    public class CellSwap
    {
        public int FromWorldIndex { get; set; }
        public int ToWorldIndex { get; set; }
    }
}
