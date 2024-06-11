using System;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Implemented by concrete classes that need to know when a particular reel
	/// event occurs.
	/// </summary>
	public interface IReelEventResponder
	{
		void OnReelSpin(int reelIndex);
		void OnReelLanding(int reelIndex);
		void OnReelStop(int reelIndex);
		void OnReelQuickStop(int reelIndex);
	}

	/// <summary>
	/// A component that forwards IReelViewResponder method invocations to all parent IReelEventResponder
	/// implementations.
	/// </summary>
	public class ReelViewEventForwarder : MonoBehaviour, IReelViewResponder, ServiceLocator.IHandler
    {
		[FieldRequiresParent] private RootReelView _rootReelView;
        [FieldRequiresParent] private IReelEventResponder[] _reelEventResponders;

        public static event EventHandler ReelSpin;
        public static event EventHandler ReelStop;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();

            if (_reelEventResponders == null)
            {
                _reelEventResponders = GetComponentsInParent<IReelEventResponder>();
            }
        }

		public void OnReelSpin()
		{
			foreach (IReelEventResponder view in _reelEventResponders)
			{
				view.OnReelSpin(_rootReelView.ReelIndex);
			}

			ReelSpin?.Invoke(this, null);
		}

		public void OnReelStop()
		{
			foreach (IReelEventResponder view in _reelEventResponders)
			{
				view.OnReelStop(_rootReelView.ReelIndex);
			}

            ReelStop?.Invoke(this, new ReelStopEventArgs(_rootReelView.ReelIndex));
        }
	}

    public class ReelStopEventArgs : EventArgs
    {
		public int ReelIndex { get; set; }

        public ReelStopEventArgs(int reelIndex)
        {
			ReelIndex = reelIndex;
        }
    }
}
