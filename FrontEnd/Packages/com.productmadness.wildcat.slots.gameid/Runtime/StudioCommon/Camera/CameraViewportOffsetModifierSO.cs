using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A scriptable object that supports camera viewport based game object position offsetting.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraViewportOffsetModifier", menuName = "NMG/Camera Viewport Offset Modifier")]
    public class CameraViewportOffsetModifierSO : ScriptableObject
    {
        [Tooltip("A vector 2 representation of an aspect ratio, such as 9:16.")]
        public Vector2 AspectRatio;

        [Tooltip("The position offset that will be assigned to the camera's parent object (should not be the top most SlotMachine object!). Not required.")]
        public Vector3 PositionOffset;

        [HideInInspector] public float AspectRatioQuotient => AspectRatio.x / AspectRatio.y;
    }
}
