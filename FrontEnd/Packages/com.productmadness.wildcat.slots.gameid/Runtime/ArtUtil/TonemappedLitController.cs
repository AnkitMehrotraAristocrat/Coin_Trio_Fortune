// Universal Render Pipeline Color correction shader by AtomicJoe
// feel free to use it as you like, just don't sell this in the assetstore
// let's keep it free for all :)
// version 1.01



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class TonemappedLitController : MonoBehaviour
    {
        [Tooltip("Be sure to disable color correction to render lightmaps!")]
        public bool enableColorCorrection = true;

        [Tooltip("Default: White")]
        public Color colorFilter = Color.white;
        [Tooltip("Default: 1.0\nPress ALT + Click to adjust slowly")]
        public float gamma = 1f;
        [Tooltip("Default: 1.0\nPress ALT + Click to adjust slowly")]
        public float contrast = 1f;
        [Tooltip("Default: 0.0\nPress ALT + Click to adjust slowly")]
        public float brightness = 0f;
        [Tooltip("Default: 1.0\nPress ALT + Click to adjust slowly")]
        public float saturation = 1f;
        [Range(-1f,1f), Tooltip("Default: 0.0")]
        public float hueShift = 0f;

        public static TonemappedLitController instance;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            UpdateShaderValues(colorFilter, gamma, contrast, brightness, saturation, hueShift);
        }

        private void OnValidate()
        {
            if (enableColorCorrection) UpdateShaderValues(colorFilter, gamma, contrast, brightness, saturation, hueShift); else UpdateShaderValues(Color.white, 1f, 1f, 0f, 1f, 0f);
        }

        public static void UpdateShaderValues(Color colorFilter, float gamma = 1f, float contrast = 1f, float brightness = 0f, float saturation = 1f, float hueShift = 0f)
        {
            Shader.SetGlobalColor("_TonemappedLitColorFilter", new Color(colorFilter.r - 1f, colorFilter.g - 1f, colorFilter.b - 1f, colorFilter.a - 1f));
            Shader.SetGlobalFloat("_TonemappedLitGamma", gamma-1f);
            Shader.SetGlobalFloat("_TonemappedLitContrast", contrast-1f);
            Shader.SetGlobalFloat("_TonemappedLitBrightness", brightness);
            Shader.SetGlobalFloat("_TonemappedLitSaturation", saturation - 1f);
            Shader.SetGlobalFloat("_TonemappedLitHueShift", hueShift);
        }
    }
}