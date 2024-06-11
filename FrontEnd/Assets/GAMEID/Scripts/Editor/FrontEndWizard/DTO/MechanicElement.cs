using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.Wizard
{
	[Serializable]
	public class MechanicElement
	{
		public SceneElementType Type;
		public string Component; // should be string and assembly qualified name
		public string Tag;
		public GameObject Prefab;
		public string ScenePath;
		public bool ReplaceExisting;
		public List<string> StateNodes;
	}
}
