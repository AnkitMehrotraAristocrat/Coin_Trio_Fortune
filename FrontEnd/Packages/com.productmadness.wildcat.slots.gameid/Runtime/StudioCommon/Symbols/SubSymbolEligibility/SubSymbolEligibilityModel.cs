using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SubSymbolEligibilityModel : IModel
	{
		#region Helper Classes
		public class CriterionDictionary : Dictionary<string, bool> { }
		public class SymbolPositionEligibity : Dictionary<int, CriterionDictionary> { }
		#endregion

		// reelId : Dictionary<rowIndex, Dictionary<criteriaName, isEligible>>
		private Dictionary<int, SymbolPositionEligibity> _positionEligibility = new Dictionary<int, SymbolPositionEligibity>();

		public SubSymbolEligibilityModel(ServiceLocator _) { }

		public bool IsPositionEligible(int reelId, int rowIndex)
		{
			if (!_positionEligibility.TryGetValue(reelId, out SymbolPositionEligibity symbolPositionEligibility))
			{
				return true;
			}

			if (!symbolPositionEligibility.TryGetValue(rowIndex, out CriterionDictionary criterionDictionary))
			{
				return true;
			}

			bool isEligible = true;
			foreach (KeyValuePair<string, bool> criterionEntry in criterionDictionary)
			{
				isEligible &= criterionEntry.Value;
			}

			return isEligible;
		}

		public void SetPositionEligibility(int reelId, int rowIndex, string criteriaDeterminer, bool isEligible)
		{
			if (_positionEligibility.TryGetValue(reelId, out SymbolPositionEligibity symbolPositionEligibility))
			{
				UpdateSymbolPositionEligibility(symbolPositionEligibility, rowIndex, criteriaDeterminer, isEligible);
			}
			else
			{
				AddSymbolPositionEligibility(reelId, rowIndex, criteriaDeterminer, isEligible);
			}
		}

		private void AddSymbolPositionEligibility(int reelId, int rowIndex, string criteriaDeterminer, bool isEligible)
		{
			_positionEligibility.Add(reelId, new SymbolPositionEligibity());
			AddCriterionEntry(_positionEligibility[reelId], rowIndex, criteriaDeterminer, isEligible);
		}

		private void UpdateSymbolPositionEligibility(SymbolPositionEligibity symbolPositionEligibility, int rowIndex, string criteriaDeterminer, bool isEligible)
		{
			if (symbolPositionEligibility.TryGetValue(rowIndex, out CriterionDictionary criterionDictionary))
			{
				UpdateCriterionEntry(criterionDictionary, criteriaDeterminer, isEligible);
			}
			else
			{
				AddCriterionEntry(symbolPositionEligibility, rowIndex, criteriaDeterminer, isEligible);
			}
		}

		private void AddCriterionEntry(SymbolPositionEligibity symbolPositionEligibility, int rowIndex, string criteriaDeterminer, bool isEligible)
		{
			symbolPositionEligibility.Add(rowIndex, new CriterionDictionary());
			symbolPositionEligibility[rowIndex].Add(criteriaDeterminer, isEligible);
		}

		private void UpdateCriterionEntry(CriterionDictionary criterionDictionary, string criteriaDeterminer, bool isEligibile)
		{
			criterionDictionary[criteriaDeterminer] = isEligibile;
		}
	}
}
