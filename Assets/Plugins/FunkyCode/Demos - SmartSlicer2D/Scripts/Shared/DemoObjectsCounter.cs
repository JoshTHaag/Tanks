using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slicer2D.Demo {
	public class DemoObjectsCounter : MonoBehaviour {

		void OnRenderObject() {
			Text text = GetComponent<Text> ();
			text.text = "objects " + Sliceable2D.GetListCount();
		}
	}
}