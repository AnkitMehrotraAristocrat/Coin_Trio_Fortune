
namespace GameBackend
{
    /// <summary>
    /// Add all steps that consume random numbers here
    /// In each step call Context.PersistentData.GaffeQueues.ConsumeCategoryQueue() 
    /// See ../Configuration/Gaffes.cs for examples
    /// </summary>
    public enum GaffeCategories
    {
        SelectReelSet,
        ReplaceSymbolsOnReels,
        GenerateSetOfRandomValues,
        DetermineCorSymbolPrizes,
        GenerateWindowWithStops
    }
}
