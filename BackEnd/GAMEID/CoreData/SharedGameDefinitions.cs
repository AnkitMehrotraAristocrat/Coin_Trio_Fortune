using GameBackend.Helpers;
using Milan.Common.Interfaces.Entities;
using System.Collections.Generic;

namespace GameBackend.Data
{
    public class PreferenceData : Dictionary<string, string>, IConfiguration
    {
        public object Clone() => Duplicate();

        public IConfiguration Duplicate()
        {
            return (IConfiguration)MemberwiseClone();
        }
    }

    public class TransitionData
    {
        public string FromState { get; private set; }
        public string ToState { get; private set; }

        public TransitionData(string fromState, string toState)
        {
            FromState = fromState;
            ToState = toState;
        }

        public TransitionData(GameStates fromState, GameStates toState)
        {
            FromState = GeneralHelper.GetGameStateString(fromState);
            ToState = GeneralHelper.GetGameStateString(toState);
        }
    }

    public class WonAwardViaFeature
    {
        public string Name { get; set; }
        public ulong Value { get; set; }
    }
}
