using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// A component that supports executing reel bounce audio and reel quick stop audio.
    /// Usually this is only required for scenarios where multiple reels can land simultaneously (such as hold and spin).
    /// </summary>
    public class ReelBounceEventView : MonoBehaviour, ServiceLocator.IHandler, ISpinResponder, IReelEventResponder
    {
        [FieldRequiresParent] AudioEventBindings _audioBindings;

        [SerializeField] ReelBounceGroupings _reelIndexGroups;

        private bool _hasQuickStopped = false;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            ValidateReelIndexGroups();
        }

        public void OnReelBounce(int reelIndex)
        {
            var group = _reelIndexGroups.Groups.FirstOrDefault(groupEntry => groupEntry.ReelIndices.Contains(reelIndex));
            group?.SetReelHasBounced(reelIndex, true);
            if (group != null && group.AllReelsBounced)
            {
                _audioBindings.Play(group.ReelStopAudioEventName);
            }
        }

        public void OnReelQuickStop(int reelIndex)
        {
            if (!_hasQuickStopped)
            {
                _hasQuickStopped = true;
                _audioBindings.Play(_reelIndexGroups.QuickStopAudioEventName);
            }
        }

        public IEnumerator<Yield> SpinStarted()
        {
            if (!gameObject.activeInHierarchy)
            {
                yield break;
            }

            _hasQuickStopped = false;

            foreach (ReelIndexBounceGroup group in _reelIndexGroups.Groups)
            {
                group.InitializeGroup();
            }

            yield break;
        }

        public IEnumerator<Yield> SpinComplete()
        {
            yield break;
        }

        public void OnReelSpin(int reelIndex)
        {
            _reelIndexGroups.Groups.FirstOrDefault(group => group.ReelIndices.Contains(reelIndex)).SetReelHasBounced(reelIndex, false);
        }

        public void OnReelStop(int reelIndex)
        {
            return;
        }

        #region Validation
        private void ValidateReelIndexGroups()
        {
            ValidateQuickStopAudioEventName();
            ValidateReelStopAudioEventNames();
            ValidateReelIndicesAreUnique();
        }

        private void ValidateQuickStopAudioEventName()
        {
            if (string.IsNullOrEmpty(_reelIndexGroups.QuickStopAudioEventName))
            {
                GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Quick stop audio event name is empty!", this);
                throw new NullReferenceException();
            }
        }

        private void ValidateReelStopAudioEventNames()
        {
            bool hasEmpty = _reelIndexGroups.Groups.Any(group => string.IsNullOrEmpty(group.ReelStopAudioEventName));
            if (hasEmpty)
            {
                GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Empty reel stop audio event name present!", this);
                throw new NullReferenceException();
            }
        }

        private void ValidateReelIndicesAreUnique()
        {
            HashSet<int> hashedReelIndices = new HashSet<int>();
            var reelIndices = _reelIndexGroups.Groups.SelectMany(group => group.ReelIndices).ToList();
            bool allUnique = reelIndices.All(hashedReelIndices.Add);
            if (!allUnique)
            {
                GameIdLogger.Logger.Error(GetType() + " (" + this.GetTag() + ") :: Duplicate reel index entries present!", this);
                throw new ArgumentException();
            }
        }

        public void OnReelLanding(int reelIndex)
        {

        }
        #endregion
    }
}
