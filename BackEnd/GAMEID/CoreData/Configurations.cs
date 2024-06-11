using System.Collections.Generic;
using Milan.Common.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System;
using GameBackend.Helpers;

namespace GameBackend.Data
{
    public class Configurations
    {
        public static Dictionary<string, string> Preferences { get; set; }

        public StatePriorityConfiguration StatesExcecutionPriority { get; set; }
        public JackpotConfigData JackpotConfig { get; set; }
        public List<PositionMapsPayload> ClientPositionMaps { get; set; }
        public Dictionary<string, List<string>> ReelSetStrips { get; set; }        

        private readonly IConfigurationProvider _configurationProvider;

        public Configurations(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task InitializeConfigurations()
        {
            Preferences ??= await _configurationProvider.GetConfiguration<PreferenceData>(GameConstants.ConfigPreferences);

            StatesExcecutionPriority ??= await _configurationProvider.GetConfiguration<StatePriorityConfiguration>(GameConstants.ConfigStatePriority);
            JackpotConfig ??= await _configurationProvider.GetConfiguration<JackpotConfigData>(GameConstants.ConfigJackpots);

            if (ClientPositionMaps == null) {
                ClientPositionMaps = new List<PositionMapsPayload>();
                if (GeneralHelper.ConfigExist(GameConstants.ConfigPositionMap)) {
                    var windowPositionMapsData = await _configurationProvider.GetConfiguration<PositionMapsData>(GameConstants.ConfigPositionMap);
                    LoadPositionMapConfiguration(windowPositionMapsData);
                }
                else {
                    GeneratePositionMapConfiguration();
                }
            }
            
            if (ReelSetStrips == null) {
                var reelSetsData = await _configurationProvider.GetConfiguration<ReelSetsData>(GameConstants.ConfigReelSets);
                ReelSetStrips = new Dictionary<string, List<string>>();
                foreach (var reelSet in reelSetsData.ReelSets) {
                    ReelSetStrips.Add(reelSet.Name, reelSet.Reels);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        protected void LoadPositionMapConfiguration(PositionMapsData windowPositionMapsData)
        {
            foreach (var data in windowPositionMapsData.Data) {
                ClientPositionMaps.Add(new() {
                    FromHeight = data.FromHeight,
                    ToHeight = data.ToHeight,
                    ReelWindows = data.ReelWindows,
                    PositionMaps = data.PositionMaps
                });
            }
        }

        protected void GeneratePositionMapConfiguration()
        {
            // TODO need to be revisited when we move from state-based to window-based
            GameStates[] gamesStates = GameConstants.VisualWindowWidthHeight.Keys.ToArray();
            for (int i = 0; i < gamesStates.Count() - 1; i++) {
                for (int j = i + 1; j < gamesStates.Count(); j++) {
                    string fromWindowName = GameConstants.StateReelWindows[gamesStates[i]][0];
                    string toWindowName = GameConstants.StateReelWindows[gamesStates[j]][0];
                    int fromWindowWidth = GameConstants.VisualWindowWidthHeight[gamesStates[i]][0];
                    int fromWindowHeight = GameConstants.VisualWindowWidthHeight[gamesStates[i]][1];
                    int toWindowWidth = GameConstants.VisualWindowWidthHeight[gamesStates[j]][0];
                    int toWindowHeight = GameConstants.VisualWindowWidthHeight[gamesStates[j]][1];
                    for (int frmWinHeight = fromWindowHeight; frmWinHeight <= GameConstants.WindowMaxHeight; frmWinHeight++) {
                        for (int toWinHeight = toWindowHeight; toWinHeight <= GameConstants.WindowMaxHeight; toWinHeight++) {
                            Size fromWindDef = new(fromWindowWidth, frmWinHeight);
                            Size toWindDef = new(toWindowWidth, toWinHeight);
                            if (GameConstants.SingleCellReels[gamesStates[i]] || GameConstants.SingleCellReels[gamesStates[j]]) {
                                GenerateMatrixToSingleCellReelsPositionMaps(fromWindowName, fromWindDef, toWindowName, toWindDef);
                            }
                            else { // case matrix to matrix conversion. eg Base->FreeSpin
                                GenerateMatrixPositionMaps(fromWindowName, fromWindDef, toWindowName, toWindDef);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluate position map where both Window have 2D matrix
        /// Evaluation starts from left-bottom, indicating that layout will map to other from left-bottom and new places are excluded from top or right
        /// 
        /// </summary>
        /// <param name="fromWind"></param>
        /// <param name="fromWindDef"></param>
        /// <param name="toWind"></param>
        /// <param name="toWindDef"></param>
        /// <returns></returns>
        protected void GenerateMatrixPositionMaps(string fromWind, Size fromWindDef, string toWind, Size toWindDef)
        {
            List<Dictionary<string, PositionMapCellData>> data = new();
            List <Dictionary<string, PositionMapCellData>> excluded = new();

            int maxSymbolCount = Math.Max(fromWindDef.Height, toWindDef.Height);
            int maxReelCount = Math.Max(fromWindDef.Width, toWindDef.Width);

            for(int symb = maxSymbolCount, fromWindSymbCount = fromWindDef.Height - 1, toWindSymbCount = toWindDef.Height - 1; symb > 0; symb--, fromWindSymbCount--, toWindSymbCount--) {
                for(int reel = 0; reel < maxReelCount; reel++) {
                    if (fromWindSymbCount > -1 && toWindSymbCount > -1 && reel < fromWindDef.Width && reel < toWindDef.Width) { // Add position common in both windows
                        data.Add(new() {
                            { fromWind , new(reel, fromWindSymbCount) },
                            { toWind , new(reel, toWindSymbCount) }
                        });
                    }
                    else { // position which doesn't have paring
                        if (fromWindSymbCount > -1 || toWindDef.Width < reel) {
                            if (reel < fromWindDef.Width)
                                excluded.Add(new() { { fromWind, new(reel, fromWindSymbCount) } });
                        } else {
                            if(reel < toWindDef.Width)
                                excluded.Add(new() { { toWind, new(reel, toWindSymbCount) } });
                        }
                    }
                }
            }
            ClientPositionMaps.Add(new() {
                FromHeight = fromWindDef.Height,
                ToHeight = toWindDef.Height,
                ReelWindows = new string[] { fromWind, toWind },
                PositionMaps = data,
                ExcludedPositions = excluded
            });
        }

        protected void GenerateMatrixToSingleCellReelsPositionMaps(string fromWind, Size fromWindDef, string toWind, Size toWindDef)
        {
            List<Dictionary<string, PositionMapCellData>> data = new();
            List<Dictionary<string, PositionMapCellData>> excluded = new();

            int totalSingleCellReels, baseSymbolCount, baseReelCount;
            string fWind, tWind; // Identify which is SingleCellReels window
            if (fromWindDef.Height > 1) {
                totalSingleCellReels = toWindDef.Height * toWindDef.Width;
                baseSymbolCount = fromWindDef.Height;
                baseReelCount = fromWindDef.Width;
                fWind = fromWind;
                tWind = toWind;
            }
            else {
                totalSingleCellReels = fromWindDef.Height * fromWindDef.Width;
                baseSymbolCount = toWindDef.Height;
                baseReelCount = toWindDef.Width;
                fWind = toWind;
                tWind = fromWind;
            }

            var state = GameConstants.StateReelWindows.First(rec => rec.Value.Contains(tWind)).Key;
            int windowWidth = GameConstants.VisualWindowWidthHeight[state][0];
            int maxReelCount = Math.Max(baseReelCount, windowWidth);
            int singleCellReelsHeight = totalSingleCellReels / windowWidth;
            int index = 0;
            
            if (baseSymbolCount < singleCellReelsHeight) { // height in SingleCellReels window is greater than height in 'From' Window
                index = (singleCellReelsHeight - baseSymbolCount) * windowWidth;
                for (int i = 0; i < index; i++) {
                    int mapIndex = GeneralHelper.GetClientIndexByWorldIndex(i, singleCellReelsHeight, windowWidth);
                    excluded.Add(new() { { tWind, new(mapIndex, 0) } }); // position which doesn't have paring
                }
            }

            for (int symb = 0; symb < baseSymbolCount; symb++) {
                for (int reel = 0; reel < maxReelCount; reel++) {
                    int mapIndex = GeneralHelper.GetClientIndexByWorldIndex(index, singleCellReelsHeight, windowWidth);
                    if (reel < windowWidth && reel < baseReelCount && symb < singleCellReelsHeight) { // Add position common in both windows
                        data.Add(new() {
                            { fWind , new(reel, symb) },
                            { tWind , new(mapIndex, 0) }
                        });
                        index++;
                    }
                    else { // position which doesn't have paring
                        if (reel < baseReelCount || symb > singleCellReelsHeight) {
                            excluded.Add(new() { { fWind, new(reel, symb) } });
                        }
                        else {
                            excluded.Add(new() { { tWind, new(mapIndex, 0) } });
                            index++;
                        }
                    }
                }
            }

            ClientPositionMaps.Add(new() {
                FromHeight = baseSymbolCount,
                ToHeight = singleCellReelsHeight,
                ReelWindows = new string[] { fWind, tWind },
                PositionMaps = data,
                ExcludedPositions = excluded
            });
        }
    }
}
