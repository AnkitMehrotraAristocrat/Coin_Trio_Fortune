using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Camera
{
    /// <summary>
    /// ScriptableObject containing data for the CameraAspectRatioRectifier.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraAspectRatioProfileData", menuName = "NMG/CameraAspectRatioProfileData", order = 0)]
    public class CameraAspectRatioProfileData : ScriptableObject
    {
        /// <summary>
        /// A vector 2 representation of an aspect ratio, such as 9:16.
        /// </summary>
        [Tooltip("A vector 2 representation of an aspect ratio, such as 9:16.")]
        public Vector2 AspectRatio;

        /// <summary>
        /// The field of view that will be assigned to the camera if the AspectRatio is detected.
        /// </summary>
        [Tooltip("Perspective Cameras Only: The field of view that will be assigned to the camera if the AspectRatio is detected.")]
        public float FieldOfView;

        /// <summary>
        /// Orthographic Cameras: Projection size of the camera.
        /// </summary>
        [Tooltip("Orthographic Cameras Only: Projection size of the camera.")]
        public float OrthographicSize;

        /// <summary>
        /// The position offset that will be assigned to the camera's parent object (should not be the top most SlotMachine object!). Not required.
        /// </summary>
        [Tooltip("The position offset that will be assigned to the camera's parent object (should not be the top most SlotMachine object!). Not required.")]
        public Vector3 PositionOffset;

        /// <summary>
        /// Gets the aspect ratio as a single float value.
        /// </summary>
        /// <returns></returns>
        public float GetAspectRatio()
        {
            return AspectRatio.x / AspectRatio.y;
        }

        /// <summary>
        /// Returns how close the current aspect ratio is from this profile's aspect ratio.
        /// </summary>
        /// <param name="currentAspect"></param>
        /// <returns></returns>
        public float GetDifferenceFromCurrentAspect(float currentAspect)
        {
            return Mathf.Abs(currentAspect - GetAspectRatio());
        }
    }
}
