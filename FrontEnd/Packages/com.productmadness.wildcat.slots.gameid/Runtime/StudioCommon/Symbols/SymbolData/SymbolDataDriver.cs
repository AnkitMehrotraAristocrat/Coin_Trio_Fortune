using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    public class SymbolDataDriver : MonoBehaviour, ServiceLocator.IHandler
    {
        [FieldRequiresParent] private MainDriver _mainDriver = default;
        [FieldRequiresGlobal] private ServiceLocator _serviceLocator = default;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            GenerateModels();

            _mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("SymbolOutcomePayload", DeserializeSymbolOutcomeData)
                .Then(OnResponse));

            _mainDriver.RegisterPayloads(new MainDriver.PayloadRegistrator()
                .Add("SymbolSpinningPayload", DeserializeSymbolSpinningData)
                .Then(OnSymbolSpinningResponse));
        }

        private void GenerateModels()
        {
            var skinConfigs = transform.root.GetComponentsInChildren<SymbolDataAssigner>()
                .SelectMany(assigner => assigner.SkinConfigs);

            // Loop through the _symbolSkinConfigs and create a new model using the symbolId as the model's tag
            foreach (SymbolSkinConfig skinConfig in skinConfigs)
            {
                _serviceLocator.GetOrCreate<SymbolOutcomeModel>();
            }

        }

        private async Task DeserializeSymbolOutcomeData(string json, MainDriver.IPayloadWriter payloadWriter)
        {
            SymbolOutcomePayload symbolOutcomePayload = await JsonUtils.DeserializeObjectAsync<SymbolOutcomePayload>(json);
            payloadWriter.Set(symbolOutcomePayload);
        }

        private void OnResponse(MainDriver.IPayloadReader payloadReader)
        {
            SymbolOutcomePayload[] symbolOutcomesPayload = payloadReader.GetAll<SymbolOutcomePayload>();
            if (_serviceLocator.TryGet(out SymbolOutcomeModel model))
            {
                foreach (SymbolOutcomePayload symbolOutcomePayload in symbolOutcomesPayload)
                {
                    string gamestate = symbolOutcomePayload.Id.ToString();
                    var symboldataByLocation = new Dictionary<Location, SymbolOutcomeData>();

                    foreach (SymbolOutcomeData symbolOutcomeData in symbolOutcomePayload.SymbolOutcomeData)
                    {
                        Location location = new Location()
                        {
                            colIndex = symbolOutcomeData.PositionData.X,
                            rowIndex = symbolOutcomeData.PositionData.Y
                        };      

                        symboldataByLocation.Add(location, symbolOutcomeData);
                    }
                    Enum.TryParse(gamestate, out GameStateEnum stateEnum);
                    model.SetSymbolData(stateEnum, symboldataByLocation);
                }
            }
        }

        private async Task DeserializeSymbolSpinningData(string json, MainDriver.IPayloadWriter payloadWriter)
        {
            SymbolSpinningData symbolSpinningData = await JsonUtils.DeserializeObjectAsync<SymbolSpinningData>(json);
            payloadWriter.Set(symbolSpinningData);
        }

        private void OnSymbolSpinningResponse(MainDriver.IPayloadReader payloadReader)
        {
            SymbolSpinningData[] symbolSpinningPayloads = payloadReader.GetAll<SymbolSpinningData>();

            foreach (SymbolSpinningData symbolSpinningPayload in symbolSpinningPayloads)
            {
                SymbolSpinningModel symbolSpinningModel = _serviceLocator.GetOrCreate<SymbolSpinningModel>(symbolSpinningPayload.Id);
                foreach (SymbolSpinningIdData dataById in symbolSpinningPayload.Data)
                {
                    foreach (SymbolSpinningGameStateData data in dataById.Data)
                    {
                        foreach (string gameState in data.GameStates)
                        {
                            Enum.TryParse(gameState, out GameStateEnum stateEnum);
                            symbolSpinningModel.SetDummySymbolData(dataById.SymbolId, stateEnum, data.Data.ToList());
                        }
                    }
                }
            }
        }
    }
}
