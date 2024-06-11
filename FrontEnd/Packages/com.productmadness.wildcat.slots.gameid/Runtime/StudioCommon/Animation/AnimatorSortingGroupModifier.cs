// v1.3

using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using RotaryHeart.Lib.AutoComplete;
using System;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Helper component that allows engineers/technical artists the ability to
    /// change which layer the game object is rendered on via an animation event.
    /// </summary>
    public class AnimatorSortingGroupModifier : MonoBehaviour
    {
        [Serializable]
        private class SortingPair
        {
            private static class SortingLayerPropertyDrawerHelper
            {
                public static string[] GetSortingLayers()
                {
                    var data = SortingLayer.layers;
                    return data.Select(layer => layer.name).ToArray();
                }
            }

            [EnabledIfTrue("_updateSortingLayers")]
            [SerializeField][AutoComplete(typeof(SortingLayerPropertyDrawerHelper), "GetSortingLayers")] private string _targetSortingLayer;

            [EnabledIfTrue("_updateSortingOrders")]
            [SerializeField] private int _targetSortingOrder;

            [SerializeField] private SortingGroup _sortingGroup;

            private int _defaultSortingLayerId;
            private int _defaultSortingOrder;
            private int _targetSortingLayerId;

#if UNITY_EDITOR
            /// <summary>
            /// For assigning default components in the editor
            /// </summary>
            /// <param name="gameObject"></param>
            public void AssignDefault(GameObject gameObject)
            {
                if (_sortingGroup == null)
                {
                    _sortingGroup = gameObject.GetComponentInChildren<SortingGroup>();
                    _targetSortingLayer = _sortingGroup.sortingLayerName;
                    _targetSortingOrder = _sortingGroup.sortingOrder;
                }
            }

            /// <summary>
            /// Constructor to automatically create a default sorting pair
            /// </summary>
            /// <param name="gameObject"></param>
            public SortingPair(GameObject gameObject)
            {
                AssignDefault(gameObject);
            }
#endif

            public void Init()
            {
                _defaultSortingLayerId = _sortingGroup.sortingLayerID;
                _defaultSortingOrder = _sortingGroup.sortingOrder;
                _targetSortingLayerId = FetchSortingLayerId(_targetSortingLayer);
            }

            /// <summary>
            /// Helper method to convert sorting layer names to sorting layer IDs
            /// </summary>
            /// <param name="sortingLayerName">Name of sorting layer to fetch</param>
            /// <returns>ID of sorting layer fetched</returns>
            private int FetchSortingLayerId(string sortingLayerName)
            {
                return SortingLayer.layers.FirstOrDefault(layer => layer.name.Equals(sortingLayerName)).id;
            }

            /// <summary>
            /// Helper method to assign the sorting group the target sorting layer ID
            /// </summary>
            public void UpdateSorting(bool updateSortingLayer, bool updateSortingOrder)
            {
                if (updateSortingLayer)
                {
                    _sortingGroup.sortingLayerID = _targetSortingLayerId;
                }
                if (updateSortingOrder)
                {
                    _sortingGroup.sortingOrder = _targetSortingOrder;
                }
            }

            /// <summary>
            /// A helper method to set the sorting group back to it's default layer ID
            /// </summary>
            public void SetDefaultLayer(bool updateSortingLayer, bool updateSortingOrder)
            {
                if (updateSortingLayer)
                {
                    _sortingGroup.sortingLayerID = _defaultSortingLayerId;
                }
                if (updateSortingOrder)
                {
                    _sortingGroup.sortingOrder = _defaultSortingOrder;
                }
            }
        }

        [SerializeField] public bool _updateSortingLayers = true;
        [SerializeField] public bool _updateSortingOrders = false;

        [SerializeField] private SortingPair[] _sortingPairs;

#if UNITY_EDITOR
        /// <summary>
        /// Editor method to auto assign the sorting group components
        /// </summary>
        private void OnValidate()
        {
            foreach (var sortingPair in _sortingPairs)
            {
                sortingPair.AssignDefault(gameObject);
            }
        }

        /// <summary>
        /// Editor method to auto create a default sorting pair based on what is assigned in the SortingGroup component
        /// </summary>
        private void Reset()
        {
            _sortingPairs = new SortingPair[]
            {
                new SortingPair(gameObject)
            };
        }
#endif

        /// <summary>
        /// Initializes the sorting pairs
        /// </summary>
        private void Awake()
        {
            foreach (var sortingPair in _sortingPairs)
            {
                sortingPair.Init();
            }
        }

        /// <summary>
        /// Animation event to update the sorting layer using pre defined sorting layers
        /// </summary>
        /// <param name="targetLayerIndex">Index of sorting layer in targetSortingLayers to switch to</param>
        private void UpdateSortingLayer(int targetIndex)
        {
            if (targetIndex >= _sortingPairs.Length)
            {
                GameIdLogger.Logger.Error("Sorting pairs on " + gameObject.name + " only contains " + _sortingPairs.Length + " sorting pairs. Animator event tried targeting index " + targetIndex);
                return;
            }

            _sortingPairs[targetIndex].UpdateSorting(_updateSortingLayers, _updateSortingOrders);
        }

        /// <summary>
        /// Animation event to reset the sorting layer to the default sorting layer
        /// </summary>
        private void SetDefaultLayer(int targetIndex)
        {
            if (targetIndex >= _sortingPairs.Length)
            {
                GameIdLogger.Logger.Error("Sorting pairs on " + gameObject.name + " only contains " + _sortingPairs.Length + " sorting pairs. Animator event tried targeting index " + targetIndex);
                return;
            }

            _sortingPairs[targetIndex].SetDefaultLayer(_updateSortingLayers, _updateSortingOrders);
        }
    }
}
