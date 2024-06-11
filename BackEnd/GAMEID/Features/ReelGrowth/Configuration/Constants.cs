
namespace GameBackend.Features.ReelGrowth.Configuration
{
    public static class Constants
    {
        // Reel growth is responsible for setting GameContext.HiddenWindowCells per spin, allowing it to accommodate any type of growth scenario 
        // Game-scaffolding initializes GameContext.HiddenWindowCells based on the active reel window's size, compared to the world window

        // Example implementation based on growth steps, step increment per landed GROWING symbol
        public const string PayloadName = "ReelGrowth";
        public static readonly string[] GrowthSymbols = { "GROWING" };

        // See GameConstants WindowMaxWidth/WindowMaxHeight to understand below flags, reel growth, and the world window
        public const int MaxGrowthStep = 4;

        // In this example all growth steps result in no new cells being exposed 
        public static readonly bool[,] GrowthScenariosHiddenCells = new bool[MaxGrowthStep, GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight];

        // You can, for example, implement 4-Growth-Steps on a 5-Column by 6-Row layout as shown below
        // See CheckForGrowth and SetHiddenCells in this features 'Steps' folder to understand how this works
        // If you use this example layout don't forget to set GameConstants.WindowMaxWidth and GameConstants.WindowMaxHeight appropriately
        // If you have alternate conditions that cause growth you will need to edit the CheckForGrowth step, and possibly add/remove constants here
        /*
        public static readonly bool[,] GrowthScenariosHiddenCells = new bool[MaxGrowthStep, GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight] {
            {
                true, true, true, true, true,
                true, true, true, true, true,
                true, true, true, true, true,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false
            },
            {
                true, true, true, true, true,
                true, true, true, true, true,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false
            },
            {
                true, true, true, true, true,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false
            },
            {
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false,
                false, false, false, false, false
            }
        }
        */
    }
}
