#region Using
using System;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Camera
{
    /// <summary>
    /// Requirements:
    ///    Meant to be attached to the main camera.
    ///    Any existing AspectRatioRectifier script must be removed.
    ///    The main camera this is attached to must be placed inside of its own game object container (i.e."CameraParent").
    ///    The reason for the CameraParent is for any cameras that are controlled by animators.
    ///    At least 3 CameraAspectRatioProfileData scriptable object that contain the normal, widest, and tallest aspect ratios expected.
    /// This script will detect the current aspect ratio and either load up a data profile for the camera to use exactly,
    /// or interpolate between the nearest two data profiles.
    /// </summary>
    [ExecuteAlways]
    [ExecuteInEditMode]
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraAspectRatioRectifier : MonoBehaviour
    {
        #region Public Fields

        /// <summary>
        /// A list of available CameraAspectRatioProfileData scriptable objects to use.
        /// </summary>
        [Tooltip("A list of available CameraAspectRatioProfileData scriptable objects to use.")]
        public List<CameraAspectRatioProfileData> AspectRatioProfileList;

        /// <summary>
        /// The camera this script is attached to.
        /// </summary>
        public UnityEngine.Camera Camera;

        #endregion

        #region Private Fields

        private Vector2 _currentDimensions;

        #endregion

        #region Monobehaviour Methods

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        protected virtual void Start()
        {
            // Quick check to make sure the AspectRatioRectifier has been removed.
            // TODO: Check for aspect ratio rectifier
            // if (this.GetComponent<AspectRatioRectifier>() != null)
            // {
            //     StaticLogForwarder.Logger.LogError("Cannot have multiple CameraAspectRatioRectifiers! Please remove the AspectRatioRectifier script from the " + name);
            // }
            // Quick check to make sure the Camera has a container game object that is not the top most game object.
            if (transform.parent == null || transform.parent.parent == null)
            {
                GameIdLogger.Logger.Error("The " + name + " needs to be inside its own container game object in the hierarchy like this: " + transform.root.name + "/CameraParent/" + name);
            }
            if (AspectRatioProfileList.Count < 3)
            {
                GameIdLogger.Logger.Error("CameraAspectRatioRectifier requires at least 3 Aspect Ratio Profiles.");
            }

            if (Camera == null)
            {
                GameIdLogger.Logger.Error("Please attach camera.");
            }

            RectifyCameraAspectRatio();
        }

        /// <summary>
        /// Called once per frame.
        /// Also called even in edit mode due to the attributes on this class.
        /// </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (ShouldUpdate())
            {
                RectifyCameraAspectRatio();
            }
#endif
        }

        #endregion

        #region Camera Rectification Methods

        public virtual void RectifyCameraAspectRatio()
        {
            // Detect the screen's current aspect ratio.
            var screenAspect = (float)Screen.width / (float)Screen.height;
            _currentDimensions = new Vector2(Screen.width, Screen.height);

            // First pass, let's see if there's a screen aspect that's almost equal to one of our aspect ratio profiles.
            foreach (var aspectRatioProfile in AspectRatioProfileList)
            {
                // If we detect an aspect ratio that is fairly close, then just use the profile that matches it.
                if (AlmostEqual(aspectRatioProfile.GetAspectRatio(), screenAspect, 0.01f))
                {
                    // Set the aspect ratio and then end the function.
                    ConfigureCamera(aspectRatioProfile);

                    // Notify responders
                    transform.root.GetComponentInParent<CameraUpdateNotifier>()?.NotifyCameraUpdated();
                    return;
                }
            }

            // If we're here, then we couldn't find an aspect ratio profile that matched exactly.
            // Now we need to find the two closest profiles.
            var profiles = GetClosestAspectRatioProfiles(screenAspect);

            // Now create a new interpolated profile.
            ConfigureCamera(InterpolatedProfileData(profiles, screenAspect));

            // Notify responders
            transform.root.GetComponentInParent<CameraUpdateNotifier>()?.NotifyCameraUpdated();
        }

        /// <summary>
        /// Configures the camera to the profile.
        /// </summary>
        /// <param name="profile"></param>
        public virtual void ConfigureCamera(CameraAspectRatioProfileData profile)
        {
            // If this is an orthographic camera, set the ortho size.
            if (Camera.orthographic)
            {
                Camera.orthographicSize = profile.OrthographicSize;
            }
            // If this is a perspective camera, set the field of view.
            else
            {
                Camera.fieldOfView = profile.FieldOfView;
            }

            // Set the position offset on the parent of the camera.
            transform.parent.transform.localPosition = profile.PositionOffset;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Detects if our stored dimensions are different from the current screen dimensions.
        /// </summary>
        /// <returns></returns>
        private bool ShouldUpdate()
        {
            return !Mathf.Approximately(_currentDimensions.x, Screen.width)
                   || !Mathf.Approximately(_currentDimensions.y, Screen.height);
        }

        /// <summary>
        /// Picks an interpolated value to get us the best camera data depending on the current aspect ratio.
        /// </summary>
        /// <param name="profiles"></param>
        /// <param name="screenAspect"></param>
        /// <returns></returns>
        public CameraAspectRatioProfileData InterpolatedProfileData(List<CameraAspectRatioProfileData> profiles, float screenAspect)
        {
            var interpolatedProfile = ScriptableObject.CreateInstance<CameraAspectRatioProfileData>();

            if (profiles.Count != 2)
            {
                GameIdLogger.Logger.Error("There should only be 2 profiles to interpolate between, something went wrong somewhere...");
            }

            var aspectA = profiles[0].GetAspectRatio();
            var aspectB = profiles[1].GetAspectRatio();
            // Get the total difference between our two available aspect ratios.
            var totalDifference = Mathf.Abs(aspectA - aspectB);
            // Get the difference between the current screen aspect and our starting point.
            var difference = aspectA > aspectB ? Mathf.Abs(aspectA - screenAspect) : Mathf.Abs(aspectB - screenAspect);
            // Divide and then we have our lerp factor.
            var lerpFactor = difference / totalDifference;

            // Now flip the lerp factor if aspectB is the larger aspect.
            if (aspectA < aspectB)
            {
                lerpFactor = 1 - lerpFactor;
            }

            // Interpolate the orthographic size for ortho cameras.
            if (Camera.orthographic)
            {
                interpolatedProfile.OrthographicSize = Mathf.Lerp(profiles[0].OrthographicSize, profiles[1].OrthographicSize, lerpFactor);
            }
            // Interpolate the FOV for perspective cameras.
            else
            {
                interpolatedProfile.FieldOfView = Mathf.Lerp(profiles[0].FieldOfView, profiles[1].FieldOfView, lerpFactor);
            }

            interpolatedProfile.PositionOffset = new Vector3(
                Mathf.Lerp(profiles[0].PositionOffset.x, profiles[1].PositionOffset.x, lerpFactor),
                Mathf.Lerp(profiles[0].PositionOffset.y, profiles[1].PositionOffset.y, lerpFactor),
                Mathf.Lerp(profiles[0].PositionOffset.z, profiles[1].PositionOffset.z, lerpFactor));

            return interpolatedProfile;
        }

        /// <summary>
        /// Finds the two closest aspect ratio profiles to the provided aspect ratio.
        /// </summary>
        /// <param name="screenAspect"></param>
        /// <returns></returns>
        public List<CameraAspectRatioProfileData> GetClosestAspectRatioProfiles(float screenAspect)
        {
            var closestProfiles = new List<CameraAspectRatioProfileData>();
            var maxClosest = 2;

            // Iterate through all aspect ratio profiles...
            foreach (var aspectRatioProfile in AspectRatioProfileList)
            {
                // Until we reach 2 profiles, just add them to the list.
                if (closestProfiles.Count < maxClosest)
                {
                    closestProfiles.Add(aspectRatioProfile);
                }
                else
                {
                    // Find out how close our current profile is versus the two others.
                    var difference = aspectRatioProfile.GetDifferenceFromCurrentAspect(screenAspect);
                    var firstProfile = closestProfiles[0].GetDifferenceFromCurrentAspect(screenAspect);
                    var secondProfile = closestProfiles[1].GetDifferenceFromCurrentAspect(screenAspect);

                    // If we're closer than either of the existing profiles...
                    if (difference < firstProfile || difference < secondProfile)
                    {
                        var profileToOverwrite = firstProfile < secondProfile ? 1 : 0;

                        closestProfiles[profileToOverwrite] = aspectRatioProfile;
                    }
                }
            }

            return closestProfiles;
        }

        /// <summary>
        /// Given two floats and a comparison value (epsilon),
        /// if the difference between the floats is less than epsilon, will return true.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool AlmostEqual(float a, float b, float epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float difference = Math.Abs(a - b);

            // We're almost equal if we're actually equal.
            if (a == b)
            {
                return true;
            }
            // If a or b is zero, or both are very close to it.
            else if (a == 0 || b == 0 || absA + absB < float.MinValue)
            {
                return difference < (epsilon * float.MinValue);
            }
            // Use the relative error.
            else
            {
                return difference / (absA + absB) < epsilon;
            }
        }

        #endregion
    }
}
