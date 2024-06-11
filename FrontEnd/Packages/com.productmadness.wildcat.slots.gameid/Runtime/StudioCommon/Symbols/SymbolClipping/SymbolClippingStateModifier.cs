using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[RequireComponent(typeof(SymbolClippingHandler))]
	public class SymbolClippingStateModifier : MonoBehaviour
	{
		protected SymbolClippingHandler _symbolClippingHandler;

		private void Awake()
		{
			_symbolClippingHandler = GetComponent<SymbolClippingHandler>();
		}

		public void EnableClipping()
		{
			_symbolClippingHandler.ClippingEnabled = true;
		}

		public void DisableClipping()
		{
			_symbolClippingHandler.ClippingEnabled = false;
		}
	}
}
