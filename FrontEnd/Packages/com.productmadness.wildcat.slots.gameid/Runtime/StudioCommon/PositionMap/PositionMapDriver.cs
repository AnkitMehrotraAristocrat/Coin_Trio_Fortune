using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Milan.FrontEnd.Core.v5_1_1.MainDriver;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps
{
    public class PositionMapDriver : MonoBehaviour, ServiceLocator.IHandler, ServiceLocator.IService
    {
        [FieldRequiresParent] private MainDriver _mainDriver = default;
        [FieldRequiresGlobal] private ServiceLocator _serviceLocator = default;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();

            _mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("PositionMaps", DeserializePositionMaps)
                .Then(OnPositionMapsResponse));
        }

        private async Task DeserializePositionMaps(string json, IPayloadWriter payloadWriter)
        {
            PositionMapData[] payload = await JsonUtils.DeserializeObjectAsync<PositionMapData[]>(json);
            payloadWriter.Set(payload);
        }

        private void OnPositionMapsResponse(IPayloadReader payloadReader)
        {
            PositionMapData[] positionMapData = payloadReader.Get<PositionMapData[]>();
            _serviceLocator.GetOrCreate<PositionMapModel>().SetMaps(positionMapData.ToList());
        }
    }
}
