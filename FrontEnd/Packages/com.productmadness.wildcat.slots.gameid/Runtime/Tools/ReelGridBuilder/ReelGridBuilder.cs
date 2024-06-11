#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Animations;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.WinLine;
using Slotsburg.Slots.SharedFeatures.Clipping;
using System.Reflection;
using Milan.FrontEnd.Bridge.Logging;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
    [CustomEditor(typeof(ReelGridBuilder))]
    public class ReelGridBuilderEditor : Editor
    {
        private float headerSpacing = 20f;
        SerializedProperty columnHeights;

        // Game Objects
        SerializedProperty reelView;
        SerializedProperty symbols;
        SerializedProperty symbol;
        SerializedProperty winLineSymbol;
        SerializedProperty winLines;
        SerializedProperty reelMask;
        SerializedProperty reelGridConfiguration;
        SerializedProperty holdAndSpinReelRenderOrder;

        void OnEnable()
        {
            columnHeights = serializedObject.FindProperty("columnHeights");
            reelView = serializedObject.FindProperty("reelViewObject");
            symbols = serializedObject.FindProperty("symbolsObject");
            symbol = serializedObject.FindProperty("symbolObject");
            winLineSymbol = serializedObject.FindProperty("winLineSymbolObject");
            winLines = serializedObject.FindProperty("winLinesObject");
            reelMask = serializedObject.FindProperty("reelMaskMaterial");
            reelGridConfiguration = serializedObject.FindProperty("reelGridConfiguration");
            holdAndSpinReelRenderOrder = serializedObject.FindProperty("holdAndSpinReelRenderOrder");
        }

        //ReelGridConfiguration reelGridConfiguration;
        //HoldAndSpinReelRenderOrder holdAndSpinReelRenderOrder;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var reelGridBuilder = target as ReelGridBuilder;

            // Reel Grid Parameters
            GUILayout.Label("REEL GRID", EditorStyles.boldLabel);
            if (reelGridBuilder.supportAsymmetricalReels == false)
            {
                Vector2Int reelMatrix = EditorGUILayout.Vector2IntField("Columns, Rows", new Vector2Int(reelGridBuilder.columns, reelGridBuilder.rows));
                reelGridBuilder.columns = reelMatrix.x;
                reelGridBuilder.rows = reelMatrix.y;
            }
            else
            {
                reelGridBuilder.reelHeightsFoldout = EditorGUILayout.Foldout(reelGridBuilder.reelHeightsFoldout, "Reel Heights", true, EditorStyles.foldout);
                if (reelGridBuilder.reelHeightsFoldout)
                {
                    ShowArrayProperty(columnHeights, "Reel Count", "Reel");
                }
            }
            reelGridBuilder.supportAsymmetricalReels = EditorGUILayout.Toggle("Change Height per Reel", reelGridBuilder.supportAsymmetricalReels);

            reelGridBuilder.supportStackedSymbols = EditorGUILayout.Toggle("Stacked Symbols Parameters", reelGridBuilder.supportStackedSymbols);
            if (reelGridBuilder.supportStackedSymbols)
            {
                EditorGUI.indentLevel++;
                reelGridBuilder.maskedRowsAbove = EditorGUILayout.IntField("Masked Rows Above Reels", reelGridBuilder.maskedRowsAbove);
                reelGridBuilder.maskedRowsBelow = EditorGUILayout.IntField("Masked Rows Below Reels", reelGridBuilder.maskedRowsBelow);
                EditorGUI.indentLevel--;
            }

            reelGridBuilder.supportExpandingReels = EditorGUILayout.Toggle("Expanding Reels Parameters", reelGridBuilder.supportExpandingReels);
            if (reelGridBuilder.supportExpandingReels)
            {
                EditorGUI.indentLevel++;
                reelGridBuilder.visibleRows = EditorGUILayout.IntField("Default Visible Reel Height", reelGridBuilder.visibleRows);
                EditorGUI.indentLevel--;
            }

            reelGridBuilder.supportPaylines = EditorGUILayout.Toggle("Generate Paylines Objects", reelGridBuilder.supportPaylines);

            // Spacing
            GUILayout.Space(headerSpacing);
            GUILayout.Label("SPACING", EditorStyles.boldLabel);

            Vector2 symbolSize = EditorGUILayout.Vector2Field("Symbol Size (Width, Height)", new Vector2(reelGridBuilder.symbolWidth, reelGridBuilder.symbolHeight));
            reelGridBuilder.symbolWidth = symbolSize.x;
            reelGridBuilder.symbolHeight = symbolSize.y;

            Vector2 symbolDividers = EditorGUILayout.Vector2Field("Symbol Dividers", new Vector2(reelGridBuilder.symbolDividerX, reelGridBuilder.symbolDividerY));
            reelGridBuilder.symbolDividerX = symbolDividers.x;
            reelGridBuilder.symbolDividerY = symbolDividers.y;

            reelGridBuilder.symbolMaskTexture = (Sprite)EditorGUILayout.ObjectField("Sprite Mask Texture (Optional)", reelGridBuilder.symbolMaskTexture, typeof(Sprite), false);
            reelGridBuilder.symbolPreviewTexture = (Sprite)EditorGUILayout.ObjectField("Symbol Preview Sprite (Optional)", reelGridBuilder.symbolPreviewTexture, typeof(Sprite), false);

            // References
            GUILayout.Space(headerSpacing);
            GUILayout.Label("REFERENCES", EditorStyles.boldLabel);

            reelGridBuilder.reelViewObject = (GameObject)EditorGUILayout.ObjectField("Reel View Object", reelGridBuilder.reelViewObject, typeof(GameObject), false);
            reelGridBuilder.symbolsObject = (GameObject)EditorGUILayout.ObjectField("Symbols Object", reelGridBuilder.symbolsObject, typeof(GameObject), false);
            reelGridBuilder.symbolObject = (GameObject)EditorGUILayout.ObjectField("Symbol Object", reelGridBuilder.symbolObject, typeof(GameObject), false);
            if (reelGridBuilder.supportPaylines)
            {
                reelGridBuilder.winLineSymbolObject = (GameObject)EditorGUILayout.ObjectField("Win Line Symbol Object", reelGridBuilder.winLineSymbolObject, typeof(GameObject), false);
                reelGridBuilder.winLinesObject = (GameObject)EditorGUILayout.ObjectField("Win Lines Object", reelGridBuilder.winLinesObject, typeof(GameObject), false);
            }
            reelGridBuilder.reelMaskMaterial = (Material)EditorGUILayout.ObjectField("Reel Mask Material", reelGridBuilder.reelMaskMaterial, typeof(Material), false);

            // Build Parameters
            GUILayout.Space(headerSpacing);
            GUILayout.Label("BUILD", EditorStyles.boldLabel);

            // Reel Spin Animation Generator Variables
            reelGridBuilder.generateReelSpinAnimations = EditorGUILayout.Toggle("Generate Reel Spin Animations", reelGridBuilder.generateReelSpinAnimations);
            if (reelGridBuilder.generateReelSpinAnimations)
            {
                EditorGUI.indentLevel++;
                reelGridBuilder.windupDistance = EditorGUILayout.FloatField("Windup Distance", reelGridBuilder.windupDistance);
                reelGridBuilder.bounceDistance = EditorGUILayout.FloatField("Bounce Distance", reelGridBuilder.bounceDistance);
                reelGridBuilder.reelSpeed = EditorGUILayout.FloatField("Reel Speed", reelGridBuilder.reelSpeed);
                reelGridBuilder.animationName = EditorGUILayout.TextField("Animation Name", reelGridBuilder.animationName);
                reelGridBuilder.useExistingController = EditorGUILayout.Toggle("Use Existing Controller", reelGridBuilder.useExistingController);
                if (reelGridBuilder.useExistingController)
                {
                    EditorGUI.indentLevel++;
                    reelGridBuilder.reelController = (AnimatorController)EditorGUILayout.ObjectField("Reel Controller", reelGridBuilder.reelController, typeof(AnimatorController), false);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            //reelGridConfiguration = (ReelGridConfiguration)EditorGUILayout.EnumPopup("Reel Configuration Type", reelGridConfiguration);
            EditorGUILayout.PropertyField(reelGridConfiguration, new GUIContent("Reel Configuration Type"));
            if (reelGridConfiguration.intValue == 1) // Hold and Spin
            {
                EditorGUI.indentLevel++;
                //holdAndSpinReelRenderOrder = (HoldAndSpinReelRenderOrder)EditorGUILayout.EnumPopup("Hold And Spin Reel Render Order", holdAndSpinReelRenderOrder);
                EditorGUILayout.PropertyField(holdAndSpinReelRenderOrder);
                EditorGUI.indentLevel--;
            }
            if (GUILayout.Button("Build"))
            {
                //ReelGridBuilder myReelGridBuilder = (ReelGridBuilder)target;
                ReelGridConfiguration index = (ReelGridConfiguration)reelGridConfiguration.intValue;
                switch (index)
                {
                    case ReelGridConfiguration.Standard:
                        reelGridBuilder.BuildReelGrid(ReelGridConfiguration.Standard);
                        break;
                    case ReelGridConfiguration.HoldAndSpin:
                        if (holdAndSpinReelRenderOrder.intValue == 0)
                            reelGridBuilder.leftToRight = true;
                        else
                            reelGridBuilder.leftToRight = false;
                        reelGridBuilder.BuildReelGrid(ReelGridConfiguration.HoldAndSpin);
                        break;
                    case ReelGridConfiguration.GhostReels:
                        reelGridBuilder.BuildReelGrid(ReelGridConfiguration.GhostReels);
                        break;
                }
            }

            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        // Show Custom Array GUI
        public void ShowArrayProperty(SerializedProperty arrayProperty, string label, string childLabel)
        {
            EditorGUILayout.PropertyField(arrayProperty.FindPropertyRelative("Array.size"), new GUIContent(label));

            EditorGUI.indentLevel++;
            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                EditorGUILayout.PropertyField(arrayProperty.GetArrayElementAtIndex(i),
                                              new GUIContent(childLabel + " " + (i + 1).ToString())
                                             );
            }
            EditorGUI.indentLevel--;
        }
    }

    public enum ReelGridConfiguration
    {
        Standard = 0,
        HoldAndSpin,
        GhostReels
    }

    public enum HoldAndSpinReelRenderOrder
    {
        LeftToRight = 0,
        TopToBottom
    }

    public class ReelGridBuilder : MonoBehaviour
    {
        // Reel Grid Parameters
        [Tooltip("Enabling Paylines support will generate Locator game objects on either side of a reel grid to draw paylines beyond the reel grid.")]
        [HideInInspector] public bool supportPaylines;
        [HideInInspector] public int rows = 4, columns = 5;
        [Tooltip("Enabling Asymmetrical Reels support will ignore rows/columns and instead use Column Heights.")]
        [HideInInspector] public bool supportAsymmetricalReels = false;
        [Tooltip("Set array size to the number of columns. Set value of each element to the column height.")]
        [HideInInspector] public int[] columnHeights;
        [Tooltip("Default value is 1. Change value if move masked symbols should display above and below the reel window (e.g. Stacked symbols).")]
        [HideInInspector] public bool supportStackedSymbols = false;
        [HideInInspector] public int maskedRowsAbove = 1, maskedRowsBelow = 1;
        [Tooltip("If your game does not change the reel height during game play, set this to the same value as rows, otherwise, set this to the number of symbols the reels show when starting the game.")]
        [HideInInspector] public bool supportExpandingReels;
        [HideInInspector] public int visibleRows;

        // Spacing
        [HideInInspector] public float symbolWidth = 256, symbolHeight = 256, symbolDividerX, symbolDividerY;
        [Tooltip("Optional. Sprite to assign to Sprite Mask component.")]
        [HideInInspector] public Sprite symbolMaskTexture;
        [Tooltip("Optional. Texture symbols will display in the Editor. For preview purposes only; will not be displayed in-game.")]
        [HideInInspector] public Sprite symbolPreviewTexture;
        // [Tooltip("Mask shape applied to each Reel. Alpha is trimmed automatically by Unity; a square-texture is recommended in most cases.")]
        // public Sprite maskShapeTexture;

        // References
        [HideInInspector] public GameObject reelViewObject;
        [HideInInspector] public GameObject symbolsObject;
        [HideInInspector] public GameObject symbolObject;
        [HideInInspector] public GameObject winLineSymbolObject;
        [HideInInspector] public GameObject winLinesObject;
        [HideInInspector] public Material reelMaskMaterial;
        [HideInInspector] public ReelGridConfiguration reelGridConfiguration;
        [HideInInspector] public HoldAndSpinReelRenderOrder holdAndSpinReelRenderOrder;

        private const int pixelsPerUnit = 100; // PPU for the Camera and Sprite assets; Affects any transform operations

        private float totalWidth; // Total Width: Spacing between symbols along the X-Axis
        private float totalHeight; // Total Height: Spacing between symbols along the Y-Axis
        private List<int> columnHeightsList;
        private int maxColumnHeight;

        // Hold and Spin variables
        //private string holdAndSpinSortingGroupName = "Symbols & Reels";
        [HideInInspector] public bool leftToRight;

        // Build
        [HideInInspector] public bool generateReelSpinAnimations = true;
        [HideInInspector] public float windupDistance = 0.5f;
        [HideInInspector] public float bounceDistance = 0.6f;
        [HideInInspector] public float reelSpeed = 1f;
        [HideInInspector] public string animationName = "Standard";
        [HideInInspector] public bool useExistingController = false;
        [HideInInspector] public AnimatorController reelController;

        // Foldout Bools
        [HideInInspector] public bool reelHeightsFoldout = false;

        public void InitializeVariables()
        {
            totalWidth = symbolWidth + symbolDividerX;
            totalHeight = symbolHeight + symbolDividerY;

            if (supportStackedSymbols == false)
            {
                maskedRowsAbove = 1;
                maskedRowsBelow = 1;
            }

            columnHeightsList = new List<int>();
            if (supportAsymmetricalReels)
            {
                for (int i = 0; i < columnHeights.Length; ++i)
                {
                    columnHeightsList.Add(columnHeights[i] + maskedRowsAbove + maskedRowsBelow);
                }
            }
            else
            {
                for (int i = 0; i < columns; ++i)
                {
                    columnHeightsList.Add(rows + maskedRowsAbove + maskedRowsBelow);
                }
            }
            maxColumnHeight = columnHeightsList.Max();
        }

        public void InitializeHoldAndSpinVariables()
        {
            supportAsymmetricalReels = false;

            totalWidth = symbolWidth + symbolDividerX;
            totalHeight = symbolHeight + symbolDividerY;

            if (supportStackedSymbols == false)
            {
                maskedRowsAbove = 1;
                maskedRowsBelow = 1;
            }

            columnHeightsList = new List<int>();
            if (supportAsymmetricalReels)
            {
                for (int i = 0; i < columnHeights.Length; ++i)
                {
                    columnHeightsList.Add(columnHeights[i] + maskedRowsAbove + maskedRowsBelow);
                }
            }
            else
            {
                for (int i = 0; i < columns * rows; ++i)
                {
                    columnHeightsList.Add(1 + maskedRowsAbove + maskedRowsBelow);
                }
            }
            maxColumnHeight = columnHeightsList.Max();
        }

        public void BuildReelGrid(ReelGridConfiguration reelConfig)
        {
            RemoveChildren();

            switch (reelConfig)
            {
                case ReelGridConfiguration.Standard:
                    GenerateReelGrid();
                    break;
                case ReelGridConfiguration.HoldAndSpin:
                    GenerateHoldAndSpinReels();
                    break;
                case ReelGridConfiguration.GhostReels:
                    GenerateReelGrid();
                    break;
                default:
                    GenerateReelGrid();
                    break;
            }

            if (reelConfig == ReelGridConfiguration.Standard)
            {
            }

            if (reelConfig == ReelGridConfiguration.HoldAndSpin)
            {
                // if (gameObject.GetComponent<SortingGroup>() == null)
                // {
                //     Undo.AddComponent<SortingGroup>(this.transform.gameObject);
                //     StaticLogForwarder.Logger.Log("Sorting Group added to '" + this.name + "' Game Object. Sorting Layer may need to be set for Reels to be visible.");
                // }
            }

            if (reelConfig == ReelGridConfiguration.GhostReels)
            {
                // if (gameObject.GetComponent<SortingGroup>() == null)
                // {
                //     Undo.AddComponent<SortingGroup>(this.transform.gameObject);
                //     StaticLogForwarder.Logger.Log("Sorting Group added to '" + this.name + "' Game Object. Sorting Layer may need to be set for Reels to be visible.");
                // }
            }

            if (generateReelSpinAnimations)
            {
                ReelSpinAnimationGenerator reelSpinAnimationGenerator = ScriptableObject.CreateInstance<ReelSpinAnimationGenerator>();

                // Initialize
                reelSpinAnimationGenerator.symbolHeight = symbolHeight;
                reelSpinAnimationGenerator.windupDistance = windupDistance;
                reelSpinAnimationGenerator.bounceDistance = bounceDistance;
                reelSpinAnimationGenerator.reelSpeed = reelSpeed;
                reelSpinAnimationGenerator.matrixColumns = columnHeightsList.Count;
                reelSpinAnimationGenerator.matrixRows = maxColumnHeight - 2; // Account for masked symbols in Reel Spin Animation Generator
                reelSpinAnimationGenerator.animationName = animationName;
                reelSpinAnimationGenerator.useExistingController = useExistingController;
                if (useExistingController)
                {
                    reelSpinAnimationGenerator.reelController = reelController;
                }

                // Generate Clips
                reelSpinAnimationGenerator.Main();
            }

            GameIdLogger.Logger.Debug("Reel Grid Builder run completed.");
        }

        public void RemoveChildren()
        {
            while (transform.childCount > 0)
            {
                Undo.DestroyObjectImmediate(this.transform.GetChild(0).gameObject);
            }
        }

        #region Game Objects Creation Functions
        public GameObject CreateReelObject()
        {
            GameObject reel = new GameObject("Reel");

            reel.AddComponent<RootReelView>();

            return reel;
        }

        public GameObject CreateReelViewObject()
        {
            GameObject reelView = Instantiate(reelViewObject);

            reelView.name = "ReelView";
            reelView.transform.localPosition = Vector3.zero;

            return reelView;
        }

        public GameObject CreateSymbolsObject()
        {
            GameObject symbols = Instantiate(symbolsObject);

            symbols.name = "Symbols";
            symbols.transform.localPosition = Vector3.zero;

            return symbols;
        }

        public void PopulateSymbolsObject(int rowCount, GameObject parent)
        {
            for (int r = 0; r < rowCount; r++)
            {
                GameObject symbol = (GameObject)PrefabUtility.InstantiatePrefab(symbolObject); // Instantiate with Prefab references
                PrefabUtility.UnpackPrefabInstance(symbol, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction); // Retain ONLY child Prefab references

                symbol.name = "Symbol" + r;
                symbol.transform.localPosition = Vector3.zero;
                symbol.transform.SetParent(parent.gameObject.transform, false);

                Transform locator = symbol.transform.Find("Locator");

                if (locator != null)
                {
                    locator.transform.localPosition = Vector3.zero;
                }
                else
                {
                    GameObject go = CreateLocatorObject();
                    locator = go.transform;
                }

                // Transform Operation
                float midRows = (float)rowCount / 2; // Row halfway point
                if (r < midRows) // Above row halfway point
                {
                    locator.transform.Translate(0, totalHeight * -(r - midRows + 0.5f) / pixelsPerUnit, 0);
                }
                else // Below row halfway point
                {
                    locator.transform.Translate(0, totalHeight * -(r + 1 - midRows - 0.5f) / pixelsPerUnit, 0);
                }

                // WinBox Transform Operation
                Transform winBox = symbol.transform.Find("WinBox");
                if (winBox != null)
                {
                    winBox.transform.position = locator.transform.position;
                }

                // Preview Symbol
                if (symbolPreviewTexture != null)
                {
                    GameObject symbolPreview = CreateSymbolPreviewObject();
                    symbolPreview.transform.SetParent(locator.gameObject.transform, false);
                }
            }
        }

        public void PopulatePaylineObject(int rowCount, GameObject parent)
        {
            for (int r = 0; r < rowCount; r++)
            {
                GameObject symbol = (GameObject)PrefabUtility.InstantiatePrefab(winLineSymbolObject); // Instantiate with Prefab references
                PrefabUtility.UnpackPrefabInstance(symbol, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction); // Retain ONLY child Prefab references

                symbol.name = "Symbol" + r;
                symbol.transform.localPosition = Vector3.zero;
                symbol.transform.SetParent(parent.gameObject.transform, false);
                symbol.transform.localPosition = Vector3.zero;

                // Transform Operation
                float midRows = (float)rowCount / 2; // Row halfway point
                if (r < midRows) // Above row halfway point
                {
                    symbol.transform.Translate(0, totalHeight * -(r - midRows + 0.5f) / pixelsPerUnit, 0);
                }
                else // Below row halfway point
                {
                    symbol.transform.Translate(0, totalHeight * -(r + 1 - midRows - 0.5f) / pixelsPerUnit, 0);
                }
            }
        }

        public GameObject CreateSymbolObject(bool addPooledSymbolView, bool addScreenSymbolView)
        {
            GameObject symbol = new GameObject("Symbol");

            if (addPooledSymbolView)
            {
                PooledSymbolView pooledSymbolView = symbol.AddComponent<PooledSymbolView>();
                pooledSymbolView.SymbolTag = "standard";
                pooledSymbolView.InitOnAwake = true;
            }
            if (addScreenSymbolView)
            {
                Transform winBox = symbol.transform.Find("WinBox");
                if (winBox != null)
                {
                    if (winBox.GetComponent<ScreenSymbolView>() == null)
                    {
                        winBox.gameObject.AddComponent<ScreenSymbolView>();
                    }
                }
                else
                {
                    GameIdLogger.Logger.Warning("Symbol does not have child game object 'WinBox'. ScreenSymbolView component has not been added. Win Boxes will need to be initialized manually.");
                }
            }

            return symbol;
        }

        public GameObject CreateLocatorObject()
        {
            GameObject locator = new GameObject("Locator");

            return locator;
        }

        public GameObject CreateSymbolPreviewObject()
        {
            GameObject symbolPreview = new GameObject("SymbolPreview");

            SpriteRenderer sr = symbolPreview.AddComponent<SpriteRenderer>();
            sr.sprite = symbolPreviewTexture;
            sr.maskInteraction = SpriteMaskInteraction.None;
            sr.sortingLayerName = "Symbols 1";

            return symbolPreview;
        }

        public GameObject CreateReelMaskObjects(int columnIndex)
        {
            // Reel Mask Parent
            GameObject reelMaskController = new GameObject("ReelMaskController");
            //reelMaskController.transform.SetParent(currentReel.transform, false);

            reelMaskController.transform.localPosition = new Vector3(0, -(columnHeightsList[columnIndex] * totalHeight / 2) / pixelsPerUnit, 0); // Position at base of reel window
            reelMaskController.transform.localPosition += new Vector3(0, (maskedRowsBelow * totalHeight) / pixelsPerUnit, 0); // Translate upward for each masked symbols below the reel window

            // if (supportExpandingReels)
            // {
            //     reelMaskController.transform.localScale = new Vector3(1, visibleRows, 1); // Scale to the number of visible rows
            // }
            // else
            // {
            //     reelMaskController.transform.localScale = new Vector3(1, columnHeightsList[c] - maskedRowsAbove - maskedRowsBelow, 1); // Scale to the number of visible rows
            // }

            // Reel Mask Child - 3D
            GameObject reelMask = GameObject.CreatePrimitive(PrimitiveType.Quad);
            reelMask.name = "ReelMask";
            var component = reelMask.GetComponent<MeshCollider>();
            DestroyImmediate(component);
            reelMask.AddComponent<SortingGroup>().sortingOrder = -100;

            if (reelMaskMaterial != null)
            {
                reelMask.GetComponent<MeshRenderer>().material = reelMaskMaterial;
            }
            else
            {
                GameIdLogger.Logger.Error("No material was assigned to Reel Mask. Clipping may not work correctly!");
            }

            // Reel Mask Child - 2D
            //GameObject reelMask = new GameObject("ReelMask");
            //reelMask.AddComponent<SpriteMask>().sprite = symbolMaskTexture; // Add a Sprite Mask, and assign symbolMaskTexture as the Mask's Sprite

            // Reel Mask Child - Transform Operations
            reelMask.transform.SetParent(reelMaskController.transform, false);
            reelMask.transform.localPosition = new Vector3(0, (totalHeight / 2) / pixelsPerUnit, 0); // Offset position to display at base of symbol (instead of center of symbol)
            reelMask.transform.localScale = new Vector3((1 + symbolDividerX / symbolWidth) * (symbolWidth / pixelsPerUnit), (1 + symbolDividerY / symbolHeight) * (symbolHeight / pixelsPerUnit), 1); // Adjust mask scale to include symbol divider y

            // Initialize Clipping Mask
            //ClippingMaskAssignment(reelMaskController, reelMask);

            return reelMaskController;
        }

        public void ClippingMaskAssignment(GameObject componentObject, GameObject maskingMeshObject, int reelLayer)
        {
            if (componentObject.GetComponent<ClippingMask>() != null)
            {
                ClippingMask clippingMask = componentObject.GetComponent<ClippingMask>();
                SetClippingRenderer(clippingMask, maskingMeshObject.GetComponent<MeshRenderer>());
                SetClippingReelLayer(clippingMask, reelLayer);
            }
            else
            {
                ClippingMask clippingMask = componentObject.AddComponent<ClippingMask>();
                SetClippingRenderer(clippingMask, maskingMeshObject.GetComponent<MeshRenderer>());
                SetClippingReelLayer(clippingMask, reelLayer);
            }

            // else if (this.GetComponent<ClippingMask2D>() != null)
            // {
            //     ClippingMask2D clippingMask2D = this.GetComponent<ClippingMask2D>();
            //     clippingMask.ClippingObject = maskObject;
            // }
            // else if (this.GetComponent<ClippingMask3D>() != null)
            // {
            //     ClippingMask3D clippingMask2D = this.GetComponent<ClippingMask3D>();
            //     clippingMask.ClippingObject = maskObject;
            // }
        }

        public void GeneratePaylineLocatorObjects()
        {
            // Payline Parent Objects
            GameObject reelLeft = new GameObject("PaylineLeft");
            GameObject reelRight = new GameObject("PaylineRight");

            PopulatePaylineObject(columnHeightsList[0], reelLeft);
            PopulatePaylineObject(columnHeightsList[columnHeightsList.Count - 1], reelRight);

            // Parent
            reelLeft.transform.SetParent(this.transform, false);
            reelRight.transform.SetParent(this.transform, false);

            // Transform Operation
            float midColumns = (float)columnHeightsList.Count / 2; // Column halfway point
            reelLeft.transform.Translate(-(totalWidth * midColumns / pixelsPerUnit), 0, 0);
            reelRight.transform.Translate((totalWidth * midColumns / pixelsPerUnit), 0, 0);
        }

        public void GenerateWinLinesObject()
        {
            if (winLinesObject != null)
            {
                // Object
                GameObject winLines = Instantiate(winLinesObject);
                winLines.name = "WinLines";

                // Parent
                winLines.transform.SetParent(this.transform, false);

                // Check for necessary components
                if (winLines.GetComponent<ParticleSystem>() == null)
                {
                    GameIdLogger.Logger.Error("The WinLinesObject prefab is missing a component: ParticleSystem.");
                }
                if (winLines.GetComponent<MeshFilter>() == null)
                {
                    GameIdLogger.Logger.Error("The WinLinesObject prefab is missing a component: MeshFilter.");
                }
                if (winLines.GetComponent<WinLineParticleView>() == null)
                {
                    GameIdLogger.Logger.Error("The WinLinesObject prefab is missing a component: WinLineParticleView.");
                }
                if (winLines.GetComponent<WinLineDrawer>() == null)
                {
                    GameIdLogger.Logger.Error("The WinLinesObject prefab is missing a component: WinLineDrawer.");
                }
            }
            else
            {
                GameIdLogger.Logger.Error("Missing a prefab reference: WinLinesObject.");
            }
        }
        #endregion

        public void GenerateReelGrid()
        {
            InitializeVariables();

            for (int c = 0; c < columnHeightsList.Count; c++) // Column is synonymous with Reel
            {
                // Generate Game Objects
                GameObject currentReel = CreateReelObject();
                currentReel.name += c;
                currentReel.transform.SetParent(this.gameObject.transform, false);

                GameObject reelView  = CreateReelViewObject();
                reelView.transform.SetParent(currentReel.transform, false);

                GameObject symbols  = CreateSymbolsObject();
                symbols.transform.SetParent(reelView.transform, false);
                PopulateSymbolsObject(maxColumnHeight, symbols);

                // Transform Operation
                float midColumns = (float)columnHeightsList.Count / 2; // Column halfway point
                if (c <= midColumns) // Left of column halfway point
                {
                    currentReel.transform.Translate((totalWidth * (c - midColumns + 0.5f) / pixelsPerUnit), 0, 0);
                }
                else // Right of column halfway point
                {
                    currentReel.transform.Translate((totalWidth * (c + 1 - midColumns - 0.5f) / pixelsPerUnit), 0, 0);
                }

                // Mask Operation
                if (supportAsymmetricalReels) // Generate Mask Per Reel
                {
                    GameObject reelMaskObject = CreateReelMaskObjects(c);
                    reelMaskObject.transform.SetParent(currentReel.transform, false);

                    if (supportExpandingReels)
                    {
                        reelMaskObject.transform.localScale = new Vector3(1, visibleRows, 1); // Scale to the number of visible rows
                        GameIdLogger.Logger.Warning("Expanding Reels will override Asymmetrical Reels mask heights. This can at times cause undesired default masking behavior. If not intended, disable Expanding Reels when using Asymmetrical Reels.");
                    }
                    else
                    {
                        reelMaskObject.transform.localScale = new Vector3(1, columnHeightsList[c] - maskedRowsAbove - maskedRowsBelow, 1); // Scale to Height of Reel Window
                    }

                    // Modify from Diamond pattern, to Audio Bars pattern
                    if (columnHeightsList[c] < maxColumnHeight)
                    {
                        float maskOffset = (float)(maxColumnHeight - columnHeightsList[c]) / 2;
                        maskOffset *= totalHeight / pixelsPerUnit;
                        reelMaskObject.transform.localPosition -= new Vector3(0, maskOffset, 0); // Scale to Height of Reel Window

                        // Align Top index with top of reel window
                        float positionOffset = (float)(maxColumnHeight - columnHeightsList[c]);
                        positionOffset *= totalHeight / pixelsPerUnit;
                        currentReel.transform.localPosition -= new Vector3(0, positionOffset, 0);
                        reelMaskObject.transform.localPosition += new Vector3(0, positionOffset, 0);
                    }

                    // Initialize Clipping Mask
                    ClippingMaskAssignment(currentReel, reelMaskObject.transform.Find("ReelMask").gameObject, 1);
                }
                else if (c == columnHeightsList.Count - 1) // Generate Single Mask
                {
                    GameObject reelMaskObject = CreateReelMaskObjects(c);
                    reelMaskObject.transform.SetParent(this.transform, false);

                    if (supportExpandingReels)
                    {
                        reelMaskObject.transform.localScale = new Vector3(columnHeightsList.Count, visibleRows, 1); // Scale to the number of visible rows + Width of Reel Window
                    }
                    else
                    {
                        reelMaskObject.transform.localScale = new Vector3(columnHeightsList.Count, columnHeightsList[c] - maskedRowsAbove - maskedRowsBelow, 1); // Scale to Height/Width of Reel Window
                    }

                    // Initialize Clipping Mask
                    ClippingMaskAssignment(this.gameObject, reelMaskObject.transform.Find("ReelMask").gameObject, 1);
                }

                // Undo Operation
                Undo.RegisterCreatedObjectUndo(currentReel, "Created Reel Object");
            }

            if (supportPaylines)
            {
                GeneratePaylineLocatorObjects();
                GenerateWinLinesObject();
            }

            InitializeComponentValues();
        }

        // Generates Hold and Spin Reels
        public void GenerateHoldAndSpinReels()
        {
            InitializeHoldAndSpinVariables();

            if (leftToRight == true)
            {
                int reelNumber = 0;
                for (int r = 0; r < rows; r++) // rows before columns will number reels left to right, top to bottom (instead of top to bottom, left to right)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        CreateHoldAndSpinReel(reelNumber, r, c);
                        reelNumber++;
                    }
                }
            }
            else if (leftToRight == false) // Top to Bottom
            {
                int reelNumber = 0;
                for (int c = 0; c < columns; c++)
                {
                    for (int r = 0; r < rows; r++)
                    {
                        CreateHoldAndSpinReel(reelNumber, r, c);
                        reelNumber++;
                    }
                }
            }

            if (supportPaylines)
            {
                GameIdLogger.Logger.Debug("Paylines not supported with Hold and Spin. Payline option disabled.");
                supportPaylines = false;
            }

            InitializeComponentValues();
        }

        public void CreateHoldAndSpinReel(int reelNumber, int r, int c)
        {
            //int reelNumber = 0;
            // for (int r = 0; r < rows; r++) // rows before columns will number reels left to right, top to bottom (instead of top to bottom, left to right)
            // {
            //     for (int c = 0; c < columns; c++)
            //     {
            GameObject currentReel = CreateReelObject();
            currentReel.name += reelNumber;
            currentReel.transform.SetParent(this.gameObject.transform, false);
            //SortingGroup sg = currentReel.AddComponent<SortingGroup>();
            //sg.sortingLayerName = holdAndSpinSortingGroupName;

            GameObject reelView  = CreateReelViewObject();
            reelView.transform.SetParent(currentReel.transform, false);

            GameObject symbols  = CreateSymbolsObject();
            symbols.transform.SetParent(reelView.transform, false);
            PopulateSymbolsObject(columnHeightsList[c], symbols);

            // Transform Operation: Left to Right
            float midColumns = (float)columns / 2; // Column halfway point
            if (c <= midColumns) // Left of column halfway point
            {
                currentReel.transform.Translate((totalWidth * (c - midColumns + 0.5f) / pixelsPerUnit), 0, 0);
            }
            else // Right of column halfway point
            {
                currentReel.transform.Translate((totalWidth * (c + 1 - midColumns - 0.5f) / pixelsPerUnit), 0, 0);
            }

            // Transform Operation: Top to Bottom
            float midRows = (float)rows / 2; // Row halfway point
            if (r <= midRows) // Above row halfway point
            {
                currentReel.transform.Translate(0, (-1 * totalHeight * (r - midRows + 0.5f) / pixelsPerUnit), 0);
            }
            else // Below of row halfway point
            {
                currentReel.transform.Translate(0, (-1 * totalHeight * (r + 1 - midRows - 0.5f) / pixelsPerUnit), 0);
            }

            // Mask Operation
            GameObject reelMaskObject = CreateReelMaskObjects(c);
            reelMaskObject.transform.SetParent(currentReel.transform, false);
            reelMaskObject.transform.localPosition = new Vector3(0, -((columnHeightsList[c] - maskedRowsAbove - maskedRowsBelow) * totalHeight / 2) / pixelsPerUnit, 0); // Override position

            if (supportExpandingReels)
            {
                reelMaskObject.transform.localScale = new Vector3(1, visibleRows, 1); // Scale to the number of visible rows
            }
            else
            {
                reelMaskObject.transform.localScale = new Vector3(1, columnHeightsList[c] - maskedRowsAbove - maskedRowsBelow, 1); // Scale to Height of Reel Window
            }

            // Initialize Clipping Mask
            int reelLayerCalc = c % 2;
            if (reelLayerCalc == 0)
            {
                reelLayerCalc = rows - r;
            }
            else
            {
                reelLayerCalc = rows - r + rows;
            }
            ClippingMaskAssignment(currentReel, reelMaskObject.transform.Find("ReelMask").gameObject, reelLayerCalc);
            if (reelLayerCalc > 15)
            {
                GameIdLogger.Logger.Error(currentReel + " Reel Layer was not configured correctly (cannot exceed a value of 15). Please configure manually, or contact tool creator.");
            }

            // Undo Operation
            Undo.RegisterCreatedObjectUndo(currentReel, "Created Reel Object");

            //reelNumber++;
            //     }
            // }
        }

        public void InitializeComponentValues()
        {
            // Reel Grid
            for (int c = 0; c < columnHeightsList.Count; ++c)
            {
                // Reel
                Transform reel = this.transform.Find("Reel" + c);

                // Symbol
                for (int r = 0; r < maxColumnHeight; ++r)
                {
                    Transform symbol = reel.transform.Find("ReelView/Symbols/Symbol" + r);
                    SetSymbolIndices(symbol.gameObject, c, r, columnHeightsList[c]);
                }
            }

            // Payline
            if (supportPaylines)
            {
                Transform paylineLeft = this.transform.Find("PaylineLeft");
                for (int r = 0; r < columnHeightsList[0]; ++r)
                {
                    Transform symbol = paylineLeft.transform.Find("Symbol" + r);
                    if (r < maskedRowsAbove || r >= columnHeightsList[0] - maskedRowsBelow)
                    {
                        DestroyImmediate(symbol.gameObject);
                    }
                    else
                    {
                        if (symbol != null)
                        {
                            SetSymbolIndices(symbol.gameObject, -1, r, columnHeightsList[0]);
                            ConfigurePaylineObject(symbol.gameObject);
                        }
                    }
                }
                Transform firstReel = this.transform.Find("Reel0");
                paylineLeft.SetParent(firstReel.transform, true);

                Transform paylineRight = this.transform.Find("PaylineRight");
                for (int r = 0; r < columnHeightsList[columnHeightsList.Count - 1]; ++r)
                {
                    Transform symbol = paylineRight.transform.Find("Symbol" + r);
                    if (r < maskedRowsAbove || r >= columnHeightsList[columnHeightsList.Count - 1] - maskedRowsBelow)
                    {
                        DestroyImmediate(symbol.gameObject);
                    }
                    else
                    {
                        if (symbol != null)
                        {
                            SetSymbolIndices(symbol.gameObject, columnHeightsList.Count, r, columnHeightsList[columnHeightsList.Count - 1]);
                            ConfigurePaylineObject(symbol.gameObject);
                        }
                    }
                }
                Transform lastReel = this.transform.Find("Reel" + (columnHeightsList.Count - 1));
                paylineRight.SetParent(lastReel.transform, true);
            }
        }

        public void SetSymbolIndices(GameObject symbol, int column, int row, int columnHeight)
        {
            int rowIndex = row - maskedRowsAbove; // rows above the reel window will be assigned a negative index; first visible row = 0 index

            if (symbol != null)
            {
                PooledSymbolView pooledSymbolView = symbol.GetComponent<PooledSymbolView>();
                if (pooledSymbolView != null)
                {
                    pooledSymbolView.Location.colIndex = column;
                    pooledSymbolView.Location.rowIndex = rowIndex;
                    int sortingIndex = row + (columnHeight * column);

                    // 1 - 9
                    // 11 -19
                    SetSortingIndex(pooledSymbolView, sortingIndex);
                }

                ScreenSymbolView screenSymbolView = symbol.GetComponent<ScreenSymbolView>();
                if (screenSymbolView != null)
                {
                    screenSymbolView.Location.colIndex = column;
                    screenSymbolView.Location.rowIndex = rowIndex;
                }

                Transform winBox = symbol.transform.Find("WinBox");
                if (winBox != null)
                {
                    ScreenSymbolView winBoxScreenSymbolView = winBox.GetComponent<ScreenSymbolView>();
                    if (winBoxScreenSymbolView != null)
                    {
                        winBoxScreenSymbolView.Location.colIndex = column;
                        winBoxScreenSymbolView.Location.rowIndex = rowIndex;
                    }

                    // Remove WinBox from Masked Symbols
                    if (row < maskedRowsAbove || row >= columnHeight - maskedRowsBelow)
                    {
                        DestroyImmediate(winBox.gameObject);
                    }
                }
            }
            else
            {
                GameIdLogger.Logger.Debug("No child with the name Symbol" + rowIndex + " attached to Reel" + column);
            }
        }

        public void ConfigurePaylineObject(GameObject symbol)
        {
            Transform locator = symbol.transform.Find("Locator");
            if (locator != null)
            {
                DestroyImmediate(locator.gameObject);
            }

            // Set values from PooledSymbolView to ScreenSymbolView
            //ScreenSymbolView screenSymbolView = symbol.gameObject.AddComponent<ScreenSymbolView>();
            //FieldInfo prop = typeof(ScreenSymbolView).GetField("_location", BindingFlags.NonPublic | BindingFlags.Instance);
            //prop.SetValue(screenSymbolView, new Milan.FrontEnd.Core.Location());
            //PooledSymbolView pooledSymbolView = symbol.gameObject.GetComponent<PooledSymbolView>();
            //screenSymbolView.Location.colIndex = pooledSymbolView.Location.colIndex;
            //screenSymbolView.Location.rowIndex = pooledSymbolView.Location.rowIndex;

            // Remove PooledSymbolView
            //DestroyImmediate(pooledSymbolView);
        }

        // Reflection; Not necessary if _sortingIndex were public
        public void SetSortingIndex (PooledSymbolView pooledSymbolView, int sortingIndex)
        {
            FieldInfo fieldInfo = typeof(PooledSymbolView).GetField("_sortingIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(pooledSymbolView, sortingIndex);
        }

        public void SetClippingRenderer(ClippingMask clippingMask, Renderer maskMeshRenderer)
        {
            FieldInfo fieldInfo = typeof(ClippingMask).GetField("_clippingRenderer", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(clippingMask, maskMeshRenderer);
        }

        public void SetClippingReelLayer(ClippingMask clippingMask, int reelLayer)
        {
            FieldInfo fieldInfo = typeof(ClippingMask).GetField("_reelLayer", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(clippingMask, reelLayer);
        }
    }
}
#endif
