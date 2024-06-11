using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// This can be created and modified in the project
    /// </summary>
    [Preserve]
    [Serializable, CreateAssetMenu(fileName = "TestDictionary", menuName = "NMG/SerializableDictionaries/TestDictionary")]
    public class TestSerializableDictionarySO : SerializableDictionarySO<int, List<int>>
    {
    }
}