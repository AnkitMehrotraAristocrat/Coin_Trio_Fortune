using System.Collections.Generic;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using TMPro;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Presenter meant to clear a text mesh pro text.
    /// </summary>
    public class ClearTextPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {
        #region Inspector

        public TextMeshPro Text;
        public StateExecutionTime ExecutionTime;

        #endregion

        #region Interface Implementation

        public string Tag => this.GetTag();
        public INotifier Notifier { private get; set; }


        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        #endregion

        #region State Handling

        public IEnumerator<Yield> Enter()
        {
            if (ExecutionTime == StateExecutionTime.Enter || ExecutionTime == StateExecutionTime.Both)
            {
                Text.text = "";
            }

            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            if (ExecutionTime == StateExecutionTime.Exit || ExecutionTime == StateExecutionTime.Both)
            {
                Text.text = "";
            }

            yield break;
        }

        #endregion
    }
}
