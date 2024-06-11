using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class WinMeterPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        public enum ShowOrHide
        {
            Show,
            Hide
        }

        [FieldRequiresModel] private WinMeterModel _winMeterModel = default;
        [FieldRequiresChild] private WinMeterView _winMeterView;

        [SerializeField] private ShowOrHide _showOrHide;

        public string Tag => this.GetTag();

        public INotifier Notifier {
            get; set;
        }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public IEnumerator<Yield> Enter()
        {
            Coroutine.Start(PlayWinMeterAnim());
            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

        private IEnumerator<Yield> PlayWinMeterAnim()
        {
            if (_showOrHide == ShowOrHide.Show)
			{
                yield return Coroutine.Start(_winMeterView.PlayShowAnim());
            }
            else if (_showOrHide == ShowOrHide.Hide)
			{
                yield return Coroutine.Start(_winMeterView.PlayHideAnim());
                _winMeterModel.WinAmount.Value = 0;
			}
        }
    }
}
