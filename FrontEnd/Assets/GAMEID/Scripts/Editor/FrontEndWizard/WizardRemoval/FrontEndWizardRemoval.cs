using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
    public class FrontEndWizardRemoval : EditorWindow
    {
        private static Vector2 _scrollbarPosition = new Vector2();
        private FrontEndWizardRemovalData _wizardRemovalData;
        private MechanicRemovalController removalController;

        /// <summary>
        /// Menu item to display the window.
        /// </summary>
        [MenuItem("Tools/NMG Vegas/Wizard Mechanic Removal")]
        public static void ShowWindow()
        {
            var window = GetWindow<FrontEndWizardRemoval>();
            FrontEndWizardHelper.CenterOnMainWin(window, 2.0f, 2.0f);
            window.Show();
        }

        public void OnEnable()
        {
            Initialize();
        }

        /// <summary>
        /// Main OnGUI loop.
        /// </summary>
        public void OnGUI()
        {
            GUIStyle style = FrontEndWizardHelper.GetDefaultStyle();
            _scrollbarPosition = GUILayout.BeginScrollView(_scrollbarPosition, style);

            ShowHeader();
            ShowExecutorOptions();
            ShowRemoveAllButton();
            ShowCloseButton();

            GUILayout.EndScrollView();
        }

        private void Initialize()
        {
            if (!FrontEndWizardHelper.GetWizardRemovalData(out _wizardRemovalData))
            {
                Debug.LogError("Wizard removal data file missing!");
                return;
            }

            removalController = new MechanicRemovalController();
        }

        private void ShowHeader()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 40;
            style.fixedHeight = 40;
            style.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("FRONT END WIZARD REMOVAL", style, GUILayout.ExpandWidth(true), GUILayout.MinHeight(40));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        private void ShowRemoveAllButton()
        {
            if (GUILayout.Button("RemoveAll"))
            {
                RemoveAll();
            }
        }

        private void ShowExecutorOptions()
        {
            foreach (MechanicConfiguration configuration in _wizardRemovalData.MechanicConfigurations)
            {
                string name = configuration.name;

                if (GUILayout.Button("Remove " + name))
                {
                    if (EditorUtility.DisplayDialog("Remove Mechanic Confirmation", "Ready to remove " + name + "?", "Yep!", "Nah.."))
                    {
                        removalController.Remove(configuration);
                    }
                }
            }
        }

        private void ShowCloseButton()
        {
            GUILayout.Space(5f);
            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        public void RemoveAll()
        {
            Initialize();
            removalController.Remove(_wizardRemovalData.MechanicConfigurations);
        }
    }
}