using System.Collections.Generic;

namespace GameBackend.Features.ReelSets.Data
{
    public class NextReelStripsWindowsData
    {
        // Dictionary<{GameState}, Dictionary<{WindowId}, NextReelStripsWindowData>>
        public Dictionary<string, Dictionary<string, NextReelStripsWindowData>> WindowData { get; set; }

        public NextReelStripsWindowsData()
        {
            WindowData = new Dictionary<string, Dictionary<string, NextReelStripsWindowData>>();
        }

        public int GetCount()
        {
            return WindowData != null ? WindowData.Count : 0;
        }
    }
}
