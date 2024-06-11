using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class SetSpinTypeStatePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        [FieldRequiresModel] private ModalReelsModel _modalReelsModel = default;

        [SerializeField] private string _targetSpinType = "normal";


        public string Tag => this.GetTag();

        public INotifier Notifier 
        {
            private get; set;
        }

        public void OnServicesLoaded()
		{
            this.InitializeDependencies();
		}

        public IEnumerator<Yield> Enter()
        {
            SetSpinType();
            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

        public void SetSpinType()
        {
            _modalReelsModel.SpinType.Value = _targetSpinType;
        }
    }
}
