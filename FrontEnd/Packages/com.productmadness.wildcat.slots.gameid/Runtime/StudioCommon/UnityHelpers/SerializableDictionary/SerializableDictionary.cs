using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// To use you must create a new class derived from this class and create an object
    /// </summary>
    /// <typeparam name="TKey">The key type that will be used in the dictionary</typeparam>
    /// <typeparam name="TValue">The value type that will be used in the dictionary</typeparam>
    public class SerializableDictionarySO<TKey, TValue> : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<KeyValueEntry> entries;
        private List<TKey> keys = new List<TKey>();

        public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey i]
        {
            get
            {
                return dictionary[i];
            }
            set
            {
                dictionary[i] = value;
            }
        }

        [Serializable]
        class KeyValueEntry
        {
            public TKey key;
            public TValue value;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (entries == null)
            {
                return;
            }

            keys.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                keys.Add(entries[i].key);
            }

            var result = keys.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(x => new { Element = x.Key, Count = x.Count() })
                             .ToList();

            if (result.Count > 0)
            {
                var duplicates = string.Join(", ", result);
                GameIdLogger.Logger.Error($"Warning {GetType().FullName} keys has duplicates {duplicates}");
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dictionary.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                dictionary.Add(entries[i].key, entries[i].value);
            }
        }
    }

    /// <summary>
    /// To use you must create a new class derived from this class and attach it to a GameObject
    /// </summary>
    /// <typeparam name="TKey">The key type that will be used in the dictionary</typeparam>
    /// <typeparam name="TValue">The value type that will be used in the dictionary</typeparam>
    public class SerializableDictionary<TKey, TValue> : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private List<KeyValueEntry> entries;
        private List<TKey> keys = new List<TKey>();

        public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey i]
        {
            get
            {
                return dictionary[i];
            }
            set
            {
                dictionary[i] = value;
            }
        }

        [Serializable]
        class KeyValueEntry
        {
            public TKey key;
            public TValue value;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (entries == null)
            {
                return;
            }

            keys.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                keys.Add(entries[i].key);
            }

            var result = keys.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(x => new { Element = x.Key, Count = x.Count() })
                             .ToList();

            if (result.Count > 0)
            {
                var duplicates = string.Join(", ", result);
                GameIdLogger.Logger.Error($"Warning {GetType().FullName} keys has duplicates {duplicates}");
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dictionary.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                dictionary.Add(entries[i].key, entries[i].value);
            }
        }
    }
}