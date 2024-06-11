using System;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.RotaryHeart.Lib.AutoComplete
{
    public class AutoCompleteAttribute : PropertyAttribute
    {
        string[] m_entries;

        public string[] Entries
        {
            get { return m_entries; }
        }

        public AutoCompleteAttribute(string[] entries)
        {
            m_entries = entries;
        }

        public AutoCompleteAttribute(Type type, string methodName)
        {
            var method = type.GetMethod(methodName);
            if (method != null)
            {
                m_entries = method.Invoke(null, null) as string[];
            }
            else
            {
                GameIdLogger.Logger.Error($"There is no method with name '{methodName}' in type '{type}' ");
            }
        }

    }
}