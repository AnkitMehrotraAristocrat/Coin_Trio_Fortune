using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A component that will position it's game object relative to the camera's viewport.
    /// Notes:
    ///  - The bottom left corner of the camera's viewport is [0, 0]
    ///  - The top right corner of the camera's viewport is [1, 1]
    /// </summary>
    public class CameraViewportPositioner : MonoBehaviour, IRecoveryHandler, ICameraUpdateResponder
    {
        private class OffsetData
        {
            public Vector3 PositionOffset;
            public float AspectRatioQuotient;
        }

        [SerializeField] private UnityEngine.Camera _camera;

        [SerializeField] private bool _useViewportX = false;
        [SerializeField] private bool _useViewportY = false;

        [SerializeField][Range(0.0f, 1.0f)] private float _viewportX = 1.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float _viewportY = 0.5f;

        [SerializeField] private float _worldSpaceTargetX;
        [SerializeField] private float _worldSpaceTargetY;
        [SerializeField] private float _worldSpaceTargetZ = 0.0f;

        [SerializeField] private List<CameraViewportOffsetModifierSO> _offsetModifiers;

        private LinkedList<OffsetData> _sortedOffsetData = new LinkedList<OffsetData>();

        public void Awake()
        {
            Validate();
            PopulateSortedOffsetData();
        }

        public void OnInitialStateReady(ServiceLocator locator)
        {
            UpdatePosition();
        }

        public void CameraUpdated()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 worldInViewportSpace = _camera.WorldToViewportPoint(new Vector3(_worldSpaceTargetX, _worldSpaceTargetY, _worldSpaceTargetZ));

            Vector3 viewportInWorldSpace = _camera.ViewportToWorldPoint(new Vector3(
                _useViewportX ? _viewportX : worldInViewportSpace.x,
                _useViewportY ? _viewportY : worldInViewportSpace.y,
                worldInViewportSpace.z));

            gameObject.transform.position = ApplyOffset(viewportInWorldSpace);
        }

        private Vector3 ApplyOffset(Vector3 origin)
        {
            float screenAspectRatio = (float)Screen.width / (float)Screen.height;
            List<OffsetData> profiles = GetClosestModifiers(screenAspectRatio);
            Vector3 derivedOffset;

            if (profiles.Count > 1)
            {
                derivedOffset = InterpolateModifers(profiles, screenAspectRatio);
            }
            else
            {
                derivedOffset = profiles[0].PositionOffset;
            }

            return CalculateOffsetPosition(origin, derivedOffset);
        }

        private Vector3 CalculateOffsetPosition(Vector3 origin, Vector3 offset)
        {
            return origin + offset;
        }

        private Vector3 InterpolateModifers(List<OffsetData> profiles, float screenAspect)
        {
            float aspectA = profiles[0].AspectRatioQuotient;
            float aspectB = profiles[1].AspectRatioQuotient;

            float totalDifference = Mathf.Abs(aspectA - aspectB);
            float difference = aspectA > aspectB ? Mathf.Abs(aspectA - screenAspect) : Mathf.Abs(aspectB - screenAspect);
            float lerpFactor = difference / totalDifference;

            if (aspectA < aspectB)
            {
                lerpFactor = 1 - lerpFactor;
            }

            return new Vector3(
                Mathf.Lerp(profiles[0].PositionOffset.x, profiles[1].PositionOffset.x, lerpFactor),
                Mathf.Lerp(profiles[0].PositionOffset.y, profiles[1].PositionOffset.y, lerpFactor),
                Mathf.Lerp(profiles[0].PositionOffset.z, profiles[1].PositionOffset.z, lerpFactor));
        }

        private List<OffsetData> GetClosestModifiers(float screenAspect)
        {
            List<OffsetData> closestProfiles = new List<OffsetData>();

            var currentNode = _sortedOffsetData.First;
            while (currentNode != null)
            {
                if (currentNode.Value.AspectRatioQuotient <= screenAspect && currentNode.Next.Value.AspectRatioQuotient >= screenAspect)
                {
                    closestProfiles.Add(currentNode.Value);
                    closestProfiles.Add(currentNode.Next.Value);
                    break;
                }
                currentNode = currentNode.Next;
            }

            if (closestProfiles.Count > 0)
            {
                return closestProfiles;
            }

            if (_sortedOffsetData.First.Value.AspectRatioQuotient > screenAspect)
            {
                closestProfiles.Add(_sortedOffsetData.First.Value);
            }
            else if (_sortedOffsetData.Last.Value.AspectRatioQuotient < screenAspect)
            {
                closestProfiles.Add(_sortedOffsetData.Last.Value);
            }
            return closestProfiles;
        }

        private void PopulateSortedOffsetData()
        {
            var sortedList = _offsetModifiers.OrderBy(aspectRatio => aspectRatio.AspectRatioQuotient).ToList();
            foreach (CameraViewportOffsetModifierSO modifer in sortedList)
            {
                OffsetData data = new OffsetData()
                {
                    PositionOffset = modifer.PositionOffset,
                    AspectRatioQuotient = modifer.AspectRatioQuotient
                };
                _sortedOffsetData.AddLast(data);
            }
        }

        #region Validation
        private void Validate()
        {
            ValidateOffsetModifierCount();
        }

        private void ValidateOffsetModifierCount()
        {
            if (_offsetModifiers.Count < 3)
            {
                GameIdLogger.Logger.Error(GetType() + " requires at least 3 CameraViewportOffsetModifiers!", this);
            }
        }

        #endregion

        #region Unused Interface Implementations
        public void OnServerConfigsReady(ServiceLocator locator)
        {
            return;
        }
        #endregion
    }
}
