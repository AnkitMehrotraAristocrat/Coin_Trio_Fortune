using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.WinLinePresentation
{
    public class WinLineModeModel : IModel
    {
        public ReactiveProperty<string> ModeName = new ReactiveProperty<string>();

        public WinLineModeModel(ServiceLocator serviceLocator)
        {
            ModeName.Value = "30";
        }
    }
}
