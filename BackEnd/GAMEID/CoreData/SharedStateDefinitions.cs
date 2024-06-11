using Milan.Common.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Data
{
    public class StatePriorityConfiguration : IConfiguration
    {
        public Dictionary<string, int> StatePriority { get; set; }

        public object Clone() => Duplicate();

        public IConfiguration Duplicate()
        {
            return new StatePriorityConfiguration {
                StatePriority = StatePriority?.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value
                )
            };
        }
    }

    public class TriggeredStateInfo
    {
        public StatePriorityQueue Queue { get; set; } = new();
    }

    public class CompareState: IComparer<KeyValuePair<int, StateInfo>>
    {
        public int Compare(KeyValuePair<int, StateInfo> x, KeyValuePair<int, StateInfo> y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }

    public class StatePriorityQueue
    {
        public List<KeyValuePair<int, StateInfo>> Queue { get; set; }
        private readonly CompareState CS;

        public StatePriorityQueue()
        {
            Queue = new List<KeyValuePair<int, StateInfo>>();
            CS = new CompareState();
        }

        public void Enqueue(int priority, string value)
        {
            Queue.Add(new KeyValuePair<int, StateInfo>(priority, new StateInfo(value)));
            Queue.Sort(CS);
        }

        public StateInfo Dequeue()
        {
            //Always keep one valid state e.g. main
            if (Queue.Count > 1) {
                StateInfo value = Queue[0].Value;
                Queue.RemoveAt(0);
                return value;
            }
            throw new ArgumentException(GameConstants.ErrorStateQueueDequeue);
        }

        public StateInfo Peek()
        {
            if (Queue.Count <= 0) {
                throw new ArgumentException(GameConstants.ErrorStateQueuePeek);
            }
            return Queue[0].Value;
        }

        public int Count()
        {
            return Queue.Count;
        }
    }

    public class StateInfo
    {
        public string State { get; set; }

        public StateInfo(string name)
        {
            State = name;
        }
    }
}
