using Milan.FrontEnd.Slots.v5_1_1.WinLine;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace Revolution.Configs
{
    [CustomEditor(typeof(BaseLinesConfig))]
    public class BaseLinesConfigEditor : UnityEditor.Editor
    {
        private string _namedArray;
        public override void OnInspectorGUI()
        {
            _namedArray = GUILayout.TextField(_namedArray);
            if (GUILayout.Button("Add by array") && !string.IsNullOrWhiteSpace(_namedArray))
            {
                var array = _namedArray.Trim();
                var arrayAsJson = JsonConvert.DeserializeObject<Modes>(array);
                var lineConfig = target as BaseLinesConfig;
                lineConfig.modes.Clear();
                lineConfig.modes = arrayAsJson;
                _namedArray = "";
            }

            DrawDefaultInspector();
        }
    }
}
