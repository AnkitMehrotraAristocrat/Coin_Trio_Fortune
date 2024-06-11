using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.WinLinePresentation
{
    class WinLineModeDriver : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresParent] private MainDriver _mainDriver = default;
        [FieldRequiresModel] WinLineModeModel _winLineModeModel = default; 
        private string _lineMode; 

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();

            _mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("LineMode", GetLineModeData)
                .Then(OnResponse));
        }

        private async Task GetLineModeData(string json, MainDriver.IPayloadWriter payloadWriter)
        {
            string lineMode = await JsonUtils.DeserializeObjectAsync<string>(json);
            LineModeJson lineModeData = new LineModeJson() { LineMode = lineMode };
            payloadWriter.Set(lineModeData);
        }

        private void OnResponse(MainDriver.IPayloadReader payloadReader)
        {
            LineModeJson lineModeData = payloadReader.Get<LineModeJson>();
            if (lineModeData != null)
            {
                _winLineModeModel.ModeName.Value = lineModeData.LineMode;
            }
        }
    }

    public class LineModeJson
    {
        public string LineMode;
    }
}
