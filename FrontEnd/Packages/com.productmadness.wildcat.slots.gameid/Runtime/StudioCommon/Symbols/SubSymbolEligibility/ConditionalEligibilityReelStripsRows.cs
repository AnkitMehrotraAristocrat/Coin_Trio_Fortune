using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Slotsburg.Slots.SharedFeatures;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class ConditionalEligibilityReelStripsRows : UsableReelStripsRows
	{
		[FieldRequiresModel] private SubSymbolEligibilityModel _subSymbolEligibilityModel;

		protected override bool CanUseReelStripRow(RootReelView rootReelView, int reelId, int rowIndex)
		{
			return _subSymbolEligibilityModel.IsPositionEligible(reelId, rowIndex);
		}
	}
}
