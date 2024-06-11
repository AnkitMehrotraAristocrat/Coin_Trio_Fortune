using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public class SetLockingReelsSpinTypeStatePresenter : BaseStatePresenter
	{
		[FieldRequiresModel] private ModalReelsModel _modalReelsModel = default;

		[SerializeField] private string _defaultSpinType = "normal";
		[SerializeField] private LockingReelsSpinTypeProvider _spinTypeProvider;

		protected override void Execute()
		{
			SetSpinType();
		}

		public void SetSpinType()
		{
			string targetSpinType = _spinTypeProvider.GetSpinType();
			if (!string.IsNullOrEmpty(targetSpinType))
			{
				_modalReelsModel.SpinType.Value = targetSpinType;
			}
			else
			{
				_modalReelsModel.SpinType.Value = _defaultSpinType;
			}
		}
	}
}
