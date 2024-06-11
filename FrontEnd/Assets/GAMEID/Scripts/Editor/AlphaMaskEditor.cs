using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.AlphaMasking
{
    [CustomEditor(typeof(AlphaMask))]
    public class AlphaMaskEditor : Editor
    {
        //TODO: see if Sprites/Default can also work
        private const string DEFAULT_SPRITE_SHADER = "Universal Render Pipeline/2D/Sprite-Unlit-Default";
        private Shader spriteDefaultShader;
        private Shader SpriteDefaultShader
        {
            get
            {
                if (!spriteDefaultShader)
                    spriteDefaultShader = Shader.Find(DEFAULT_SPRITE_SHADER);
                return spriteDefaultShader;
            }
        }

        private const string DEFAULT_PARTICLE_SHADER = "Particles/Alpha Blended Premultiply";
        private Shader particleDefaultShader;
        private Shader ParticleDefaultShader
        {
            get
            {
                if (!particleDefaultShader) particleDefaultShader = Shader.Find(DEFAULT_PARTICLE_SHADER);
                return particleDefaultShader;
            }
        }

        private const string UNLIT_TRANSPARENT_SHADER = "Unlit/Transparent";
        private Shader unlitTransparentShader;
        public Shader UnlitTransparentShader 
        {
            get
            {
                if (!unlitTransparentShader) unlitTransparentShader = Shader.Find(UNLIT_TRANSPARENT_SHADER);
                return unlitTransparentShader;
            }
        }

        private const string DEFAULT_UI_SHADER = "UI/Default";
        private Shader defaultShaderUI;
        private Shader DefaultShaderUI
        {
            get
            {
                if (!defaultShaderUI)
                    defaultShaderUI = Shader.Find(DEFAULT_UI_SHADER);
                return defaultShaderUI;
            }
        }

        private const string DEFAULT_FONT_SHADER = "UI/Default Font";
        private Shader defaultFontShaderUI;
        private Shader DefaultFontShaderUI
        {
            get
            {
                if (!defaultFontShaderUI)
                    defaultFontShaderUI = Shader.Find(DEFAULT_FONT_SHADER);
                return defaultFontShaderUI;
            }
        }

        private Shader maskedUnlitShader;
        private Shader MaskedUnlitShader
        {
            get
            {
                if (!maskedUnlitShader)
                    maskedUnlitShader = Shader.Find(AlphaMask.MASKED_UNLIT_SHADER);
                return maskedUnlitShader;
            }
        }

        private Shader maskedSpriteShader;
        private Shader MaskedSpriteShader
        {
            get
            {
                if (!maskedSpriteShader)
                    maskedSpriteShader = Shader.Find(AlphaMask.MASKED_SPRITE_SHADER);
                return maskedSpriteShader;
            }
        }

        public override void OnInspectorGUI()
        {
            AlphaMask alphaMaskTarget = (AlphaMask)target;
            bool changesMade = false;

            if (alphaMaskTarget.GetComponents<AlphaMask>().Length > 1)
            {
                GUILayout.Label("More than one instance of AlphaMask attached.\nPlease only use one.");
                return;
            }

            if (alphaMaskTarget.GetComponent<MeshRenderer>().sharedMaterial != null)
            {
                Texture maskTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Base Map:", "The texture that will be used to mask objects"), 
                    alphaMaskTarget.BaseMap, typeof(Texture), true);
                if (maskTexture != alphaMaskTarget.BaseMap)
                {
                    alphaMaskTarget.BaseMap = maskTexture;
                    changesMade = true;
                }

                Vector2 tiling = EditorGUILayout.Vector2Field(new GUIContent("Tiling:", "The tiling of the texture that will be used to mask objects"), 
                    alphaMaskTarget.BaseMapTiling);
                if (tiling != alphaMaskTarget.BaseMapTiling)
                {
                    alphaMaskTarget.BaseMapTiling = tiling;
                    changesMade = true;
                }

                Vector2 offset = EditorGUILayout.Vector2Field(new GUIContent("Offset:", "The offset of the texture that will be used to mask objects"), 
                    alphaMaskTarget.BaseMapOffset);
                if (offset != alphaMaskTarget.BaseMapOffset)
                {
                    alphaMaskTarget.BaseMapOffset = offset;
                    changesMade = true;
                }

                bool isMaskingEnabled = EditorGUILayout.Toggle(new GUIContent("Masking Enabled", "Does the mask need to have the effect on the siblings?"), 
                    alphaMaskTarget.IsMaskingEnabled);
                if (isMaskingEnabled != alphaMaskTarget.IsMaskingEnabled)
                {
                    alphaMaskTarget.IsMaskingEnabled = isMaskingEnabled;
                    changesMade = true;
                }

                bool cropMask = EditorGUILayout.Toggle(new GUIContent("Crop Mask", "Keep mask to its Game Object bounds."), 
                    alphaMaskTarget.CropMask);
                if (cropMask != alphaMaskTarget.CropMask)
                {
                    alphaMaskTarget.CropMask = cropMask;
                    changesMade = true;
                }

                if (cropMask)
                {
                    float croppedSize = EditorGUILayout.Slider(new GUIContent("Cropped Size", "Percentage of Game Object crop used. 1 = 100%"), 
                        alphaMaskTarget.CroppedSize, 0f, 1f);
                    if (croppedSize != alphaMaskTarget.CroppedSize)
                    {
                        alphaMaskTarget.CroppedSize = croppedSize;
                        changesMade = true;
                    }
                }

                bool clampAlphaHorizontally = EditorGUILayout.Toggle(new GUIContent("Clamp Horizontally", 
                    "If the texture isn't clamped by Unity (in import settings), then you can choose to clamp it horizontally only(it will be repeated vertically, unless chosen otherwise)"), 
                    alphaMaskTarget.ClampAlphaHorizontally);
                if (clampAlphaHorizontally != alphaMaskTarget.ClampAlphaHorizontally)
                {
                    alphaMaskTarget.ClampAlphaHorizontally = clampAlphaHorizontally;
                    changesMade = true;
                }

                bool clampAlphaVertically = EditorGUILayout.Toggle(new GUIContent("Clamp Vertically", 
                    "If the texture isn't clamped by Unity (in import settings), then you can choose to clamp it vertically only(it will be repeated horizontally, unless chosen otherwise)."), 
                    alphaMaskTarget.ClampAlphaVertically);
                if (clampAlphaVertically != alphaMaskTarget.ClampAlphaVertically)
                {
                    alphaMaskTarget.ClampAlphaVertically = clampAlphaVertically;
                    changesMade = true;
                }

                if (clampAlphaHorizontally || clampAlphaVertically)
                {
                    float clampingBorder = EditorGUILayout.FloatField(new GUIContent("Clamping Border",
                        "If one of the two above settings are enabled, you can use this variable to tweak the “edge” of clamping.Depending on the alpha texture size and its usage, you might runinto texture clamping issues.In that case, try increasing(or lowering) the Clamping Border value."), 
                        alphaMaskTarget.ClampingBorder);
                    if (clampingBorder != alphaMaskTarget.ClampingBorder)
                    {
                        alphaMaskTarget.ClampingBorder = clampingBorder;
                        changesMade = true;
                    }
                }

                bool useMaskAlphaChannel = EditorGUILayout.Toggle(new GUIContent("Use Mask Alpha Channel (not RGB)", "The mask uses the texture RGB channels by default. Togglethis option to use the Alpha channel of the texture instead."), 
                    alphaMaskTarget.UseMaskAlphaChannel);
                if (useMaskAlphaChannel != alphaMaskTarget.UseMaskAlphaChannel)
                {
                    alphaMaskTarget.UseMaskAlphaChannel = useMaskAlphaChannel;
                    changesMade = true;
                }

                if (!Application.isPlaying)
                {
                    bool doDisplayMask = EditorGUILayout.Toggle(new GUIContent("Display Mask", "Toggle this setting to enable or disable the visibility of the mask (only the visibility, not its effect).This setting is only available in the Editor and when not running the game."), 
                        alphaMaskTarget.displayMask);
                    if (alphaMaskTarget.displayMask != doDisplayMask)
                    {
                        alphaMaskTarget.displayMask = doDisplayMask;
                        changesMade = true;
                    }
                }

                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("Apply Mask to Siblings in Hierarchy"))
                    {
                        ApplyMask();
                        changesMade = true;
                    }
                }
            }
            else
            {
                GUILayout.Label("Please assign Mask-Material to mesh renderer.");
            }

            if (changesMade)
            {
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Applies a Mask to a game object's renderer
        /// </summary>
        private void ApplyMask()
        {
            AlphaMask alphaMaskTarget = (AlphaMask)target;
            alphaMaskTarget.ScheduleMaskRefresh();

            if (MaskedSpriteShader == null || MaskedUnlitShader == null)
            {
                Debug.Log("Shaders necessary for masking don't seem to be present in the project.");
                return;
            }

            List<Renderer> maskedRenderers = new List<Renderer>();
            alphaMaskTarget.transform.parent.gameObject.GetComponentsInChildren(true, maskedRenderers);
            Renderer exludedRenderer = alphaMaskTarget.transform.parent.GetComponent<Renderer>();
            if (exludedRenderer != null) maskedRenderers.Remove(exludedRenderer);

            // TODO: Add UI Renderers

            // Mask materials that already exist in the mask hierarchy
            List<Material> reusableMaterials = GetAllReusableMaterials(maskedRenderers);

            List<Material> originalMaterials = new List<Material>();
            List<Material> newMaterials = new List<Material>();

            foreach (Renderer renderer in maskedRenderers)
            {
                //Don't mask the mask
                if (renderer.gameObject == alphaMaskTarget.gameObject)
                {
                    continue;
                }

                if (renderer is SpriteRenderer)
                {
                    ApplyMaskToSpriteRenderer((SpriteRenderer)renderer, alphaMaskTarget);
                }
                else
                {
                    ApplyMaskToGenericRenderer(renderer, alphaMaskTarget, originalMaterials, newMaterials, reusableMaterials);
                }

                //TODO: add UI material loop
                Debug.Log("Alpha Mask applied." + (alphaMaskTarget.IsMaskingEnabled ? "" : " Keep in mind that masking is disabled, so you will not see the effects until you enable alpha masking."));
            }
        }

        /// <summary>
        /// Gets all Materials that have already been instantiated
        /// </summary>
        /// <param name="renderers"></param>
        /// <returns></returns>
        private List<Material> GetAllReusableMaterials(List<Renderer> renderers)
        {
            List<Material> reusableMaterials = new List<Material>();
            AlphaMask maskTarget = (AlphaMask)target;

            foreach (Renderer mRenderer in renderers)
            {
                if (mRenderer.gameObject != maskTarget.gameObject)
                {
                    for (int i = 0; i < mRenderer.sharedMaterials.Length; i++)
                    {
                        Material material = mRenderer.sharedMaterials[i];

                        if (material != null && (material.shader == MaskedUnlitShader || material.shader == MaskedSpriteShader) && !reusableMaterials.Contains(material))
                        {
                            reusableMaterials.Add(material);
                        }
                    }
                }
            }

            return reusableMaterials;
        }

        /// <summary>
        /// Applies a mask to a Sprite Renderer from Alpha Mask
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="alphaMaskTarget"></param>
        private void ApplyMaskToSpriteRenderer(SpriteRenderer spriteRenderer, AlphaMask alphaMaskTarget)
        {
            Material materialToReplace = spriteRenderer.sharedMaterial;
            if (materialToReplace == null)
            {
                return;
            }

            if (!alphaMaskTarget.IsMaskedMaterial(materialToReplace))
            {
                if (IsSupported2DShader(spriteRenderer.sharedMaterial.shader))
                {
                    materialToReplace = alphaMaskTarget.SpritesAlphaMaskWorldCoords;
                }
                spriteRenderer.sharedMaterial = materialToReplace;
            }
        }

        /// <summary>
        /// Apply a mask to a renderer that isn't a Sprite Renderer from Alpha Mask
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="alphaMaskTarget"></param>
        /// <param name="originalMaterials"></param>
        /// <param name="newMaterials"></param>
        /// <param name="reusableMaterials"></param>
        private void ApplyMaskToGenericRenderer(Renderer renderer, AlphaMask alphaMaskTarget, List<Material> originalMaterials, List<Material> newMaterials, List<Material> reusableMaterials)
        {
            List<Material> currentSharedMaterials = new List<Material>();

            currentSharedMaterials.AddRange(renderer.sharedMaterials);

            bool materialsChanged = false;
            for (int i = 0; i < currentSharedMaterials.Count; i++)
            {
                Material material = currentSharedMaterials[i];
                if(currentSharedMaterials[i] == null)
                {
                    continue;
                }

                if (!originalMaterials.Contains(material))
                {
                    if ((material.shader == UnlitTransparentShader) || (material.shader = SpriteDefaultShader))
                    {
                        Material reusableMaterial = FindSuitableMaskedMaterial(material, reusableMaterials, 0);

                        if (reusableMaterial == null)
                        {
                            Material newMaterial = new Material(material);

                            newMaterial.shader = GetMaskedShaderEquivalent(material.shader);

                            newMaterial.name = material.name + "Masked";
                            newMaterial.SetTexture("_AlphaTex", alphaMaskTarget.BaseMap);

                            originalMaterials.Add(material);
                            newMaterials.Add(newMaterial);

                            currentSharedMaterials[i] = newMaterial;
                            materialsChanged = true;
                        }
                        else
                        {
                            currentSharedMaterials[i] = reusableMaterial;
                            materialsChanged = true;

                            reusableMaterial.SetTexture("_AlphaTex", alphaMaskTarget.BaseMap);
                        }
                    }
                    else if((material.shader == MaskedSpriteShader) || (material.shader == MaskedUnlitShader))
                    {
                        if(material.GetTexture("_AlphaTex") != alphaMaskTarget.BaseMap)
                        {
                            material.SetTexture("_AlphaTex", alphaMaskTarget.BaseMap);
                        }
                    }
                }
                else
                {
                    int index = originalMaterials.IndexOf(material);

                    currentSharedMaterials[i] = newMaterials[index];
                    materialsChanged = true;
                }
            }

            if(materialsChanged == true)
            {
                renderer.sharedMaterials = currentSharedMaterials.ToArray();
            }
        }

        /// <summary>
        /// Searches for a Masked Material to match the gameObjects current material
        /// </summary>
        /// <param name="nonMaskedMaterial"></param>
        /// <param name="diffReusableMaterials"></param>
        /// <param name="isThisTextParam"></param>
        /// <returns></returns>
        private Material FindSuitableMaskedMaterial(Material nonMaskedMaterial, List<Material> diffReusableMaterials, float isThisTextParam)
        {
            foreach (Material material in diffReusableMaterials)
            {
                if ((nonMaskedMaterial.shader == SpriteDefaultShader) || nonMaskedMaterial.shader == ParticleDefaultShader || (nonMaskedMaterial.shader == DefaultShaderUI) && (material.shader == MaskedSpriteShader))
                {
                    if ((material.name == nonMaskedMaterial.name + " Masked") &&
                        (!material.HasProperty("PixelSnap") || !nonMaskedMaterial.HasProperty("PixelSnap") || (material.GetFloat("PixelSnap") == nonMaskedMaterial.GetFloat("PixelSnap"))) &&
                        (material.GetFloat("_IsThisText") == isThisTextParam))
                    {
                        return material;
                    }
                }
                else if (nonMaskedMaterial.shader == UnlitTransparentShader && material.shader == MaskedUnlitShader)
                {
                    if (material.name == nonMaskedMaterial.name + " Masked" && material.mainTexture == nonMaskedMaterial.mainTexture)
                    {
                        return material;
                    }
                }
                else if (nonMaskedMaterial.shader == DefaultFontShaderUI && material.shader == MaskedSpriteShader)
                {
                    if (material.name == nonMaskedMaterial.name + " Masked")
                    {
                        return material;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find necessary shader for a new reusable material
        /// </summary>
        /// <param name="defaultShader"></param>
        /// <returns></returns>
        private Shader GetMaskedShaderEquivalent(Shader defaultShader)
        {
            if (defaultShader == SpriteDefaultShader || defaultShader == DefaultShaderUI || defaultShader == DefaultFontShaderUI)
            {
                return MaskedSpriteShader;
            }
            if (defaultShader == UnlitTransparentShader)
            {
                return MaskedUnlitShader;
            }
            return defaultShader;
        }

        /// <summary>
        /// Checks to see if a 2D shader can be masked
        /// </summary>
        /// <param name="shader"></param>
        /// <returns></returns>
        private bool IsSupported2DShader (Shader shader)
        {
            return (shader == SpriteDefaultShader); // ||
                   
                   // TODO: apply UI
                   //(shader == DefaultShaderUI) ||
                   //(shader == DefaultFontShaderUI);
        }
    }
}
