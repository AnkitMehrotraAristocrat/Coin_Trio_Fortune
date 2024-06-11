namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
	public class DynamicSymbolClientModelStatePresenter : BaseClientModelStateOptionalPresenter<DynamicSymbolServerModel, DynamicSymbolClientModel>
	{
        protected override void SetResult()
        {
            _clientModel.SetDynamicSymbolReplacements(_serverModel.DynamicSymbolReplacements.Value);
        }
    }
}
