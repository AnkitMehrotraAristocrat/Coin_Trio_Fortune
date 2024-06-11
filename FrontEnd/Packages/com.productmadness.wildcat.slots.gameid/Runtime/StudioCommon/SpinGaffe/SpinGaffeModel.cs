using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SpinGaffeModel : IModel
	{
		private Queue<SpinGaffeData> _records = new Queue<SpinGaffeData>();

		public int Count => _records.Count;
		public int MaxCount = 10;

		public SpinGaffeModel(ServiceLocator _)
		{
			// does nothing, here for service locator
		}

		public void Add(SpinGaffeData data)
		{
			while (Count >= MaxCount)
			{
				_records.Dequeue();
			}

			_records.Enqueue(data);
		}

		public IEnumerator<SpinGaffeData> GetEnumerator()
		{
			return _records.GetEnumerator();
		}
	}
}
