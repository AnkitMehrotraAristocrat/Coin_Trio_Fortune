using System.Runtime.CompilerServices;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	/// <summary>
	/// A struct providing some custom colors used by the wizard
	/// </summary>
	public struct CustomColor
	{
		public static Color emeraldGreen
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(80.0f / 255.0f, 200.0f / 255.0f, 120.0f / 255.0f, 1.0f);
			}
		}

		public static Color orangePeel
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(255.0f / 255.0f, 159.0f / 255.0f, 0.0f / 255.0f, 1.0f);
			}
		}

		public static Color rajah
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(250.0f / 255.0f, 186.0f / 255.0f, 95.0f / 255.0f, 1.0f);
			}
		}
	}
}
