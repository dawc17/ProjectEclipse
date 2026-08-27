using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Scripts
{
	public class OnlyMobileElements : MonoBehaviour
	{
		public List<GameObject> Elements = new List<GameObject>();

		private void Awake()
		{
			foreach (GameObject element in Elements)
			{
				element.SetActive(SystemProperties.DBBOCENKMGD());
			}
		}
	}
}
