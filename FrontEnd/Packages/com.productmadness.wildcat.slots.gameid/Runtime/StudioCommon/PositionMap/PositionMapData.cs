using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps
{
	public class PositionMapData
	{
		public int FromHeight;
		public int ToHeight;
		public string[] ReelWindows;
		public List<Dictionary<string, WindowPosition>> PositionMaps;
		public List<Dictionary<string, WindowPosition>> ExcludedPositions;
	}
}
