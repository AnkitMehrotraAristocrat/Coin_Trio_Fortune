using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SpinGaffeMenu : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI _textBox;

		private SpinGaffeModel _spinGaffeModel;

		private void LazyInit()
		{
			if (_spinGaffeModel == null)
			{
				_spinGaffeModel = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>().GetOrCreate<SpinGaffeModel>();
			}
		}

		private void OnEnable()
		{
			LazyInit();
			_textBox.text = CreateSpinGaffeText();
		}

		private string CreateSpinGaffeText()
		{
			string gaffes = string.Empty;
			int index = 0;
			IEnumerator<SpinGaffeData> gaffesEnumerator = _spinGaffeModel.GetEnumerator();

			while (gaffesEnumerator.MoveNext())
			{
				gaffes += gaffesEnumerator.Current.AsText(index) + "\r\n";
				++index;
			}

			return gaffes;
		}

		public void CopyGaffesToClipboard()
		{
			GUIUtility.systemCopyBuffer = CreateSpinGaffeText();
		}

		public void SetQueueSize(string size)
		{
			size = string.IsNullOrEmpty(size) ? "0" : size;
			if (int.TryParse(size, out int max))
			{
				max = max > 0 ? max : 10; // default to 10
				_spinGaffeModel.MaxCount = max;
			}
		}
	}
}
