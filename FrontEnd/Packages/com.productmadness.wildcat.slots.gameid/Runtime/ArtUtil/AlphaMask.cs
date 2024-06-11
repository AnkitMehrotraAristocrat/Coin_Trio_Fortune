using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PixelUnited.NMG.Slots.Milan.GAMEID.AlphaMasking
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    public class AlphaMask : MonoBehaviour
    {
        [SerializeField] private bool isMaskingEnabled = true;
        [SerializeField] private bool clampAlphaHorizontally = false;
        [SerializeField] private bool clampAlphaVertically = false;
        [SerializeField][HideInInspector] private float clampingBorder = 0.01f;
        [SerializeField] private bool useMaskAlphaChannel = false;
        [SerializeField] private bool cropMask = true;
        [SerializeField] private float croppedSize = 1f;
        [SerializeField] private Texture baseMap;
        [SerializeField] private Vector2 baseMapTiling = new Vector2(1, 1);
        [SerializeField] private Vector2 baseMapOffset = new Vector2(0, 0);
        [SerializeField] [HideInInspector] private Material maskMaterial;

        private bool maskRefresh = true;

        private const string SHADER_MASK_TAG = "AlphaMasked";
        private const string MASK_RESOURCES_ADDRESS = "Materials/URP AlphaMask";
        private const string _BASEMAP = "_BaseMap";
        private const string URP_UNLIT_SHADER = "Universal Render Pipeline/Unlit";
        private const string SPRITES_RESOURCES_ADDRESS = "Materials/Sprites AlphaMasked - World Coords";
        public const string MASKED_SPRITE_SHADER = "AlphaMask/Sprites Alpha Masked - World Coords"; // TODO: HLSL "Universal Render Pipeline/Alpha Masked/Sprites Alpha Masked - World Coords";
        public const string MASKED_UNLIT_SHADER = "AlphaMask/Unlit Alpha Masked - World Coords"; // TODO: HLSL
        
        public bool IsMaskingEnabled
        {
            get { return isMaskingEnabled; }
            set
            {
                if(value != isMaskingEnabled)
                {
                    ScheduleMaskRefresh();
                    isMaskingEnabled = value;
                }
            }
        }

        public bool ClampAlphaHorizontally
        {
            get { return clampAlphaHorizontally; }
            set
            {
                if(value != clampAlphaHorizontally)
                {
                    ScheduleMaskRefresh();
                    clampAlphaHorizontally = value;
                }
            }
        }

        public bool ClampAlphaVertically
        {
            get { return clampAlphaVertically; }
            set
            {
                if (value != clampAlphaVertically)
                {
                    ScheduleMaskRefresh();
                    clampAlphaVertically = value;
                }
            }
        }

        public float ClampingBorder
        {
            get { return clampingBorder; }
            set
            {
                if (value != clampingBorder)
                {
                    ScheduleMaskRefresh();
                    clampingBorder = value;
                }
            }
        }

        public bool UseMaskAlphaChannel
        {
            get { return useMaskAlphaChannel; }
            set
            {
                if (value != useMaskAlphaChannel)
                {
                    ScheduleMaskRefresh();
                    useMaskAlphaChannel = value;
                }
            }
        }

        public bool CropMask 
        { 
            get { return cropMask; }
            set
            {
                if (value != cropMask)
                {
                    ScheduleMaskRefresh();
                    cropMask = value;
                }
            }
        }

        public float CroppedSize 
        { 
            get { return croppedSize; }
            set
            {
                if (value != croppedSize)
                {
                    ScheduleMaskRefresh();
                    croppedSize = value;
                }
            }
        }

        public Texture BaseMap 
        { 
            get { return baseMap; } 
            set 
            {
                if(value != baseMap)
                {
                    if (value == null)
                    {
                        value = Texture2D.whiteTexture;
                    }
                    ScheduleMaskRefresh();
                    baseMap = value;
                }

            } 
        }
        public Vector2 BaseMapTiling 
        { 
            get { return baseMapTiling; } 
            set 
            {
                if (value != baseMapTiling)
                {
                    ScheduleMaskRefresh();
                    baseMapTiling = value;
                }
            } 
        }
        public Vector2 BaseMapOffset
        {
            get { return baseMapOffset; }
            set 
            { 
                if (value != baseMapOffset) 
                {
                    ScheduleMaskRefresh();
                    baseMapOffset = value; 
                } 
            }
        }

        private Matrix4x4 oldWorldToMask = Matrix4x4.identity;
        private Matrix4x4 OldWorldToMask { get { return oldWorldToMask; } set { oldWorldToMask = value; } }
        private MeshRenderer meshrenderer;
        public MeshRenderer MeshRenderer 
        { 
            get 
            {
                if(meshrenderer == null) meshrenderer = GetComponent<MeshRenderer>();
                return meshrenderer;
            } 
        }

        private Material spritesAlphaMaskWorldCoords;
        public Material SpritesAlphaMaskWorldCoords
        {
            get
            {
                if (spritesAlphaMaskWorldCoords == null)
                {
                    spritesAlphaMaskWorldCoords = Resources.Load<Material>(SPRITES_RESOURCES_ADDRESS);
                    if (spritesAlphaMaskWorldCoords == null)
                    {
                        GameIdLogger.Logger.Error(SPRITES_RESOURCES_ADDRESS + " not found!");
                    }
                }
                return spritesAlphaMaskWorldCoords;
            }
            set { spritesAlphaMaskWorldCoords = value;}
        }
        public Material MaskMaterial
        {   
            get 
            {
                if (maskMaterial == null)
                {
                    maskMaterial = Resources.Load<Material>(MASK_RESOURCES_ADDRESS);

                    if (maskMaterial == null)
                    {
                        GameIdLogger.Logger.Debug(MASK_RESOURCES_ADDRESS + " not found!");
                    }
                }
                return maskMaterial;
            }
            set { maskMaterial = value; }
        }

        private MaterialPropertyBlock maskedPropertyBlock;
        public MaterialPropertyBlock MaskedPropertyBlock
        {
            get
            {
                if (maskedPropertyBlock == null)
                {
                    maskedPropertyBlock = new MaterialPropertyBlock();
                }
                return maskedPropertyBlock;
            }
            set
            {
                maskedPropertyBlock = value;
            }
        }

        private MaterialPropertyBlock maskPropertyBlock;
        public MaterialPropertyBlock MaskPropertyBlock
        {
            get
            {
                if (maskPropertyBlock == null)
                {
                    maskPropertyBlock = new MaterialPropertyBlock();
                }
                return maskPropertyBlock;
            }
            set
            {
                maskPropertyBlock = value;
            }
        }

        private Shader defaultMaskedSpriteShader = null;
        private Shader DefaultMaskedSpriteShader
        {
            get
            {
                if (defaultMaskedSpriteShader == null) defaultMaskedSpriteShader = Shader.Find(MASKED_SPRITE_SHADER);
                return defaultMaskedSpriteShader;
            }
            set { defaultMaskedSpriteShader = value; }
        }

        private Shader defaultMaskedUnlitShader = null;
        private Shader DefaultMaskedUnlitShader
        {
            get
            {
                if (defaultMaskedUnlitShader == null) defaultMaskedUnlitShader = Shader.Find(MASKED_UNLIT_SHADER);
                return defaultMaskedUnlitShader;
            }
            set
            {
                defaultMaskedUnlitShader = value;
            }
        }

#if UNITY_EDITOR

        [SerializeField] private bool maskUpdate = true;
        [SerializeField] private bool maskedUpdate = true;
        private Matrix4x4 maskQuadMatrix = Matrix4x4.identity;

        public Mesh editorMesh;
        public bool displayMask { get { return MeshRenderer.enabled; } set { MeshRenderer.enabled = value; } }
        private List<Material> materialGarbage = new List<Material> ();

        // UNITY EDITOR EVENTS---------------------------------------

        private void Awake()
        {
            if (baseMap == null)
            {
                baseMap = Texture2D.whiteTexture;
            }
            EditorInitialization();
            UpgradeMask();
        }

        private void Reset()
        {
            EditorInitialization();
            UpgradeMask();
        }

        private void OnRenderObject()
        {
            DrawMask(out maskQuadMatrix);
        }

        public void OnValidate()
        {
            UpgradeMask(false);
            // TODO: Unity Editor Copy and Paste Events
            RefreshMaskPropertyBlock();
            UpdateMasking();
        }

        /// <summary>
        /// Sets MaskMaterial to the MeshRenderer
        /// </summary>
        private void EditorInitialization()
        {
            if (MeshRenderer.sharedMaterial == null)
            {
                MeshRenderer.sharedMaterial = MaskMaterial;
                MeshRenderer.enabled = false;
            }
            InitializeMeshRenderer(MeshRenderer);
            ScheduleMaskRefresh();
        }

        /// <summary>
        /// For if AlphaMask is upgraded and masks need to be updated
        /// </summary>
        /// <param name="componentImmediate"></param>
        private void UpgradeMask(bool componentImmediate = true)
        {
            if (maskUpdate)
            {
                if (MeshRenderer.sharedMaterial.shader == Shader.Find(URP_UNLIT_SHADER))
                {
                    if (!MeshRenderer.sharedMaterial.Equals(MaskMaterial))
                    {
                        ScheduleMaskRefresh();
                        GameIdLogger.Logger.Debug("Version upgrade on: " + gameObject.name, gameObject);

                        BaseMap = MeshRenderer.sharedMaterial.GetTexture(_BASEMAP);
                        BaseMapOffset = MeshRenderer.sharedMaterial.GetTextureOffset(_BASEMAP);
                        BaseMapTiling = MeshRenderer.sharedMaterial.GetTextureScale(_BASEMAP);

                        DestroyImmediate(MeshRenderer.sharedMaterial);
                        MeshRenderer.sharedMaterial = MaskMaterial;
                        MeshFilter filter = GetComponent<MeshFilter>();
                        if(filter)
                        {
                            if (componentImmediate)
                            {
                                DestroyImmediate(filter);
                            }
                            else
                            {
                                EditorApplication.delayCall += () => 
                                { 
                                    DestroyImmediate(filter); 
                                };
                            }
                        }
                        RefreshMaskPropertyBlock();
                    }
                }
                maskUpdate = false;
            }
        }

        private void UpgradeMasked(SpriteRenderer targetRen)
        {
            if (!AssetDatabase.Contains(targetRen.sharedMaterial))
            {
                if (targetRen.sharedMaterial.shader.ToString() == DefaultMaskedSpriteShader.ToString())
                {
                    if (!targetRen.sharedMaterial.Equals(SpritesAlphaMaskWorldCoords))
                    {
                        GameIdLogger.Logger.Debug("Version upgrade on: " + targetRen.gameObject.name, targetRen.gameObject);
                        if (!materialGarbage.Contains(targetRen.material))
                        {
                            materialGarbage.Add(targetRen.material);
                        }
                        targetRen.material = SpritesAlphaMaskWorldCoords;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the image of the mask on screen
        /// </summary>
        /// <param name="objectMatrix"></param>
        private void DrawMask(out Matrix4x4 objectMatrix)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            Rect texR = new Rect();
            texR.position = BaseMapOffset;
            texR.size = BaseMapTiling;

            Vector3 objectSize = transform.lossyScale;
            if (rectTransform != null) objectSize = Vector3.Scale(objectSize, rectTransform.rect.size);
            objectSize.z = 0.1f; //Because this is mask quad, and it doesn't have thickness

            // Scale difference based on material tiling value
            Vector3 tilingAdjustment = BaseMapTiling;
            tilingAdjustment.x = 1 / tilingAdjustment.x;
            tilingAdjustment.y = 1 / tilingAdjustment.y;

            Vector3 objectSizeTilingAdjusted = Vector3.Scale(objectSize, tilingAdjustment);

            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, objectSizeTilingAdjusted);
            
            Vector3 pivotTransposition = transform.rotation * -objectSize * .5f;
            Vector3 offsetTransposition = Vector3.Scale(BaseMapOffset, -objectSizeTilingAdjusted);

            Matrix4x4 deltaMatrix = Matrix4x4.TRS(pivotTransposition + offsetTransposition, Quaternion.identity, Vector3.one);

            Matrix4x4 completeMatrix = deltaMatrix * matrix;

            if (displayMask)
            {
                if (editorMesh == null)
                {
                    editorMesh = new Mesh();
                    editorMesh.name = "Mask Editor Mesh";
                }
                Texture originalTex = MeshRenderer.sharedMaterial.GetTexture(_BASEMAP);
                Vector2 originalOffset = MeshRenderer.sharedMaterial.GetTextureOffset(_BASEMAP);
                Vector2 originalScale = MeshRenderer.sharedMaterial.GetTextureScale(_BASEMAP);

                MeshRenderer.sharedMaterial.SetTexture(_BASEMAP, BaseMap);
                MeshRenderer.sharedMaterial.SetTextureOffset(_BASEMAP, BaseMapOffset);
                MeshRenderer.sharedMaterial.SetTextureScale(_BASEMAP, BaseMapTiling);
                if (MeshRenderer.sharedMaterial.SetPass(0))
                {
                    GetMaskQuad(editorMesh, texR);
                    Graphics.DrawMeshNow(editorMesh, completeMatrix);
                }

                MeshRenderer.sharedMaterial.SetTexture(_BASEMAP, originalTex);
                MeshRenderer.sharedMaterial.SetTextureOffset(_BASEMAP, originalOffset);
                MeshRenderer.sharedMaterial.SetTextureScale(_BASEMAP, originalScale);
            }
            objectMatrix = completeMatrix;
        }
#endif

        private void Start()
        {
            if(Application.isPlaying)
            {
                MeshRenderer.enabled = false;
            }
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR

            UpgradeMask();
            if (!Application.isPlaying) ScheduleMaskRefresh();
#endif
            RefreshMaskPropertyBlock();
            UpdateMasking();
        }

        /// <summary>
        /// Refreshes Mask next OnValidate() or LateUpdate()
        /// </summary>
        public void ScheduleMaskRefresh()
        {
            maskRefresh = true;
        }

        // TODO: Make an AlphaMaskHelper static and move some private methods out
        private void GetMaskQuad(Mesh mesh, Rect r)
        {
            Vector3[] vertices = new Vector3[4];

            vertices[0] = new Vector3(r.xMin, r.yMin, 0);
            vertices[1] = new Vector3(r.xMax, r.yMin, 0);
            vertices[2] = new Vector3(r.xMin, r.yMax, 0);
            vertices[3] = new Vector3(r.xMax, r.yMax, 0);

            int[] triangles = new int[6];

            // Lower left triangle
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;

            // Upper right triangle
            triangles[3] = 2;
            triangles[4] = 3;
            triangles[5] = 1;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            if (!BasicArrayCompare(mesh.vertices, vertices)) mesh.vertices = vertices;
            if (!BasicArrayCompare(mesh.triangles, triangles)) mesh.triangles = triangles;
            if (!BasicArrayCompare(mesh.normals, normals)) mesh.normals = normals;
            if (!BasicArrayCompare(mesh.uv, uv)) mesh.uv = uv;
        }

        /// <summary>
        /// Checks to see if two arrays are the same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        private bool BasicArrayCompare<T>(T[] one, T[] two)
        {
            if (one.Length != two.Length) return false;
            for (int i = 0; i < one.Length; i++)
            {
                if(!one[i].Equals(two[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Turns off shadows for Mesh
        /// </summary>
        /// <param name="meshRenderer"></param>
        private void InitializeMeshRenderer(MeshRenderer meshRenderer)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
        }

        /// <summary>
        /// Resets the texture of the AlphaMask Material
        /// </summary>
        public void RefreshMaskPropertyBlock()
        {
            if (MaskPropertyBlock == null) 
            { 
                MaskPropertyBlock = new MaterialPropertyBlock(); 
            }

            MeshRenderer.GetPropertyBlock(MaskPropertyBlock);

            if (BaseMap != null)
            {
                MaskPropertyBlock.SetTexture(_BASEMAP, BaseMap);
            }

            MaskPropertyBlock.SetVector(_BASEMAP + "_ST", new Vector4(BaseMapTiling.x, BaseMapTiling.y, baseMapOffset.x, baseMapOffset.y));
            MeshRenderer.SetPropertyBlock(MaskPropertyBlock);
        }

        public void UpdateMasking()
        {
            // TODO: Check to see if MaskedUnlitShader are present in the project 
            if (DefaultMaskedSpriteShader == null)
            {
                GameIdLogger.Logger.Debug("Shaders necessary for masking don't seem to be present in the project.");
                return;
            }

            if (transform.parent != null)
            {
                Vector4 tilingAndOffset = new Vector4(BaseMapTiling.x, BaseMapTiling.y, baseMapOffset.x, BaseMapOffset.y);
                
                RectTransform maskRectTransform = GetComponent<RectTransform>();
                Matrix4x4 worldToMask = transform.worldToLocalMatrix;
                Vector3 maskSize = Vector3.one;

                if (maskRectTransform)
                {
                    maskSize = maskRectTransform.rect.size;
                    maskSize.z = 1;
                }

                maskSize = Vector3.Scale(maskSize, transform.lossyScale);

                worldToMask.SetTRS(transform.position, transform.rotation, maskSize);
                worldToMask = Matrix4x4.Inverse(worldToMask);

                if (worldToMask != OldWorldToMask)
                {
                    ScheduleMaskRefresh();
                }

                OldWorldToMask = worldToMask;

                if (maskRefresh)
                {
                    List<Renderer> renderers = new List<Renderer>();
                    transform.parent.gameObject.GetComponentsInChildren(true, renderers);
                    Renderer excludedRenderer = transform.parent.GetComponent<Renderer>();
                    if (excludedRenderer != null) renderers.Remove(excludedRenderer);
                    

                    // TODO: get Graphic UI Components for masking

                    List<SpriteRenderer> diffSpriteRenderers = new List<SpriteRenderer>();
                    List<Material> diffActiveMaterials = new List<Material>();

                    foreach (Renderer renderer in renderers)
                    {
                        // TODO: set up masking for symbol animations
                        //if (renderer.TryGetComponent<AlphaMasked>(out AlphaMasked symbolMasker))
                        //{
                        //    if (symbolMasker.UseMask == false)
                        //    {
                        //        continue;
                        //    }
                        //}

                        if (renderer is SpriteRenderer)
                        {
                            if (renderer.gameObject != gameObject && !diffSpriteRenderers.Contains((SpriteRenderer)renderer))
                            {
                                diffSpriteRenderers.Add((SpriteRenderer)renderer);
                            }
                        }
                        else
                        {
                            foreach (Material material in renderer.sharedMaterials)
                            {
                                if (material != null && !diffActiveMaterials.Contains(material))
                                {
                                    diffActiveMaterials.Add(material);
                                }
                            }
                        }
                    }

                    UpdateInstanciatedMaterials(diffActiveMaterials, worldToMask);
                    UpdateSpriteMaterials(diffSpriteRenderers, worldToMask, tilingAndOffset);

                    maskRefresh = false;
                }
#if UNITY_EDITOR
                if (maskedUpdate)
                {
                    for (int i = 0; i < materialGarbage.Count; i++)
                    {
                        DestroyImmediate(materialGarbage[i]);
                        materialGarbage[i] = null;
                    }
                    materialGarbage = new List<Material>();
                    maskedUpdate = false;
                }
#endif
            }
        }

        /// <summary>
        /// Used to update all of the masked instanced materials
        /// </summary>
        /// <param name="diffMaterials"></param>
        /// <param name="worldToMask"></param>
        private void UpdateInstanciatedMaterials(List<Material> diffMaterials, Matrix4x4 worldToMask)
        {
            foreach (Material material in diffMaterials)
            {
                ValidateShader(material);

                if (IsMaskedMaterial(material))
                {
                    material.SetTexture("_AlphaTex", BaseMap);
                    //Set calculations
                    material.SetTextureOffset("_AlphaTex", BaseMapOffset);
                    material.SetTextureScale("_AlphaTex", BaseMapTiling);

                    material.SetFloat("_ClampHoriz", ClampAlphaHorizontally ? 1 : 0);
                    material.SetFloat("_ClampVert", ClampAlphaVertically ? 1 : 0);
                    material.SetFloat("_UseAlphaChannel", UseMaskAlphaChannel ? 1 : 0);
                    material.SetFloat("_Enabled", IsMaskingEnabled ? 1 : 0);
                    material.SetFloat("_ClampBorder", ClampingBorder);
                    material.SetFloat("_IsThisText", 0);
                    material.SetFloat("_CropMask", CropMask ? 1 : 0);
                    material.SetFloat("_CroppedSize", CroppedSize);

                    material.SetFloat("_ScreenSpaceUI", 0);


                    material.SetMatrix("_WorldToMask", worldToMask);
                }
            }
        }

        /// <summary>
        /// Used to update all masked sprite materials
        /// </summary>
        /// <param name="diffSpriteRenderers"></param>
        /// <param name="worldToMask"></param>
        /// <param name="tilingAndOffset"></param>
        private void UpdateSpriteMaterials(List<SpriteRenderer> diffSpriteRenderers, Matrix4x4 worldToMask, Vector4 tilingAndOffset)
        {
            foreach (SpriteRenderer spriteRenderer in diffSpriteRenderers)
            {
#if UNITY_EDITOR
                if (maskedUpdate)
                {
                    UpgradeMasked(spriteRenderer);
                }
#endif
                if (IsMaskedMaterial(spriteRenderer.sharedMaterial))
                {
                    spriteRenderer.GetPropertyBlock(MaskedPropertyBlock);

                    if (BaseMap != null)
                    {
                        MaskedPropertyBlock.SetTexture("_AlphaTex", BaseMap);
                    }

                    MaskedPropertyBlock.SetFloat("_ClampHoriz", ClampAlphaHorizontally ? 1 : 0);
                    MaskedPropertyBlock.SetFloat("_ClampVert", ClampAlphaVertically ? 1 : 0);
                    MaskedPropertyBlock.SetFloat("_UseAlphaChannel", UseMaskAlphaChannel ? 1 : 0);
                    MaskedPropertyBlock.SetFloat("_Enabled", IsMaskingEnabled ? 1 : 0);
                    MaskedPropertyBlock.SetFloat("_ClampingBorder", ClampingBorder);
                    MaskedPropertyBlock.SetFloat("_IsThisText", 0);
                    MaskedPropertyBlock.SetFloat("_CropMask", CropMask ? 1 : 0);
                    MaskedPropertyBlock.SetFloat("_CroppedSize", CroppedSize);

                    MaskedPropertyBlock.SetVector("_AlphaTex_ST", tilingAndOffset);
                    MaskedPropertyBlock.SetMatrix("_WorldToMask", worldToMask);

                    spriteRenderer.SetPropertyBlock(MaskedPropertyBlock);
                }
            }
        }

        /// <summary>
        /// Checkes to see if the material is masked or not via a shader tag
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public bool IsMaskedMaterial(Material material)
        {
            string shaderTag = material.GetTag(SHADER_MASK_TAG, false, "false").ToLowerInvariant();
            return shaderTag.Equals("true");
        }

        /// <summary>
        /// Makes sure there isn't a duplicate name and ID of the shader in the project
        /// </summary>
        /// <param name="material"></param>
        private void ValidateShader(Material material)
        {
            if ((material.shader.ToString() == DefaultMaskedSpriteShader.ToString()) &&
                material.shader.GetInstanceID() != DefaultMaskedSpriteShader.GetInstanceID())
            {
                GameIdLogger.Logger.Debug("There seems to be more than one masked shader in the project with the same display name, and it's preventing the mask from being properly applied.");
                DefaultMaskedSpriteShader = null;
            }

            if ((material.shader.ToString() == DefaultMaskedUnlitShader.ToString()) &&
                (material.shader.GetInstanceID() != DefaultMaskedUnlitShader.GetInstanceID()))
            {
                GameIdLogger.Logger.Debug("There seems to be more than one masked shader in the project with the same display name, and it's preventing the mask from being properly applied.");
                DefaultMaskedUnlitShader = null;
            }
        }
    }
}
