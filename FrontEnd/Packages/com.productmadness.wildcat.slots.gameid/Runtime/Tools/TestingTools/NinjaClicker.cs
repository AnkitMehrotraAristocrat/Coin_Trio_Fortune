using Milan.FrontEnd.Bridge.Meta;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Clicks like a ninja that's trained their body and mind for one singular purpose:
    /// to click faster than any mere mortal could imagine.
    /// Performs one short press every frame.
    /// </summary>
    public class NinjaClicker : MonoBehaviour
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private bool _useNoise;
        [SerializeField] private int _maxUpdateDelay;

        private IProxyButton _spinShortPress;
        private int _updateCount = 0;
        private int _updateDelay = 0;

        public void OnCoreLoaded(MetaLocator metaLocator)
        {
            _spinShortPress = metaLocator.Get<IProxyButton>("SpinShortPress");
            _updateDelay = Random.Range(0, _maxUpdateDelay);
        }

        public void OnMetaLoaded(MetaLocator metaLocator)
        {
            // does nothing
        }

        void Update()
        {
            if (!_enabled)
            {
                return;
            }
            if (_useNoise)
            {
                if (_updateCount != _updateDelay)
                {
                    ++_updateCount;
                    return;
                }

                _updateCount = 0;
                _updateDelay = Random.Range(0, _maxUpdateDelay);
            }

            _spinShortPress.Press();
        }
    }
}
