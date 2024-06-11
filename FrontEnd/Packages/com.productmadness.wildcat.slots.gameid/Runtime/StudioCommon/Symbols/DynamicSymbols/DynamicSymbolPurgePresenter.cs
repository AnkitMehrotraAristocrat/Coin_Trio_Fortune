using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols
{
	/// <summary>
	/// State machine presenter that will purge the DynamicSymbolModel's dynamic symbol replacements by
	/// replacing the current value with a fresh empty list (handled by the model).
	/// </summary>
	public class DynamicSymbolPurgePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private DynamicSymbolClientModel _dynamicSymbolClientModel;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public string Tag => this.GetTag();

		public INotifier Notifier
		{
			get; set;
		}

		public IEnumerator<Yield> Enter()
		{
			_dynamicSymbolClientModel.ResetDynamicSymbolReplacements();
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}
	}
}
