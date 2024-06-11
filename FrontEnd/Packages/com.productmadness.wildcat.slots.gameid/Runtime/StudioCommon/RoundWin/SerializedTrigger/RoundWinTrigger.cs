using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SerializedTrigger
{
	public class RoundWinTrigger : DynamicTrigger<RoundWinTriggerData>
	{
		private RoundWinAmountProvider _roundWinAmountProvider;

		public RoundWinTrigger(RoundWinTriggerData data, ServiceLocator serviceLocator)
			: base(data)
		{
			_roundWinAmountProvider = serviceLocator.transform.root.GetComponentInChildren<RoundWinAmountProvider>(Data.Tag, true);
		}

		public override void OnStateEnter() { }

		public override void OnStateExit() { }

		public override bool IsTriggered()
		{
			return _roundWinAmountProvider.WinAmount > 0;
		}
	}
}
