using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[CreateAssetMenu(fileName = "MechanicConfiguration", menuName = "NMG/Wizard/Mechanic Configuration")]
	public class MechanicConfiguration : ScriptableObject
	{
		[SerializeField] private string _id;
		public string Id => _id;

		[SerializeField] private List<SubGraphElement> _subGraphElements;
		public List<SubGraphElement> SubGraphElements => _subGraphElements;

		[SerializeField] private List<TriggerElement> _triggerElements;
		public List<TriggerElement> TriggerElements => _triggerElements;

		[SerializeField] private List<MechanicElement> _mechanicElements;
		public List<MechanicElement> MechanicElements => _mechanicElements;
	}
}
