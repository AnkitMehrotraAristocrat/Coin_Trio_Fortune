using System;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SpinGaffeData
	{
		private readonly string _menuFormat = "Spin #{0} | {1} | {2} | ID {3}\r\n{4}\r\n";

		public string GameState { get; set; }
		public string TimeStamp { get; set; }
		public Guid SpinGuid { get; set; }
		public List<ulong> RandomValues { get; set; }

		public string AsText(int index)
		{
			string text = string.Format(_menuFormat,
				index,
				GameState,
				TimeStamp,
				SpinGuid,
				string.Join(",", RandomValues));
			return text;
		}
	}
}
