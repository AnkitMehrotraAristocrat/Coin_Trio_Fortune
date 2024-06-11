using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps
{
    public class PositionMapModel : IModel
    {
		private Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<WindowPosition, WindowPosition>>>>> reelWindowMaps 
			= new Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<WindowPosition, WindowPosition>>>>>();

		public PositionMapModel(ServiceLocator serviceLocator) 
		{ 
		}

		public void SetMaps(List<PositionMapData> maps)
        {
			foreach(var map in maps)
            {
				var reelWindows = map.ReelWindows;

				foreach (var reelWindow in reelWindows)
                {
					if(!reelWindowMaps.ContainsKey(reelWindow))
                    {
						reelWindowMaps.Add(reelWindow, new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<WindowPosition, WindowPosition>>>>());
					}
				}

				var reelWindowA = reelWindows[0];
				var reelWindowB = reelWindows[1];

				foreach (var positionMap in map.PositionMaps)
				{			
					if(!reelWindowMaps[reelWindowA].ContainsKey(reelWindowB))
                    {
						reelWindowMaps[reelWindowA].Add(reelWindowB, new Dictionary<int, Dictionary<int, Dictionary<WindowPosition, WindowPosition>>>());
					}
					
					if(!reelWindowMaps[reelWindowB].ContainsKey(reelWindowA))
                    {
						reelWindowMaps[reelWindowB].Add(reelWindowA, new Dictionary<int, Dictionary<int, Dictionary<WindowPosition, WindowPosition>>>());
					}

					if(!reelWindowMaps[reelWindowA][reelWindowB].ContainsKey(map.FromHeight))
                    {
						reelWindowMaps[reelWindowA][reelWindowB].Add(map.FromHeight, new Dictionary<int, Dictionary<WindowPosition, WindowPosition>>());
					}

					if (!reelWindowMaps[reelWindowA][reelWindowB][map.FromHeight].ContainsKey(map.ToHeight))
					{
						reelWindowMaps[reelWindowA][reelWindowB][map.FromHeight].Add(map.ToHeight, new Dictionary<WindowPosition, WindowPosition>(new WindowPosition.EqualityComparer()));
					}

					if (!reelWindowMaps[reelWindowB][reelWindowA].ContainsKey(map.FromHeight))
					{
						reelWindowMaps[reelWindowB][reelWindowA].Add(map.FromHeight, new Dictionary<int, Dictionary<WindowPosition, WindowPosition>>());
					}

					if (!reelWindowMaps[reelWindowB][reelWindowA][map.FromHeight].ContainsKey(map.ToHeight))
					{
						reelWindowMaps[reelWindowB][reelWindowA][map.FromHeight].Add(map.ToHeight, new Dictionary<WindowPosition, WindowPosition>(new WindowPosition.EqualityComparer()));
					}

					var windowPositionA = positionMap[reelWindowA];
					var windowPositionB = positionMap[reelWindowB];

					reelWindowMaps[reelWindowA][reelWindowB][map.FromHeight][map.ToHeight][windowPositionA] = windowPositionB;
					reelWindowMaps[reelWindowB][reelWindowA][map.FromHeight][map.ToHeight][windowPositionB] = windowPositionA;
				}
			}
		}

		public WindowPosition GetPositionMap(string sourceWindow, string targetWindow, int fromHeight, int toHeight, WindowPosition sourceWindowPosition)
        {
			return reelWindowMaps[sourceWindow][targetWindow][fromHeight][toHeight][sourceWindowPosition];
        }
	}
}
