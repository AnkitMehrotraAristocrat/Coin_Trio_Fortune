using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	public class BasicConditionalSymbolSoundProvider : BaseConditionalSymbolSoundProvider
	{
		public BasicConditionalSymbolSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, SymbolAudioEvent[] audioEvents, bool considerTriggerAudio, BaseEligibilityModifier[] eligibilityModifiers)
			: base(condition, serviceLocator, symbolLocator, audioEvents, considerTriggerAudio, eligibilityModifiers)
		{

		}

		/// <summary>
		/// Returns the first audio event. The intent is that this is a simple
		/// sound provider (the audio event itself can implement what type of source it is).
		/// </summary>
		/// <param name="location"></param>
		/// <param name="groupIndex"></param>
		/// <returns></returns>
		public override SymbolAudioEvent GetAudioEvent(Location location, int groupIndex)
		{
			return _audioEvents[0];
		}

		public override void Reset()
		{
			// Unused
		}
	}
}
