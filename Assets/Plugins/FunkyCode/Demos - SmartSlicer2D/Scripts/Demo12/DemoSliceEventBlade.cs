using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D.Demo {
	public class DemoSliceEventBlade : MonoBehaviour {

		void Start () {
			Sliceable2D slicer = GetComponent<Sliceable2D>();
			if (slicer != null) {
				slicer.AddAnchorResultEvent(sliceEvent);
			}
		}
		
		void sliceEvent (Slice2D slice) {
			foreach(GameObject g in slice.GetGameObjects()) {
				Destroy(g.GetComponent<DemoSpinBlade>());
				Destroy(g.GetComponent<DemoSliceEventBlade>());
				g.transform.parent = null;
			}
		}
	}
}