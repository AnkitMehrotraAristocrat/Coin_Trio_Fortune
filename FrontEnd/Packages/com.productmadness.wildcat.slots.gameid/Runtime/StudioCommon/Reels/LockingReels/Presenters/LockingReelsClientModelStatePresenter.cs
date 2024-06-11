namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
    public class LockingReelsClientModelStatePresenter : BaseClientModelStateOptionalPresenter<LockingReelsServerModel, LockingReelsClientModel>
    {
        protected override void SetResult()
        {
            // syncs the data from the tagged server model to the tagged client model
            _clientModel.SetLockedReels(_serverModel.LockedReels);
        }
    }
}