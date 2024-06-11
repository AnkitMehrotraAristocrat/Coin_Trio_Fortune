namespace PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips
{
	public class NextReelStripsClientModelStatePresenter : BaseClientModelStateOptionalPresenter<NextReelStripsServerModel, NextReelStripsClientModel>
	{
		protected override void SetResult()
		{
			_clientModel.ActiveReelStrips.Value = _serverModel.GetNextReelStrips(_betModel.Index.Value);
		}
	}
}
